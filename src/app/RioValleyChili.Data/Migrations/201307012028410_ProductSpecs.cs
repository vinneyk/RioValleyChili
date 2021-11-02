namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ProductSpecs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChileProductAttributeRanges", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.ChileProductAttributeRanges", "TimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.ChileProductIngredients", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.ChileProductIngredients", "TimeStamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChileProductIngredients", "TimeStamp");
            DropColumn("dbo.ChileProductIngredients", "User");
            DropColumn("dbo.ChileProductAttributeRanges", "TimeStamp");
            DropColumn("dbo.ChileProductAttributeRanges", "User");
        }
    }
}
