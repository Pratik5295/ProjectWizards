using System.Collections.Generic;
using UnityEngine;

namespace Team.Gameplay.TurnSystem
{
    public class GameTurnManager : MonoBehaviour
    {
        private TurnQueue<GameObject> turnManager;

        [SerializeField] private List<GameObject> characterTurns;

        public bool HasCharacterTurns => !turnManager.IsQueueEmpty();


        [ContextMenu("Load all Units")]
        public void LoadCharacterUnits()
        {
            if(turnManager == null) turnManager = new TurnQueue<GameObject>();

            if (HasCharacterTurns)
            {
                //Values exist. Clear them
                turnManager.ClearQueue();
            }

            LoadQueue();
        }

        private void LoadQueue()
        {
            foreach (var unit in characterTurns)
            {
                turnManager.AddToQueue(unit);
            }

            Debug.Log("Loading complete");
        }

        [ContextMenu("Play All Turns")]
        public void PlayTurns()
        {
            if (turnManager.IsQueueEmpty())
            {
                LoadQueue();
            }

            //Till the current null is not empty, keep on pushing turns
            while (!turnManager.IsQueueEmpty())
            {
                PlayNextTurn();
            }

            Debug.Log("Nothing in queue, all turns are empty");
        }

        [ContextMenu("Play Next Turns")]
        public void PlayNextTurn()
        {
            if (turnManager.PeekCurrentTurn())
            {
                //Turn exist
                GameObject next = turnManager.NextTurn();
                Debug.Log($"Performing Action: {next.name}");
            }
            else
            {
                //Next turn is empty
                Debug.Log("No action exist for next turn");
            }
        }
    }
}
