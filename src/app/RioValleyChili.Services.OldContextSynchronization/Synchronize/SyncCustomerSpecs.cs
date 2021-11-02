using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncCustomerSpecs)]
    public class SyncCustomerSpecs : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, SynchronizeCustomerProductSpecs>
    {
        public SyncCustomerSpecs(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SynchronizeCustomerProductSpecs> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var parameters = getInput();
            int prodID;
            string company_IA;

            if(parameters.Delete != null)
            {
                Delete(parameters.Delete, out prodID, out company_IA);
            }
            else if(parameters.ChileProductKey != null && parameters.CustomerKey != null)
            {
                Serialize(parameters.ChileProductKey, parameters.CustomerKey, out prodID, out company_IA);
            }
            else
            {
                return;
            }

            OldContext.SaveChanges();

            Console.WriteLine(ConsoleOutput.UpdatedCustomerSpecs, company_IA, prodID);
        }

        private void Delete(SerializedSpecKey specKey, out int prodId, out string companyIA)
        {
            prodId = specKey.ProdID;
            companyIA = specKey.Company_IA;

            var specs = OldContext.SerializedCustomerProdSpecs.FirstOrDefault(s => s.ProdID == specKey.ProdID && s.Company_IA == specKey.Company_IA);
            if(specs != null)
            {
                OldContext.SerializedCustomerProdSpecs.DeleteObject(specs);
            }
        }

        private void Serialize(ChileProductKey chileProductKey, CustomerKey customerKey, out int prodId, out string companyIA)
        {
            var chileProduct = UnitOfWork.ChileProductRepository.FindByKey(chileProductKey, c => c.Product);
            var customer = UnitOfWork.CustomerRepository.FindByKey(customerKey,
                c => c.Company,
                c => c.ProductSpecs);

            var pId = prodId = int.Parse(chileProduct.Product.ProductCode);
            var cIA = companyIA = customer.Company.Name;
            var existing = OldContext.SerializedCustomerProdSpecs.FirstOrDefault(s => s.ProdID == pId && s.Company_IA == cIA);

            var specs = customer.ProductSpecs.Where(s => s.ChileProductId == chileProduct.Id).ToList();
            if(!specs.Any())
            {
                if(existing != null)
                {
                    OldContext.SerializedCustomerProdSpecs.DeleteObject(existing);
                }
            }
            else
            {
                if(existing == null)
                {
                    existing = new SerializedCustomerProdSpecs
                        {
                            ProdID = prodId,
                            Company_IA = companyIA
                        };
                    OldContext.SerializedCustomerProdSpecs.AddObject(existing);
                }

                existing.Serialized = SerializableCustomerSpec.Serialize(specs);
            }
        }
    }
}