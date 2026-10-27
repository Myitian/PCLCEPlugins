namespace PCLCEPluginManager;

public interface IPCLCEPlugin
{
    static abstract Version? MinSupportedVersion { get; }
    static abstract Version? MaxSupportedVersion { get; }
    static abstract void Initialize(string? arg);
}