// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class FacilityProjectors
    {
        internal static Expression<Func<Facility, FacilityKeyReturn>> SelectFacilityKey()
        {
            return w => new FacilityKeyReturn
                {
                    FacilityKey_Id = w.Id
                };
        }

        internal static Expression<Func<Facility, FacilityReturn>> Select(bool includeDetails, bool includeShippingInfo)
        {
            var key = SelectFacilityKey();

            var projector = Projector<Facility>.To(f => new FacilityReturn
                {
                    FacilityKeyReturn = key.Invoke(f),
                    FacilityName = f.Name,
                    Active = f.Active
                });

            if(includeDetails)
            {
                var location = LocationProjectors.SelectLocation();
                projector = projector.Merge(f => new FacilityReturn
                    {
                        Locations = f.Locations.Select(l => location.Invoke(l)),
                        FacilityType = f.FacilityType
                    });
            }

            if(includeShippingInfo)
            {
                projector = projector.Merge(f => new FacilityReturn
                    {
                        ShippingLabel = new ShippingLabelReturn
                            {
                                Name = f.ShippingLabelName,
                                Phone = f.PhoneNumber,
                                EMail = f.EMailAddress,
                                AddressLine1 = f.Address.AddressLine1,
                                AddressLine2 = f.Address.AddressLine2,
                                AddressLine3 = f.Address.AddressLine3,
                                City = f.Address.City,
                                State = f.Address.State,
                                PostalCode = f.Address.PostalCode,
                                Country = f.Address.Country
                            }
                    });
            }

            return projector;
        }

        public static Expression<Func<Facility, InventoryCycleCountReturn>> SelectInventoryCycleCount(string groupName)
        {
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var lotProduct = LotProjectors.SelectDerivedProduct();
            var packaging = ProductProjectors.SelectPackagingProduct();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return Projector<Facility>.To(f => new InventoryCycleCountReturn
                {
                    FacilityName = f.Name,
                    GroupName = groupName,

                    LocationsSelect = f.Locations
                        .Where(l => l.Description.Contains(groupName))
                        .Select(l => new InventoryCycleCountLocationSelect
                            {
                                Location = l.Description,
                                InventorySelect = l.Inventory.Select(i => new InventoryCycleCountInventorySelect
                                    {
                                        LotKeyReturn = lotKey.Invoke(i.Lot),
                                        ProductionDate = new [] { i.Lot.ChileLot }.Where(c => c != null && c.Production != null && c.Production.Results != null)
                                            .Select(c => c.Production.Results.ProductionEnd).DefaultIfEmpty(i.LotDateCreated).FirstOrDefault(),
                                        ProductSelect = lotProduct.Invoke(i.Lot),
                                        PackagingSelect = packaging.Invoke(i.PackagingProduct),
                                        TreatmentSelect = treatment.Invoke(i.Treatment),
                                        Quantity = i.Quantity,
                                        Weight = i.Quantity * i.PackagingProduct.Weight
                                    }).Concat(l.PickedInventoryItems.Where(p => !p.PickedInventory.Archived).Select(p => new InventoryCycleCountInventorySelect
                                    {
                                        LotKeyReturn = lotKey.Invoke(p.Lot),
                                        ProductionDate = new[] { p.Lot.ChileLot }.Where(c => c != null && c.Production != null && c.Production.Results != null)
                                            .Select(c => c.Production.Results.ProductionEnd).DefaultIfEmpty(p.LotDateCreated).FirstOrDefault(),
                                        ProductSelect = lotProduct.Invoke(p.Lot),
                                        PackagingSelect = packaging.Invoke(p.PackagingProduct),
                                        TreatmentSelect = treatment.Invoke(p.Treatment),
                                        Quantity = p.Quantity,
                                        Weight = p.Quantity * p.PackagingProduct.Weight
                                    }))
                            })
                });
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup