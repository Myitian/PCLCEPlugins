# PCL-CE Plugins
在启动 [PCL 社区版](https://github.com/PCL-Community/PCL-CE)时一并加载自定义插件。
## 使用方法
0. 确保设备上已安装 [.NET 10 SDK](https://get.dot.net/10)。
1. 在项目根目录执行命令以编译子项目：
   ```
   dotnet publish -p:PublishProfile=win-x64
   ```
   或者可以使用 Visual Studio 等图形化 IDE 来进行编译和发布。
2. 将生成的以下文件复制到 PCL 社区版可执行文件所在目录：
   - `PCLCEPluginManager\bin\Publish\win-x64\PCLCEPluginManager.dll`
   - `Launcher\bin\Publish\win-x64\Launcher.exe`

   或者可以从 GitHub Action 下载已经编译好的版本。
3. 使用 `Launcher.exe` 启动 PCL 社区版。默认情况下，会自动搜寻目录下名字匹配类似 `PCL CE` / `Plain Craft Launcher Community Edition` 等模式的 EXE 文件。若不想使用 Launcher 启动，也可在启动 PCL 社区版时手动指定环境变量 `DOTNET_STARTUP_HOOKS` 为 `UnlockPCLCE.dll` 的完整路径。此时无需编译 Launcher 子项目。
4. **（如果第三步无法启动，且 `UnlockPCLCE.log` 日志中出现 `Cannot find the PCLCE executable`）**\
   修改 PCL 社区版可执行文件名。也可使用带参数启动：
   ```
   Launcher.exe <你的PCL社区版可执行文件名>
   ```
   可使用批处理文件或快捷方式来保存此类带参数启动方案。
5. 确认可以启动后，可以发现目录下出现了 `PCLCEPlugins.txt` 和 `PCLCEPlugins.log`。
   `PCLCEPlugins.txt` 是配置文件，允许设置日志级别、在启动时是否清空日志、要加载的插件列表。
   `PCLCEPlugins.log` 是日志文件。
6. 在 `PCLCEPlugins.txt` 中的 `> Plugins:` 下方添加要加载的插件 DLL 文件路径并保存。再次启动 Launcher，启动后插件应该已经加载完毕，可以查看 `PCLCEPlugins.log` 进行确认。
## 附加资源
### UnlockPCLCE
UnlockPCLCE 允许解锁 PCL 社区版的各种限制，仅用于演示插件的能力，请勿滥用。采用 [Lib.Harmony](https://github.com/pardeike/Harmony) 以实现启动时注入，无需修改 PCL 社区版可执行文件本身。