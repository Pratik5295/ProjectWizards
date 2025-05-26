using System.Collections.Generic;
using Team.Gameplay.TurnSystem;
using UnityEngine;

public class GameTurnManager : MonoBehaviour
{
    private Queue<GameTurn> turnManager;

    public List<GameObject> characterTurns = new List<GameObject>();

    public bool HasCharacterTurns => turnManager.Count > 0;

    [ContextMenu("Load all Units")]
    public void LoadCharacterUnits()
    {
        if (turnManager == null)
            turnManager = new Queue<GameTurn>();

        if (HasCharacterTurns)
        {
            turnManager.Clear();
        }

        LoadQueue();
    }

    public void LoadQueue()
    {
        foreach (var unit in characterTurns)
        {
            turnManager.Enqueue(unit.GetComponent<GameTurn>());
        }

        Debug.Log($"Loading complete: {turnManager.Count}");

        foreach(var t in turnManager)
        {
            Debug.Log($"Turn: {t.name}");
        }
    }

    [ContextMenu("Play All Turns")]
    public async void PlayTurns()
    {
        Debug.Log("Starting async turn execution...");

        while(turnManager.Count > 0)
        {
            GameTurn turn = turnManager.Dequeue();
            await turn.PerformAsync();
        }

        Debug.Log("All turns completed.");
    }

    [ContextMenu("Play next turn")]
    public async void PlayNextTurn()
    {
        GameTurn turn = turnManager.Dequeue();
        await turn.PerformAsync();

        Debug.Log("Done");
    }

}
