/*
Script to set tblStagedFG.CustLot and CustProductCode columns to values in associated tblOrderDetail.

2016/08/30 - Created script. - Roman Issa
*/
UPDATE       tblStagedFG
SET                CustLot = tblOrderDetail.CustLot, CustProductCode = tblOrderDetail.CustProductCode
FROM            tblOrderDetail INNER JOIN
                         tblStagedFG ON tblStagedFG.ODetail = tblOrderDetail.ODetail
