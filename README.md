# UnlockPCLCE
Remove restrictions in Plain Craft Launcher Community Edition.

移除 PCL 社区版的各种限制。

## Usage 使用方法

1. 在项目根目录执行：
   ```bat
   dotnet build -c Release UnlockPCLCE
   dotnet publish Launcher
   ```
2. 将以下文件复制到 PCL 社区版可执行文件所在目录：
   ```
   Launcher\bin\Release\net10.0\publish\Launcher.exe
   UnlockPCLCE\bin\Release\net10.0\UnlockPCLCE.dll
   ```
3. 使用 `Launcher.exe` 启动 PCL 社区版。默认情况下，会自动搜寻目录下名字匹配类似 `PCL CE` / `Plain Craft Launcher Community Edition` 等模式的文件名。
4. **（如果第三步无法启动，且 `UnlockPCLCE.log` 日志中出现 `Cannot find the PCLCE executable`）**\
   修改 PCL 社区版可执行文件名。也可使用带参数启动：
   ```bat
   Launcher.exe <你的PCL社区版可执行文件名>
   ```
   可使用批处理文件或快捷方式来保存此类带参数启动方案。