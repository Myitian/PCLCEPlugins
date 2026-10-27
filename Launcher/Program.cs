using System.Diagnostics;
using System.Text.RegularExpressions;

static partial class Program
{
    [GeneratedRegex(@"P(lain)?\s*C(raft)?\s*L(auncher)?.*C(ommunity)?\s*E(dition)?", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline, "zh-CN")]
    static partial Regex ExeName { get; }
    static void Main(string[] args)
    {
        string? exeFile = null;
        if (args is [string exe, ..] && exe is not "")
            exeFile = exe;
        else
        {
            string dir = Environment.CurrentDirectory;
            foreach (string file in Directory.EnumerateFiles(dir, "*.exe"))
            {
                if (ExeName.IsMatch(Path.GetFileName(file.AsSpan())))
                {
                    exeFile = file;
                    break;
                }
            }
        }
        if (!File.Exists(exeFile))
        {
            File.AppendAllText("LauncherError.log", $"[{DateTime.Now:O}] 无法找到 PCL-CE 可执行文件：{exeFile ?? "<未指定>"}\n");
            return;
        }
        ProcessStartInfo psi = new(exeFile);
        for (int i = 1; i < args.Length; i++)
            psi.ArgumentList.Add(args[i]);
        psi.EnvironmentVariables["DOTNET_STARTUP_HOOKS"] = Path.GetFullPath(Path.Join(Environment.CurrentDirectory, "PCLCEPluginManager.dll"));
        Process.Start(psi);
    }
}