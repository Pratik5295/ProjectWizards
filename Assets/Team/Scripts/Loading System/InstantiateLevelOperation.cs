using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class InstantiateLevelOperation : ILoadingOperation
{
    [SerializeField] private GameObject levelPrefab;

    public string Description => $"Instantiating Level: {(levelPrefab ? levelPrefab.name : "Unknown")}";

    public void SetLevelPrefab(GameObject prefab)
    {
        levelPrefab = prefab;
    }

    public async UniTask<GameObject> LoadAsync(IProgress<float> progress = null)
    {
        if (levelPrefab == null)
        {
            throw new ArgumentNullException(nameof(levelPrefab), "Level prefab is not set!");
        }

        try
        {
            progress?.Report(0.0f);
            Debug.Log($"Instantiating level prefab: {levelPrefab.name}");

            // Simulate instantiation time for heavy prefabs
            await UniTask.Delay(100);
            progress?.Report(0.5f);

            var instantiatedLevel = UnityEngine.Object.Instantiate(levelPrefab);

            progress?.Report(0.8f);
            await UniTask.Yield(); // Allow instantiation to complete

            progress?.Report(1.0f);
            Debug.Log($"Level prefab instantiated successfully: {instantiatedLevel.name}");

            return instantiatedLevel;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to instantiate level prefab: {ex.Message}");
            throw;
        }
    }

    // ILoadingOperation implementation (returns UniTask instead of UniTask<GameObject>)
    async UniTask ILoadingOperation.LoadAsync(IProgress<float> progress)
    {
        await LoadAsync(progress);
    }
}
