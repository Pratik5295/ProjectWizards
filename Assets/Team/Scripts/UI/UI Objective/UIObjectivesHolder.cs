using System.Collections.Generic;
using Team.Gameplay.ObjectiveSystem;
using UnityEngine;

namespace Team.UI
{
    public class UIObjectivesHolder : MonoBehaviour
    {
        [SerializeField]
        private GameObject uiObjectivePrefab;

        [SerializeField]
        private Transform objectivesHolder;

        public Dictionary<GameObjectiveData, UIObjective> UIObjectivesMap = new Dictionary<GameObjectiveData, UIObjective>();

        public void AddObjective(GameObjectiveData _data)
        {
            var spawnedObject = Instantiate(uiObjectivePrefab, objectivesHolder);
            var uiObjective = spawnedObject.GetComponent<UIObjective>();
            uiObjective.Populate(_data.ObjectiveName);

            UIObjectivesMap.Add(_data, uiObjective);
        }

        public void UpdateObjective(GameObjectiveData _data,bool res)
        {
            if (UIObjectivesMap.ContainsKey(_data))
            {
                UIObjectivesMap[_data].Toogle(res);
            }
        }

        public void ClearAllObjectives()
        {
            if (UIObjectivesMap.Count == 0) return;

            foreach (var obj in UIObjectivesMap.Values)
            {
                Destroy(obj.gameObject);
            }

            UIObjectivesMap.Clear();
        }

        public void ResetAllObjectives()
        {
            foreach (var obj in UIObjectivesMap.Values)
            {
                obj.Toogle(false);
            }
        }
    }
}