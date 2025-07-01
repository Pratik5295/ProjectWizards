using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Team.GameConstants;
using Team.Managers;
using Team.UI;
using UnityEngine;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float ShowPostGameScreenAfter = 2f;
    }
}

namespace Team.Gameplay.ObjectiveSystem
{
    //Make sure it runs after Game Turn Manager
    [DefaultExecutionOrder(3)]
    public class LevelObjectiveManager : MonoBehaviour,ILoadingOperation
    {
        #region Variables
        public static LevelObjectiveManager Instance = null;

        [SerializeField]
        private List<GameObjectiveData> _objectiveMap = new List<GameObjectiveData>();

        [Header("Private Local cache, dont fill")]
        [SerializeField]
        private List<GenericObjective> _levelObjectives = new List<GenericObjective>();


        [Header("Components")]
        [SerializeField]
        private GameTurnManager turnManager;

        [SerializeField]
        private UIObjectivesHolder objectivesHolder;

        public string Description => throw new NotImplementedException();
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

        public void LoadObjectivesFromLevelData(List<GameObjectiveData> _objectives)
        {
            CleanUp();

            _objectiveMap.Clear();

            //Load all objectives
            foreach(var objective in _objectives)
            {
                _objectiveMap.Add(objective);
            }

            //Load the objectives
            InitalizeObjectives();
        }

        /// <summary>
        /// Initialize the objectives for this level
        /// </summary>
        public void InitalizeObjectives()
        {
            CharacterManager characterManager = CharacterManager.Instance;
            foreach (var data in _objectiveMap)
            {
                var objective = ObjectiveFactory.CreateObjective(data);

                foreach (var objectTarget in data.ObjectiveTargets)
                {
                    var characterObject = characterManager.GetCharacter(objectTarget);
                    if (characterObject == null)
                    {
                        Debug.LogError($"Could not find character target for objective: {data.ObjectiveName}", gameObject);
                        continue;
                    }
                    objective.AddCharacterReference(characterObject);
                }

                _levelObjectives.Add(objective);
                objectivesHolder.AddObjective(data);
            }
        }

        public void CleanUp()
        {
            objectivesHolder.ClearAllObjectives();
        }

        public void ResetAllObjectives()
        {
            foreach(var objective in _levelObjectives)
            {
                objective.ResetObjective();
                objectivesHolder.UpdateObjective(objective.Data, false);
            }
        }

        #endregion

        #region Private Methods

        private void OnRoundTurnsCompletedHandler()
        {
            if (_levelObjectives.Count == 0)
            {
                Debug.LogWarning("There are no objectives for this level?");
                return;
            }

            //Init Level Completed as true
            bool levelCompleted = true;

            foreach (var objective in _levelObjectives)
            {
                //Returns true if completed
                bool result = objective.CheckObjectiveComplete();

                //If Any objective fails, then level not completed
                if (!result)
                {
                    levelCompleted = false;
                }

                objectivesHolder.UpdateObjective(objective.Data,result);
            }

            //Check if actually the level was completed
            if (levelCompleted)
            {
                LevelManager.Instance.OnCurrentLevelCompleted();
                UIManager.Instance.ShowEmptyUI();
                Invoke(nameof(ShowLevelCompletedUI), MetaConstants.ShowPostGameScreenAfter);
               
            }
        }

        private void ShowLevelCompletedUI()
        {
          
            UIManager.Instance.ShowPostGameUI();
        }

        public UniTask<GameObject> LoadAsync(IProgress<float> progress)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
