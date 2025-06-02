namespace Team.Gameplay.ObjectiveSystem
{

    public class SurvivalObjective : GenericObjective
    {
        public override bool CheckObjectiveComplete()
        {
            return characterRef.IsAlive;
        }
    }
}
