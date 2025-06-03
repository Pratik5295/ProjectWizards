using UnityEngine;
using System.Collections.Generic;
using Team.Managers;

namespace Team.Gameplay.ObjectiveSystem
{
    //Make sure it runs after Game Turn Manager
    [DefaultExecutionOrder(3)]
    public class LevelObjectiveManager : MonoBehaviour
    {
        #region Variables
        public static LevelObjectiveManager Instance = null;

        [SerializeField]
        private List<GenericObjective> levelObjectives = new List<GenericObjective>();

        [SerializeField]
        private List<GameObjectiveData> objectiveMap = new List<GameObjectiveData>();

        [Header("Components")]
        [SerializeField]
        private GameTurnManager turnManager;
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
            turnManager = GameTurnManager.Instance;

            if(turnManager == null)
            {
                Debug.LogWarning("Game Turn Manager not found");
            }

            //Turn Manager wouldn't have loaded here, need to handle this via the game load data?
            RegisterEvents();

            InitalizeObjectives();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        #endregion

        #region Event Listeners and Handlers

        private void RegisterEvents()
        {
            if(turnManager != null)
            {
                turnManager.OnAllTurnsCompleted += OnRoundTurnsCompletedHandler;
            }
        }

        private void UnregisterEvents()
        {
            if (turnManager != null)
            {
                turnManager.OnAllTurnsCompleted -= OnRoundTurnsCompletedHandler;
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize the objectives for this level
        /// </summary>
        public void InitalizeObjectives()
        {
            CharacterManager characterManager = CharacterManager.Instance;
            foreach (var data in objectiveMap)
            {
                var objective = ObjectiveFactory.CreateObjective(data);

                var characterObject = characterManager.GetCharacter(data.ObjectiveTargetName);
                if (characterObject == null)
                {
                    Debug.LogError($"Could not find character target for objective: {data.ObjectiveName}", gameObject);
                    continue;
                }

                objective.SetCharacterReference(characterObject);
                levelObjectives.Add(objective);
            }
        }

        #endregion

        #region Private Methods

        private void OnRoundTurnsCompletedHandler()
        {
            if (levelObjectives.Count == 0)
            {
                Debug.LogWarning("There are no objectives for this level?");
                return;
            }

            foreach (var objective in levelObjectives)
            {
                bool result = objective.CheckObjectiveComplete();
                Debug.Log($"Objective: {objective.Data.ObjectiveName} is {(result ? "Complete" : "Incomplete")}");
            }
        }

        #endregion
    }
}
