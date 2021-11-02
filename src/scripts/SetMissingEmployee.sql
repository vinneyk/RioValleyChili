/*
Script to set missing employee references for dehydrated, mill and wetdown, and
inventory adjustment records created since 2014.

2014/02/03 - Created script, Roman Issa
*/

/* Set dehydrated records of this year to have been created by Paula */
update [RioAccessSQL].[dbo].[tblLot]
set EmployeeID = 2
where PTypeID = 1 and EntryDate >= '20140101' and EmployeeID is null

update [RioAccessSQL].[dbo].[tblIncoming]
set EmployeeID = 2
where TTypeID = 1 and EntryDate >= '20140101' and EmployeeID is null
  
/* Set mill and wetdown records of this year to have been created by Chelle */
update [RioAccessSQL].[dbo].[tblLot]
set EmployeeID = 15
where PTypeID = 2 and EntryDate >= '20140101' and EmployeeID is null
  
update [RioAccessSQL].[dbo].[tblIncoming]
set EmployeeID = 15
where TTypeID = 2 and EntryDate >= '20140101' and EmployeeID is null
  
/* Set adjustment records of this year to have been created by Paula */
update [RioAccessSQL].[dbo].[tblAdjust]
set EmployeeID = 2
where AdjustID >= '20140101' and EmployeeID is null

update [RioAccessSQL].[dbo].[tblOutgoing]
set EmployeeID = 2
where TTypeID = 4 and EntryDate >= '20140101' and EmployeeID is null