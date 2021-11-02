using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data
{
    public class RioValleyChiliDataContext : DbContext, IBulkInsertContext
    {
        private string _connectionString;

        #region Constructors

        public RioValleyChiliDataContext()
        {
            Initialize();
        }


        public RioValleyChiliDataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Initialize();
        }

        #endregion

        private void Initialize()
        {
            Configuration.LazyLoadingEnabled = false;
            _connectionString = Database.Connection.ConnectionString;
        }

        #region Configuration

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            ConfigureRelationshipConstraints(modelBuilder);
            ConfigureTableNaming(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureTableNaming(DbModelBuilder modelBuilder)
        {
        }

        private static void ConfigureRelationshipConstraints(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PackagingProduct>()
                .HasRequired(p => p.Product)
                .WithOptional();

            modelBuilder.Entity<ChileProduct>()
                .HasRequired(p => p.Product)
                .WithOptional();

            modelBuilder.Entity<AdditiveProduct>()
                .HasRequired(p => p.Product)
                .WithOptional();

            modelBuilder.Entity<ChileLot>()
                .HasRequired(c => c.Lot)
                .WithOptional(l => l.ChileLot);

            modelBuilder.Entity<AdditiveLot>()
                .HasRequired(c => c.Lot)
                .WithOptional(l => l.AdditiveLot);

            modelBuilder.Entity<PackagingLot>()
                .HasRequired(c => c.Lot)
                .WithOptional(l => l.PackagingLot);

            modelBuilder.Entity<LotDefect>()
                .HasOptional(d => d.Resolution);

            modelBuilder.Entity<LotDefectResolution>()
                .HasRequired(r => r.Defect)
                .WithOptional(d => d.Resolution);

            modelBuilder.Entity<ProductionBatch>()
                .HasRequired(b => b.PackSchedule)
                .WithMany(p => p.ProductionBatches)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChileLotProduction>()
                .HasRequired(p => p.ResultingChileLot)
                .WithOptional(c => c.Production);

            modelBuilder.Entity<ChileLotProduction>()
                .HasOptional(p => p.Results)
                .WithRequired(c => c.Production);

            modelBuilder.Entity<PickedInventoryItem>()
                .HasRequired(i => i.PickedInventory)
                .WithMany(i => i.Items)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChileProductIngredient>()
                .HasRequired(ingredient => ingredient.ChileProduct)
                .WithMany(prod => prod.Ingredients);

            modelBuilder.Entity<ChileProductIngredient>()
                .HasRequired(ingredient => ingredient.AdditiveType)
                .WithMany();

            modelBuilder.Entity<ChileMaterialsReceived>()
                .HasRequired(d => d.ChileLot)
                .WithOptional();

            modelBuilder.Entity<Customer>()
                .HasRequired(c => c.Company)
                .WithOptional(c => c.Customer);

            modelBuilder.Entity<SampleOrderItemSpec>()
                .HasRequired(s => s.Item)
                .WithOptional(c => c.Spec);

            modelBuilder.Entity<SampleOrderItemMatch>()
                .HasRequired(s => s.Item)
                .WithOptional(c => c.Match);
        }

        #endregion

        #region Table Properties

        public DbSet<DataLoadResult> DataLoadResult { get; set; }
        public DbSet<Employee> Employees { get; set; }

        #region DbSet properties

        #region inventory DbSet properties

        public DbSet<Product> Products { get; set; }
        public DbSet<ChileProduct> ChileProducts { get; set; }
        public DbSet<PackagingProduct> PackagingProducts { get; set; }
        public DbSet<AdditiveProduct> AdditiveProducts { get; set; }
        public DbSet<AdditiveType> AdditiveTypes { get; set; }
        public DbSet<ChileType> ChileTypes { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<LotStatusName> LotStatusNames { get; set; }
        public DbSet<LotStatusValue> LotStatusValue { get; set; }
        public DbSet<LotDefect> LotDefects { get; set; }
        public DbSet<LotDefectResolution> LotDefectResolutions { get; set; }
        public DbSet<LotAttributeDefect> LotProductSpecDefects { get; set; }
        public DbSet<AttributeName> AttributeNames { get; set; }
        public DbSet<ChileLot> ChileLots { get; set; }
        public DbSet<PackagingLot> PackagingLots { get; set; }
        public DbSet<AdditiveLot> AdditiveLots { get; set; }
        public DbSet<Inventory> Inventory { get; set; }

        #endregion

        #region production DbSet properties

        public DbSet<ChileProductIngredient> ChileProductIngredients { get; set; }
        public DbSet<PackSchedule> PackSchedules { get; set; }
        public DbSet<ProductionBatch> ProductionBatches { get; set; }
        public DbSet<ProductionSchedule> ProductionSchedules { get; set; }
        public DbSet<ProductionScheduleItem> ProductionScheduleItems { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<ChileMaterialsReceived> DehydratedMaterialsReceived { get; set; }
        public DbSet<ChileMaterialsReceivedItem> DehydratedMaterialsReceivedItems { get; set; }

        #endregion

        #region Inventory Transactions properties.

        public DbSet<Location> Locations { get; set; }
        public DbSet<PickedInventory> InventoryMovements { get; set; }
        public DbSet<PickedInventoryItem> InventoryMovementItems { get; set; }
        public DbSet<InventoryPickOrder> MovementOrders { get; set; }
        public DbSet<InventoryPickOrderItem> MovementOrderItems { get; set; }
        public DbSet<TreatmentOrder> TreatmentOrders { get; set; }
        public DbSet<IntraWarehouseOrder> IntraWarehouseMovementOrders { get; set; }
        public DbSet<ShipmentInformation> ShipmentInformation { get; set; }
        public DbSet<InventoryAdjustment> InventoryAdjustments { get; set; }

        #endregion

        #endregion

        #region Customer Contracts properties.

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerProductCode> CustomerProductCodes { get; set; }
        public DbSet<CustomerProductAttributeRange> CustomerProductAttributeRanges { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractItem> ContractItems { get; set; }

        #endregion

        #region Customer Orders properties.

        public DbSet<SalesOrder> CustomerOrders { get; set; }
        public DbSet<SalesOrderItem> CustomerOrderItems { get; set; }
        public DbSet<SalesOrderPickedItem> CustomerOrderPickedItems { get; set; }

        #endregion

        public DbSet<Company> Companies { get; set; }
        public DbSet<InventoryShipmentOrder> InventoryShipmentOrders { get; set; }

        public DbSet<SalesQuote> SalesQuotes { get; set; }
        public DbSet<SalesQuoteItem> SalesQuoteItems { get; set; }

        #endregion

        public DbSet<SampleOrder> SampleOrders { get; set; }

        #region IBulkInsertContext properties

        string IBulkInsertContext.ConnectionString { get { return _connectionString; } }
        ObjectContext IBulkInsertContext.ObjectContext { get { return ((IObjectContextAdapter) this).ObjectContext; } }

        #endregion
    }
}
