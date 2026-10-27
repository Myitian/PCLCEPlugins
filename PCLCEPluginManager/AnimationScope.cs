using PCL;

namespace PCLCEPluginManager;

public readonly struct AnimationScope : IDisposable
{
    public AnimationScope()
    {
        ModAnimation.AniControlEnabled++;
    }
    public void Dispose()
    {
        ModAnimation.AniControlEnabled--;
    }
}