namespace Team.Gameplay.ObjectiveSystem
{

    public class SurvivalObjective : GenericObjective
    {
        public SurvivalObjective(GameObjectiveData data) : base(data) { }

        public override bool CheckObjectiveComplete()
        {
            return isCompleted = characterRef.IsAlive;
        }
    }
}
