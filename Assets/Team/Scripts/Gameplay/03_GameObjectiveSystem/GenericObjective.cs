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

    public class GenericObjective : MonoBehaviour
    {
        [SerializeField]
        protected GameObjectiveData data;

        public GameObjectiveData Data => data;

        [SerializeField]
        protected Base_Ch characterRef;

        [SerializeField]
        protected bool isCompleted = false;

        public bool IsCompleted => isCompleted;

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

        [ContextMenu("Complete Objective")]
        public void ForceCompleteObjectiveStatus() => isCompleted = true;
    }
}
