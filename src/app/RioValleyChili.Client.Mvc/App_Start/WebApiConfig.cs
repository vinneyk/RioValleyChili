using System.Web.Http;
using System.Web.Http.Routing;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            RegisterConstraintResolver(config);

            RegisterEmployeeRoutes(config);
            RegisterProductRoutes(config);
            RegisterLotInventoryRoutes(config);
            RegisterQualityControlRoutes(config);
            RegisterInventoryOrderRoutes(config);
            RegisterInventoryMovementRoutes(config);
            RegisterCustomerRoutes(config);
            RegisterContactRoutes(config);

            config.Routes.MapHttpRoute(
                name: "Notes",
                routeTemplate: "api/notebooks/{notebookKey}/notes/{id}",
                defaults: new { controller = "Notes", id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "ProductionResultsDefault",
                routeTemplate: "api/productionresults/{id}",
                defaults: new { controller = "ProductionResults", id = RouteParameter.Optional });
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
        }

        private static void RegisterConstraintResolver(HttpConfiguration config)
        {
            var constraintsResolver = new DefaultInlineConstraintResolver();
            constraintsResolver.ConstraintMap.Add("values", typeof(ValuesConstraint));
            config.MapHttpAttributeRoutes(constraintsResolver);
        }

        private static void RegisterEmployeeRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "EmployeeClaims",
                routeTemplate: "api/employees/{employeeKey}/claims",
                defaults: new {controller = "EmployeeClaims", employeeKey = RouteParameter.Optional}
            );
        }

        private static void RegisterProductRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "ProductSpecsDefault",
                routeTemplate: "api/products/{productKey}/specs/{attributeNameKey}",
                defaults: new { controller = "ProductSpecs", productKey = RouteParameter.Optional, attributeNameKey = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                name: "ProductIngredientsDefault",
                routeTemplate: "api/products/{productKey}/ingredients/{id}",
                defaults: new { controller = "ProductIngredients", id = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                name: "ProductsDefault",
                routeTemplate: "api/products/{lotType}/{id}",
                defaults: new { controller = "Products", id = RouteParameter.Optional });
        }

        private static void RegisterLotInventoryRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "PickedInventoryCalculations",
                routeTemplate: "api/inventory/pickedCalculations",
                defaults: new { controller = "PickedInventoryCalculations" });

            config.Routes.MapHttpRoute(
                name: "InventoryByKey",
                routeTemplate: "api/inventory/{lotKey}",
                defaults: new {controller = "Inventory"});

            config.Routes.MapHttpRoute(
                name: "Inventory",
                routeTemplate: "api/inventory/{productType}",
                defaults: new {controller = "Inventory", productType = ProductTypeEnum.Chile});
        }
        
        private static void RegisterQualityControlRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "LotDefects",
                routeTemplate: "api/lots/{lotKey}/defects/{id}",
                defaults: new { controller = "LotDefects", id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "LotHolds",
                routeTemplate: "api/lots/{lotKey}/holds",
                defaults: new { controller = "LotHolds" });

            config.Routes.MapHttpRoute(
                name: "LotsDefault",
                routeTemplate: "api/lots/{lotKey}",
                defaults: new { controller = "Lots", lotKey = RouteParameter.Optional });
        }
        
        private static void RegisterInventoryOrderRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "InventoryPickOrders",
                routeTemplate: "api/{inventoryOrder}/pickOrders/{id}", 
                defaults: new { controller = "InventoryOrders", id = RouteParameter.Optional }
                );
        }

        private static void RegisterInventoryMovementRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "IntraWarehouseMovements",
                routeTemplate: string.Format("api/movements/{0}/{{id}}", InventoryOrderEnum.WarehouseMovements),
                defaults: new { controller = "IntraWarehouseInventoryMovements", id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "InterWarehouseMovements",
                routeTemplate: string.Format("api/movements/{0}/{{id}}", InventoryOrderEnum.TransWarehouseMovements),
                defaults: new {controller = "TransWarehouseInventoryMovements", id = RouteParameter.Optional});
        }

        private static void RegisterCustomerRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "CustomerContractsByCustomer",
                routeTemplate: "api/customers/{customerKey}/contracts/{id}",
                defaults: new { controller = "CustomerContracts", customerKey = RouteParameter.Optional, id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "CustomerContractsDefault",
                routeTemplate: "api/contracts/{id}",
                defaults: new { controller = "CustomerContracts", id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "BrokerCustomersDefault",
                routeTemplate: "api/brokers/{brokerKey}/customers/{id}",
                defaults: new {controller = "BrokerCustomers", id = RouteParameter.Optional });
        }

        private static void RegisterContactRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "ContactsByCompany",
                routeTemplate: "api/companies/{companyKey}/contacts/{id}",
                defaults: new { controller = "Contacts", companyKey = RouteParameter.Optional, id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "ContactsDefault",
                routeTemplate: "api/contacts/{id}",
                defaults: new { controller = "Contacts", id = RouteParameter.Optional });
        }
    }
}