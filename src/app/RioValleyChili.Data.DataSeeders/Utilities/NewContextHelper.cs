using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class NewContextHelper
    {
        private readonly RioValleyChiliDataContext _newContext;
        private readonly bool _noTracking;

        public NewContextHelper(RioValleyChiliDataContext newContext)
        {
            if(newContext == null) {  throw new ArgumentNullException("newContext"); }
            _newContext = newContext;
            _noTracking = false;
        }

        public Dictionary<string, Location> LocationsByDescription
        {
            get { return _locationsByDescriptions ?? (_locationsByDescriptions = GetSet<Location>().ToDictionary(p => p.Description, p => p)); }
        }
        private Dictionary<string, Location> _locationsByDescriptions;

        public Location GetProductionLine(int? productionLine)
        {
            if(productionLine == null)
            {
                return null;
            }
            
            Location productionLocation;
            LocationsByDescription.TryGetValue(LocationDescriptionHelper.GetDescription("Line", productionLine.Value), out productionLocation);
            return productionLocation;
        }

        public Location GetProductionLocation(ILocation location)
        {
            Location productionLocation;
            LocationsByDescription.TryGetValue(LocationDescriptionHelper.GetDescription(location.Street, location.Row ?? 0), out productionLocation);
            return productionLocation;
        }

        private Dictionary<string, ChileProduct> ChileProducts
        {
            get { return _chileProducts ?? (_chileProducts =
                GetSet<ChileProduct>()
                .Select(c => new
                    {
                        chileProduct = c,
                        product = c.Product,
                        attributeRanges = c.ProductAttributeRanges
                    }).ToDictionary(c => c.product.ProductCode, c => c.chileProduct)); }
        }
        private Dictionary<string, ChileProduct> _chileProducts;

        public ChileProduct GetChileProduct(int? productId)
        {
            if(productId == null)
            {
                return null;
            }

            ChileProduct chileProduct;
            ChileProducts.TryGetValue(productId.ToString(), out chileProduct);
            return chileProduct;
        }

        public Dictionary<string, AdditiveProduct> AdditiveProducts
        {
            get { return _additiveProducts ?? (_additiveProducts =
                GetSet<AdditiveProduct>()
                .Select(a => new { additiveProduct = a, product = a.Product }).ToDictionary(a => a.product.ProductCode, a => a.additiveProduct)); }
        }
        private Dictionary<string, AdditiveProduct> _additiveProducts;

        public AdditiveProduct GetAdditiveProduct(int? productId)
        {
            if(productId == null)
            {
                return null;
            }

            AdditiveProduct additiveProduct;
            AdditiveProducts.TryGetValue(productId.ToString(), out additiveProduct);
            return additiveProduct;
        }

        public Dictionary<string, PackagingProduct> PackagingProducts
        {
            get { return _packagingProducts ?? (_packagingProducts =
                GetSet<PackagingProduct>()
                .Select(p => new { packagingProduct = p, product = p.Product }).ToDictionary(p => p.product.ProductCode, p => p.packagingProduct)); }
        }
        private Dictionary<string, PackagingProduct> _packagingProducts;

        public PackagingProduct GetPackagingProduct(int? packageId)
        {
            if(packageId == null)
            {
                return null;
            }

            PackagingProduct packagingProduct;
            PackagingProducts.TryGetValue(packageId.ToString(), out packagingProduct);
            return packagingProduct;
        }

        public PackagingProduct GetPackagingProduct(string packageId)
        {
            if(packageId == null)
            {
                return null;
            }

            PackagingProduct packagingProduct;
            PackagingProducts.TryGetValue(packageId, out packagingProduct);
            return packagingProduct;
        }

        public PackagingProduct NoPackagingProduct
        {
            get { return _noPackagingProduct ?? (_noPackagingProduct = PackagingProducts.Select(p => p.Value).FirstOrDefault(p => p.Product.Name == "No Packaging")); }
        }
        private PackagingProduct _noPackagingProduct;

        public Dictionary<string, Product> NonInventoryProducts
        {
            get
            {
                return _nonInventoryProducts ?? (_nonInventoryProducts = GetSet<Product>()
                    .Where(p => p.ProductType == ProductTypeEnum.NonInventory)
                    .ToDictionary(p => p.ProductCode));
            }
        }
        private Dictionary<string, Product> _nonInventoryProducts;

        public Product GetNonInventoryProduct(int? prodId)
        {
            if(prodId == null)
            {
                return null;
            }

            Product product;
            return NonInventoryProducts.TryGetValue(prodId.ToString(), out product) ? product : null;
        }

        public ProductResult GetProduct(int? prodId)
        {
            var result = new ProductResult
                {
                    AdditiveProduct = GetAdditiveProduct(prodId),
                    ChileProduct = GetChileProduct(prodId),
                    PackagingProduct = GetPackagingProduct(prodId),
                    NonInventoryProduct = GetNonInventoryProduct(prodId)
                };
            return result.ProductKey == null ? null : result;
        }

        public Dictionary<string, AdditiveType> AdditiveTypes
        {
            get { return _additiveTypes ?? (_additiveTypes = GetSet<AdditiveType>().ToList().ToDictionary(a => (a.Description ?? "").Replace(" ", "").ToUpper(), a => a)); }
        }
        private Dictionary<string, AdditiveType> _additiveTypes;

        public AdditiveType GetAdditiveType(string ingrDesc)
        {
            ingrDesc = (ingrDesc ?? "").Replace(" ", "").ToUpper();

            AdditiveType additiveType;
            AdditiveTypes.TryGetValue(ingrDesc, out additiveType);
            return additiveType;
        }

        public Dictionary<int, Facility> Facilities
        {
            get
            {
                return _facilities ?? (_facilities =
                    GetSet<Facility>()
                    .Where(f => f.WHID != null)
                    .Select(f => new { facility = f, locations = f.Locations }).ToList().ToDictionary(w => w.facility.WHID.Value, w => w.facility));
            }
        }
        private Dictionary<int, Facility> _facilities;

        public Facility RinconFacility
        {
            get { return _rinconFacility ?? (_rinconFacility = Facilities.Values.First(f => f.Name.ToUpper().StartsWith("RINCON"))); }
        }
        private Facility _rinconFacility;

        public Facility GetFacility(int? WHID)
        {
            if(WHID == null)
            {
                return null;
            }

            Facility facility;
            Facilities.TryGetValue(WHID.Value, out facility);
            return facility;
        }

        public Location GetLocation(int? LocID)
        {
            Location location = null;
            if(LocID != null)
            {
                LocationsByLocID.TryGetValue(LocID.Value, out location);
            }
            return location;
        }

        public Dictionary<int, Location> LocationsByLocID
        {
            get { return _locations != null ? _locations : (_locations = _newContext.Locations.Where(l => l.LocID != null).ToDictionary(l => l.LocID.Value, l => l)); }
        }
        private Dictionary<int, Location> _locations;

        public Location UnknownFacilityLocation
        {
            get { return _unknownFacilityLocation ?? (_unknownFacilityLocation = GetSet<Location>().FirstOrDefault(l => l.Description.StartsWith("Unk"))); }
        }
        private Location _unknownFacilityLocation;

        public Dictionary<int, InventoryTreatment> InventoryTreatments
        {
            get { return _inventoryTreatments ?? (_inventoryTreatments = GetSet<InventoryTreatment>().ToDictionary(t => t.Id, t => t)); }
        }
        private Dictionary<int, InventoryTreatment> _inventoryTreatments;

        public InventoryTreatment GetInventoryTreatment(int treatmentId)
        {
            InventoryTreatment inventoryTreatment;
            InventoryTreatments.TryGetValue(treatmentId, out inventoryTreatment);
            return inventoryTreatment;
        }

        public InventoryTreatment NoTreatment
        {
            get { return _noTreatment ?? (_noTreatment = GetSet<InventoryTreatment>().FirstOrDefault(t => t.Id == StaticInventoryTreatments.NoTreatment.Id)); }
        }
        private InventoryTreatment _noTreatment;

        private HashSet<string> _lotKeys
        {
            get
            {
                if(_lotKeysField == null)
                {
                    _lotKeysField = new HashSet<string>();
                    var lotKeys = GetSet<Lot>().Select(l => new LotKeySelect
                        {
                            LotKey_LotTypeId = l.LotTypeId,
                            LotKey_DateCreated = l.LotDateCreated,
                            LotKey_DateSequence = l.LotDateSequence
                        });
                    foreach(var lot in lotKeys)
                    {
                        _lotKeysField.Add(new LotKey(lot));
                    }
                }
                return _lotKeysField;
            }
        }
        private HashSet<string> _lotKeysField;

        public bool LotLoaded(LotKey lotKey)
        {
            return _lotKeys.Contains(lotKey);
        }

        private Dictionary<LotKey, ChileLot> ChileLotsWithProducts
        {
            get
            {
                return _chileLotsWithProducts ?? (_chileLotsWithProducts =
                    GetSet<ChileLot>()
                    .Select(c => new
                    {
                        chileLot = c,
                        c.Lot,
                        c.ChileProduct.Product
                    }).ToDictionary(c => new LotKey(c.Lot), a => a.chileLot));
            }
        }
        private Dictionary<LotKey, ChileLot> _chileLotsWithProducts;

        public ChileLot GetChileLotWithProduct(LotKey lotKey)
        {
            ChileLot chileLot;
            ChileLotsWithProducts.TryGetValue(lotKey, out chileLot);
            return chileLot;
        }

        private Dictionary<LotKey, ChileLot> ChileLots
        {
            get
            {
                return _chileLots ?? (_chileLots = GetSet<ChileLot>(false).ToDictionary(c => new LotKey(c), c => c));
            }
        }
        private Dictionary<LotKey, ChileLot> _chileLots;

        public ChileLot GetChileLot(LotKey lotKey)
        {
            ChileLot chileLot;
            ChileLots.TryGetValue(lotKey, out chileLot);
            return chileLot;
        }

        public Dictionary<LotKey, AdditiveLot> AdditiveLotsWithProduct
        {
            get
            {
                return _additiveLotsWithProduct ?? (_additiveLotsWithProduct =
                    GetSet<AdditiveLot>()
                    .Select(a => new
                        {
                            additiveLot = a,
                            a.Lot,
                            a.AdditiveProduct.Product
                        })
                    .ToDictionary(a => new LotKey(a.Lot), a => a.additiveLot));
            }
        }
        private Dictionary<LotKey, AdditiveLot> _additiveLotsWithProduct;

        public AdditiveLot GetAdditiveLotWithProduct(LotKey lotKey)
        {
            AdditiveLot additiveLot;
            AdditiveLotsWithProduct.TryGetValue(lotKey, out additiveLot);
            return additiveLot;
        }

        private Dictionary<LotKey, NotebookKeySelect> _batchInstructionNotebookKeyByLotKey;
        public INotebookKey GetBatchInstructionNotebookKeyByLotKey(LotKey lotKey)
        {
            if(_batchInstructionNotebookKeyByLotKey == null)
            {
                _batchInstructionNotebookKeyByLotKey = _newContext.ProductionBatches.Select(b => new
                    {
                        LotKey = new LotKeySelect
                            {
                                LotKey_DateCreated = b.LotDateCreated,
                                LotKey_DateSequence = b.LotDateSequence,
                                LotKey_LotTypeId = b.LotTypeId
                            },
                        NotebookKey = new NotebookKeySelect
                            {
                                NotebookKey_Date = b.InstructionNotebookDateCreated,
                                NotebookKey_Sequence = b.InstructionNotebookSequence
                            }
                    }).ToDictionary(b => new LotKey(b.LotKey), b => b.NotebookKey);
            }

            NotebookKeySelect result;
            return _batchInstructionNotebookKeyByLotKey.TryGetValue(lotKey, out result) ? result : null;
        }

        public Dictionary<LotKey, PackagingLot> PackagingLotsWithProduct
        {
            get
            {
                return _packagingLotsWithProduct ?? (_packagingLotsWithProduct =
                    GetSet<PackagingLot>()
                    .Select(p => new
                        {
                            packagingLot = p,
                            p.Lot,
                            p.PackagingProduct.Product
                        })
                    .ToDictionary(a => new LotKey(a.Lot), a => a.packagingLot));
            }
        }
        private Dictionary<LotKey, PackagingLot> _packagingLotsWithProduct;

        public PackagingLot GetPackagingLotWithProduct(LotKey lotKey)
        {
            PackagingLot packagingLot;
            PackagingLotsWithProduct.TryGetValue(lotKey, out packagingLot);
            return packagingLot;
        }

        public ShipmentInformationKeys ShipmentInformationKeys { get { return _shipmentInformationKeys ?? (_shipmentInformationKeys = new ShipmentInformationKeys(_newContext)); } }
        private ShipmentInformationKeys _shipmentInformationKeys;

        public Dictionary<string, Models.Company> Companies
        {
            get { return _companies ?? (_companies =
                GetSet<Models.Company>()
                .Select(c => new { company = c, companyTypes = c.CompanyTypes }).ToList()
                .ToDictionary(c => c.company.Name.ToUpper(), c =>
                    {
                        c.company.CompanyTypes = c.company.CompanyTypes ?? new List<CompanyTypeRecord>();
                        return c.company;
                    })); }
        }
        private Dictionary<string, Models.Company> _companies;

        public Models.Company GetCompany(string companyName, params CompanyType[] companyTypes)
        {
            if(companyName == null)
            {
                return null;
            }

            var companies = (companyTypes.Any() ? Companies.Where(c => c.Value.CompanyTypes.Any(t => companyTypes.Contains(t.CompanyTypeEnum))) : Companies).ToList();

            companyName = companyName.ToUpper();

            var splitCompanyName = companyName.Split(' ').Select(s => s.ToUpper()).ToList();
            if(splitCompanyName.Any(n => n == "AMZAC"))
            {
                return companies.FirstOrDefault(n => n.Key.StartsWith("AMZAC")).Value;
            }

            if(splitCompanyName.Any(n => n == "ISOMEDIX"))
            {
                return companies.FirstOrDefault(n => n.Key.EndsWith("ISOMEDIX")).Value;
            }

            var company = companies.Select(c => c.Value).FirstOrDefault(c => c.Name.ToUpper() == companyName);
            if(company == null)
            {
                company = companies.Select(c => c.Value).FirstOrDefault(c => c.Name.ToUpper().Contains(companyName));
            }

            return company;
        }

        public Dictionary<DateTime, PackSchedule> PackSchedules
        {
            get { return _packSchedules ?? (_packSchedules = GetSet<PackSchedule>()
                    .Select(p => new
                        {
                            packSchedule = p,
                            packagingProduct = p.PackagingProduct
                        })
                    .ToDictionary(p => p.packSchedule.PackSchID, p => p.packSchedule)); }
        }
        private Dictionary<DateTime, PackSchedule> _packSchedules;

        public PackSchedule GetPackSchedule(DateTime? packSchID)
        {
            if(packSchID == null)
            {
                return null;
            }

            PackSchedule packSchedule;
            PackSchedules.TryGetValue(packSchID.Value, out packSchedule);
            return packSchedule;
        }

        public PackScheduleKey GetLotPackScheduleKey(ILotKey lotKey)
        {
            PackScheduleKey packScheduleKey;
            if(LotPackScheduleKeys.TryGetValue(new LotKey(lotKey), out packScheduleKey))
            {
                return packScheduleKey;
            }
            return null;
        }

        public Dictionary<LotKey, PackScheduleKey> LotPackScheduleKeys
        {
            get
            {
                if(_lotPackScheduleKeys == null)
                {
                    _lotPackScheduleKeys = _newContext.PackSchedules.SelectMany(p =>
                        p.ProductionBatches.Select(b => new
                            {
                                lotKey = new LotKeySelect
                                    {
                                        LotKey_DateCreated = b.LotDateCreated,
                                        LotKey_DateSequence = b.LotDateSequence,
                                        LotKey_LotTypeId = b.LotTypeId,
                                    },
                                packScheduleKey = new PackScheduleKeySelect
                                    {
                                        PackScheduleKey_DateCreated = p.DateCreated,
                                        PackScheduleKey_DateSequence = p.SequentialNumber
                                    }
                            }))
                        .ToDictionary(k => new LotKey(k.lotKey), k => new PackScheduleKey(k.packScheduleKey));
                }
                return _lotPackScheduleKeys;
            }
        }
        private Dictionary<LotKey, PackScheduleKey> _lotPackScheduleKeys;

        private const string DefaultEmployeeUserName = "DataLoadUser";
        public Employee DefaultEmployee
        {
            get
            {
                if(_defaultEmployee == null)
                {
                    _defaultEmployee = _newContext.Employees.FirstOrDefault(e => e.UserName == DefaultEmployeeUserName);
                    if(_defaultEmployee == null)
                    {
                        var nextId = (_newContext.Employees.Any() ? _newContext.Employees.Max(e => e.EmployeeId) : 0) + 1;
                        _defaultEmployee = _newContext.Employees.Add(new Employee
                            {
                                EmployeeId = nextId,
                                UserName = DefaultEmployeeUserName,
                                DisplayName = DefaultEmployeeUserName
                            });
                        _newContext.SaveChanges();
                    }
                }
                return _defaultEmployee;
            }
        }
        private Employee _defaultEmployee;

        public Dictionary<string, AttributeName> AttributesNames
        {
            get { return _attributesNames ?? (_attributesNames = GetSet<AttributeName>().ToDictionary(a => a.ShortName, a => a)); }
        }
        private Dictionary<string, AttributeName> _attributesNames;

        private IQueryable<TEntity> GetSet<TEntity>(bool? noTrackingOverride = null) where TEntity : class
        {
            var noTracking = noTrackingOverride != null ? noTrackingOverride.Value : _noTracking;
            return noTracking ? _newContext.Set<TEntity>().AsNoTracking() : _newContext.Set<TEntity>();
        }

        private Dictionary<DateTime, IContractItemKey> ContractItemKeys
        {
            get { return _contractItemKeys != null ? _contractItemKeys : (_contractItemKeys = _newContext.ContractItems
                .Where(i => i.KDetailID != null)
                .Select(i => new
                    {
                        Key = i.KDetailID.Value,
                        Value = new ContractItemKeySelect
                            {
                                ContractKey_Year = i.ContractYear,
                                ContractKey_Sequence = i.ContractSequence,
                                ContractItemKey_Sequence = i.ContractItemSequence
                            }
                    })
                .ToDictionary(i => i.Key, i => (IContractItemKey)i.Value)); }
        }
        private Dictionary<DateTime, IContractItemKey> _contractItemKeys;

        public IContractItemKey GetContractItemKey(DateTime? KDetailID)
        {
            if(KDetailID != null)
            {
                IContractItemKey key;
                if(ContractItemKeys.TryGetValue(KDetailID.Value, out key))
                {
                    return key;
                }
            }
            return null;
        }

        private Dictionary<int, IContractKey> ContractKeys
        {
            get
            {
                return _contractKeys ?? (_contractKeys = _newContext.Contracts
                    .Where(c => c.ContractId != null)
                    .Select(c => new
                        {
                            Key = c.ContractId.Value,
                            Value = new ContractKeySelect
                                {
                                    ContractKey_Year = c.ContractYear,
                                    ContractKey_Sequence = c.ContractSequence
                                }
                        })
                    .ToDictionary(s => s.Key, s => (IContractKey)s.Value));
            }
        }
        private Dictionary<int, IContractKey> _contractKeys;

        public IContractKey GetContractKey(int contractID)
        {
            IContractKey contractKey;
            return ContractKeys.TryGetValue(contractID, out contractKey) ? contractKey : null;
        }

        private SafeDictionary<DateTime, IntraWarehouseOrder> IntraWarehouseOrders
        {
            get { return _intraWarehouseOrders ?? (_intraWarehouseOrders = new SafeDictionary<DateTime, IntraWarehouseOrder>(_newContext.IntraWarehouseMovementOrders
                .Where(o => o.RinconID != null)
                .ToDictionary(o => o.RinconID.Value, o => o))); }
        }
        private SafeDictionary<DateTime, IntraWarehouseOrder> _intraWarehouseOrders;

        public IntraWarehouseOrder GetIntraWarehouseOrder(DateTime? rinconId)
        {
            return rinconId == null ? null : IntraWarehouseOrders[rinconId.Value];
        }

        private SafeDictionary<InventoryShipmentOrderTypeEnum, SafeDictionary<int, InventoryShipmentOrder>> InventoryShipmentOrders
        {
            get
            {
                if(_inventoryShipmentOrders == null)
                {
                    _inventoryShipmentOrders = new SafeDictionary<InventoryShipmentOrderTypeEnum, SafeDictionary<int, InventoryShipmentOrder>>(
                        _newContext.InventoryShipmentOrders
                        .Where(o => o.MoveNum != null)
                        .GroupBy(o => o.OrderType)
                        .ToDictionary(g => g.Key, g => new SafeDictionary<int, InventoryShipmentOrder>(g.ToDictionary(o => o.MoveNum.Value, o => o))));
                }
                return _inventoryShipmentOrders;
            }
        }
        private SafeDictionary<InventoryShipmentOrderTypeEnum, SafeDictionary<int, InventoryShipmentOrder>> _inventoryShipmentOrders;

        public InventoryShipmentOrder GetInventoryShipmentOrder(int? moveNum, params InventoryShipmentOrderTypeEnum[] orderTypes)
        {
            if(moveNum == null)
            {
                return null;
            }

            return orderTypes.Select(t => InventoryShipmentOrders[t])
                .Select(o => o == null ? null : o[moveNum.Value])
                .FirstOrDefault(o => o != null);
        }

        private SafeDictionary<DateTime, InventoryAdjustment> InventoryAdjustments
        {
            get
            {
                return _inventoryAdjustments ?? (_inventoryAdjustments = new SafeDictionary<DateTime, InventoryAdjustment>(_newContext.InventoryAdjustments
                    .ToDictionary(a => a.TimeStamp, a => a)));
            }
        }

        private SafeDictionary<DateTime, InventoryAdjustment> _inventoryAdjustments;

        public InventoryAdjustment GetInventoryAdjustment(DateTime? timestamp)
        {
            return timestamp == null ? null : InventoryAdjustments[timestamp.Value];
        }

        private class SafeDictionary<TKey, TValue>
        {
            public TValue this[TKey key]
            {
                get
                {
                    TValue result;
                    return _values == null ? default(TValue) : _values.TryGetValue(key, out result) ? result : default(TValue);
                }
            }
            private readonly IDictionary<TKey, TValue> _values;

            public SafeDictionary(IDictionary<TKey, TValue> values)
            {
                _values = values;
            }
        }

        public class ProductResult
        {
            public IProductKey ProductKey
            {
                get
                {
                    if(AdditiveProduct != null)
                    {
                        return AdditiveProduct;
                    }
                    if(ChileProduct != null)
                    {
                        return ChileProduct;
                    }
                    if(PackagingProduct != null)
                    {
                        return PackagingProduct;
                    }
                    if(NonInventoryProduct != null)
                    {
                        return NonInventoryProduct;
                    }
                    return null;
                }
            }

            public AdditiveProduct AdditiveProduct { get; set; }
            public ChileProduct ChileProduct { get; set; }
            public PackagingProduct PackagingProduct { get; set; }
            public Product NonInventoryProduct { get; set; }
        }

        private class ContractItemKeySelect : IContractItemKey
        {
            public int ContractKey_Year { get; set; }
            public int ContractKey_Sequence { get; set; }
            public int ContractItemKey_Sequence { get; set; }
        }

        private class ContractKeySelect : IContractKey 
        {
            public int ContractKey_Year { get; set; }
            public int ContractKey_Sequence { get; set; }
        }

        private class LotKeySelect : ILotKey
        {
            public DateTime LotKey_DateCreated { get; set; }
            public int LotKey_DateSequence { get; set; }
            public int LotKey_LotTypeId { get; set; }
        }

        private class PackScheduleKeySelect : IPackScheduleKey
        {
            public DateTime PackScheduleKey_DateCreated { get; set; }
            public int PackScheduleKey_DateSequence { get; set; }
        }

        private class NotebookKeySelect : INotebookKey
        {
            public DateTime NotebookKey_Date { get; set; }
            public int NotebookKey_Sequence { get; set; }
        }
    }
}