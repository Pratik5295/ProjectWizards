using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Team.Gameplay.GameLevelSystem;
using System;


namespace Team.Managers
{
    public class GameLoadManager : MonoBehaviour
    {
        [SerializeField]
        private InstantiateLevelOperation levelOperation;

        public async UniTask<GameLevel> LoadGameLevelAsync(GameObject _levelPrefab)
        {
            levelOperation.SetLevelPrefab(_levelPrefab);

            GameLevel level = new GameLevel();
            List<ILoadingOperation> operations = new List<ILoadingOperation>
            {
               levelOperation
            };

            float totalProgress = 0f;
            int stepCount = operations.Count;

            for (int i = 0; i < stepCount; i++)
            {
                var operation = operations[i];
                //loadingScreenUI.SetMessage(operation.Description);

                await operation.LoadAsync(new Progress<float>(p =>
                {
                    float progressPerStep = 1f / stepCount;
                    float currentStepProgress = p * progressPerStep;
                    float overallProgress = (i * progressPerStep) + currentStepProgress;
                    //loadingScreenUI.SetProgress(overallProgress);
                }));
            }


            await UniTask.Delay(500);

            Debug.Log("Loading the level has been completed");

            return level;
        }
    }
}
