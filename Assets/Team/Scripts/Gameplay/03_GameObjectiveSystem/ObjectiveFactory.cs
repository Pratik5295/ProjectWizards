using System;

namespace Team.Gameplay.ObjectiveSystem
{
    /// <summary>
    /// Static factory class to create objectives at runtime
    /// </summary>
    public static class ObjectiveFactory
    {
        public static GenericObjective CreateObjective(GameObjectiveData data)
        {
            if (data.Type == ObjectiveType.ELIMINATION)
                return new EliminateObjective(data);
            else if (data.Type == ObjectiveType.SURVIVAL)
                return new SurvivalObjective(data);
            else if(data.Type == ObjectiveType.LOCATION)
                return new LocationObjective(data);
            else
                throw new NotSupportedException($"Unsupported objective type: {data.Type}");
        }
    }
}
