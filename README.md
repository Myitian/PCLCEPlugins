# UnlockPCLCE
移除 [PCL 社区版](https://github.com/PCL-Community/PCL-CE)的各种限制。采用 [Lib.Harmony](https://github.com/pardeike/Harmony) 以实现启动时注入，无需修改 PCL 社区版可执行文件本身。

## 使用方法

0. 确保设备上已安装 [.NET 10 SDK](https://get.dot.net/10)。
1. 在项目根目录执行：
   ```
   dotnet build -c Release UnlockPCLCE
   dotnet publish Launcher
   ```
2. 将以下文件复制到 PCL 社区版可执行文件所在目录：
   ```
   Launcher\bin\Release\net10.0\publish\Launcher.exe
   UnlockPCLCE\bin\Release\net10.0\UnlockPCLCE.dll
   ```
3. 使用 `Launcher.exe` 启动 PCL 社区版。默认情况下，会自动搜寻目录下名字匹配类似 `PCL CE` / `Plain Craft Launcher Community Edition` 等模式的文件。
4. **（如果第三步无法启动，且 `UnlockPCLCE.log` 日志中出现 `Cannot find the PCLCE executable`）**\
   修改 PCL 社区版可执行文件名。也可使用带参数启动：
   ```
   Launcher.exe <你的PCL社区版可执行文件名>
   ```
   可使用批处理文件或快捷方式来保存此类带参数启动方案。

若不想使用 Launcher 启动，也可在启动 PCL 社区版时手动指定环境变量 `DOTNET_STARTUP_HOOKS` 为 `UnlockPCLCE.dll` 的完整路径。