namespace PCLCEPluginManager;

public interface IPCLCEPlugin
{
    void Initialize(scoped ReadOnlySpan<char> args);
}