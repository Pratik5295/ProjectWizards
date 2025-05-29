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

    public class GenericObjective : MonoBehaviour
    {
        [SerializeField]
        protected ObjectiveType type;

        [SerializeField]
        protected bool isCompleted = false;

        public bool IsCompleted => isCompleted;

        private void LoadObjectiveData()
        {

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
