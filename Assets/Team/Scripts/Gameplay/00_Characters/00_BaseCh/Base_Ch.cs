using System.Collections;
using System.Collections.Generic;
using Team.Gameplay.GridSystem;
using Team.Enum.Character;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Base_Ch : MonoBehaviour, IMoveable, IProjectileHittable, IUsableAbility
{
    [Header("Enumerations")]
    [SerializeField] private Enum_CharacterState CharState = Enum_CharacterState.Alive;

    public bool IsAlive => CharState == Enum_CharacterState.Alive;


    [Header("Script References")]
    [SerializeField] private GridManager ref_gridManager;

    [SerializeField] private Base_Rotation baseRotation;



    [Header("Movement Variables")]
    [SerializeField] private TileID currentTileID = new TileID(0, 0);

    private float OffsetValue;
    private float smoothingTime = 1f; //Time to reach the target position.
    private float currentTime; //Current elapsed Time for movement lerp.
    private float lerpingDelayTime = 0.001f;

    private int movementIteration;

    private Vector3 startPosition;

    private bool alreadyMoving;

    #region Vars_InvalidMovementShake

    private float shakeTimer = 0f;

    private float maxShakeAmount = 0.3f;
    #endregion


    public delegate void Evnt_stateChange();
    public event Evnt_stateChange OnStateChanged;

    void Start()
    {
        ref_gridManager = GridManager.Instance;
        OffsetValue = ref_gridManager.GridSlot_Offset;
    }

    #region Debugging Movement Button Functions
    public void buttonForwardBackward(bool moveForward)
    {
        if (alreadyMoving)
        {
            return;
        }
        if (moveForward)
        {
            StartCoroutine(MoveByAmount(2, new Vector2(0, 1)));
        }
        else 
        { 
            StartCoroutine(MoveByAmount(2, new Vector2(0, -1)));
        }
    }
    public void buttonLeftRight(bool moveRight)
    {
        if (alreadyMoving)
        {
            return;
        }
        if (moveRight)
        {
            StartCoroutine(MoveByAmount(2, new Vector2(1, 0)));
        }
        else 
        { 
            StartCoroutine(MoveByAmount(2, new Vector2(-1, 0)));
        }
    }
    #endregion

    //Moves by a defined amount in a direction, if the tile exists and player can move there. Then passes to lerp.
    public virtual IEnumerator MoveByAmount(int movementAmount, Vector2 dir)
    {
        for(int i = 0; i < movementAmount; i++)
        {
            alreadyMoving = true;
            Vector3 desiredLocation = new Vector3(currentTileID.x + (dir.x * OffsetValue), transform.position.y, currentTileID.y + (dir.y * OffsetValue));

            TileID desiredTileID = new TileID(currentTileID.x + (int)dir.x, currentTileID.y + (int)dir.y);
            GridTile targetTile = ref_gridManager.FindTile(desiredTileID);

            if (targetTile)
            {
                Vector3 targetPosition = new Vector3(targetTile.TilePosition.x, desiredLocation.y, targetTile.TilePosition.z);
                currentTileID = targetTile.TileID;

                yield return StartCoroutine(LerpingMovement(targetPosition));
            }
            else
            {
                StartCoroutine(ShakeCharacter(0.25f));
                alreadyMoving = false;
                yield break;
            }
        }
    }

    
    //Lerps the movement to the next available tile.
    public virtual IEnumerator LerpingMovement(Vector3 targetPosition)
    {
        while (currentTime < smoothingTime)
        {
            currentTime += Time.deltaTime;

            float lerpAmount = currentTime / smoothingTime;

            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpAmount);

            yield return new WaitForSeconds(lerpingDelayTime);
        }

        if (currentTime >= smoothingTime)
        {
            currentTime = 0;
            transform.position = targetPosition;
            startPosition = transform.position;

        }

        alreadyMoving = false;
    }

    //Shakes character if path or tile is invalid.
    private IEnumerator ShakeCharacter(float MaxShakeTime)
    {
        Vector3 defaultPos = transform.localPosition;

        shakeTimer = 0f;
        
        while(shakeTimer < MaxShakeTime)
        {
            transform.localPosition = defaultPos;
            shakeTimer += Time.deltaTime;

            float shakeAmountX = Random.Range(-0.3f, 0.4f) * maxShakeAmount;
            float shakeAmountZ = Random.Range(-0.3f, 0.4f) * maxShakeAmount;

            transform.localPosition = new Vector3(transform.localPosition.x + shakeAmountX, transform.localPosition.y, transform.localPosition.z + shakeAmountZ);

            yield return null;
        }
        transform.localPosition = defaultPos;
    }



    public virtual void HitByProjectile(Enum_ProjectileType projectileType)
    {
        switch (projectileType)
        {
            case Enum_ProjectileType.Fireball:
                CharState = Enum_CharacterState.Dead;
                break;
            case Enum_ProjectileType.NonLethalRound:
                CharState = Enum_CharacterState.Incapacitated;
                break;
        }
        OnStateChanged();
    }

    public bool checkStateOfChar()
    {
        if (CharState == Enum_CharacterState.Alive) { return true; }
        else return false;
    }

    private void resetCharState()
    {
        if (CharState == Enum_CharacterState.Incapacitated)
        {
            OnStateChanged();
            CharState = Enum_CharacterState.Alive;
        }
    }


    public virtual void UseAbility()
    {
        Debug.LogError($" {gameObject.name} Ability not programmed for character");
    }

}
