namespace RMQGrainsBeta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {

           
            CreateTable(
                "dbo.Cements",
                c => new
                    {
                        CementID = c.Int(nullable: false, identity: true),
                        SOnumber = c.Int(nullable: false),
                        CompanyName = c.String(),
                        BuyingTo = c.String(),
                        CurrentDate = c.DateTime(nullable: false),
                        CementType = c.String(),
                        TotalQuantity = c.Int(nullable: false),
                        PaymentMethod = c.Int(nullable: false),
                        ChequeNumber = c.Int(nullable: false),
                        Remarks = c.String(),
                        ImageLocation = c.String(),
                    })
                .PrimaryKey(t => t.CementID);
            
            CreateTable(
                "dbo.Deliveries",
                c => new
                    {
                        DeliveryID = c.Int(nullable: false, identity: true),
                        Hauling = c.Int(nullable: false),
                        HaulerCompany = c.String(),
                        PlateNumber = c.String(),
                        DriverName = c.String(),
                        QuantityToDeliver = c.Int(nullable: false),
                        Destination = c.Int(nullable: false),
                        DateOfArrival = c.DateTime(nullable: false),
                        DeliveryTo = c.String(),
                        DRNumber = c.String(),
                        CementID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DeliveryID)
                .ForeignKey("dbo.Cements", t => t.CementID, cascadeDelete: true)
                .Index(t => t.CementID);
            
            CreateTable(
                "dbo.Expenses",
                c => new
                    {
                        ExpenseID = c.Int(nullable: false, identity: true),
                        Expenses = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                        PaymentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ExpenseID)
                .ForeignKey("dbo.Payments", t => t.PaymentID, cascadeDelete: true)
                .Index(t => t.PaymentID);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        BankName = c.String(),
                        Branch = c.String(),
                        TermsOfPayment = c.Int(nullable: false),
                        DateofCheque = c.DateTime(nullable: false),
                        DeliveryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentID)
                .ForeignKey("dbo.Deliveries", t => t.DeliveryID, cascadeDelete: true)
                .Index(t => t.DeliveryID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Expenses", "PaymentID", "dbo.Payments");
            DropForeignKey("dbo.Payments", "DeliveryID", "dbo.Deliveries");
            DropForeignKey("dbo.Deliveries", "CementID", "dbo.Cements");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Payments", new[] { "DeliveryID" });
            DropIndex("dbo.Expenses", new[] { "PaymentID" });
            DropIndex("dbo.Deliveries", new[] { "CementID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Payments");
            DropTable("dbo.Expenses");
            DropTable("dbo.Deliveries");
            DropTable("dbo.Cements");
        }
    }
}
