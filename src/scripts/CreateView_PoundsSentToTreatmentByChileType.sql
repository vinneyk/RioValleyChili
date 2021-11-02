USE [RvcData]
GO

/****** Object:  View [dbo].[PoundsSentToTreatmentByChileType]    Script Date: 1/15/2015 2:16:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[PoundsSentToTreatmentByChileType]
AS
SELECT dbo.InventoryTreatments.ShortName, dbo.ChileTypes.Description AS ChileType, dbo.ChileLots.LotTypeId AS LotType, SUM(dbo.PackagingProducts.Weight * dbo.PickedInventoryItems.Quantity) AS TotalWeight
FROM   dbo.PickedInventory INNER JOIN
             dbo.InventoryTreatments INNER JOIN
             dbo.TreatmentOrders ON dbo.InventoryTreatments.Id = dbo.TreatmentOrders.InventoryTreatmentId INNER JOIN
             dbo.InventoryPickOrders ON dbo.TreatmentOrders.DateCreated = dbo.InventoryPickOrders.DateCreated AND dbo.TreatmentOrders.Sequence = dbo.InventoryPickOrders.Sequence ON dbo.PickedInventory.DateCreated = dbo.InventoryPickOrders.DateCreated AND 
             dbo.PickedInventory.Sequence = dbo.InventoryPickOrders.Sequence INNER JOIN
             dbo.PickedInventoryItems ON dbo.PickedInventory.DateCreated = dbo.PickedInventoryItems.DateCreated AND dbo.PickedInventory.Sequence = dbo.PickedInventoryItems.Sequence INNER JOIN
             dbo.ChileLots INNER JOIN
             dbo.ChileProducts ON dbo.ChileLots.ChileProductId = dbo.ChileProducts.Id INNER JOIN
             dbo.ChileTypes ON dbo.ChileProducts.ChileTypeId = dbo.ChileTypes.Id ON dbo.PickedInventoryItems.LotDateCreated = dbo.ChileLots.LotDateCreated AND dbo.PickedInventoryItems.LotDateSequence = dbo.ChileLots.LotDateSequence AND 
             dbo.PickedInventoryItems.LotTypeId = dbo.ChileLots.LotTypeId INNER JOIN
             dbo.ShipmentInformation ON dbo.TreatmentOrders.ShipmentInfoDateCreated = dbo.ShipmentInformation.DateCreated AND dbo.TreatmentOrders.ShipmentInfoSequence = dbo.ShipmentInformation.Sequence INNER JOIN
             dbo.PackagingProducts ON dbo.PickedInventoryItems.PackagingProductId = dbo.PackagingProducts.Id
WHERE (dbo.TreatmentOrders.DateCreated >= CONVERT(DATETIME, '2012-01-01 00:00:00', 102)) AND (dbo.TreatmentOrders.DateCreated <= CONVERT(DATETIME, '2012-12-31 00:00:00', 102))
GROUP BY dbo.InventoryTreatments.ShortName, dbo.ChileTypes.Description, dbo.ChileLots.LotTypeId
HAVING (dbo.InventoryTreatments.ShortName = N'ET')

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "InventoryTreatments"
            Begin Extent = 
               Top = 269
               Left = 22
               Bottom = 473
               Right = 234
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TreatmentOrders"
            Begin Extent = 
               Top = 2
               Left = 55
               Bottom = 199
               Right = 385
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryPickOrders"
            Begin Extent = 
               Top = 50
               Left = 426
               Bottom = 217
               Right = 746
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ChileLots"
            Begin Extent = 
               Top = 77
               Left = 1426
               Bottom = 274
               Right = 1701
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "ChileTypes"
            Begin Extent = 
               Top = 268
               Left = 2058
               Bottom = 411
               Right = 2280
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ChileProducts"
            Begin Extent = 
               Top = 153
               Left = 1753
               Bottom = 323
               Right = 2028
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PickedInventory"
            Begin Extent = 
               Top = 35
               Left = 797
               Bottom = 232
               Righ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PoundsSentToTreatmentByChileType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N't = 1019
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PickedInventoryItems"
            Begin Extent = 
               Top = 22
               Left = 1068
               Bottom = 467
               Right = 1376
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "ShipmentInformation"
            Begin Extent = 
               Top = 247
               Left = 339
               Bottom = 444
               Right = 705
            End
            DisplayFlags = 280
            TopColumn = 38
         End
         Begin Table = "PackagingProducts"
            Begin Extent = 
               Top = 309
               Left = 1501
               Bottom = 452
               Right = 1723
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1580
         Width = 1300
         Width = 2310
         Width = 1420
         Width = 2460
         Width = 1240
         Width = 1000
         Width = 1000
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PoundsSentToTreatmentByChileType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PoundsSentToTreatmentByChileType'
GO


