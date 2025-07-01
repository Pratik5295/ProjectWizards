using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class InstantiateLevelOperation : MonoBehaviour, ILoadingOperation
{
    public string Description => throw new NotImplementedException();

    public GameObject levelPrefab;

    public void SetLevelPrefab(GameObject _levelPrefab)
    {
        levelPrefab = _levelPrefab;
    }

    public async UniTask<GameObject> LoadAsync(IProgress<float> progress)
    {
        var op = InstantiateAsync(levelPrefab);
        while (!op.isDone)
        {
            progress.Report(op.progress);
            await UniTask.Yield();
        }
        progress.Report(1f);


        GameObject spawned = op.Result[0];
        return spawned;
    }

}
