/*
RioAccessSQL DataLoad.sql
	- Adds columns to RioAccessSQL database necessary for the data load program. - Roman Issa 2014/04/17	
	- Drops and recreates ViewInventoryLoad and ViewInventoryLoadSelected. - Roman Issa 2014/04/24
*/
USE RioAccessSQL

/* Add tblLot.Gluten */
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblLot]')
	AND name = 'Gluten'
)
BEGIN
	ALTER TABLE [dbo].[tblLot]
	ADD Gluten money
	PRINT 'Added Gluten column to tblLot.'
END
ELSE
	PRINT 'Gluten column already exists in tblLot.'

/* Add tblLotAttributeHistory.Gluten */
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblLotAttributeHistory]')
	AND name = 'Gluten'
)
BEGIN
	ALTER TABLE [dbo].[tblLotAttributeHistory]
	ADD Gluten money
	PRINT 'Added Gluten column to tblLotAttributeHistory.'
END
ELSE
	PRINT 'Gluten column already exists in tblLotAttributeHistory.'
	
/* Add tblLot.Serialized */
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblLot]')
	AND name = 'Serialized'
)
BEGIN
	ALTER TABLE [dbo].[tblLot]
	ADD Serialized nvarchar(max)
	PRINT 'Added Serialized column to tblLot.'
END
ELSE
	PRINT 'Serialized column already exists in tblLot.'

/* tblPackSch.Serialized */	
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblPackSch]')
	AND name = 'Serialized'
)
BEGIN
	ALTER TABLE [dbo].[tblPackSch]
	ADD Serialized nvarchar(max)
	PRINT 'Added Serialized column to tblPackSch.'
END
ELSE
	PRINT 'Serialized column already exists in tblPackSch.'
	
/* tblBatchInstr.Serialized */	
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblBatchInstr]')
	AND name = 'Serialized'
)
BEGIN
	ALTER TABLE [dbo].[tblBatchInstr]
	ADD Serialized nvarchar(max)
	PRINT 'Added Serialized column to tblBatchInstr.'
END
ELSE
	PRINT 'Serialized column already exists in tblBatchInstr.'