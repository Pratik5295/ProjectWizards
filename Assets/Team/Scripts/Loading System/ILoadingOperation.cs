using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ILoadingOperation
{
    string Description { get; }
    UniTask<GameObject> LoadAsync(IProgress<float> progress);
}
