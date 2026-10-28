try {
    Add-Type -Path 'g:\Decision System\DSS_App\bin\Release\net10.0-windows\DSS_App.dll'
    $f = New-Object DSS_App.Form1
    Write-Output "SUCCESS: Form1 instantiated without errors!"
} catch {
    Write-Output "ERROR: $($_.Exception.ToString())"
    if ($_.Exception.InnerException) {
        Write-Output "INNER: $($_.Exception.InnerException.ToString())"
    }
}
