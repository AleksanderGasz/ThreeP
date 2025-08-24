namespace Mac.Modules.Progress;

public class ProgressService
{
    public event Action? OnProgressChanged;
    public bool IsLoading { get; set; }
    
    public async Task SetProgress(bool value)
    {
        if (IsLoading==value) return;
        IsLoading = value;
        OnProgressChanged?.Invoke();
    }
}