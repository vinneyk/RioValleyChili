using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Client.Reporting
{
    internal class AutoMapperConfiguration
    {
        internal static void Configure()
        {
            SetupLabResultsReport();
            SetupProductionBatchPacketReport();
            SetupPackSchedulePickSheetReport();
            SetupCustomerContractMapping();
            SetupWarehouseOrderAcknowledgingReport();
            SetupWarehouseOrderPackingListReport();
            SetupWarehouseOrderBillOfLadingReport();
            SetupPickSheet();
            SetupCertificateOfAnalysis();
            SetupProductionRecapReport();
            SetupPendingOrderDetails();
            SetupProductionAdditiveInputs();
            SetupCustomerOrderInvoice();
            SetupSampleMatchingSummary();
            SetupSampleRequest();
            SetupChileMaterialsReceivedRecap();
            SetupProductionSchedule();
            SetupInventoryCycleCount();
            SetupSalesQuote();

            Mapper.AssertConfigurationIsValid();
        }

        private static void SetupCustomerContractMapping()
        {
            Mapper.CreateMap<ICustomerContractDetailReturn, CustomerContract>()
                .ForMember(m => m.CustomerKey, opt => opt.MapFrom(m => m.Customer.CompanyKey))
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(m => m.Customer.Name))
                .ForMember(m => m.BrokerCompanyKey, opt => opt.MapFrom(m => m.Broker.CompanyKey))
                .ForMember(m => m.BrokerCompanyName, opt => opt.MapFrom(m => m.Broker.Name))
                .ForMember(m => m.DistributionWarehouseKey, opt => opt.MapFrom(m => m.DefaultPickFromFacility.FacilityKey))
                .ForMember(m => m.DistributionWarehouseName, opt => opt.MapFrom(m => m.DefaultPickFromFacility.FacilityName))
                .ForMember(m => m.AddressLabel, opt => opt.MapFrom(m => new AddressLabel
                    {
                        AttentionLine = m.ContactName,
                        CompanyName = m.Customer.Name,
                        AddressLine1 = m.ContactAddress.AddressLine1,
                        AddressLine2 = m.ContactAddress.AddressLine2,
                        AddressLine3 = m.ContactAddress.AddressLine3,
                        City = m.ContactAddress.City,
                        State = m.ContactAddress.State,
                        Country = m.ContactAddress.Country,
                        PostalCode = m.ContactAddress.PostalCode,
                    }));

            Mapper.CreateMap<IContractItemReturn, CustomerContract.ContractItem>()
                .ForMember(m => m.ChileProductKey, opt => opt.MapFrom(m => m.ChileProduct.ProductKey))
                .ForMember(m => m.ChileProductName, opt => opt.MapFrom(m => string.Format("{0} - {1}", m.ChileProduct.ProductCode, m.ChileProduct.ProductName)))
                .ForMember(m => m.PackagingProductKey, opt => opt.MapFrom(m => m.PackagingProduct.ProductKey))
                .ForMember(m => m.PackagingProductName, opt => opt.MapFrom(m => m.PackagingProduct.ProductName))
                .ForMember(m => m.Treatment, opt => opt.MapFrom(m => m.Treatment.TreatmentKey))
                .ForMember(m => m.Price, opt => opt.ResolveUsing(m => m.PriceBase + m.PriceFreight + m.PriceTreatment + m.PriceWarehouse - m.PriceRebate))
                .ForMember(m => m.TotalWeight, opt => opt.ResolveUsing(m => m.PackagingProduct.Weight * m.Quantity))
                .ForMember(m => m.Treatment, opt => opt.ResolveUsing(m => m.Treatment.TreatmentNameShort));

            Mapper.CreateMap<IContractShipmentSummaryReturn, IEnumerable<CustomerContractItemDrawSummary>>()
                .ConstructUsing(m => m.Items.Select(i => new CustomerContractItemDrawSummary
                    {
                        CompanyName = m.CustomerName,
                        ContractType = m.ContractType.ToString(),
                        ContractKey = string.Format("{0} ({1})", m.ContractKey, m.ContractNumber),
                        ContractTermBegin = m.TermBegin,
                        ContractTermEnd = m.TermEnd,
                        CustomerProductCode = i.CustomerProductCode,
                        OldContractNumber = m.ContractNumber.HasValue ? m.ContractNumber.ToString() : string.Empty,
                        ProductName = string.Format("{0} - {1}", i.ChileProduct.ProductCode, i.ChileProduct.ProductName),
                        TotalPoundsContracted = i.TotalWeight,
                        TotalPoundsPending = i.TotalWeightPending,
                        TotalPoundsRemaining = i.TotalWeightRemaining,
                        TotalPoundsShipped = i.TotalWeightShipped,
                    }));
        }

        private static void SetupLabResultsReport()
        {
            Mapper.CreateMap<ILotCustomerAllowanceReturn, LotCustomerAllowanceModel>();
            Mapper.CreateMap<ILabReportReturn, IEnumerable<LabResultsReportModel>>()
                .ConvertUsing(dto =>
                {
                    var reportItems = Mapper.Map<IEnumerable<LabResultsReportModel>>(dto.ChileLots).ToList();
                    reportItems.ForEach(i =>
                    {
                        var product = dto.ChileProducts[i.ChileProductKey];
                        i.ProductSpec = Mapper.Map<ChileProductSpec>(product);
                        i.ProductKey = product.ProductKey;
                        i.ProductName = product.ProductName;
                        i.ProductCode = product.ProductCode;
                    });
                    return reportItems;
                });

            Mapper.CreateMap<IDehydratedInputReturn, DehydratedInputModel>();
            Mapper.CreateMap<IProductAttributeRangeReturn, ProductAttributeSpec>();

            Mapper.CreateMap<ILabReportChileLotReturn, LabResultsReportModel>()
                .ForMember(m => m.LotAttributeValues, opt => opt.MapFrom(dto => dto.Attributes))
                .ForMember(m => m.ProductionShift, opt => opt.MapFrom(dto => dto.ProductionShiftKey))
                .ForMember(m => m.ProductSpec, opt => opt.Ignore())
                .ForMember(m => m.ProductKey, opt => opt.Ignore())
                .ForMember(m => m.ProductName, opt => opt.Ignore())
                .ForMember(m => m.ProductCode, opt => opt.Ignore());

            Mapper.CreateMap<IWeightedLotAttributeReturn, LotAttributeValues>()
                .ForMember(m => m.LabTestValue, opt => opt.MapFrom(dto => dto.Value));

            Mapper.CreateMap<IDictionary<string, IWeightedLotAttributeReturn>, LabResults>()
                .ConvertUsing(dic => new LabResults
                {
                    AB = dic.ContainsKey(Constants.ChileAttributeKeys.AB)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.AB])
                        : new LotAttributeValues(),
                    AIA = dic.ContainsKey(Constants.ChileAttributeKeys.AIA)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.AIA])
                        : new LotAttributeValues(),
                    Ash = dic.ContainsKey(Constants.ChileAttributeKeys.Ash)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Ash])
                        : new LotAttributeValues(),
                    Asta = dic.ContainsKey(Constants.ChileAttributeKeys.Asta)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Asta])
                        : new LotAttributeValues(),
                    AToxin = dic.ContainsKey(Constants.ChileAttributeKeys.AToxin)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.AToxin])
                        : new LotAttributeValues(),
                    BI = dic.ContainsKey(Constants.ChileAttributeKeys.BI)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.BI])
                        : new LotAttributeValues(),
                    ColiF = dic.ContainsKey(Constants.ChileAttributeKeys.ColiF)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.ColiF])
                        : new LotAttributeValues(),
                    EColi = dic.ContainsKey(Constants.ChileAttributeKeys.EColi)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.EColi])
                        : new LotAttributeValues(),
                    Ethox = dic.ContainsKey(Constants.ChileAttributeKeys.Ethox)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Ethox])
                        : new LotAttributeValues(),
                    Gluten = dic.ContainsKey(Constants.ChileAttributeKeys.Gluten)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Gluten])
                        : new LotAttributeValues(),
                    Gran = dic.ContainsKey(Constants.ChileAttributeKeys.Gran)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Gran])
                        : new LotAttributeValues(),
                    H2O = dic.ContainsKey(Constants.ChileAttributeKeys.H2O)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.H2O])
                        : new LotAttributeValues(),
                    InsP = dic.ContainsKey(Constants.ChileAttributeKeys.InsP)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.InsP])
                        : new LotAttributeValues(),
                    Lead = dic.ContainsKey(Constants.ChileAttributeKeys.Lead)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Lead])
                        : new LotAttributeValues(),
                    Mold = dic.ContainsKey(Constants.ChileAttributeKeys.Mold)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Mold])
                        : new LotAttributeValues(),
                    RodHrs = dic.ContainsKey(Constants.ChileAttributeKeys.RodHrs)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.RodHrs])
                        : new LotAttributeValues(),
                    Sal = dic.ContainsKey(Constants.ChileAttributeKeys.Sal)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Sal])
                        : new LotAttributeValues(),
                    Scan = dic.ContainsKey(Constants.ChileAttributeKeys.Scan)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Scan])
                        : new LotAttributeValues(),
                    Scov = dic.ContainsKey(Constants.ChileAttributeKeys.Scov)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Scov])
                        : new LotAttributeValues(),
                    TPC = dic.ContainsKey(Constants.ChileAttributeKeys.TPC)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.TPC])
                        : new LotAttributeValues(),
                    Yeast = dic.ContainsKey(Constants.ChileAttributeKeys.Yeast)
                        ? Mapper.Map<LotAttributeValues>(dic[Constants.ChileAttributeKeys.Yeast])
                        : new LotAttributeValues(),
                });

            Mapper.CreateMap<ILabReportChileProduct, ChileProductSpec>()
                .ConvertUsing(dto => new ChileProductSpec
                {
                    Asta = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Asta)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Asta])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Asta},
                    H2O = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.H2O)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.H2O])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.H2O},
                    Scan = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Scan)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Scan])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Scan},
                    AB = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.AB)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.AB])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.AB},
                    Gran = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Gran)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Gran])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Gran},
                    Scov = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Scov)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Scov])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Scov},
                    Ash = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Ash)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Ash])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Ash},
                    AIA = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.AIA)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.AIA])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.AIA},
                    TPC = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.TPC)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.TPC])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.TPC},
                    Yeast = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Yeast)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Yeast])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Yeast},
                    Mold = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Mold)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Mold])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Mold},
                    ColiF = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.ColiF)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.ColiF])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.ColiF},
                    Sal = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Sal)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Sal])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Sal},
                    EColi = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.EColi)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.EColi])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.EColi},
                    AToxin = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.AToxin)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.AToxin])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.AToxin},
                    Lead = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Lead)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Lead])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Lead},
                    Gluten = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Gluten)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Gluten])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Gluten},
                    BI = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.BI)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.BI])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.BI},
                    Ethox = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.Ethox)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.Ethox])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.Ethox},
                    InsP = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.InsP)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.InsP])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.InsP},
                    RodHrs = dto.AttributeRanges.ContainsKey(Constants.ChileAttributeKeys.RodHrs)
                        ? Mapper.Map<ProductAttributeSpec>(dto.AttributeRanges[Constants.ChileAttributeKeys.RodHrs])
                        : new ProductAttributeSpec {AttributeName = Constants.ChileAttributeKeys.RodHrs},
                });
        }

        private static void SetupProductionBatchPacketReport()
        {
            Mapper.CreateMap<IProductionPacketReturn, IEnumerable<ProductionBatchPacketReportModel>>()
                .ConstructUsing(ps => ps.Batches.Select(b => new ProductionBatchPacketReportModel
                {
                    Header = new ProductionBatchPacketReportModel.ProductionBatchPacketReportHeader
                    {
                        BatchType = ps.WorkType.Description,
                        LotNumber = b.LotKey,
                        PSNum = ps.PSNum ?? 0,
                        PackScheduleKey = ps.PackScheduleKey,
                        PackScheduleDate = ps.DateCreated,
                        ProductNameDisplay = ps.ChileProduct.ProductCodeAndName,
                        Description = ps.SummaryOfWork,
                        TargetWeight = b.TargetParameters.BatchTargetWeight,
                        TargetAsta = b.TargetParameters.BatchTargetAsta,
                        TargetScan = b.TargetParameters.BatchTargetScan,
                        TargetScoville = b.TargetParameters.BatchTargetScoville,
                        CalculatedAsta =  b.CalculatedParameters.BatchTargetAsta,
                        CalculatedScan = b.CalculatedParameters.BatchTargetScan,
                        CalculatedScoville = b.CalculatedParameters.BatchTargetScoville,
                        BatchNotes = b.Notes,
                    },
                    PickedChileInventoryItemsByChileType = b.PickedItems
                        .Where(p => p.LotProduct.ProductType.HasValue && p.LotProduct.ProductType.Value == ProductTypeEnum.Chile)
                        .GroupBy(
                            item => item.LotKey.Substring(0, 2),
                            item => new ProductionBatchPacketReportModel.PickedChileInventoryItem
                            {
                                LotNumber = item.LotKey,
                                LotStatus = item.QualityStatus.ToString(),
                                PackagingName = item.PackagingProduct.ProductName,
                                PickedFromWarehouseLocationName = LocationDescriptionHelper.FormatLocationDescription(item.Location.Description),
                                ProductNameDisplay = item.LotProduct.ProductName,
                                Quantity = item.QuantityPicked,
                                WeightPicked = item.QuantityPicked * item.PackagingProduct.Weight,
                                Treatment = item.InventoryTreatment.TreatmentNameShort,
                            },
                            (g, items) => new KeyValuePair<string, IEnumerable<ProductionBatchPacketReportModel.PickedChileInventoryItem>>(ConvertLotType(g),
                                items.OrderBy(i => i.PickedFromWarehouseLocationName))),

                    PickedAdditiveInventoryItemsByAdditiveType = b.PickedItems
                        .Where(p => p.LotProduct.ProductType.HasValue && p.LotProduct.ProductType.Value == ProductTypeEnum.Additive)
                        .GroupBy(
                            item => item.LotKey.Substring(0, 2),
                            item => new ProductionBatchPacketReportModel.PickedAdditiveInventoryItem
                            {
                                LotNumber = item.LotKey,
                                PackagingName = item.PackagingReceived.ProductName,// ?? item.PackagingProduct.ProductName,
                                PickedFromWarehouseLocationName = LocationDescriptionHelper.FormatLocationDescription(item.Location.Description),
                                ProductNameDisplay = item.LotProduct.ProductName,
                                Quantity = item.QuantityPicked,
                                WeightPicked = item.QuantityPicked * item.PackagingProduct.Weight,
                            },
                            (g, items) => new KeyValuePair<string, IEnumerable<ProductionBatchPacketReportModel.PickedAdditiveInventoryItem>>(ConvertLotType(g),
                                items.OrderBy(i => i.PickedFromWarehouseLocationName))),

                    PickedPackagingInventoryItems = b.PickedItems
                        .Where(p => p.LotProduct.ProductType.HasValue && p.LotProduct.ProductType.Value == ProductTypeEnum.Packaging)
                        .Select(item => new ProductionBatchPacketReportModel.PickedPackagingInventoryItem
                        {
                            LotNumber = item.LotKey,
                            PickedFromWarehouseLocationName = LocationDescriptionHelper.FormatLocationDescription(item.Location.Description),
                            ProductNameDisplay = item.LotProduct.ProductName,
                            Quantity = item.QuantityPicked,
                        })
                        .OrderBy(i => i.PickedFromWarehouseLocationName),
                    
                    BatchInstructions = b.Instructions.Notes.Select(n => new KeyValuePair<int, string>(0, n.Text)).Distinct(), // fix indexer
                    ExpectedOutputs = b.PickedItems
                        .Where(i => i.LotProduct.ProductType.HasValue && i.LotProduct.ProductType.Value == ProductTypeEnum.Packaging)
                        .Select(p => p.LotProduct.ProductName)
                        .Concat(new[] {"", ""})
                        .Select(s => new ProductionBatchPacketReportModel.ExpectedOutput
                        {
                            PackagingName = s
                        }),
                }));
        }

        private static void SetupPackSchedulePickSheetReport()
        {
            Mapper.CreateMap<IPackSchedulePickSheetReturn, IEnumerable<PackSchedulePickSheetReportModel>>()
                .ConstructUsing(packSchedule => packSchedule.PickedItems.Select(pickedItem => new PackSchedulePickSheetReportModel
                {
                    BatchLotNumber = pickedItem.NewLotKey,
                    BatchType = packSchedule.WorkType,
                    LoBac = pickedItem.LoBac ?? false,
                    PSNum = packSchedule.PSNum.ToString(),
                    PackScheduleDate = packSchedule.DateCreated,
                    PackScheduleDescription = packSchedule.SummaryOfWork,
                    PackScheduleKey = packSchedule.PackScheduleKey,
                    PackagingName = pickedItem.LotProduct.ProductType == ProductTypeEnum.Packaging ? "" : pickedItem.PackagingProduct.ProductName,
                    PickedLotNumber = pickedItem.LotKey,
                    PickedProductName = pickedItem.LotProduct.ProductName,
                    QuantityPicked = pickedItem.QuantityPicked,
                    PoundsPicked = pickedItem.LotProduct.ProductType == ProductTypeEnum.Packaging ? 0 : pickedItem.PackagingProduct.Weight * pickedItem.QuantityPicked,
                    ProductName = packSchedule.ChileProduct.ProductName,
                    Treatment = pickedItem.InventoryTreatment.TreatmentNameShort,
                    WarehouseLocation = pickedItem.Location.Description
                }.Initialize()));
        }

        private static void SetupWarehouseOrderAcknowledgingReport()
        {
            Mapper.CreateMap<RioValleyChili.Core.Models.Address, AddressLabel>()
                .ForMember(m => m.AttentionLine, opt => opt.Ignore())
                .ForMember(m => m.CompanyName, opt => opt.Ignore())
                .ForMember(m => m.Phone, opt => opt.Ignore());
            Mapper.CreateMap<RioValleyChili.Core.Models.ShippingLabel, ShippingLabel>()
                .AfterMap((s, d) =>
                    {
                        d.Address.CompanyName = s.Name;
                        d.Address.Phone = s.Phone;
                    });
            Mapper.CreateMap<ISalesOrderItemInternalAcknowledgement, SalesOrderItemInternalAcknowledgement>();
            Mapper.CreateMap<ICustomerNoteReturn, CustomerNoteReturn>();
            Mapper.CreateMap<ICustomerNotesReturn, CustomerNotesReturn>();
            Mapper.CreateMap<ICompanyHeaderReturn, CompanyHeaderReturn>();
            Mapper.CreateMap<IPickOrderItemReturn, InventoryPickOrderItemReturn>()
                .Ignoring
                (
                    m => m.TotalPrice,
                    m => m.CustomerOrContractLabel,
                    m => m.ContractKey
                );
            Mapper.CreateMap<IShipmentInformationReturn, ShipmentInformation>();
            Mapper.CreateMap<IShippingInstructions, ShippingInstructions>()
                .ForMember(i => i.ShipFromShippingLabel, opt => opt.MapFrom(m => m.ShipFromOrSoldToShippingLabel));
            Mapper.CreateMap<ITransitInformation, TransitInformation>();
            Mapper.CreateMap<ISalesOrderInternalAcknowledgementReturn, SalesOrderInternalAcknowledgement>();
            Mapper.CreateMap<IInternalOrderAcknowledgementReturn, InternalOrderAcknowledgement>()
                .Ignoring(d => d.GroupedCustomerNotes)
                .AfterMap((s, d) => d.Initialize());

            Mapper.CreateMap<ISalesOrderItemReturn, SalesOrderItem>()
                .Ignoring(m => m.TotalPrice, m => m.CustomerOrContractLabel);
            Mapper.CreateMap<ISalesOrderAcknowledgementReturn, SalesOrderAcknowledgement>();
        }

        private static void SetupWarehouseOrderPackingListReport()
        {
            Mapper.CreateMap<IPackingListPickedInventoryItem, PackingListPickedInventoryItem>();
            Mapper.CreateMap<IInventoryProductReturn, InventoryProductReturn>();
            Mapper.CreateMap<IPackagingProductReturn, PackagingProductReturn>();
            Mapper.CreateMap<IInventoryTreatmentReturn, InventoryTreatmentReturn>();
            Mapper.CreateMap<IInventoryShipmentOrderPackingListReturn, WarehouseOrderPackingList>();
        }

        private static void SetupWarehouseOrderBillOfLadingReport()
        {
            Mapper.CreateMap<IInventoryShipmentOrderBillOfLadingReturn, WarehouseOrderBillOfLading>();
        }

        private static void SetupPickSheet()
        {
            Mapper.CreateMap<IInventoryShipmentOrderPickSheetReturn, WarehouseOrderPickSheet>();
            Mapper.CreateMap<IPickSheetItemReturn, PickSheetItemReturn>()
                .Ignoring(m => m.Street, m => m.Row, m => m.LocationDescription)
                .AfterMap((s, d) => d.Initialize());
        }

        private static void SetupCertificateOfAnalysis()
        {
            Mapper.CreateMap<INoteReturn, NoteReturn>();
            Mapper.CreateMap<ILotAttributeReturn, LotAttributeReturn>();
            Mapper.CreateMap<IInventoryShipmentOrderItemAnalysisReturn, InventoryShipmentOrderItemAnalysisReturn>();
            Mapper.CreateMap<IInventoryShipmentOrderCertificateOfAnalysisReturn, InventoryShipmentOrderCertificateOfAnalysis>();
        }

        private static void SetupProductionRecapReport()
        {
            Mapper.CreateMap<IProductionRecapWeightItem, WeightItem>()
                .Ignoring(m => m.Parent);

            Mapper.CreateMap<IProductionRecapWeightGroup, WeightItemGroup<WeightItem>>()
                .Ignoring(m => m.Parent);

            Mapper.CreateMap<IProductionRecap_ByLineProduct_ByLine_ByType, WeightItemGroup<WeightItem>>()
                .Ignoring(m => m.Parent, m => m.Target, m => m.Produced)
                .ForMember(m => m.Name, opt => opt.MapFrom(m => m.Type))
                .ForMember(m => m.Items, opt => opt.MapFrom(m => m.ItemsByProduct));

            Mapper.CreateMap<IProductionRecap_ByLineProduct_ByLine, WeightItemGroup<WeightItemGroup<WeightItem>>>()
                .Ignoring(m => m.Parent, m => m.Target, m => m.Produced)
                .ForMember(m => m.Name, opt => opt.MapFrom(m => m.Line))
                .ForMember(m => m.Items, opt => opt.MapFrom(m => m.ItemsByType));

            Mapper.CreateMap<IProductionRecap_ByLineProduct, WeightItemGroup<WeightItemGroup<WeightItemGroup<WeightItem>>>>()
                .Ignoring(m => m.Parent, m => m.Name, m => m.Target, m => m.Produced)
                .ForMember(m => m.Items, opt => opt.MapFrom(m => m.ItemsByLine));

            Mapper.CreateMap<IProductionRecapTestItem, TestItem>()
                .Ignoring(m => m.Parent);

            Mapper.CreateMap<IProductionRecapTestGroup, TestItemGroup>();

            Mapper.CreateMap<IProductionRecapTimeItem, TimeItem>()
                .Ignoring(m => m.Parent, m => m.BudgetHrs);

            Mapper.CreateMap<IProductionRecapTimeGroup, TimeGroup>();

            Mapper.CreateMap<IProductionRecap_Lot, ProductDetailLot>()
                .Ignoring(m => m.Parent, m => m.Mode, b => b.BdgtTime);

            Mapper.CreateMap<IProductionRecap_LotGroup, ProductDetailsGroup>()
                .Ignoring(m => m.Parent);

            Mapper.CreateMap<IProductionRecap_LotGroupSection, ProductDetailsSection>();

            Mapper.CreateMap<IProductionRecapReportReturn, ProductionRecapReportModel>()
                .AfterMap((s, d) => d.Initialize());
        }

        private static void SetupPendingOrderDetails()
        {
            Mapper.CreateMap<IPendingOrderItem, PendingOrderItem>();
            Mapper.CreateMap<IPendingWarehouseOrderDetail, PendingWarehouseOrder>();
            Mapper.CreateMap<IPendingCustomerOrderDetail, PendingCustomerOrder>();
            Mapper.CreateMap<IPendingOrderDetails, PendingOrderDetails>()
                .Ignoring(d => d.PendingCustomerOrderSections, d => d.PendingWarehouseOrderSections)
                .AfterMap((s, d) => d.Initialize());
        }

        private static void SetupProductionAdditiveInputs()
        {
            Mapper.CreateMap<IProductionAdditiveInputsReportReturn, ProductionAdditiveInputs>();
            Mapper.CreateMap<IProductionAdditiveInputs_ByDateReturn, ProductionAdditiveInputs_ByDate>();
            Mapper.CreateMap<IProductionLotAdditiveInputs, ProductionLotAdditiveInputs>();
            Mapper.CreateMap<IProductionAdditiveInputs_ByAdditiveTypeReturn, ProductionAdditiveInputs_ByAdditiveType>();
            Mapper.CreateMap<IProductionAdditiveInputPicked, ProductionAdditiveInputPicked>()
                .ForMember(m => m.AdditiveType, opt => opt.ResolveUsing(m => m.AdditiveType == null ? null : m.AdditiveType.ToUpper()));
            Mapper.CreateMap<IProductionAdditiveInputs_Totals, ProductionAdditiveInputs_Totals>()
                .ForMember(m => m.AdditiveType, opt => opt.ResolveUsing(m => m.AdditiveType == null ? null : m.AdditiveType.ToUpper()));
        }

        private static void SetupCustomerOrderInvoice()
        {
            Mapper.CreateMap<ISalesOrderInvoice, SalesOrderInvoice>()
                .AfterMap((s, d) => d.Initialize());
            Mapper.CreateMap<ISalesOrderInvoicePickedItem, SalesOrderInvoicePickedItem>()
                .Ignoring
                    (
                        i => i.Contract,
                        i => i.QuantityOrdered,
                        i => i.PriceBase,
                        i => i.PriceFreight,
                        i => i.PriceTreatment,
                        i => i.PriceWarehouse,
                        i => i.PriceRebate,
                        i => i.TotalPrice,
                        i => i.OrderItem
                    )
                .AfterMap((source, mapped) =>
                    {
                        if(source.LoBac == true && source.TreatmentNameShort == "NA")
                        {
                            mapped.TreatmentNameShort = "LB";
                        }
                    });
            Mapper.CreateMap<ISalesOrderInvoiceOrderItem, SalesOrderInvoiceOrderItem>();
        }

        private static void SetupSampleMatchingSummary()
        {
            Mapper.CreateMap<ISampleOrderMatchingItemReturn, SampleOrderMatchingSummaryItem>();
            Mapper.CreateMap<ISampleOrderMatchingSummaryReportReturn, SampleOrderMatchingSummaryReportModel>();
        }

        private static void SetupSampleRequest()
        {
            Mapper.CreateMap<ISampleOrderRequestItemReportReturn, SampleOrderRequestItemReportModel>();
            Mapper.CreateMap<ISampleOrderRequestReportReturn, SampleOrderRequestReportModel>()
                .AfterMap((i, o) =>
                {
                    o.ShipTo.Address.CompanyName = i.ShipToCompanyName;
                    o.ShipTo.Address.AttentionLine = i.ShipTo.Name;
                    o.RequestedBy.Address.CompanyName = i.RequestedByCompanyName;
                    o.RequestedBy.Address.AttentionLine = i.RequestedBy.Name;
                });
        }

        private static void SetupChileMaterialsReceivedRecap()
        {
            Mapper.CreateMap<IChileMaterialsReceivedRecapReturn, ChileMaterialsReceivedRecapReportModel>();
            Mapper.CreateMap<IChileMaterialsReceivedRecapItemReturn, ChileMaterialsReceivedRecapItemReportModel>();
        }

        private static void SetupProductionSchedule()
        {
            Mapper.CreateMap<IProductionScheduleReportReturn, ProductionScheduleReportModel>();
            Mapper.CreateMap<IProductionScheduleItemReportReturn, ProductionScheduleItemReportModel>();
            Mapper.CreateMap<IProductionScheduleBatchReturn, ProductionScheduleBatchReportModel>();
        }

        private static void SetupInventoryCycleCount()
        {
            Mapper.CreateMap<IInventoryCycleCountReportReturn, InventoryCycleCountReportModel>()
                .AfterMap((s, d) => d.Initialize());
            Mapper.CreateMap<IInventoryCycleCountLocation, InventoryCycleCountLocation>()
                .Ignoring
                (
                    m => m.Header,
                    m => m.FacilityName,
                    m => m.GroupName,
                    m => m.ReportDateTime
                );
            Mapper.CreateMap<IInventoryCycleCount, InventoryCycleCount>();
        }

        private static string ConvertLotType(string lotType)
        {
            switch(lotType)
            {
                case "01": return "Dehydrated Base";
                case "02": return "WIP Base";
                case "03": return "Finished Goods Base";
                default: return "Other Base";
            }
        }

        private static void SetupSalesQuote()
        {
            Mapper.CreateMap<ISalesQuoteReportReturn, SalesQuoteReportModel>();
            Mapper.CreateMap<ISalesQuoteItemReportReturn, SalesQuoteItemReportModel>();
        }
    }
}
