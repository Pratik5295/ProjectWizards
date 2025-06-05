using Team.Gameplay.GridSystem;
using UnityEngine;

namespace Team.Gameplay.ObjectiveSystem
{
    public enum ObjectiveType
    {
        DEFAULT = 0,
        ELIMINATION = 1,
        SURVIVAL = 2,
        LOCATION = 3,
        TURN_LIMIT = 4
    }

    [System.Serializable]
    public class GameObjectiveData
    {
        [Tooltip("This text will be shown on the UI")]
        public string ObjectiveName;
        [Tooltip("Type the character name from character list of whats the target of this objective")]
        public string ObjectiveTargetName;
        [Tooltip("What type of objective is this")]
        public ObjectiveType Type;

        //For Location
        [Tooltip("Fill only for LOCATION Objective")]
        public TileID LocationTileID;
    }

    [System.Serializable]
    public class GenericObjective
    {
        public string ObjectiveName;

        [SerializeField]
        protected GameObjectiveData data;

        public GameObjectiveData Data => data;

        [SerializeField]
        protected Base_Ch characterRef;

        [SerializeField]
        protected bool isCompleted = false;

        public bool IsCompleted => isCompleted;

        public GenericObjective(GameObjectiveData data)
        {
            this.ObjectiveName = data.ObjectiveName;
            this.data = data;
        }


        public void SetCharacterReference(Base_Ch _character)
        {
            characterRef = _character;
        }
        /// <summary>
        /// Checks the status of the objective based on type.
        /// By default returns false
        /// Sets the local variable isCompleted to be referred
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckObjectiveComplete()
        {
            return isCompleted;
        }

        public void ResetObjective()
        {
            isCompleted = false;
        }
    }
}
