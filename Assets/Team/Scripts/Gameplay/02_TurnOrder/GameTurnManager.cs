using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team.Gameplay.TurnSystem;
using UnityEngine;

public class GameTurnManager : MonoBehaviour
{
    public static GameTurnManager Instance = null;

    #region Variables
    [Header("Components")]
    private Queue<GameTurn> turnManager;

    public List<GameObject> originalOrder = new List<GameObject>();
    public List<GameObject> currentTurnOrder = new List<GameObject>(); //This will be used to reset the Queue

    [SerializeField]
    private Transform turnHolder;

    public bool HasCharacterTurns => turnManager.Count > 0;

    public Action OnAllTurnsCompleted;  //TODO: Update this to include the round integer

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ForceRebuildTurns(true);
    }

    #endregion

    #region Public Methods

    public async Task LoadQueue()
    {
        //Wait till the end of frame
        await Task.Yield();
        foreach (var unit in currentTurnOrder)
        {
            turnManager.Enqueue(unit.GetComponent<GameTurn>());
        }

        Debug.Log($"Loading complete: {turnManager.Count}");

        //foreach(var t in turnManager)
        //{
        //    Debug.Log($"Turn: {t.name}");
        //}
    }

    public void ForceRebuildTurns(bool start = false)
    {
        if(turnHolder.childCount == 0)
        {
            Debug.LogError("Character turns are missing");
            return;
        }

        currentTurnOrder.Clear();
        for (int i = 0; i < turnHolder.childCount; i++)
        {
            currentTurnOrder.Add(turnHolder.GetChild(i).gameObject);

            if (start)
            {
                originalOrder.Add(turnHolder.GetChild(i).gameObject);
            }
        }
    }

    #endregion


    #region Context Menu Methods

    [ContextMenu("Play All Turns")]
    public async void PlayTurns()
    {
        if(turnManager == null)
        {
            turnManager = new Queue<GameTurn>();
        }

        if(turnManager.Count == 0)
        {
            await LoadQueue();
        }

        Debug.Log("Has moves, now play all turns");
        while(turnManager.Count > 0)
        {
            GameTurn turn = turnManager.Dequeue();

            if (turn.IsAlive())
            {
                await turn.PerformAsync();
            }
            else
            {
                Debug.Log($"{turn.name} Move character is dead, turn skipped");
            }
        }

        Debug.Log("All turns completed.");

        OnAllTurnsCompleted?.Invoke();
    }

    #endregion
}
