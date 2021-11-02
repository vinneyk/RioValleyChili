powershell -executionpolicy remotesigned -command "&{.\sql_backup.ps1 "^
 "-ServerName ".\RVCSQL01" "^
 "-AccessDatabaseName "RioAccessSQL" "^
 "-ProdDatabaseName "RvcData" "^
 "-BackupDirectory "D:\SQLBackup\Uncompressed" "^
 "-ZipProgram """C:\Program Files\7-zip\7z.exe""" "^
 "-BackupZipDirectory "D:\SQLBackup\Compressed\Dropbox" }"