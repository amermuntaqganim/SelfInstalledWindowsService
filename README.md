## Visual Studio 2022
## .Net Framework 4.7.2 (Windows Service)

Normally we have to install and uninstall windows service using InstallUtil.exe.
</br>We can find InstallUtil.exe in directory C:\Windows\Microsoft.NET\Framework or Framework64 folder for x86 and x64.
</br>Using this exe we have to set command.
</br>For installing .\InstallUtil.exe .\myservice.exe
</br>For uninstalling .\InstallUtil.exe /u .\myservice.exe

**This example is for not using InstallUtil.exe**


# Test Method
- Build the project
- Open powershell with Admin mode from bin folder where exe exists
- Put command for installing the service
- .\SelfInstalledWindowsService.exe -i
- Find "SelfInstalledService" in Services app
- Put command for uninstalling the service
- .\SelfInstalledWindowsService.exe -u
- It will remove "SelfInstalledService" from Services app
