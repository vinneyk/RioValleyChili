namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChileProduct_IngredientsDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChileProducts", "IngredientsDescription", c => c.String());
            Sql(@"CREATE VIEW [dbo].[PrintLabelView]
AS
SELECT

    [Extent1].[LotDateSequence] + ((DATEPART(dy, [Extent1].[LotDateCreated])) * 100) + (((DATEPART(yy, [Extent1].[LotDateCreated])) % 100) * 100000) + ([Extent1].[LotTypeId] * 10000000) AS [Lot],
    FORMAT([Extent1].[LotDateSequence] + ((DATEPART(dy, [Extent1].[LotDateCreated])) * 100) + (((DATEPART(yy, [Extent1].[LotDateCreated])) % 100) * 100000) + ([Extent1].[LotTypeId] * 10000000), '00 00 000 00') AS [LotText],
    CASE WHEN ([Element1].[C1] IS NULL) THEN CAST(NULL AS varchar(1)) WHEN ([Element1].[C1] = 0) THEN [Element1].[ProductCode] WHEN ([Element1].[C1] = 1) THEN [Element1].[ProductCode1] ELSE [Element1].[ProductCode2] END AS [ProdID],
    CASE WHEN ([Element1].[C1] IS NULL) THEN CAST(NULL AS varchar(1)) WHEN ([Element1].[C1] = 0) THEN [Element1].[Name] WHEN ([Element1].[C1] = 1) THEN [Element1].[Name1] ELSE [Element1].[Name2] END AS [Product],
    CASE WHEN ([Element1].[C1] IS NULL) THEN CAST(NULL AS varchar(1)) WHEN ([Element1].[C1] = 0) THEN [Element1].[ProductCode] WHEN ([Element1].[C1] = 1) THEN [Element1].[ProductCode1] ELSE [Element1].[ProductCode2] END + '-' + CASE WHEN ([Element1].[C1] IS NULL) THEN CAST(NULL AS varchar(1)) WHEN ([Element1].[C1] = 0) THEN [Element1].[Name] WHEN ([Element1].[C1] = 1) THEN [Element1].[Name1] ELSE [Element1].[Name2] END AS [ProductText], 
    (DATEPART(dy, [Extent1].[LotDateCreated])) + (((DATEPART(yy, [Extent1].[LotDateCreated])) % 100) * 1000) AS [Julian], 
    CASE WHEN (0 = [Extent1].[LotTypeId]) THEN N'Raw' WHEN (1 = [Extent1].[LotTypeId]) THEN N'DeHy' WHEN (2 = [Extent1].[LotTypeId]) THEN N'WIP' WHEN (3 = [Extent1].[LotTypeId]) THEN N'FG' WHEN (4 = [Extent1].[LotTypeId]) THEN N'Ingr' WHEN (5 = [Extent1].[LotTypeId]) THEN N'Pkg' WHEN (12 = [Extent1].[LotTypeId]) THEN N'GRP' ELSE N'Other' END AS [PType], 
    [Extent15].[PSNum] AS [PSNum], 
    [Join16].[Name] AS [Company_IA], 
    [Extent19].[AllAttributesAreLoBac] AS [LoBac], 
    CAST(NULL AS varchar(1)) AS [Trtmt], 
    [Extent21].[IngredientsDescription] AS [IngrDesc], 
    [Extent1].[LotDateCreated] AS [ProductionDate]

    FROM [dbo].[Lots] AS [Extent1]
    OUTER APPLY (SELECT TOP (1) 
        [UnionAll2].[C1] AS [C1], 
        [Project5].[LotDateCreated] AS [LotDateCreated], 
        [Project5].[LotDateSequence] AS [LotDateSequence], 
        [Project5].[LotTypeId] AS [LotTypeId], 
        [Project5].[ChileProductId] AS [ChileProductId], 
        [Extent3].[Id] AS [Id], 
        [Join3].[Id1] AS [Id1], 
        [Join3].[Name] AS [Name], 
        [Join3].[ProductCode] AS [ProductCode], 
        [Join3].[Id2] AS [Id2], 
        [Project6].[LotDateCreated] AS [LotDateCreated1], 
        [Project6].[LotDateSequence] AS [LotDateSequence1], 
        [Project6].[LotTypeId] AS [LotTypeId1], 
        [Project6].[AdditiveProductId] AS [AdditiveProductId], 
        [Extent7].[Id] AS [Id3], 
        [Join7].[Id3] AS [Id4], 
        [Join7].[Name] AS [Name1], 
        [Join7].[ProductCode] AS [ProductCode1], 
        [Join7].[Id4] AS [Id5], 
        [Project7].[LotDateCreated] AS [LotDateCreated2], 
        [Project7].[LotDateSequence] AS [LotDateSequence2], 
        [Project7].[LotTypeId] AS [LotTypeId2], 
        [Project7].[PackagingProductId] AS [PackagingProductId], 
        [Extent11].[Id] AS [Id6], 
        [Join11].[Id5] AS [Id7], 
        [Join11].[Name] AS [Name2], 
        [Join11].[ProductCode] AS [ProductCode2], 
        [Join11].[Id6] AS [Id8]
        FROM (SELECT 
            [UnionAll1].[C1] AS [C1]
            FROM  (SELECT 
                0 AS [C1]
                FROM  ( SELECT 1 AS X ) AS [SingleRowTable1]
            UNION ALL
                SELECT 
                1 AS [C1]
                FROM  ( SELECT 1 AS X ) AS [SingleRowTable2]) AS [UnionAll1]
        UNION ALL
            SELECT 
            2 AS [C1]
            FROM  ( SELECT 1 AS X ) AS [SingleRowTable3]) AS [UnionAll2]
        LEFT OUTER JOIN  (SELECT 
            [Extent2].[LotDateCreated] AS [LotDateCreated], 
            [Extent2].[LotDateSequence] AS [LotDateSequence], 
            [Extent2].[LotTypeId] AS [LotTypeId], 
            [Extent2].[ChileProductId] AS [ChileProductId]
            FROM [dbo].[ChileLots] AS [Extent2]
            WHERE ([Extent1].[LotDateCreated] = [Extent2].[LotDateCreated]) AND ([Extent1].[LotDateSequence] = [Extent2].[LotDateSequence]) AND ([Extent1].[LotTypeId] = [Extent2].[LotTypeId]) ) AS [Project5] ON 1 = 1
        LEFT OUTER JOIN [dbo].[ChileProducts] AS [Extent3] ON [Project5].[ChileProductId] = [Extent3].[Id]
        LEFT OUTER JOIN  (SELECT [Extent4].[Id] AS [Id1], [Extent4].[Name] AS [Name], [Extent4].[ProductCode] AS [ProductCode], [Extent5].[Id] AS [Id2]
            FROM  [dbo].[Products] AS [Extent4]
            LEFT OUTER JOIN [dbo].[ChileProducts] AS [Extent5] ON [Extent4].[Id] = [Extent5].[Id] ) AS [Join3] ON [Extent3].[Id] = [Join3].[Id2]
        LEFT OUTER JOIN  (SELECT 
            [Extent6].[LotDateCreated] AS [LotDateCreated], 
            [Extent6].[LotDateSequence] AS [LotDateSequence], 
            [Extent6].[LotTypeId] AS [LotTypeId], 
            [Extent6].[AdditiveProductId] AS [AdditiveProductId]
            FROM [dbo].[AdditiveLots] AS [Extent6]
            WHERE ([Extent1].[LotDateCreated] = [Extent6].[LotDateCreated]) AND ([Extent1].[LotDateSequence] = [Extent6].[LotDateSequence]) AND ([Extent1].[LotTypeId] = [Extent6].[LotTypeId]) ) AS [Project6] ON 1 = 1
        LEFT OUTER JOIN [dbo].[AdditiveProducts] AS [Extent7] ON [Project6].[AdditiveProductId] = [Extent7].[Id]
        LEFT OUTER JOIN  (SELECT [Extent8].[Id] AS [Id3], [Extent8].[Name] AS [Name], [Extent8].[ProductCode] AS [ProductCode], [Extent9].[Id] AS [Id4]
            FROM  [dbo].[Products] AS [Extent8]
            LEFT OUTER JOIN [dbo].[AdditiveProducts] AS [Extent9] ON [Extent8].[Id] = [Extent9].[Id] ) AS [Join7] ON [Extent7].[Id] = [Join7].[Id4]
        LEFT OUTER JOIN  (SELECT 
            [Extent10].[LotDateCreated] AS [LotDateCreated], 
            [Extent10].[LotDateSequence] AS [LotDateSequence], 
            [Extent10].[LotTypeId] AS [LotTypeId], 
            [Extent10].[PackagingProductId] AS [PackagingProductId]
            FROM [dbo].[PackagingLots] AS [Extent10]
            WHERE ([Extent1].[LotDateCreated] = [Extent10].[LotDateCreated]) AND ([Extent1].[LotDateSequence] = [Extent10].[LotDateSequence]) AND ([Extent1].[LotTypeId] = [Extent10].[LotTypeId]) ) AS [Project7] ON 1 = 1
        LEFT OUTER JOIN [dbo].[PackagingProducts] AS [Extent11] ON [Project7].[PackagingProductId] = [Extent11].[Id]
        LEFT OUTER JOIN  (SELECT [Extent12].[Id] AS [Id5], [Extent12].[Name] AS [Name], [Extent12].[ProductCode] AS [ProductCode], [Extent13].[Id] AS [Id6]
            FROM  [dbo].[Products] AS [Extent12]
            LEFT OUTER JOIN [dbo].[PackagingProducts] AS [Extent13] ON [Extent12].[Id] = [Extent13].[Id] ) AS [Join11] ON [Extent11].[Id] = [Join11].[Id6]
        WHERE CASE WHEN ([UnionAll2].[C1] = 0) THEN [Join3].[Id1] WHEN ([UnionAll2].[C1] = 1) THEN [Join7].[Id3] ELSE [Join11].[Id5] END IS NOT NULL ) AS [Element1]
    LEFT OUTER JOIN [dbo].[ProductionBatches] AS [Extent14] ON ([Extent1].[LotDateSequence] = [Extent14].[LotDateSequence]) AND ([Extent1].[LotDateCreated] = [Extent14].[LotDateCreated]) AND ([Extent1].[LotTypeId] = [Extent14].[LotTypeId])
    LEFT OUTER JOIN [dbo].[PackSchedules] AS [Extent15] ON ([Extent14].[PackScheduleSequence] = [Extent15].[SequentialNumber]) AND ([Extent14].[PackScheduleDateCreated] = [Extent15].[DateCreated])
    LEFT OUTER JOIN [dbo].[Customers] AS [Extent16] ON [Extent15].[CustomerId] = [Extent16].[Id]
    LEFT OUTER JOIN  (SELECT [Extent17].[Name] AS [Name], [Extent18].[Id] AS [Id7]
        FROM  [dbo].[Companies] AS [Extent17]
        LEFT OUTER JOIN [dbo].[Customers] AS [Extent18] ON [Extent17].[Id] = [Extent18].[Id] ) AS [Join16] ON [Extent16].[Id] = [Join16].[Id7]
    LEFT OUTER JOIN [dbo].[ChileLots] AS [Extent19] ON ([Extent1].[LotTypeId] = [Extent19].[LotTypeId]) AND ([Extent1].[LotDateSequence] = [Extent19].[LotDateSequence]) AND ([Extent1].[LotDateCreated] = [Extent19].[LotDateCreated])
    LEFT OUTER JOIN [dbo].[ChileLots] AS [Extent20] ON ([Extent1].[LotTypeId] = [Extent20].[LotTypeId]) AND ([Extent1].[LotDateSequence] = [Extent20].[LotDateSequence]) AND ([Extent1].[LotDateCreated] = [Extent20].[LotDateCreated])
    LEFT OUTER JOIN [dbo].[ChileProducts] AS [Extent21] ON [Extent20].[ChileProductId] = [Extent21].[Id]

WHERE [Extent1].[LotDateCreated] > GETDATE() - 150");
        }
        
        public override void Down()
        {
            Sql(@"DROP VIEW [dbo].[PrintLabelView]");
            DropColumn("dbo.ChileProducts", "IngredientsDescription");
        }
    }
}
