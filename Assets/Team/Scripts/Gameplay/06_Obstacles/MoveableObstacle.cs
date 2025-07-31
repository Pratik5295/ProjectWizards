using System.Collections;
using System.Collections.Generic;
using Team.Gameplay.GridSystem;
using UnityEngine;

public class MoveableObstacle : Base_Obstacle, IMoveable
{

    public Stack<PlayerMove> HistoryStack = new Stack<PlayerMove>();

    [Header("Movement Variables")]
    private float smoothingTime = 1f; //Time to reach the target position.
    private float currentTime; //Current elapsed Time for movement lerp.
#region Grid Variables

    private int movementIteration;

    private Vector3 startPosition;

    private bool alreadyMoving;

    public PushProjectile PushProjectileInstance;
#endregion

    #region Vars_InvalidMovementShake

    private float shakeTimer = 0f;

    private float maxShakeAmount = 0.3f;
    #endregion

    public IEnumerator MoveByAmount(int movementAmount, Vector2 dir, bool wasPushed)
    {
        _previousTileID = _currentTileID;

        for (int i = 0; i < movementAmount; i++)
        {
            alreadyMoving = true;
            Vector3 desiredLocation = new Vector3(_currentTileID.x + (dir.x * OffsetValue), transform.position.y, _currentTileID.y + (dir.y * OffsetValue));

            TileID desiredTileID = new TileID(_currentTileID.x + (int)dir.x, _currentTileID.y + (int)dir.y);
            GridTile targetTile = ref_gridManager.FindTile(desiredTileID);

            if (targetTile && targetTile.IsTileWalkable())
            {
                if (targetTile.IsIceTile())
                {
                    movementAmount = IceTileLogic(movementAmount);
                    wasPushed = true;
                }
                if (_currentGridTile.IsIceTile() && !targetTile.IsIceTile())
                {
                    smoothingTime = 1f;
                }

                Vector3 targetPosition = new Vector3(targetTile.TilePosition.x, desiredLocation.y, targetTile.TilePosition.z);

                _currentTileID = targetTile.TileID;
                _currentGridTile = ref_gridManager.FindTile(_currentTileID);

                yield return StartCoroutine(LerpingMovement(targetPosition, wasPushed));
            }
            else
            {
                StartCoroutine(ShakeCharacter(0.25f));
                alreadyMoving = false;
                WasPushed(wasPushed);
                yield break;
            }
        }
        _previousGridTile.UpdateOccupiedStatus(false);

        _currentGridTile.UpdateOccupiedStatus(true, gameObject);
        transform.parent = _currentGridTile.transform;

        WasPushed(wasPushed);


        //Obstacles Interacted with

        List<ObstacleInteraction> obstacleInteractions = new List<ObstacleInteraction>();

        ObstacleInteraction obstacleInteraction = new ObstacleInteraction();

        obstacleInteraction.ObstacleInteracted = this;
        obstacleInteraction.PreviousTileID = _previousGridTile.TileID;
        obstacleInteraction.PreviousDirection = baseRotation.DirectionFacing;

        obstacleInteractions.Add(obstacleInteraction);

        PlayerMove playerMove = new PlayerMove(PlayerMoveType.PUSH,null, obstacleInteractions);
        HistoryStack.Push(playerMove);
    }

    public IEnumerator LerpingMovement(Vector3 targetPosition, bool wasPushed)
    {
        currentTime = 0;
        //Vector3 startingPosition = transform.position;
        while (currentTime < smoothingTime)
        {
            currentTime += Time.deltaTime;

            float lerpAmount = currentTime / smoothingTime;

            transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosition.x, lerpAmount),
                                             transform.position.y, Mathf.Lerp(transform.position.z, targetPosition.z, lerpAmount));

            yield return null;
        }

        if (currentTime >= smoothingTime)
        {
            currentTime = 0;
            transform.position = targetPosition;
            startPosition = transform.position;

        }

        alreadyMoving = false;
    }

    private int IceTileLogic(int movementAmount)
    {
        if (!_currentGridTile.IsIceTile())
        {
            movementAmount = 0; movementAmount += 2;
        }
        else movementAmount++;

        smoothingTime = .1f;
        return movementAmount;
    }

    //Shakes character if path or tile is invalid.
    protected IEnumerator ShakeCharacter(float MaxShakeTime)
    {
        Vector3 defaultPos = transform.localPosition;

        shakeTimer = 0f;

        while (shakeTimer < MaxShakeTime)
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

    private void WasPushed(bool wasPushed)
    {
        if (wasPushed)
        {
            if (PushProjectileInstance)
            {
                PushProjectileInstance.CleanUp();
                PushProjectileInstance = null;
            }
        }
    }

    #region Undo Functions
    [ContextMenu("Undo Movement")]
    public virtual void UndoAction()
    {
        while (HistoryStack.Count > 0)
        {
            var move = HistoryStack.Pop();

            if (move.Type == PlayerMoveType.PUSH)
            {
                UndoMovement();
            }
        }
    }

    protected void UndoMovement()
    {
        if (_currentTileID == _startTileID) { return; }
        _currentGridTile.UpdateOccupiedStatus();

        _currentTileID = _startTileID;
        _currentGridTile = ref_gridManager.FindTile(_currentTileID);

        _currentGridTile.SetObjectOccupyingTile(this.gameObject);

        transform.position = new Vector3(_currentGridTile.TilePosition.x, transform.position.y, _currentGridTile.TilePosition.z);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_characterResponsible">Who Fired the trigger</param>
    /// <param name="_movedBackTo"></param>
    public void ForceUndoMovement(Base_Ch _characterResponsible,TileID _movedBackTo)
    {
        //if (_currentTileID == _movedBackTo) { return; }
        _currentGridTile.UpdateOccupiedStatus();

        _currentTileID = _movedBackTo;
        _currentGridTile = ref_gridManager.FindTile(_currentTileID);

        _currentGridTile.SetObjectOccupyingTile(this.gameObject);

        transform.position = new Vector3(_currentGridTile.TilePosition.x, transform.position.y, _currentGridTile.TilePosition.z);

        _characterResponsible.OnUndoAbilityCompleted();
    }
    #endregion
}
