namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InstructionType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Instructions", "TypeId", "dbo.InstructionTypes");
            DropIndex("dbo.Instructions", new[] { "TypeId" });
            AddColumn("dbo.Instructions", "InstructionType", c => c.Int(nullable: false));
            DropColumn("dbo.Instructions", "TypeId");
            DropTable("dbo.InstructionTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.InstructionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Instructions", "TypeId", c => c.Int(nullable: false));
            DropColumn("dbo.Instructions", "InstructionType");
            CreateIndex("dbo.Instructions", "TypeId");
            AddForeignKey("dbo.Instructions", "TypeId", "dbo.InstructionTypes", "Id");
        }
    }
}
