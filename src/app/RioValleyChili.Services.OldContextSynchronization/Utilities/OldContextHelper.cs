using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public class OldContextHelper
    {
        public readonly RioAccessSQLEntities OldContext;

        public OldContextHelper(RioAccessSQLEntities oldContext)
        {
            OldContext = oldContext;
        }

        public tblProduct GetProduct(string productCode)
        {
            tblProduct product;
            Products.TryGetValue(productCode, out product);
            return product;
        }

        public tblProduct GetProductFromPackagingId(string packagingId, out tblPackaging packaging)
        {
            if(!Packagings.TryGetValue(packagingId, out packaging))
            {
                return null;
            }

            var pkgId = packaging.PkgID;
            var products = Products.Values.Where(p => p.PkgID == pkgId).ToList();
            if(products.Count == 1)
            {
                return products.Single();
            }

            var description = packaging.Packaging.Replace(" ", "").ToUpper();
            return Products.Values.FirstOrDefault(p => p.Product != null && p.Product.Replace(" ", "").ToUpper() == description);
        }

        public tblProduct GetProduct(ChileProduct chileProduct)
        {
            tblProduct product;
            Products.TryGetValue(chileProduct.Product.ProductCode, out product);
            return product;
        }

        public tblPackaging GetPackaging(PackagingProduct packagingProduct)
        {
            tblPackaging packaging;
            Packagings.TryGetValue(packagingProduct.Product.ProductCode, out packaging);
            return packaging;
        }

        public tblLocation GetLocation(Location location)
        {
            tblLocation tblLocation = null;
            if(location.LocID != null)
            {
                Locations.TryGetValue(location.LocID.Value, out tblLocation);
            }
            return tblLocation;
        }

        public tblTreatment GetTreatment(IInventoryTreatmentKey inventoryTreatmentKey)
        {
            tblTreatment treatment;
            Treatments.TryGetValue(inventoryTreatmentKey.InventoryTreatmentKey_Id, out treatment);
            return treatment;
        }

        public int GetNextProductId(int pTypeId)
        {
            var products = Products.Values.Where(p => p.PTypeID == pTypeId).ToList();
            var nextId = products.Any() ? products.Max(p => p.ProdID) + 1 : 1;
            while(Products.Values.Any(p => p.ProdID == nextId))
            {
                nextId++;
            }
            return nextId;
        }

        public tblWarehouse GetWarehouse(string warehouseName)
        {
            tblWarehouse warehouse;
            Warehouses.TryGetValue(warehouseName, out warehouse);
            return warehouse;
        }

        private Dictionary<string, tblProduct> Products
        {
            get { return _products ?? (_products = OldContext.tblProducts.ToDictionary(p => p.ProdID.ToString(), p => p)); }
        }
        private Dictionary<string, tblProduct> _products;

        private Dictionary<string, tblPackaging> Packagings
        {
            get { return _packaging ?? (_packaging = OldContext.tblPackagings.ToDictionary(p => p.PkgID.ToString(), p => p)); }
        }
        private Dictionary<string, tblPackaging> _packaging;

        private Dictionary<int, tblLocation> Locations
        {
            get { return _locations ?? (_locations = OldContext.tblLocations.ToDictionary(l => l.LocID, l => l)); }
        }
        private Dictionary<int, tblLocation> _locations;

        private Dictionary<int, tblTreatment> Treatments
        {
            get { return _treatments ?? (_treatments = OldContext.tblTreatments.ToDictionary(t => t.TrtmtID, t => t)); }
        }
        private Dictionary<int, tblTreatment> _treatments;

        private Dictionary<string, tblVariety> Varieties
        {
            get { return _varieties ?? (_varieties = OldContext.tblVarieties.ToDictionary(v => v.Variety.ToString(), v => v)); }
        }
        private Dictionary<string, tblVariety> _varieties;

        private Dictionary<string, tblWarehouse> Warehouses
        {
            get { return _warehouses ?? (_warehouses = OldContext.tblWarehouses
                .GroupBy(w => w.WhouseAbbr)
                .ToDictionary(w => w.Key, w => w.First())); }
        }
        private Dictionary<string, tblWarehouse> _warehouses;

        private static readonly Regex ProductionLine = new Regex(@"[Ll]ine.*?(\d+?)", RegexOptions.Compiled);
        public int? GetProductionLine(Location productionLocation)
        {
            var match = ProductionLine.Match(productionLocation.Description);
            if(!match.Success)
            {
                return null;
            }

            return int.Parse(match.Groups[1].Value);
        }
    }
}