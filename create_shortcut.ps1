$wsh = New-Object -ComObject WScript.Shell
$desktop = [System.Environment]::GetFolderPath('Desktop')
$shortcut = $wsh.CreateShortcut("$desktop\DSS Retail Studio 2026.lnk")
$shortcut.TargetPath = "g:\Decision System\DSS_App\bin\Release\net10.0-windows\win-x64\publish\DSS_App.exe"
$shortcut.WorkingDirectory = "g:\Decision System\DSS_App\bin\Release\net10.0-windows\win-x64\publish"
$shortcut.Description = "He thong Ho tro Ra Quyet dinh Ban hang IS402"
$shortcut.Save()
Write-Output "Shortcut created successfully on Desktop!"
