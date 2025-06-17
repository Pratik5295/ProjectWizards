namespace Team.Gameplay.ObjectiveSystem
{

    public class LocationObjective : GenericObjective
    {
        public LocationObjective(GameObjectiveData data) : base(data)
        {
        }

        public override bool CheckObjectiveComplete()
        {
            //Location objective will only have one target!

            var target = characterRefList[0];

            return target.CurrentTileID == data.LocationTileID;
        }
    }
}
