#:package ICSharpCode.ILSpyX@*

using ICSharpCode.ILSpyX;

using (HttpClient client = new())
{
    Console.WriteLine("Downloading PCL2_CE_Release_x64.exe");
    using Stream stream = await client.GetStreamAsync("https://github.com/PCL-Community/PCL-CE/releases/download/v2.15.1/PCL2_CE_Release_x64.exe");
    using FileStream fs = new("PCL2_CE_Release_x64.exe", FileMode.Create, FileAccess.Write, FileShare.Read);
    await CopyStream(stream, fs);
}
LoadedPackage? package = LoadedPackage.FromBundle("PCL2_CE_Release_x64.exe");
if (package is null)
{
    Console.WriteLine("Failed to load package");
    return;
}
foreach (PackageEntry entry in package.Entries)
{
    if (entry.Name is not ("Plain Craft Launcher 2.dll" or "PCL.Core.dll"))
        continue;
    Console.WriteLine($"Extracting {entry.Name}");
    using Stream? es = entry.TryOpenStream();
    if (es is null)
    {
        Console.WriteLine("Failed to open stream");
        continue;
    }
    using FileStream fs = new(entry.Name, FileMode.Create, FileAccess.Write, FileShare.Read);
    await CopyStream(es, fs);
}

static async ValueTask CopyStream(Stream input, Stream output)
{
    byte[] buffer = new byte[65536];
    int lastProgress = 0;
    int progress = 0;
    int bytesRead;
    while ((bytesRead = await input.ReadAsync(buffer)) > 0)
    {
        progress += bytesRead;
        if (progress - lastProgress >= 2000000)
        {
            lastProgress = progress;
            Console.WriteLine($"Copied {progress} bytes");
        }
        await output.WriteAsync(buffer.AsMemory(0, bytesRead));
    }
    if (progress != lastProgress)
        Console.WriteLine($"Copied {progress} bytes");
}