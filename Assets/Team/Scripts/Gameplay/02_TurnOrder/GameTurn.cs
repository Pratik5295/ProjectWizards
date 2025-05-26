using System;
using UnityEngine;


namespace Team.Gameplay.TurnSystem
{
    [System.Serializable]
    public class GameTurn : MonoBehaviour, IGameMove
    {
        public GameObject Character; //Character being affected by this turn? TODO: Turn it into the base character

        public Action OnTurnStartedEvent;
        public Action OnTurnEndedEvent;

        public void Perform()
        {
            Debug.Log($"{gameObject.name} performs turn");
        }
    }
}
