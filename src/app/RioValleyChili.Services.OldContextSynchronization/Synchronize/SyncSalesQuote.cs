using System;
using System.Linq;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SalesQuote)]
    public class SyncSalesQuote : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, SyncSalesQuoteParameters>
    {
        public SyncSalesQuote(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SyncSalesQuoteParameters> getInput)
        {
            var parameters = getInput();
            var key = parameters.SalesQuoteKey;
            var salesQuote = UnitOfWork.SalesQuoteRepository.FindByKey(key,
                q => q.SourceFacility,
                q => q.Customer.Company,
                q => q.Broker,
                q => q.ShipmentInformation,
                q => q.Items.Select(i => i.Product),
                q => q.Items.Select(i => i.PackagingProduct.Product),
                q => q.Items.Select(i => i.Treatment));
            if(salesQuote == null)
            {
                throw new Exception(string.Format("Could not find SalesQuote[{0}].", key));
            }

            var commitNewContext = false;
            tblQuote tblQuote;
            if(salesQuote.QuoteNum == null || parameters.New)
            {
                var quoteNum = (salesQuote.TimeStamp.Year * 100) - 1;
                quoteNum = OldContext.tblQuotes.Select(q => q.QuoteNum)
                                     .Where(q => q > quoteNum)
                                     .DefaultIfEmpty(quoteNum)
                                     .Max() + 1;
                tblQuote = new tblQuote
                    {
                        QuoteNum = quoteNum,
                        TTypeID = 19,
                        Status = 1,
                        PreSample = false,
                        s_GUID = Guid.NewGuid(),
                        EmployeeID = salesQuote.EmployeeId,
                        EntryDate = salesQuote.TimeStamp.ConvertUTCToLocal().RoundMillisecondsForSQL()
                    };
                OldContext.tblQuotes.AddObject(tblQuote);

                salesQuote.QuoteNum = quoteNum;
                commitNewContext = true;
            }
            else
            {
                tblQuote = OldContext.tblQuotes
                                     .Include(q => q.tblQuoteDetails)
                                     .FirstOrDefault(q => q.QuoteNum == salesQuote.QuoteNum);
                if(tblQuote == null)
                {
                    throw new Exception(string.Format("Could not find tblQuote[{0}]", salesQuote.QuoteNum));
                }
            }

            tblQuote.Date = salesQuote.QuoteDate;
            tblQuote.DateRecd = salesQuote.DateReceived;
            tblQuote.From = salesQuote.CalledBy;
            tblQuote.TakenBy = salesQuote.TakenBy;
            tblQuote.WHID = salesQuote.SourceFacility == null ? null : salesQuote.SourceFacility.WHID;
            tblQuote.Company_IA = salesQuote.Customer == null ? null : salesQuote.Customer.Company.Name;
            tblQuote.Broker = salesQuote.Broker == null ? null : salesQuote.Broker.Name;

            tblQuote.PalletOR = (decimal?) salesQuote.ShipmentInformation.PalletWeight;
            tblQuote.PalletQty = salesQuote.ShipmentInformation.PalletQuantity;
            tblQuote.FreightBillType = salesQuote.ShipmentInformation.FreightBillType;
            tblQuote.ShipVia = salesQuote.ShipmentInformation.ShipmentMethod;
            tblQuote.Driver = salesQuote.ShipmentInformation.DriverName;
            tblQuote.Carrier = salesQuote.ShipmentInformation.CarrierName;
            tblQuote.TrlNbr = salesQuote.ShipmentInformation.TrailerLicenseNumber;
            tblQuote.ContSeal = salesQuote.ShipmentInformation.ContainerSeal;
            tblQuote.DelDueDate = salesQuote.ShipmentInformation.RequiredDeliveryDate.ConvertUTCToLocal().RoundMillisecondsForSQL();
            tblQuote.SchdShipDate = salesQuote.ShipmentInformation.ShipmentDate.ConvertUTCToLocal().RoundMillisecondsForSQL();

            tblQuote.CCompany = salesQuote.SoldTo.Name;
            tblQuote.CAddress1 = salesQuote.SoldTo.Address.AddressLine1;
            tblQuote.CAddress2 = salesQuote.SoldTo.Address.AddressLine2;
            tblQuote.CAddress3 = salesQuote.SoldTo.Address.AddressLine3;
            tblQuote.CCity = salesQuote.SoldTo.Address.City;
            tblQuote.CState = salesQuote.SoldTo.Address.State;
            tblQuote.CZip = salesQuote.SoldTo.Address.PostalCode;
            tblQuote.CCountry = salesQuote.SoldTo.Address.Country;

            tblQuote.SCompany = salesQuote.ShipmentInformation.ShipTo.Name;
            tblQuote.SPhone = salesQuote.ShipmentInformation.ShipTo.Phone;
            tblQuote.SAddress1 = salesQuote.ShipmentInformation.ShipTo.Address.AddressLine1;
            tblQuote.SAddress2 = salesQuote.ShipmentInformation.ShipTo.Address.AddressLine2;
            tblQuote.SAddress3 = salesQuote.ShipmentInformation.ShipTo.Address.AddressLine3;
            tblQuote.SCity = salesQuote.ShipmentInformation.ShipTo.Address.City;
            tblQuote.SState = salesQuote.ShipmentInformation.ShipTo.Address.State;
            tblQuote.SZip = salesQuote.ShipmentInformation.ShipTo.Address.PostalCode;
            tblQuote.SCountry = salesQuote.ShipmentInformation.ShipTo.Address.Country;

            tblQuote.Company = salesQuote.ShipmentInformation.FreightBill.Name;
            tblQuote.Phone = salesQuote.ShipmentInformation.FreightBill.Phone;
            tblQuote.Email = salesQuote.ShipmentInformation.FreightBill.EMail;
            tblQuote.Address1 = salesQuote.ShipmentInformation.FreightBill.Address.AddressLine1;
            tblQuote.Address2 = salesQuote.ShipmentInformation.FreightBill.Address.AddressLine2;
            tblQuote.Address3 = salesQuote.ShipmentInformation.FreightBill.Address.AddressLine3;
            tblQuote.City = salesQuote.ShipmentInformation.FreightBill.Address.City;
            tblQuote.State = salesQuote.ShipmentInformation.FreightBill.Address.State;
            tblQuote.Zip = salesQuote.ShipmentInformation.FreightBill.Address.PostalCode;
            tblQuote.Country = salesQuote.ShipmentInformation.FreightBill.Address.Country;

            var QDetail = OldContext.tblQuoteDetails.Select(d => d.QDetail).DefaultIfEmpty(DateTime.Now).Max();
            var detailsToRemove = tblQuote.tblQuoteDetails.ToDictionary(d => d.QDetail);
            foreach(var item in salesQuote.Items)
            {
                tblQuoteDetail detail;
                if(item.QDetailID != null)
                {
                    if(detailsToRemove.TryGetValue(item.QDetailID.Value, out detail))
                    {
                        detailsToRemove.Remove(item.QDetailID.Value);
                    }
                    else
                    {
                        throw new Exception(string.Format("Could not find tblQuoteDetail[{0}]", item.QDetailID));
                    }
                }
                else
                {
                    QDetail = QDetail.AddSeconds(1).RoundMillisecondsForSQL();
                    detail = new tblQuoteDetail
                        {
                            QDetail = QDetail,
                            QuoteNum = tblQuote.QuoteNum,
                            s_GUID = Guid.NewGuid()
                        };
                    OldContext.tblQuoteDetails.AddObject(detail);

                    item.QDetailID = detail.QDetail;
                    commitNewContext = true;
                }

                var product = OldContextHelper.GetProduct(item.Product.ProductCode);
                var packaging = OldContextHelper.GetPackaging(item.PackagingProduct);
                var treatment = OldContextHelper.GetTreatment(item.Treatment);

                detail.ProdID = product.ProdID;
                detail.PkgID = packaging.PkgID;
                detail.TrtmtID = treatment.TrtmtID;

                detail.Quantity = item.Quantity;
                detail.Price = (decimal?) item.PriceBase;
                detail.FreightP = (decimal?) item.PriceFreight;
                detail.TrtnmntP = (decimal?) item.PriceTreatment;
                detail.WHCostP = (decimal?) item.PriceWarehouse;
                detail.Rebate = (decimal?) item.PriceRebate;
                detail.CustProductCode = item.CustomerProductCode;
            }

            foreach(var detail in detailsToRemove.Values)
            {
                OldContext.tblQuoteDetails.DeleteObject(detail);
            }

            OldContext.SaveChanges();
            if(commitNewContext)
            {
                UnitOfWork.Commit();
            }

            Console.WriteLine(ConsoleOutput.SyncTblQuote, tblQuote.QuoteNum);
        }
    }
}