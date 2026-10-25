using System.Diagnostics;
using System.Text.RegularExpressions;

static partial class Program
{
    [GeneratedRegex(@"P(lain)?\s*C(raft)?\s*L(auncher)?.*C(ommunity)?\s*E(dition)?", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline, "zh-CN")]
    static partial Regex ExeName { get; }
    static void Main(string[] args)
    {
        if (args is not [string exe, ..] || exe is "")
        {
            exe = "";
            string dir = Environment.CurrentDirectory;
            foreach (string file in Directory.EnumerateFiles(dir, "*.exe"))
            {
                if (ExeName.IsMatch(Path.GetFileName(file.AsSpan())))
                {
                    exe = file;
                    break;
                }
            }
        }
        if (!File.Exists(exe))
        {
            Console.WriteLine($"Cannot find the PCLCE executable: {exe}");
            return;
        }
        ProcessStartInfo psi = new(exe);
        for (int i = 1; i < args.Length; i++)
            psi.ArgumentList.Add(args[i]);
        psi.EnvironmentVariables["DOTNET_STARTUP_HOOKS"] = Path.GetFullPath(Path.Join(Environment.CurrentDirectory, "PCLCEPluginManager.dll"));
        Process.Start(psi);
    }
}