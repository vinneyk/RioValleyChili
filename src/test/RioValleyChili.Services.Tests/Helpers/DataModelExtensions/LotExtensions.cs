using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotExtensions
    {
        internal static Lot SetLotKey(this Lot lot, ILotKey lotKey)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }

            if(lotKey != null)
            {
                lot.LotDateCreated = lotKey.LotKey_DateCreated;
                lot.LotDateSequence = lotKey.LotKey_DateSequence;
                lot.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            return lot;
        }

        internal static Lot SetLotKey(this Lot lot, LotTypeEnum lotType, DateTime dateCreated, int sequence)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }

            lot.LotTypeEnum = lotType;
            lot.LotDateCreated = dateCreated.Date;
            lot.LotDateSequence = sequence;

            return lot;
        }

        internal static Lot EmptyLot(this Lot lot)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }

            lot.Inventory = null;
            lot.Attributes = null;
            lot.LotDefects = null;
            lot.Hold = null;
            lot.HoldDescription = null;

            return lot;
        }

        internal static Lot NullDerivedLots(this Lot lot)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }
            lot.ChileLot = null;
            lot.AdditiveLot = null;
            lot.PackagingLot = null;
            return lot;
        }

        internal static Lot SetProductSpec(this Lot lot, bool complete, bool outOfRange)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }

            lot.ProductSpecComplete = complete;
            lot.ProductSpecOutOfRange = outOfRange;
            
            return lot;
        }

        internal static Lot SetValidToPick(this Lot lot)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }
            
            lot.ProductionStatus = LotProductionStatus.Produced;
            lot.QualityStatus = LotQualityStatus.Released;
            lot.Hold = null;
            lot.HoldDescription = null;

            return lot;
        }

        internal static Lot SetChileLot(this Lot lot)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }

            lot.LotTypeEnum = LotTypeTestHelper.GetRandomChileLotType();
            if(lot.ChileLot != null)
            {
                lot.ChileLot.LotTypeEnum = lot.LotTypeEnum;
            }

            lot.AdditiveLot = null;
            lot.PackagingLot = null;
            return lot;
        }

        internal static Lot SetAdditiveLot(this Lot lot)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }

            lot.LotTypeEnum = LotTypeEnum.Additive;
            if(lot.AdditiveLot != null)
            {
                lot.AdditiveLot.LotTypeEnum = lot.LotTypeEnum;
            }

            lot.ChileLot = null;
            lot.PackagingLot = null;
            return lot;
        }

        internal static Lot SetPackagingLot(this Lot lot)
        {
            if(lot == null) { throw new ArgumentNullException("lot"); }

            lot.LotTypeEnum = LotTypeEnum.Packaging;
            if(lot.PackagingLot != null)
            {
                lot.PackagingLot.LotTypeEnum = lot.LotTypeEnum;
            }

            lot.ChileLot = null;
            lot.AdditiveLot = null;
            return lot;
        }

        internal static Lot AddCustomerAllowance(this Lot lot, ICustomerKey customer)
        {
            if(lot.CustomerAllowances == null)
            {
                lot.CustomerAllowances = new List<LotCustomerAllowance>();
            }

            lot.CustomerAllowances.Add(new LotCustomerAllowance
                {
                    LotTypeId = lot.LotTypeId,
                    LotDateCreated = lot.LotDateCreated,
                    LotDateSequence = lot.LotDateSequence,
                    CustomerId = customer.CustomerKey_Id
                });

            return lot;
        }

        internal static Lot AddOrderAllowance(this Lot lot, ISalesOrderKey order)
        {
            if(lot.SalesOrderAllowances == null)
            {
                lot.SalesOrderAllowances = new List<LotSalesOrderAllowance>();
            }

            lot.SalesOrderAllowances.Add(new LotSalesOrderAllowance
                {
                    LotTypeId = lot.LotTypeId,
                    LotDateCreated = lot.LotDateCreated,
                    LotDateSequence = lot.LotDateSequence,
                    SalesOrderDateCreated = order.SalesOrderKey_DateCreated,
                    SalesOrderSequence = order.SalesOrderKey_Sequence
                });

            return lot;
        }

        internal static Lot AddContractAllowance(this Lot lot, IContractKey contract)
        {
            if(lot.ContractAllowances == null)
            {
                lot.ContractAllowances = new List<LotContractAllowance>();
            }

            lot.ContractAllowances.Add(new LotContractAllowance
                {
                    LotTypeId = lot.LotTypeId,
                    LotDateCreated = lot.LotDateCreated,
                    LotDateSequence = lot.LotDateSequence,
                    ContractYear = contract.ContractKey_Year,
                    ContractSequence = contract.ContractKey_Sequence
                });

            return lot;
        }
    }
}