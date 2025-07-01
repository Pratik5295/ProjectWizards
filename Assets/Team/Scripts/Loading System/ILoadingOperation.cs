public interface ILoadingOperation
{
    string Description { get; }
    UniTask LoadAsync(IProgress<float> progress);
}
