powershell -executionpolicy remotesigned -command "&{.\dataLoad.ps1 "^
 "-ServerName ".\RVCSQL01" "^
 "-DatabaseName "RioAccessSQL" "^
 "-DataLoadPath "D:\RVCDataLoad\DataInitializeProgram\RioValleyChili.Data.Initialize.exe" "^
 "-DataLoadSeedOption "*" "^
 "-DataLogFolder "D:\RVCDataLoad\DropBox\RVCDataLoad\Logs" "^
 "-NewDatabaseName "RvcData" "^
 "-NewDatabaseLogins "IIS_RvcInternalApp", "labeluser" "^
 "-NewDatabaseRoles "db_datareader", "db_datawriter" "^
 "-BackupDirectory "D:\RVCDataLoad\Backup" "^
 "-ZipProgram """C:\Program Files\7-zip\7z.exe""" "^
 "-BackupZipDirectory "D:\RVCDataLoad\DropBox\RVCDataLoad\CompressedBackups" }"