param($installPath, $toolsPath, $package, $project)

if (-not ([System.Management.Automation.PSTypeName]'FileChangerV3').Type)
{
    write-host "Registering FileChanger..."

    $Source = Get-Content -Path "packages\WebApiFilters4Log.1.3.2\tools\WebApiFilters4Log.Install.v3.cs" | Out-String

    Add-Type -TypeDefinition $Source -Language CSharp
}
else
{
    write-host "FileChanger found."
}

$fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $project.FullName
$projectDirectory = $fileInfo.DirectoryName

write-host $projectDirectory

$fileChanger = New-Object FileChangerV3

$msg = $fileChanger.Install($projectDirectory)

write-host $msg
