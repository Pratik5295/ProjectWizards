namespace Team.Gameplay.ObjectiveSystem
{

    public class EliminateObjective : GenericObjective
    {
        public override bool CheckObjectiveComplete()
        {
            return !characterRef.IsAlive;
        }
    }
}
