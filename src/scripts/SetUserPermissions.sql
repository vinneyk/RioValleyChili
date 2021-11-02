USE [RvcData]
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'pbrrvc@yahoo.com', [IsActive]=1, [Claims]=N'{"Inventory":"full", "LotHistory":"full", "LotInventory":"full","DehydratedMaterials":"full","MillAndWetdown":"full","Production":"full","ProductionResults":"full","InventoryAdjustments":"full", "InterWarehouseMovements":"full", "IntrawarehouseMovements":"full", "WarehouseLocations":"full", "TreatmentOrders":"full","superuser":"true"}' 
WHERE EmployeeID = 2
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'sharongrvc@gmail.com', [IsActive]=1, [Claims]=N'{"superuser":"true"}'
WHERE EmployeeId = 8
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'mikeswickrvc@gmail.com', [IsActive]=1, [Claims]=N'{"QualityControl":"full","LabResults":"full","QAHolds":"full", "superuser":"full"}'
WHERE EmployeeId = 19
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'vinneyk@solutionhead.com', [IsActive]=1, [Claims] =N'{"QualityControl":"full","LabResults":"full","QAHolds":"full"}'
WHERE EmployeeId = 11
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'chellecrvc@yahoo.com', [IsActive]=1, [Claims] =N'{"superuser":"true","Inventory":"full","DehydratedMaterials":"full","MillAndWetdown":"full","Production":"full","ProductionResults":"full", "Sales":"full", "CustomerContracts":"full", "SalesReports":"full"}'
WHERE EmployeeId = 15
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'stevyeleervc@yahoo.com', [IsActive]=1, [Claims] =N'{"Sales":"full", "CustomerContracts":"full", "SalesReports","full"}'
WHERE EmployeeId = 4
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'vinneyk@live.com', [IsActive]=1, [Claims] =N'{"DehydratedMaterials":"full","MillAndWetdown":"full","Inventory":"full","Production":"full","superuser":"true", "solutionhead":"true"}'
WHERE EmployeeId = 18
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'norma@solutionhead.com', [IsActive]=1, [Claims] =N'{"QualityControl":"full","LabResults":"full","QAHolds":"full"}'
WHERE EmployeeId = 17
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'', [IsActive]=1, [Claims] =N'{"QualityControl":"full","LabResults":"full","QAHolds":"full"}'
WHERE EmployeeId = 11
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'mramirezrvc@yahoo.com', [IsActive]=1, [Claims]=N'{"Production":"full", "PackSchedules":"full", "ProductionBatch":"full","Production":"full","ProductionResults":"full", "Inventory":"full", "TreatmentOrders":"full","superuser":"true"}'
WHERE EmployeeId = 5
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'melissa@riovalleychili.com', [IsActive]=1, [Claims]=N'{"superuser":"true"}' 
WHERE EmployeeID = 20
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'n712carson@yahoo.com', [IsActive]=1, [Claims]=N'{"Inventory":"full", "LotHistory":"full", "LotInventory":"full","DehydratedMaterials":"full","MillAndWetdown":"full","Production":"full","ProductionResults":"full","InventoryAdjustments":"full", "InterWarehouseMovements":"full", "IntrawarehouseMovements":"full", "WarehouseLocations":"full", "TreatmentOrders":"full","superuser":"true"}' 
WHERE EmployeeID = 21
GO

UPDATE [dbo].[Employees] SET [EmailAddress]=N'mochoa.rvc@gmail.com', [IsActive]=1, [Claims]=N'{"superuser":"true"}' 
WHERE EmployeeID = 22
GO

UPDATE [dbo].[Employees] SET [Claims]=N'{"superuser":"true"}'
WHERE EmployeeId = 17
GO
