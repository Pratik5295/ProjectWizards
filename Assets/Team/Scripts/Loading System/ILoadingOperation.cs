using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ILoadingOperation
{
    string Description { get; }
    UniTask LoadAsync(IProgress<float> progress = null);
}
