using System.Linq;

namespace Team.Gameplay.ObjectiveSystem
{

    public class EliminateObjective : GenericObjective
    {
        public EliminateObjective(GameObjectiveData data) : base(data) { }


        public override bool CheckObjectiveComplete()
        {
            return isCompleted = characterRefList.All(character => !character.IsAlive);
        }
    }
}
