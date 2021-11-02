SELECT tblLot.Lot AS WIPLot, tblLot_Dehy.Lot AS DehyLot, tblLot_Dehy.EntryDate AS DehyLotDate, tblProduct_Dehy.Product AS DehyProduct, tblLot_Dehy.Company_IA AS Dehydrator, tblIncoming_Dehy.DehyLocale AS Grower, tblVariety.Variety, 
             tblLotStatus.LotStat AS WIPLotStat, tblProduct.Product AS WIPProduct, tblLot.AvgAsta, tblLot.AoverB, tblLot.AvgScov, tblLot.H2O, tblLot.Scan, tblLot.Yeast, tblLot.Mold, tblLot.Coli, tblLot.TPC, tblLot.EColi, tblLot.Sal, tblLot.InsPrts, tblLot.RodHrs, tblLot.Ethox, tblLot.Lead, 
             tblLot.AToxin
FROM   tblLot 
	INNER JOIN tblProduct ON tblLot.ProdID = tblProduct.ProdID 
	LEFT OUTER JOIN tblLotStatus ON tblLot.LotStat = tblLotStatus.LotStatID 

	/* All lots which received the current lot (tblLot) as input */
	INNER JOIN tblOutgoing ON tblLot.Lot = tblOutgoing.NewLot 

	/* All input materials for the lots which received the current lot (tblLot) as input */
	INNER JOIN tblIncoming AS tblIncoming_Dehy ON tblOutgoing.Lot = tblIncoming_Dehy.Lot 

	/* The lot data for the input materials above */
	INNER JOIN tblLot AS tblLot_Dehy ON tblIncoming_Dehy.Lot = tblLot_Dehy.Lot 

	/* Product info for the input materials used in the creation of the current lot (tblLot) */
	LEFT OUTER JOIN tblProduct AS tblProduct_Dehy ON tblLot_Dehy.ProdID = tblProduct_Dehy.ProdID 

	/* Variety info for the input materials used in the creation of the current lot (tblLot) */
	LEFT OUTER JOIN tblVariety ON tblIncoming_Dehy.VarietyID = tblVariety.VarietyID
WHERE (tblIncoming_Dehy.TTypeID = 1)
GROUP BY tblLot.Lot, tblLot_Dehy.Lot, tblLot_Dehy.EntryDate, tblProduct_Dehy.Product, tblLot_Dehy.Company_IA, tblIncoming_Dehy.DehyLocale, tblVariety.Variety, tblLotStatus.LotStat, tblProduct.Product, tblLot.AvgAsta, tblLot.AoverB, tblLot.AvgScov, tblLot.H2O, tblLot.Scan, 
             tblLot.Yeast, tblLot.Mold, tblLot.Coli, tblLot.TPC, tblLot.EColi, tblLot.Sal, tblLot.InsPrts, tblLot.RodHrs, tblLot.Ethox, tblLot.Lead, tblLot.AToxin
HAVING (tblLot_Dehy.EntryDate >= CONVERT(DATETIME, '2015-10-01 00:00:00', 102))