using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Team.Gameplay.ObjectiveSystem
{

    public class SurvivalObjective : GenericObjective
    {
        public SurvivalObjective(GameObjectiveData data) : base(data) { }

        public override bool CheckObjectiveComplete()
        {
            return isCompleted = characterRefList.All(character => character.IsAlive);
        }
    }
}
