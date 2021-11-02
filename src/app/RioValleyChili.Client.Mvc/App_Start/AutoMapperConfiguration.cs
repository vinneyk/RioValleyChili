using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.ChileMaterialsReceived;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.CustomerProductCodes;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.InventoryTransactions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.SampleRequests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.TreatmentOrders;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Client.Mvc.Models.Shipping;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService;
using RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.Interfaces.Returns.TreatmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            #region common mappings

            // convert AttributeNamesByType to a string key.
            Mapper.CreateMap<IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>>, IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, string>>>>>()
                .ConvertUsing(dto => dto.Select(attr => new KeyValuePair<string, IEnumerable<KeyValuePair<string, string>>>(attr.Key.ToString(), attr.Value)));

            Mapper.CreateMap<IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>>, IDictionary<string, IEnumerable<KeyValuePair<string, string>>>>()
                .ConvertUsing(dto => dto.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value));

            Mapper.CreateMap<LotProductionStatus, string>()
                  .ConvertUsing(status =>
                  {
                      switch (status)
                      {
                          case LotProductionStatus.Batched: return "Batched";
                          case LotProductionStatus.Produced: return "Produced";
                          default: return "Unknown";
                      }
                  });
            Mapper.CreateMap<IInventoryTreatmentReturn, InventoryTreatmentResponse>();

            Mapper.CreateMap<CreateCompanyRequest, CreateCompanyParameters>()
                .ForMember(m => m.BrokerKey, opt => opt.MapFrom(m => m.BrokerCompanyKey))
                .Ignoring(m => m.UserToken);
            Mapper.CreateMap<UpdateCompanyRequest, UpdateCompanyParameters>()
                .ForMember(m => m.BrokerKey, opt => opt.MapFrom(m => m.BrokerCompanyKey))
                .Ignoring(m => m.UserToken);

            Mapper.CreateMap<ContactAddressResponse, ContactAddressParameters>();
            Mapper.CreateMap<CreateContactRequest, CreateContactParameters>()
                .Ignoring(m => m.UserToken);
            Mapper.CreateMap<UpdateContractRequest, UpdateContactParameters>()
                .Ignoring(m => m.ContactKey, m => m.UserToken);
            Mapper.CreateMap<IContactAddressReturn, ContactAddressResponse>();
            Mapper.CreateMap<IContactSummaryReturn, ContactSummaryResponse>();

            Mapper.CreateMap<ICompanyHeaderReturn, CompanyResponse>();
            Mapper.CreateMap<ICompanySummaryReturn, CompanySummaryResponse>();
            Mapper.CreateMap<ICompanyDetailReturn, CompanyDetailResponse>()
                .ForMember(m => m.CustomerResponse, opt => opt.MapFrom(m => m.Customer));
            Mapper.CreateMap<ICustomerCompanyReturn, CustomerCompanyResponse>();
            Mapper.CreateMap<ICustomerCompanyNoteReturn, CustomerCompanyNoteResponse>();

            Mapper.CreateMap<SetCustomerNoteRequest, CreateCustomerNoteParameters>()
                .Ignoring(m => m.UserToken, m => m.CustomerKey);
            Mapper.CreateMap<SetCustomerNoteRequest, UpdateCustomerNoteParameters>()
                .Ignoring(m => m.UserToken, m => m.CustomerNoteKey);

            Mapper.CreateMap<INotebookReturn, Notebook>();
            Mapper.CreateMap<INoteReturn, Note>();
            Mapper.CreateMap<IAdditiveTypeReturn, Ingredient>()
                .ForMember(m => m.Key, opt => opt.MapFrom(dto => dto.AdditiveTypeKey))
                .ForMember(m => m.Description, opt => opt.MapFrom(dto => dto.AdditiveTypeDescription));
            
            Mapper.CreateMap<IShippingLabelReturn, ShippingLabel>()                
                .ForMember(m => m.Address, opt => opt.ResolveUsing(m => new Address
                {
                        AddressLine1 = m.AddressLine1,
                        AddressLine2 = m.AddressLine2,
                        AddressLine3 = m.AddressLine3,
                        City = m.City,
                        State = m.State,
                        PostalCode = m.PostalCode,
                        Country = m.Country
                    }));

            Mapper.CreateMap<IUserSummaryReturn, UserSummaryResponse>();

            #endregion

            #region Production models

            Mapper.CreateMap<CreateProductionScheduleRequest, CreateProductionScheduleParameters>()
                .Ignoring(m => m.UserToken);
            Mapper.CreateMap<UpdateProductionScheduleRequest, UpdateProductionScheduleParameters>()
                .Ignoring(m => m.UserToken, m => m.ProductionScheduleKey);
            Mapper.CreateMap<SetProductionScheduleItemRequest, SetProductionScheduleItemParameters>();

            Mapper.CreateMap<IProductionScheduleSummaryReturn, ProductionScheduleSummaryResponse>();
            Mapper.CreateMap<IProductionScheduleDetailReturn, ProductionScheduleDetailResponse>()
                .Ignoring(m => m.Links);
            Mapper.CreateMap<IProductionScheduleItemReturn, ProductionScheduleItemResponse>();
            Mapper.CreateMap<IScheduledPackScheduleReturn, ScheduledPackScheduleResponse>();

            Mapper.CreateMap<IWorkTypeReturn, WorkType>();
            Mapper.CreateMap<IChileProductAdditiveIngredientSummaryReturn, ChileProductAdditiveIngredientSummary>();
            Mapper.CreateMap<IProductionBatchMaterialsSummaryReturn, ProductionBatchMaterialsSummary>()
                .ForMember(m => m.InventoryType, opt => opt.MapFrom(dto => dto.ProductType));
            Mapper.CreateMap<IProductionBatchSummaryReturn, ProductionBatchSummary>();
            Mapper.CreateMap<IProductionBatchDetailReturn, ProductionBatchDetails>()
                .ForMember(m => m.IsLocked, opt => opt.ResolveUsing(m => m.HasProductionBeenCompleted))
                .AfterMap((batchIn, batchOut) =>
                {
                    var picked = batchOut.PickedInventoryItems.ToList();
                    picked.ForEach(i => i.ValidForPicking = !batchOut.IsLocked);
                    batchOut.PickedInventoryItems = picked;
                });
            Mapper.CreateMap<IProductionBatchPackagingMaterialSummaryReturn, ProductionBatchPackagingMaterialSummary>();
            Mapper.CreateMap<CreateProductionBatchDto, CreateProductionBatchParameters>()
                .ForMember(m => m.LotDateCreated, opt => opt.ResolveUsing(m => 
                    m.LotDateCreated == null 
                        ? (DateTime?)null
                        : DateTime.SpecifyKind(m.LotDateCreated.Value, DateTimeKind.Local)
                ))
                .ForMember(m => m.UserToken, opt => opt.Ignore());

            Mapper.CreateMap<IMillAndWetdownSummaryReturn, MillAndWetdownSummary>()
                .ForMember(m => m.ProductionLineDescription, opt => opt.ResolveWithWarehouseLocationFormatting(m => m.ProductionLineDescription));
            Mapper.CreateMap<IMillAndWetdownDetailReturn, MillAndWetdownDetail>()
                .ForMember(m => m.ProductionLineDescription, opt => opt.ResolveWithWarehouseLocationFormatting(m => m.ProductionLineDescription));
            Mapper.CreateMap<IMillAndWetdownResultItemReturn, MillAndWetdownResultItem>();
            Mapper.CreateMap<IMillAndWetdownPickedItemReturn, MillAndWetdownPickedItem>()
                .ForMember(m => m.Product, opt => opt.MapFrom(m => m.LotProduct));
            
            #region Pack Schedule models

            Mapper.CreateMap<IPackScheduleSummaryReturn, PackScheduleSummary>()
                .ForMember(m => m.ProductionLineDescription, opt => opt.ResolveWithWarehouseLocationFormatting(m => m.ProductionLineDescription));

            Mapper.CreateMap<IPackScheduleDetailReturn, PackScheduleDetails>()
                .ForMember(m => m.ProductionLineDescription, 
                    opt => opt.ResolveWithWarehouseLocationFormatting(m => m.ProductionLineDescription));

            Mapper.CreateMap<IProductionBatchTargetParameters, ProductionBatchTargets>();

            Mapper.CreateMap<CreatePackSchedule, CreatePackScheduleParameters>()
                .ForMember(m => m.UserToken, opt => opt.Ignore());
            
            Mapper.CreateMap<UpdatePackSchedule, UpdatePackScheduleParameters>()
                .ForMember(m => m.UserToken, opt => opt.Ignore());
            
            #endregion

            #region Production Results

            Mapper.CreateMap<ProductionResultItemDto, BatchProductionResultInventoryItemSummary>();
            Mapper.CreateMap<CreateProductionBatchResultsDto, CreateProductionBatchResultsParameters>()
                .ForMember(m => m.UserToken, opt => opt.Ignore());
            Mapper.CreateMap<UpdateProductionBatchResultsDto, UpdateProductionBatchResultsParameters>()
                .ForMember(m => m.UserToken, opt => opt.Ignore())
                .ForMember(m => m.ProductionResultKey, opt => opt.Ignore());

            Mapper.CreateMap<IProductionResultDetailReturn, ProductionResultDetail>()
                .ForMember(m => m.ProductionLine, opt => opt.MapFrom(m => m.ProductionLocation));
            Mapper.CreateMap<IProductionResultItemReturn, ProductionResultDetail.ProductionResultItem>()
                .ForMember(m => m.WarehouseLocation, opt => opt.MapFrom(m => m.Location));

            #endregion

            #endregion

            #region Inventory models

            Mapper.CreateMap<IProductReturn, InventoryProductResponse>()
                .ForMember(m => m.ProductType, opt => opt.Ignore())
                .ForMember(m => m.ProductSubType, opt => opt.Ignore());
            Mapper.CreateMap<IInventoryProductReturn, InventoryProductResponse>();
            Mapper.CreateMap<IChileProductReturn, ChileProductResponse>()
                .ForMember(m => m.ProductType, opt => opt.UseValue(ProductTypeEnum.Chile))
                .ForMember(m => m.ProductSubType, opt => opt.MapFrom(m => m.ChileStateName));

            Mapper.CreateMap<PickedInventoryDto, SetPickedInventoryParameters>();
            Mapper.CreateMap<PickedInventoryItemDto, SetPickedInventoryItemParameters>()
                .ForMember(m => m.Quantity, opt => opt.MapFrom(dto => dto.QuantityPicked));
            Mapper.CreateMap<PickedInventoryItemWithDestinationDto, IntraWarehouseOrderPickedItemParameters>()
                .ForMember(m => m.Quantity, opt => opt.MapFrom(dto => dto.QuantityPicked));

            MapInventorySummaryType<IInventorySummaryReturn, InventoryItem>();
            MapInventorySummaryType<IPickableInventorySummaryReturn, PickableInventoryItem>();
            
            Mapper.CreateMap<IPickedInventoryItemReturn, PickedInventoryItem>()
                .ForMember(m => m.LotStatus, opt => opt.MapFrom(dto => dto.QualityStatus))
                .ForMember(m => m.LotProductionStatus, opt => opt.MapFrom(dto => dto.ProductionStatus))
                .ForMember(m => m.Product, opt => opt.MapFrom(dto => dto.LotProduct))
                .ForMember(m => m.ReceivedPackagingName, opt => opt.MapFrom(dto => dto.PackagingReceived.ProductName))
                .ForMember(m => m.CustomerKey, opt => opt.MapFrom(dto => dto.Customer.CompanyKey))
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(dto => dto.Customer.Name))
                .ForMember(m => m.ValidForPicking, opt => opt.UseValue(true));

            Mapper.CreateMap<IPickedInventoryItemReturn, PickedInventoryItemWithDestination>()
                .ForMember(m => m.LotStatus, opt => opt.MapFrom(dto => dto.QualityStatus))
                .ForMember(m => m.LotProductionStatus, opt => opt.MapFrom(dto => dto.ProductionStatus))
                .ForMember(m => m.Product, opt => opt.MapFrom(dto => dto.LotProduct))
                .ForMember(m => m.ReceivedPackagingName, opt => opt.MapFrom(dto => dto.PackagingReceived.ProductName))
                .ForMember(m => m.CustomerKey, opt => opt.MapFrom(dto => dto.Customer.CompanyKey))
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(dto => dto.Customer.Name))
#warning DestinationLocation is mapped to CurrentLocation - VK
                //...But what the hell is it supposed to be doing instead? Is there some reason this decision was made? Is it a valid mapping? - VK 2015-8-5
                .ForMember(m => m.DestinationLocation, opt => opt.MapFrom(m => m.CurrentLocation))
                .ForMember(m => m.ValidForPicking, opt => opt.UseValue(true));

            Mapper.CreateMap<ReceiveInventoryDto, ReceiveInventoryParameters>()
                .ForMember(m => m.UserToken, opt => opt.Ignore());

            Mapper.CreateMap<ReceiveInventoryDto.ReceiveInventoryItemDto, ReceiveInventoryItemParameters>();

            Mapper.CreateMap<IInventoryPickOrderSummaryReturn, InventoryPickOrderSummary>()
                .ForMember(m => m.PoundsOnOrder, opt => opt.MapFrom(m => m.TotalWeight));

            Mapper.CreateMap<IPickedInventorySummaryReturn, PickedInventorySummary>()
                .ForMember(m => m.PoundsPicked, opt => opt.MapFrom(m => m.TotalWeightPicked));

            Mapper.CreateMap<IPickOrderDetailReturn<IPickOrderItemReturn>, InventoryPickOrderDetail>();

            Mapper.CreateMap<IPickOrderItemReturn, InventoryPickOrderItemResponse>();

            Mapper.CreateMap<SetInventoryPickOrderItemRequest, SetInventoryPickOrderItemParameters>();

            Mapper.CreateMap<IPickedInventoryDetailReturn, PickedInventory>();
            Mapper.CreateMap<IPickedInventoryDetailReturn, PickedInventoryWithDestination>();

            #region Chile Materials Recieved

            Mapper.CreateMap<CreateChileMaterialsReceivedRequest, CreateChileMaterialsReceivedParameters>()
                .Ignoring(m => m.UserToken);
            Mapper.CreateMap<UpdateChileMaterialsReceivedRequest, UpdateChileMaterialsReceivedParameters>()
                .Ignoring(m => m.UserToken, m => m.LotKey);
            Mapper.CreateMap<UpdateChileMaterialsReceivedItemRequest, UpdateChileMaterialsReceivedItemParameters>();
            Mapper.CreateMap<CreateChileMaterialsReceivedItemRequest, CreateChileMaterialsReceivedItemParameters>();

            Mapper.CreateMap<IChileMaterialsReceivedSummaryReturn, ChileMaterialsReceivedSummaryResponse>();
            Mapper.CreateMap<IChileMaterialsReceivedDetailReturn, ChileMaterialsReceivedDetailResponse>()
                .Ignoring(m => m.Links)
                .ForMember(m => m.IsEditingEnabled, opt => opt.UseValue(true));
            Mapper.CreateMap<IChileMaterialsReceivedItemReturn, ChileMaterialsReceivedItemResponse>();

            #endregion

            #region Mill & Wetdown

            Mapper.CreateMap<CreateMillAndWetdownRequest, CreateMillAndWetdownParameters>();
            Mapper.CreateMap<UpdateMillAndWetdownRequest, UpdateMillAndWetdownParameters>();
            Mapper.CreateMap<MillAndWetdownPickedItemRequest, MillAndWetdownPickedItemParameters>();
            Mapper.CreateMap<MillAndWetdownResultItemRequest, MillAndWetdownResultItemParameters>();

            #endregion

            #region IntraWarehouseOrder mappings
            
            Mapper.CreateMap<IIntraWarehouseOrderDetailReturn, IntraWarehouseOrderDetails>()
                .Map(m => m.PickedInventoryDetail, m => m.PickedInventory)
                .Map(m => m.OrderKey, m => m.MovementKey);

            Mapper.CreateMap<IIntraWarehouseOrderSummaryReturn, IntraWarehouseOrderSummary>()
                .Map(m => m.OrderKey, m => m.MovementKey);

            Mapper.CreateMap<CreateIntraWarehouseOrder, CreateIntraWarehouseOrderParameters>()
                .Map(m => m.PickedItems, m => m.PickedInventoryItems);

            Mapper.CreateMap<UpdateIntraWarehouseOrder, UpdateIntraWarehouseOrderParameters>();

            #endregion

            #region InterWarehouse Movements

            Mapper.CreateMap<SetPickedInventoryItemCodesRequestParameter, SetPickedInventoryItemCodesParameters>();

            Mapper.CreateMap<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>, InterWarehouseOrderDetails>()
                .ForMember(m => m.ShipmentDate, opt => opt.MapFrom(m => m.Shipment.ShippingInstructions.ShipmentDate))
                .ForMember(m => m.IsLocked, opt => opt.ResolveUsing(m => m.OrderStatus == OrderStatus.Fulfilled))
                .MapLinkedResource()
                .AfterMap((mIn, mOut) =>
                {
                    var picked = mOut.PickedInventory.PickedInventoryItems.ToList();
                    picked.ForEach(i => i.ValidForPicking = !mOut.IsLocked);
                    mOut.PickedInventory.PickedInventoryItems = picked;
                });

            Mapper.CreateMap<IInventoryShipmentOrderSummaryReturn, InterWarehouseOrderSummary>();

            Mapper.CreateMap<SetInventoryShipmentOrderParameters, UpdateInterWarehouseOrderParameters>()
                .ForMember(m => m.SourceFacilityKey, opt => opt.MapFrom(m => m.OriginFacilityKey))
                .ForMember(m => m.InventoryShipmentOrderKey, opt => opt.Ignore())
                .ForMember(m => m.UserToken, opt => opt.Ignore())
                .ForMember(m => m.HeaderParameters, opt => opt.ResolveUsing(m => new SetOrderHeaderParameters
                    {
                        CustomerPurchaseOrderNumber = m.PurchaseOrderNumber,
                        DateOrderReceived = m.DateOrderReceived,
                        OrderRequestedBy = m.OrderRequestedBy,
                        OrderTakenBy = m.OrderTakenBy,
                    }))
                .ForMember(m => m.SetShipmentInformation, opt => opt.MapFrom(m => m.Shipment))
                .AfterMap((s, d) => d.SetShipmentInformation.ShippingInstructions.ShipmentDate = s.ShipmentDate);

            Mapper.CreateMap<SetOrderHeaderRequestParameter, SetOrderHeaderParameters>();

            Mapper.CreateMap<SetInventoryShipmentOrderParameters, SetOrderParameters>()
                .ForMember(m => m.HeaderParameters, opt => opt.ResolveUsing(m => new SetOrderHeaderRequestParameter
                    {
                        CustomerPurchaseOrderNumber = m.PurchaseOrderNumber,
                        DateOrderReceived = m.DateOrderReceived,
                        OrderRequestedBy = m.OrderRequestedBy,
                        OrderTakenBy = m.OrderTakenBy,
                        ShipmentDate = m.ShipmentDate,
                    }))
                .ForMember(m => m.SetShipmentInformation, opt => opt.MapFrom(m => m.Shipment))
                .ForMember(m => m.SourceFacilityKey, opt => opt.MapFrom(m => m.OriginFacilityKey))
                .ForMember(m => m.UserToken, opt => opt.Ignore())
                .AfterMap((s, d) => d.SetShipmentInformation.ShippingInstructions.ShipmentDate = s.ShipmentDate);

            #endregion

            #region Inventory Adjustment mappings

            Mapper.CreateMap<IInventoryAdjustmentReturn, InventoryAdjustment>();
            Mapper.CreateMap<IInventoryAdjustmentItemReturn, InventoryAdjustmentItem>();

            #endregion

            #endregion

            #region Quality Control

            Mapper.CreateMap<IEnumerable<ILotDefectReturn>, IEnumerable<LotDefect>>()
                .ConvertUsing(
                    (input) => input.AsEnumerable().Select(defect => 
                        defect.DefectType != DefectTypeEnum.InHouseContamination
                            ? Mapper.Map<LotAttributeDefect>(defect)
                            : Mapper.Map<LotDefect>(defect)));

            Mapper.CreateMap<ILotDefectReturn, LotAttributeDefect>()
                  .ForMember(m => m.AttributeShortName, opt => opt.MapFrom(dto => dto.AttributeDefect.AttributeShortName))
                  .ForMember(m => m.OriginalMaxLimit, opt => opt.MapFrom(dto => dto.AttributeDefect.OriginalMaxLimit))
                  .ForMember(m => m.OriginalMinLimit, opt => opt.MapFrom(dto => dto.AttributeDefect.OriginalMinLimit))
                  .ForMember(m => m.OriginalValue, opt => opt.MapFrom(dto => dto.AttributeDefect.OriginalValue));

            Mapper.CreateMap<ILotDefectReturn, LotDefect>();
            Mapper.CreateMap<ILotAttributeDefectReturn, AttributeDefect>();

            Mapper.CreateMap<ILotDefectResolutionReturn, LotDefectResolutionReturn>();

            #endregion

            #region Lot models

            Mapper.CreateMap<ILotCustomerAllowanceReturn, LotCustomerAllowanceResponse>();
            Mapper.CreateMap<ILotCustomerOrderAllowanceReturn, LotCustomerOrderAllowanceResponse>();
            Mapper.CreateMap<ILotContractAllowanceReturn, LotContractAllowanceResponse>();

            Mapper.CreateMap<ILotQualitySummaryReturn, LotQualitySummaryResponse>()
                .ForMember(m => m.LotDate, opt => opt.MapFrom(dto => dto.LotDateCreated))
                .ForMember(m => m.Product, opt => opt.MapFrom(dto => dto.LotProduct))
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(dto => dto.Customer.Name))
                .ForMember(m => m.CustomerKey, opt => opt.MapFrom(dto => dto.Customer.CompanyKey))
                .ForMember(m => m.OldContextLotStat, opt => opt.Ignore());

            Mapper.CreateMap<ILotQualitySingleSummaryReturn, LotDetailsResponse>();
            
            Mapper.CreateMap<ILotAttributeReturn, LotAttribute>()
                .ForMember(m => m.AttributeDate, opt => opt.ResolveUsing(dto => dto.AttributeDate.ToShortDateString()));

            Mapper.CreateMap<ILotStatInfoReturn, LotStatInfoResponse>();
            Mapper.CreateMap<ICreateLotDefectReturn, CreateLotDefectResponse>();

            Mapper.CreateMap<LotAttributeRequest, AttributeValueParameters>();
            Mapper.CreateMap<DefectResolutionRequest, DefectResolutionParameters>();
            Mapper.CreateMap<LotAttributeInfoRequest, AttributeInfoParameters>();

            Mapper.CreateMap<UpdateLotRequest, SetLotAttributeParameters>()
                .Ignoring(m => m.UserToken)
                .ForMember(m => m.Attributes, opt => opt.ResolveUsing(m => m.Attributes.ToDictionary(a => a.AttributeKey, a => a.Map().To<AttributeValueParameters>())));

            Mapper.CreateMap<AddLotAttributesRequest, AddLotAttributesParameters>()
                .Ignoring(m => m.UserToken)
                .ForMember(m => m.Attributes, opt => opt.ResolveUsing(m => m.Attributes.ToDictionary(a => a.AttributeKey, a => a.Map().To<AttributeValueParameters>())));

            Mapper.CreateMap<ILotHistoryReturn, LotHistoryResponse>();
            Mapper.CreateMap<ILotHistoryAttributeReturn, LotHistoryAttributeResponse>();
            Mapper.CreateMap<ILotHistoryRecordReturn, LotHistoryRecordResponse>();
            Mapper.CreateMap<ILotOutputTraceReturn, LotOutputTraceResponse>();
            Mapper.CreateMap<ILotOutputTraceInputReturn, LotOutputTraceInputResponse>();
            Mapper.CreateMap<ILotOutputTraceOrdersReturn, LotOutputTraceOrdersResponse>();

            Mapper.CreateMap<ILotInputTraceReturn, LotInputTraceResponse>();

            #endregion

            #region Product models

            Mapper.CreateMap<SetChileProductIngredientRequest, SetChileProductIngredientParameters>();
            Mapper.CreateMap<SetChileProductIngredientsRequest, SetChileProductIngredientsParameters>()
                .Ignoring(m => m.UserToken, m => m.ChileProductKey);

            Mapper.CreateMap<AttributeRangeRequest, SetAttributeRangeParameters>();
            Mapper.CreateMap<SetChileProductAttributeRangesRequest, SetChileProductAttributeRangesParameters>()
                .Ignoring(m => m.UserToken, m => m.ChileProductKey);
            
            Mapper.CreateMap<IPackagingProductReturn, PackagingProductResponse>();
            Mapper.CreateMap<IProductAttributeRangeReturn, ProductAttributeRangeReturnViewModel>();
            Mapper.CreateMap<IProductIngredientReturn, ProductIngredientViewModel>();

            #endregion

            #region customer models

            Mapper.CreateMap<SalesOrderItem, SalesOrderItemParameters>();
            Mapper.CreateMap<CreateSalesOrderRequest, CreateSalesOrderParameters>()
                .ForMember(m => m.FreightCharge, opt => opt.ResolveUsing(m => m.FreightCharge.Value))
                .Ignoring(m => m.UserToken)
                .AfterMap((s, d) => d.SetShipmentInformation.ShippingInstructions.ShipmentDate = s.HeaderParameters.ShipmentDate);
            Mapper.CreateMap<UpdateSalesOrderRequest, UpdateSalesOrderParameters>()
                .Ignoring(m => m.UserToken)
                .AfterMap((s, d) => d.SetShipmentInformation.ShippingInstructions.ShipmentDate = s.HeaderParameters.ShipmentDate);

            Mapper.CreateMap<CustomerProductRangeRequest, SetCustomerProductAttributeRangeParameters>();
            Mapper.CreateMap<SetCustomerProductRangesRequest, SetCustomerProductAttributeRangesParameters>()
                .Ignoring(m => m.UserToken);

            Mapper.CreateMap<ICustomerChileProductAttributeRangeReturn, CustomerChileProductAttributeRangeReturn>();
            Mapper.CreateMap<ICustomerChileProductAttributeRangesReturn, CustomerChileProductAttributeRangesReturn>();

            Mapper.CreateMap<ICustomerContractSummaryReturn, CustomerContractSummaryResponse>()
                .ForMember(m => m.TermBegin, opt => opt.ResolveUsing(m => m.TermBegin.HasValue ? m.TermBegin.Value.ToShortDateString() : "")) 
                .ForMember(m => m.TermEnd, opt => opt.ResolveUsing(m => m.TermEnd.HasValue ? m.TermEnd.Value.ToShortDateString() : "")) 
                .ForMember(m => m.ContractDate, opt => opt.ResolveUsing(m => m.ContractDate.ToShortDateString())) 
                .ForMember(m => m.CustomerKey, opt => opt.MapFrom(m => m.Customer.CompanyKey))
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(m => m.Customer.Name))
                .ForMember(m => m.BrokerCompanyKey, opt => opt.MapFrom(m => m.Broker.CompanyKey))
                .ForMember(m => m.BrokerCompanyName, opt => opt.MapFrom(m => m.Broker.Name))
                .ForMember(m => m.ContactName, opt => opt.MapFrom(m => m.ContactName))
                .ForMember(m => m.DistributionWarehouseKey, opt => opt.MapFrom(m => m.DefaultPickFromFacility.FacilityKey))
                .ForMember(m => m.DistributionWarehouseName, opt => opt.MapFrom(m => m.DefaultPickFromFacility.FacilityName));

            Mapper.CreateMap<ICustomerContractDetailReturn, CustomerContractResponse>()
                .ForMember(m => m.ContractDate, opt => opt.MapFrom(m => m.ContractDate))
                .ForMember(m => m.CustomerKey, opt => opt.MapFrom(m => m.Customer.CompanyKey))
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(m => m.Customer.Name))
                .ForMember(m => m.BrokerCompanyKey, opt => opt.MapFrom(m => m.Broker.CompanyKey))
                .ForMember(m => m.BrokerCompanyName, opt => opt.MapFrom(m => m.Broker.Name))
                .ForMember(m => m.DistributionWarehouseKey, opt => opt.MapFrom(m => m.DefaultPickFromFacility.FacilityKey))
                .ForMember(m => m.DistributionWarehouseName, opt => opt.MapFrom(m => m.DefaultPickFromFacility.FacilityName))
                .ForMember(m => m.CommentsNotebookKey, opt => opt.MapFrom(m => m.Comments.NotebookKey))
                .MapLinkedResource();

            Mapper.CreateMap<IContractItemReturn, CustomerContractResponse.ContractItem>()
                .ForMember(m => m.ChileProductClassificationKey, opt => opt.MapFrom(m => m.ChileProduct.ChileTypeKey))
                .ForMember(m => m.ChileProductKey, opt => opt.MapFrom(m => m.ChileProduct.ProductKey))
                .ForMember(m => m.ChileProductName, opt => opt.MapFrom(m => m.ChileProduct.ProductName))
                .ForMember(m => m.ChileProductCode, opt => opt.MapFrom(m => m.ChileProduct.ProductCode))
                .ForMember(m => m.PackagingProductKey, opt => opt.MapFrom(m => m.PackagingProduct.ProductKey))
                .ForMember(m => m.PackagingProductName, opt => opt.MapFrom(m => m.PackagingProduct.ProductName))
                .ForMember(m => m.TreatmentKey, opt => opt.MapFrom(m => m.Treatment.TreatmentKey));

            Mapper.CreateMap<CreateCustomerContractRequest, CreateCustomerContractParameters>()
                .ForMember(m => m.DefaultPickFromFacilityKey, opt => opt.MapFrom(m => m.DistributionWarehouseKey));
            Mapper.CreateMap<CreateCustomerContractRequest.ContractItem, CreateCustomerContractParameters.ContractItem>()
                .ForMember(m => m.CustomerCodeOverride, opt => opt.MapFrom(m => m.CustomerProductCode));

            Mapper.CreateMap<UpdateCustomerContractRequest, UpdateCustomerContractParameters>()
                .ForMember(m => m.ContractKey, opt => opt.Ignore())
                .ForMember(m => m.DefaultPickFromWarehouseKey, opt => opt.MapFrom(m => m.DistributionWarehouseKey));
            Mapper.CreateMap<UpdateCustomerContractRequest.ContractItem, UpdateCustomerContractParameters.ContractItem>()
                .ForMember(m => m.CustomerCodeOverride, opt => opt.MapFrom(m => m.CustomerProductCode));

            Mapper.CreateMap<SetContractsStatusRequest, SetContractsStatusParameters>();

            Mapper.CreateMap<IContractShipmentSummaryReturn, IEnumerable<ContractShipmentSummaryItem>>()
                .ConvertUsing(c => c.Items.Select(i => new ContractShipmentSummaryItem
                {
                    ContractKey = c.ContractKey,
                    ContractNumber = c.ContractNumber.HasValue ? c.ContractNumber.ToString() : string.Empty,
                    ContractStatus = c.ContractStatus.ToString(),

                    ProductName = i.ChileProduct.ProductCodeAndName,
                    CustomerProductCode = i.CustomerProductCode,
                    PackagingName = i.PackagingProduct.ProductName,
                    Treatment = i.Treatment.TreatmentNameShort,

                    ContractBeginDate = c.TermBegin.HasValue ? c.TermBegin.Value.ToShortDateString() : null,
                    ContractEndDate = c.TermEnd.HasValue ? c.TermEnd.Value.ToShortDateString() : null,

                    BasePrice = i.BasePrice,
                    ContractItemValue = i.TotalValue,
                    ContractItemPounds = i.TotalWeight,

                    TotalPoundsShippedForContractItem = i.TotalWeightShipped,
                    TotalPoundsPendingForContractItem = i.TotalWeightPending,
                    TotalPoundsRemainingForContractItem = i.TotalWeightRemaining,
                }));

            Mapper.CreateMap<ISalesOrderItemReturn, SalesOrderPickOrderItemResponse>();

            Mapper.CreateMap<IPickOrderDetailReturn<ISalesOrderItemReturn>, SalesOrderPickOrderDetail>();

            Mapper.CreateMap<ISalesOrderSummaryReturn, SalesOrderSummaryResponse>()
                .ForMember(m => m.ShipmentStatus, opt => opt.MapFrom(m => m.Shipment.Status))
                .ForMember(m => m.OrderStatus, opt => opt.MapFrom(m => m.SalesOrderStatus))
                .ForMember(m => m.OrderNum, opt => opt.MapFrom(m => m.MoveNum));

            Mapper.CreateMap<ISalesOrderDetailReturn, SalesOrderDetailsResponse>()
                .ForMember(m => m.ShipmentDate, opt => opt.MapFrom(m => m.Shipment.ShippingInstructions.ShipmentDate))
                .ForMember(m => m.OrderNum, opt => opt.MapFrom(m => m.MoveNum))
                .ForMember(m => m.IsLocked, opt => opt.ResolveUsing(m => m.OrderStatus == OrderStatus.Fulfilled))
                .AfterMap((s, d) => d.Shipment.ShippingInstructions.ShipFromOrSoldTo = s.ShipFromReplace)
                .MapLinkedResource();

            #endregion

            #region Inventory Treatment Orders

            Mapper.CreateMap<ITreatmentOrderDetailReturn, TreatmentOrderDetail>()
                .ForMember(m => m.ShipmentDate, opt => opt.MapFrom(m => m.Shipment.ShippingInstructions.ShipmentDate))
                .ForMember(m => m.PickOrder, opt => opt.MapFrom(m => m.PickOrder))
                .ForMember(m => m.PickedInventory, opt => opt.MapFrom(m => m.PickedInventory))
                .ForMember(m => m.Shipment, opt => opt.MapFrom(m => m.Shipment))
                .ForMember(m => m.OriginFacility, opt => opt.MapFrom(m => m.OriginFacility))
                .ForMember(m => m.IsLocked, opt => opt.ResolveUsing(m => m.OrderStatus == OrderStatus.Fulfilled))
                .ForMember(m => m.EnableReturnFromTreatment, opt => opt.ResolveUsing(m => m.OrderStatus == OrderStatus.Scheduled && m.Shipment.Status == ShipmentStatus.Shipped))
                .MapLinkedResource()
                .AfterMap((mIn, mOut) =>
                {
                    var picked = mOut.PickedInventory.PickedInventoryItems.ToList();
                    picked.ForEach(i => i.ValidForPicking = !mOut.IsLocked);
                    mOut.PickedInventory.PickedInventoryItems = picked;
                });

            Mapper.CreateMap<ITreatmentOrderSummaryReturn, TreatmentOrderSummary>()
                .ForMember(m => m.Shipment, opt => opt.MapFrom(m => m.Shipment))
                .ForMember(m => m.PickedInventory, opt => opt.MapFrom(m => m.PickedInventory))
                .ForMember(m => m.PickOrder, opt => opt.MapFrom(m => m.PickOrder))
                .ForMember(m => m.OriginFacility, opt => opt.MapFrom(m => m.OriginFacility))
                .ForMember(m => m.EnableReturnFromTreatment, opt => opt.ResolveUsing(m => m.OrderStatus == OrderStatus.Scheduled && m.Shipment.Status == ShipmentStatus.Shipped));

            Mapper.CreateMap<CreateTreatmentOrderRequestParameter, CreateTreatmentOrderParameters>()
                  .ForMember(m => m.UserToken, opt => opt.Ignore())
                  .AfterMap((s, d) => d.SetShipmentInformation.ShippingInstructions.ShipmentDate = s.HeaderParameters.ShipmentDate);

            Mapper.CreateMap<UpdateTreatmentOrderRequestParameter, UpdateTreatmentOrderParameters>()
                .ForMember(m => m.UserToken, opt => opt.Ignore())
                .AfterMap((s, d) => d.SetShipmentInformation.ShippingInstructions.ShipmentDate = s.HeaderParameters.ShipmentDate);

            Mapper.CreateMap<ReceiveTreatmentOrderRequestParameter, ReceiveTreatmentOrderParameters>()
                  .ForMember(m => m.UserToken, opt => opt.Ignore());

            #endregion

            #region Shipments

            Mapper.CreateMap<SetShipmentInformationRequestParameter, SetInventoryShipmentInformationParameters>()
                .ForMember(m => m.InventoryShipmentOrderKey, opt => opt.Ignore())
                .ForMember(m => m.TransitInformation, opt => opt.MapFrom(m => m.Transit));
            Mapper.CreateMap<SetShippingInstructionsRequestParameter, SetShippingInstructionsParameters>()
                .Ignoring(m => m.ShipmentDate)
                .ForMember(m => m.FreightBillTo, opt => opt.MapFrom(m => m.FreightBill));
            Mapper.CreateMap<SetTransitInformationRequestParameter, SetTransitInformationParameter>();

            Mapper.CreateMap<IShipmentSummaryReturn, ShipmentSummary>().
                ForMember(m => m.ScheduledShipDate, opt => opt.ResolveUsing(m => m.ShipmentDate));

            Mapper.CreateMap<IShipmentDetailReturn, ShipmentDetails>()
                .ForMember(m => m.Transit, opt => opt.MapFrom(m => m.TransitInformation));
            
            Mapper.CreateMap<IShippingInstructions, ShippingInstructions>()
                .ForMember(m => m.ScheduledShipDateTime, opt => opt.MapFrom(m => m.ShipmentDate))
                .ForMember(m => m.ShipFromOrSoldTo, opt => opt.MapFrom(m => m.ShipFromOrSoldToShippingLabel))
                .ForMember(m => m.FreightBill, opt => opt.MapFrom(m => m.FreightBillToShippingLabel))
                .ForMember(m => m.ShipTo, opt => opt.MapFrom(m => m.ShipToShippingLabel));

            Mapper.CreateMap<ITransitInformation, TransitInformation>();
            Mapper.CreateMap<ITransitInformation, TransitViewModel>()
                .ConstructUsing(TransitViewModel.Create);

            Mapper.CreateMap<PostItemDestinationsRequestParameter, PostItemParameters>();
            Mapper.CreateMap<PostAndCloseShipmentOrderRequestParameter, PostParameters>()
                .ForMember(m => m.UserToken, opt => opt.Ignore())
                .ForMember(m => m.OrderKey, opt => opt.Ignore());

            #endregion

            #region Warehouses / Warehouse Locations

            Mapper.CreateMap<IFacilitySummaryReturn, FacilityResponse>()
                .ForMember(m => m.ShippingLabel, opt => opt.Ignore());

            Mapper.CreateMap<IFacilityDetailReturn, FacilityResponse>();

            Mapper.CreateMap<IFacilityDetailReturn, FacilityDetailsResponse>();

            Mapper.CreateMap<SaveFacilityParameter, CreateFacilityParameters>()
                .ForMember(m => m.Name, opt => opt.MapFrom(m => m.FacilityName));

            Mapper.CreateMap<SaveFacilityParameter, UpdateFacilityParameters>()
                .ForMember(m => m.Name, opt => opt.MapFrom(m => m.FacilityName))
                .ForMember(m => m.FacilityKey, opt => opt.Ignore());
            
            Mapper.CreateMap<ILocationReturn, WarehouseLocationResponse>()
                .ForMember(m => m.WarehouseKey, opt => opt.MapFrom(m => m.FacilityKey))
                .ForMember(m => m.WarehouseName, opt => opt.MapFrom(m => m.FacilityName))
                .ForMember(m => m.WarehouseLocationKey, opt => opt.MapFrom(m => m.LocationKey))
                .ForMember(m => m.LocationName, opt => opt.MapFrom(m => m.Description));

            Mapper.CreateMap<ILocationReturn, FacilityLocationResponse>()
                .ForMember(m => m.GroupName, opt => opt.Ignore())
                .ForMember(m => m.Row, opt => opt.Ignore());

            Mapper.CreateMap<UpdateLocationParameter, UpdateLocationParameters>()
                .ForMember(m => m.Description, opt => opt.ResolveUsing(m => LocationDescriptionHelper.GetDescription(m.GroupName, m.Row)))
                .ForMember(m => m.Active, opt => opt.ResolveUsing(m => m.Status != LocationStatus.InActive))
                .ForMember(m => m.Locked, opt => opt.ResolveUsing(m => m.Status == LocationStatus.Locked));

            Mapper.CreateMap<CreateLocationParameter, CreateLocationParameters>()
                .ForMember(m => m.Description, opt => opt.ResolveUsing(m => LocationDescriptionHelper.GetDescription(m.GroupName, m.Row)))
                .ForMember(m => m.Active, opt => opt.ResolveUsing(m => m.Status != LocationStatus.InActive))
                .ForMember(m => m.Locked, opt => opt.ResolveUsing(m => m.Status == LocationStatus.Locked));

            #endregion

            Mapper.CreateMap<IInventoryTransactionReturn, LotInventoryTransactionResponse>()
                .ForMember(m => m.TransactionDate, opt => opt.ResolveUsing(m => m.TimeStamp))
                .ForMember(m => m.FacilityName, opt => opt.ResolveUsing(m => m.Location.FacilityName))
                .ForMember(m => m.ProductName, opt => opt.ResolveUsing(m => m.Product.ProductName))
                .ForMember(m => m.Treatment, opt => opt.MapFrom(m => m.Treatment.TreatmentNameShort))
                .ForMember(m => m.TransactionSourceReferenceKey, opt => opt.ResolveUsing(m => m.SourceReference));

            Mapper.CreateMap<IInventoryTransactionsByLotReturn, LotInputResponse>()
                .ForMember(m => m.InputItems, opt => opt.ResolveUsing(ToLotInputResponse));

            Mapper.CreateMap<ICustomerProductCodeReturn, CustomerProductCodeResponse>();

            #region Sample Requests

            Mapper.CreateMap<ISampleOrderSummaryReturn, SampleRequestSummaryResponse>();

            Mapper.CreateMap<ISampleOrderItemMatchReturn, SampleRequestItemLabResultsResponse>();
            Mapper.CreateMap<ISampleOrderItemSpecReturn, SampleRequestItemSpecResponse>();
            Mapper.CreateMap<ISampleOrderItemReturn, SampleRequestItemResponse>()
                .Ignoring(m => m.Links);
            Mapper.CreateMap<ISampleOrderJournalEntryReturn, SampleRequestJournalEntryResponse>();
            Mapper.CreateMap<ISampleOrderDetailReturn, SampleRequestDetailResponse>()
                .Ignoring(m => m.Links);

            Mapper.CreateMap<CreateSampleOrderRequest, SetSampleOrderParameters>()
                 .ForMember(m => m.ShipmentMethod, opt => opt.MapFrom(m => m.ShipVia))
                 .Ignoring(m => m.UserToken, m => m.SampleOrderKey);
            Mapper.CreateMap<CreateSampleItemRequest, SampleOrderItemParameters>()
                  .Ignoring(m => m.SampleOrderItemKey);

            Mapper.CreateMap<SetSampleOrderRequest, SetSampleOrderParameters>()
                .ForMember(m => m.ShipmentMethod, opt => opt.MapFrom(m => m.ShipVia))
                .Ignoring(m => m.UserToken, m => m.SampleOrderKey);
            Mapper.CreateMap<UpdateSampleItemRequest, SampleOrderItemParameters>();

            Mapper.CreateMap<SetSampleSpecsRequest, SetSampleSpecsParameters>()
                .Ignoring(m => m.SampleOrderItemKey);
            Mapper.CreateMap<SetSampleMatchRequest, SetSampleMatchParameters>()
                .Ignoring(m => m.SampleOrderItemKey);
            Mapper.CreateMap<SetJournalEntryRequest, SetSampleOrderJournalEntryParameters>()
                .Ignoring(m => m.UserToken, m => m.SampleOrderKey, m => m.JournalEntryKey);

            #endregion

            #region Sales Quotes

            Mapper.CreateMap<ISalesQuoteSummaryReturn, SalesQuoteSummaryResponse>()
                .ForMember(m => m.ScheduledShipDate, opt => opt.ResolveUsing(m => m.ShipmentDate));
            Mapper.CreateMap<ISalesQuoteDetailReturn, SalesQuoteDetailResponse>()
                .MapLinkedResource()
                .AfterMap((s, d) =>
                    {
                        d.Shipment.ShippingInstructions.ShipFromOrSoldTo = s.ShipFromReplace;
                        d.Links = new ResourceLinkCollection();
                    });
            Mapper.CreateMap<ISalesQuoteItemReturn, SalesQuoteItemResponse>();

            Mapper.CreateMap<CreateSalesQuoteRequest, SalesQuoteParameters>()
                .Ignoring(m => m.UserToken, m => m.SalesQuoteNumber)
                .AfterMap((s, d) =>
                    {
                        if(d.ShipmentInformation != null && d.ShipmentInformation.ShippingInstructions != null)
                        {
                            if(s.ShipmentInformation != null && s.ShipmentInformation.ShippingInstructions != null)
                            {
                                d.ShipmentInformation.ShippingInstructions.ShipmentDate = s.ShipmentInformation.ShippingInstructions.ScheduledShipDateTime;
                            }
                        }
                    });
            Mapper.CreateMap<CreateSalesQuoteItemRequest, SalesQuoteItemParameters>()
                .Ignoring(m => m.SalesQuoteItemKey);

            Mapper.CreateMap<UpdateSalesQuoteRequest, SalesQuoteParameters>()
                .Ignoring(m => m.UserToken, m => m.SalesQuoteNumber)
                .AfterMap((s, d) =>
                    {
                        if(d.ShipmentInformation != null && d.ShipmentInformation.ShippingInstructions != null)
                        {
                            if(s.ShipmentInformation != null && s.ShipmentInformation.ShippingInstructions != null)
                            {
                                d.ShipmentInformation.ShippingInstructions.ShipmentDate = s.ShipmentInformation.ShippingInstructions.ScheduledShipDateTime;
                            }
                        }
                    });
            Mapper.CreateMap<UpdateSalesQuoteItemRequest, SalesQuoteItemParameters>();

            #endregion


            Mapper.AssertConfigurationIsValid();
        }

        private static IEnumerable<LotInventoryTransactionResponse> ToLotInputResponse(IInventoryTransactionsByLotReturn input) {
            var response = input.InputItems.Project().To<LotInventoryTransactionResponse>().ToList();
            response.ForEach(r =>
                {
                    r.Quantity = r.Quantity * -1;
                    r.Weight = r.Weight * -1;
                });

            return response;
        }

        private static IMappingExpression<TSource, TDest> MapInventorySummaryType<TSource, TDest>()
            where TSource : IInventorySummaryReturn
            where TDest : InventoryItem
        {
            return Mapper.CreateMap<TSource, TDest>()
                .ForMember(m => m.LotStatus, opt => opt.MapFrom(dto => dto.QualityStatus))
                .ForMember(m => m.LotProductionStatus, opt => opt.MapFrom(dto => dto.ProductionStatus))
                .ForMember(m => m.Product, opt => opt.MapFrom(dto => dto.LotProduct))
                .ForMember(m => m.ReceivedPackagingName, opt => opt.MapFrom(dto => dto.PackagingReceived.ProductName))
                .ForMember(m => m.CustomerKey, opt => opt.MapFrom(dto => dto.Customer.CompanyKey))
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(dto => dto.Customer.Name))
                .AfterMap((s, d) => d.Initialize(s.AstaCalcDate));
        }
    }
}