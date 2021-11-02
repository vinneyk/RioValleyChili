# Modified to not create and restore back up of old context database - Roman Issa 2014/11/18
Param
(
    [string]$ServerName,
    [string]$DatabaseName,
    [string]$BackupDirectory,    
    [string]$DataLoadPath,
    [string]$DataLoadSeedOption,
    [string]$DataLogFolder,
    [string]$NewDatabaseName,
    [string[]]$NewDatabaseLogins,
    [string[]]$NewDatabaseRoles,
    [string]$ZipProgram,
    [string]$BackupZipDirectory
)

[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null

function Backup([Microsoft.SqlServer.Management.Smo.Server] $Server, [string] $DBName, [string] $BackupFilePath)
{
    try
    {
        $Database = $Server.Databases.Item($DBName)
        if(Test-Path $BackupFilePath)
        {
    	   Remove-Item $BackupFilePath
        }

        $BackupDevice = New-Object Microsoft.SqlServer.Management.Smo.BackupDeviceItem($BackupFilePath, [Microsoft.SqlServer.Management.Smo.DeviceType]::File)
        $Backup = New-Object Microsoft.SqlServer.Management.Smo.Backup -Property @{
            Action = [Microsoft.SqlServer.Management.Smo.BackupActionType]::Database
    		Database = $Database.Name
    		Incremental = $false
    		BackupSetDescription = "Full Backup of " + $DatabaseName
    	}
        $Backup.Devices.Add($BackupDevice)
        Write-Host ("Backing up " + $Server.Name + "\" + $DataBase.Name + " to " + $BackupFilePath)
        $Backup.SqlBackup($Server)
        Write-Host "Database backup completed successfully"
        $Backup.Devices.Remove($BackupDevice)
    }
    catch [Exception]
    {
	   "Database backup failed:`n`n " + $_.Exception
    }
}

function Restore([Microsoft.SqlServer.Management.Smo.Server] $Server, [string] $NewDBName, [string] $BackupFilePath, [bool] $IsNetworkPath = $true)
{
    try
    {                 
        if($Server.Databases[$NewDBName] -ne $null)
        {
            $Server.KillAllProcesses($NewDBName);            
            $Server.KillDatabase($NewDBName);
        }
        
        if($IsNetworkPath)
        {
            $FileName = [IO.Path]::GetFileName($BackupFilePath)
            $LocalPath = Join-Path -Path $Server.DefaultFile -ChildPath $FileName
            Copy-Item $BackupFilePath $LocalPath
            $BackupFilePath = $LocalPath
        }        
        
        $SMORestore = New-Object Microsoft.SqlServer.Management.Smo.Restore -Property @{
            Action = [Microsoft.SqlServer.Management.Smo.RestoreActionType]::Database
            Database = $NewDBName
            NoRecovery = $false
            ReplaceDatabase = $true                
        }
        
        $BackupDevice = New-Object Microsoft.SqlServer.Management.Smo.BackupDeviceItem($BackupFilePath, [Microsoft.SqlServer.Management.Smo.DeviceType]::File)
        $SMORestore.Devices.Add($BackupDevice)
        
        $SMORestoreDataFile = New-Object Microsoft.SqlServer.Management.Smo.RelocateFile
        $DefaultData = $Server.DefaultFile
        if(($DefaultData -eq $null) -or ($DefaultData -eq ""))
        {
            $DefaultData = $Server.MasterDBPath
        }
        $SMORestoreDataFile.PhysicalFileName = Join-Path -Path $DefaultData -ChildPath ($NewDBName + "_Data.mdf")
        
        $SMORestoreLogFile = New-Object Microsoft.SqlServer.Management.Smo.RelocateFile
        $DefaultLog = $Server.DefaultLog
        if(($DefaultLog -eq $null) -or ($DefaultLog -eq ""))
        {
            $DefaultLog = $Server.MasterDBLogPath
        }
        $SMORestoreLogFile.PhysicalFileName = Join-Path -Path $DefaultLog -ChildPath ($NewDBName + "_Log.ldf")        
        
        $DBFileList = $SMORestore.ReadFileList($Server)        
        $SMORestoreDataFile.LogicalFileName = $DBFileList.Select("Type = 'D'")[0].LogicalName
        $SMORestoreLogFile.LogicalFileName = $DBFileList.Select("Type = 'L'")[0].LogicalName
        $SMORestore.RelocateFiles.Add($SMORestoreDataFile) | Out-Null
        $SMORestore.RelocateFiles.Add($SMORestoreLogFile) | Out-Null

        Write-Host ("Restoring " + $Server.name + "\" + $NewDBName + " from " + $BackupFilePath)
        $SMORestore.SqlRestore($Server)
        Write-Host "Database restore completed successfully"
    }
    catch [Exception]
    {
	   "Database restore failed:`n`n " + $_.Exception
    }
    finally
    {        
        if($IsNetworkPath)
        {
            Remove-Item $BackupFilePath
        }
    }
}

function SetRoles([Microsoft.SqlServer.Management.Smo.Server] $Server, [string] $DBName, [string[]] $Logins, [string[]] $Roles)
{    
    $Database = $Server.Databases[$DBName]
    if($Database -eq $null)
    {
        Write-Host ("Database " + $DBName + " not found on server " + $Server.Name)
        return
    }
    
    $DatabaseRoles = @()
    foreach($RoleName in $Roles)
    {
        $DatabaseRole = $Database.Roles[$RoleName]
        if($DatabaseRole -eq $null)
        {
            Write-Host ("Role " + $RoleName + " not found in database " + $DBName)
        }
        else
        {
            $DatabaseRoles += $DatabaseRole
        }
    }    
    
    foreach($Login in $Logins)
    {
        $ServerLogin = $Server.Logins[$Login]
        if($ServerLogin -eq $null)
        {
            Write-Host ("Login " + $Login + " not found for server " + $Server.Name)
            continue
        }
        
        $MappedUser = $ServerLogin.EnumDatabaseMappings() | where {$_.DBName -eq $DBName} | select -first 1
        $UserName
        if($MappedUser -eq $null)
        {
            $NewUser = New-Object Microsoft.SqlServer.Management.Smo.User($Database, $Login.Replace("\", "-"))
            $NewUser.Login = $Login
            $NewUser.Create()
            
            $UserName = $NewUser.Name
            
            Write-Host ("Created new user " + $UserName + " for database " + $DBName)
        }
        else
        {
            $UserName = $MappedUser.UserName
            Write-Host ("Found existing user " + $UserName + " for database " + $DBName)
        }
        

        foreach($DatabaseRole in $DatabaseRoles)
        {
            if(-not $DatabaseRole.EnumMembers().Contains($UserName))
            {
		        if($DatabaseRole.Name -eq "db_datawriter" -and $UserName -eq "labeluser") {
                    Write-Host ("Not assigning role " + $DatabaseRole.Name + " to user " + $UserName)
                } else {
                    $DatabaseRole.AddMember($UserName)
                    $DatabaseRole.Alter()
                    Write-Host ("Added role " + $DatabaseRole.Name + " to user " + $UserName)
                }
            }
            else
            {
                Write-Host ("Role " + $DatabaseRole.Name + " already assigned to user " + $UserName)
            }
        }
    }
}

function KillDatabaseProcesses([Microsoft.SqlServer.Management.Smo.Server] $Server, [string] $DBName)
{
    if($Server.Databases[$DBName] -ne $null)
    {
        $Server.KillAllProcesses($DBName);        
    }
}

function CompressFile([string] $source, [string] $destination)
{
    Write-Host ("Compressing " + $source + " to " + $destination)
    if(Test-Path $destination)
    {
        Remove-Item $destination
    }
    [string]$pathToZipExe = $ZipProgram;
    [Array]$arguments = "a", "-t7z", "$destination", "$source", "-r";
    & $pathToZipExe $arguments;
}

$SQLServer = New-Object Microsoft.SqlServer.Management.Smo.Server($ServerName)

KillDatabaseProcesses $SqlServer $NewDatabaseName | Out-Null
Write-Host "`nRunning Data Load"
&$DataLoadPath $DataLoadSeedOption ("-LogFolder " + $DataLogFolder)
if($LASTEXITCODE -ge 0)
{
    Write-Host ("`nAssigning " + $NewDatabaseName + " roles.")
    SetRoles $SQLServer $NewDatabaseName $NewDatabaseLogins $NewDatabaseRoles | Out-Null

    Write-Host ""
    $OldBackupFile = $BackupDirectory + "\" + $DatabaseName + ".bak"
    Backup $SQLServer $DatabaseName $OldBackupFile | Out-Null

    $NewBackupFile = $BackupDirectory + "\" + $NewDatabaseName + ".bak"
    Backup $SQLServer $NewDatabaseName $NewBackupFile | Out-Null
    Write-Host ""

    CompressFile $OldBackupFile ($BackupZipDirectory + "\" + $DatabaseName + ".7z")
    CompressFile $NewBackupFile ($BackupZipDirectory + "\" + $NewDatabaseName + ".7z")
}