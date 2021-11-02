/***************************************************************
Add RioAccessSQL Columns.sql - Roman Issa 2014/04/30
	- Adds columns to RioAccessSQL database necessary for the data load program.	
	- Creates SerializedCustomerSpecs table - Roman Issa 2014/09/08
	- Adds tblContract.Serialized & tblContract.SerializedKey columns - Roman Issa 2014/11/17
	- Adds tblPackSch.SerializedKey column - Roman Issa 2014/11/18
	- Adds Serialized table - Roman Issa 2014/11/24
	- Adds tblOrder.SerializedKey column - Roman Issa 2014/12/2
	- Adds tblMove.Serialized column - Roman Issa 2015/02/09
	- Removes SerializedCustomerSpecs and adds SerializedCustomerProdSpecs tables. - Roman Issa 2016/09/07
	- Adds Company.Serialized column - Roman Issa 2016/10/04
***************************************************************/

/**************************************************************/
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

/**************************************************************/
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
	
/**************************************************************/
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

/**************************************************************/
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

/**************************************************************/	
/* tblPackSch.SerializedKey */	
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblPackSch]')
	AND name = 'SerializedKey'
)
BEGIN
	ALTER TABLE [dbo].[tblPackSch]
	ADD SerializedKey nvarchar(max)
	PRINT 'Added SerializedKey column to tblPackSch.'
END
ELSE
	PRINT 'Serialized column already exists in tblPackSch.'
	
/**************************************************************/
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

/**************************************************************/
/* Add tblContract.Serialized */
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblContract]')
	AND name = 'Serialized'
)
BEGIN
	ALTER TABLE [dbo].[tblContract]
	ADD Serialized nvarchar(max)
	PRINT 'Added Serialized column to tblContract.'
END
ELSE
	PRINT 'Serialized column already exists in tblContract.'

/**************************************************************/
/* Add tblContract.SerializedKey */
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblContract]')
	AND name = 'SerializedKey'
)
BEGIN
	ALTER TABLE [dbo].[tblContract]
	ADD SerializedKey nvarchar(max)
	PRINT 'Added SerializedKey column to tblContract.'
END
ELSE
	PRINT 'SerializedKey column already exists in tblContract.'
	
/**************************************************************/
/* SerializedCustomerSpecs */
IF NOT EXISTS
(
	SELECT * FROM sysobjects
	WHERE name = 'SerializedCustomerSpecs'	
	AND xtype = 'U'
)
	PRINT 'SerializedCustomerSpecs table already removed.'
ELSE
BEGIN
	DROP TABLE SerializedCustomerSpecs	
	PRINT 'Removed SerializedCustomerSpecs table.'
END
	
/**************************************************************/
/* SerializedCustomerProdSpecs */
IF NOT EXISTS
(
	SELECT * FROM sysobjects
	WHERE name = 'SerializedCustomerProdSpecs'	
	AND xtype = 'U'
)
BEGIN
	CREATE TABLE SerializedCustomerProdSpecs
	(
		ProdID INTEGER,
		Company_IA nvarchar(255),
		Serialized nvarchar(max)
		PRIMARY KEY(ProdID, Company_IA)
	)
	PRINT 'Created SerializedCustomerProdSpecs table.'
END	
ELSE
	PRINT 'SerializedCustomerProdSpecs table already exists.'

/**************************************************************/
/* Serialized */
IF NOT EXISTS
(
	SELECT * FROM sysobjects
	WHERE name = 'Serialized'
	AND xtype = 'U'
)
BEGIN
	CREATE TABLE Serialized
	(
		Type INTEGER,
		OldKey nvarchar(50),		
		Data nvarchar(max)
		PRIMARY KEY(Type, OldKey)
	)	
	PRINT 'Created Serialized table.'
END	
ELSE
	PRINT 'Serialized table already exists.'

/**************************************************************/
/* Add tblOrder.SerializedKey */
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblOrder]')
	AND name = 'SerializedKey'
)
BEGIN
	ALTER TABLE [dbo].[tblOrder]
	ADD SerializedKey nvarchar(max)
	PRINT 'Added SerializedKey column to tblOrder.'
END
ELSE
	PRINT 'SerializedKey column already exists in tblOrder.'

/**************************************************************/
/* Add tblMove.Serialized */
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[tblMove]')
	AND name = 'Serialized'
)
BEGIN
	ALTER TABLE [dbo].[tblMove]
	ADD Serialized nvarchar(max)
	PRINT 'Added Serialized column to tblMove.'
END
ELSE
	PRINT 'Serialized column already exists in tblMove.'

/**************************************************************/
/* Add Company.Serialized */
IF NOT EXISTS
(
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID('[dbo].[Company]')
	AND name = 'Serialized'
)
BEGIN
	ALTER TABLE [dbo].[Company]
	ADD Serialized nvarchar(max)
	PRINT 'Added Serialized column to Company.'
END
ELSE
	PRINT 'Serialized column already exists in Company.'