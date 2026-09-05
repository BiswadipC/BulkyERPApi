using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models;

public partial class BulkyContext : DbContext
{
    public BulkyContext()
    {
    }

    public BulkyContext(DbContextOptions<BulkyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountsCategoryMaster> AccountsCategoryMasters { get; set; }

    public virtual DbSet<AccountsPo> AccountsPos { get; set; }

    public virtual DbSet<AttrHead> AttrHeads { get; set; }

    public virtual DbSet<Attrdtl> Attrdtls { get; set; }

    public virtual DbSet<ItemDtl> ItemDtls { get; set; }

    public virtual DbSet<ItemGst> ItemGsts { get; set; }

    public virtual DbSet<ItemHead> ItemHeads { get; set; }

    public virtual DbSet<ItemOpStock> ItemOpStocks { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<ModulePolicyMapping> ModulePolicyMappings { get; set; }

    public virtual DbSet<PartyMaster> PartyMasters { get; set; }

    public virtual DbSet<PolicyDetail> PolicyDetails { get; set; }

    public virtual DbSet<PostOffice> PostOffices { get; set; }

    public virtual DbSet<PurBillDtl> PurBillDtls { get; set; }

    public virtual DbSet<PurBillHead> PurBillHeads { get; set; }

    public virtual DbSet<PurOrderDtl> PurOrderDtls { get; set; }

    public virtual DbSet<PurOrderHead> PurOrderHeads { get; set; }

    public virtual DbSet<StockDtl> StockDtls { get; set; }

    public virtual DbSet<SystemVariable> SystemVariables { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("server=flower; database=bulky; User Id=sa;Password=welcome@123; encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__349DA5A6168B75DD");

            entity.HasIndex(e => e.AccountName, "UQ__Accounts__406E0D2E7FC2BAE1").IsUnique();

            entity.Property(e => e.AccountName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.AccountNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Add1)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.BranchCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Category)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IFSCCode");
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Pin)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Schedule)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TaxStructure)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AccountsCategoryMaster>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__Accounts__B773C9997AE6DFD3");

            entity.ToTable("AccountsCategoryMaster");

            entity.Property(e => e.CategoryCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AccountsPo>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__Accounts__B773C999C4FFF383");

            entity.Property(e => e.Credit)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.Debit)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.DocNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("docNo");
            entity.Property(e => e.DrCr)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AttrHead>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__AttrHead__B773C999296189D4");

            entity.ToTable("AttrHead");

            entity.HasIndex(e => e.AttrName, "UQ__AttrHead__3DB14A841F612406").IsUnique();

            entity.Property(e => e.AttrName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Attrdtl>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__Attrdtl__B773C99994CC44B9");

            entity.ToTable("Attrdtl");

            entity.Property(e => e.AttrValue)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.AttrHeadIdNoNavigation).WithMany(p => p.Attrdtls)
                .HasForeignKey(d => d.AttrHeadIdNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attrdtl__AttrHea__5165187F");
        });

        modelBuilder.Entity<ItemDtl>(entity =>
        {
            entity.HasKey(e => e.Idno).HasName("PK__ItemDtl__B770CDB17B191E49");

            entity.ToTable("ItemDtl");

            entity.HasOne(d => d.AttrDtlIdNoNavigation).WithMany(p => p.ItemDtls)
                .HasForeignKey(d => d.AttrDtlIdNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemDtl__AttrDtl__778AC167");

            entity.HasOne(d => d.AttrHeadIdNoNavigation).WithMany(p => p.ItemDtls)
                .HasForeignKey(d => d.AttrHeadIdNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemDtl__AttrHea__76969D2E");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemDtls)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemDtl__ItemId__75A278F5");
        });

        modelBuilder.Entity<ItemGst>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__ItemGST__B773C999FAB51EC3");

            entity.ToTable("ItemGST");

            entity.Property(e => e.PurCgstperc)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("PurCGSTPerc");
            entity.Property(e => e.PurIgstperc)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("PurIGSTPerc");
            entity.Property(e => e.PurSgstperc)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("PurSGSTPerc");
            entity.Property(e => e.SalesCgstperc)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("SalesCGSTPerc");
            entity.Property(e => e.SalesIgstperc)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("SalesIGSTPerc");
            entity.Property(e => e.SalesSgstperc)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("SalesSGSTPerc");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemGsts)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemGST__ItemId__7B5B524B");
        });

        modelBuilder.Entity<ItemHead>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__ItemHead__727E838BC00D679E");

            entity.ToTable("ItemHead");

            entity.HasIndex(e => e.ItemName, "UQ__ItemHead__4E4373F7E38A0BAD").IsUnique();

            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Prate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("PRate");
            entity.Property(e => e.ReOrderLevel)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.Srate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("SRate");
        });

        modelBuilder.Entity<ItemOpStock>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__ItemOpSt__B773C99976CD9A08");

            entity.ToTable("ItemOpStock");

            entity.Property(e => e.Amount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.Rate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemOpStocks)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__ItemOpSto__ItemI__0E6E26BF");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleName).HasName("PK__Modules__EAC9AEC27852F62C");

            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ModulePolicyMapping>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__ModulePo__B773C9992F105B4F");

            entity.Property(e => e.IsAdmin)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("No");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PermissionType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PolicyName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.UserIdNoNavigation).WithMany(p => p.ModulePolicyMappings)
                .HasForeignKey(d => d.UserIdNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ModulePol__UserI__619B8048");
        });

        modelBuilder.Entity<PartyMaster>(entity =>
        {
            entity.HasKey(e => e.PartyCode).HasName("PK__PartyMas__39A9713CBC26F32B");

            entity.ToTable("PartyMaster");

            entity.HasIndex(e => e.PartyName, "UQ__PartyMas__A9C886321B3FFC82").IsUnique();

            entity.Property(e => e.Add1)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Add2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DrugLicenceNo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Gstno)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("GSTNo");
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PartyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Pin)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PolicyDetail>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__PolicyDe__B773C9994FCA2837");

            entity.HasIndex(e => e.PolicyNo, "UQ__PolicyDe__2E13219608C14B65").IsUnique();

            entity.Property(e => e.MaturityAmount).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.PolicyAmount).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.PolicyNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.PoIdNoNavigation).WithMany(p => p.PolicyDetails)
                .HasForeignKey(d => d.PoIdNo)
                .HasConstraintName("FK__PolicyDet__PoIdN__2057CCD0");
        });

        modelBuilder.Entity<PostOffice>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__PostOffi__B773C99902DB02FA");

            entity.ToTable("PostOffice");

            entity.HasIndex(e => e.PoName, "UQ__PostOffi__89E4EE1197FCCEE8").IsUnique();

            entity.Property(e => e.PoName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PurBillDtl>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__PurBillD__B773C99969414BC0");

            entity.ToTable("PurBillDtl");

            entity.Property(e => e.Amount).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.AmountAfterDiscount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.BillNo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Cgst)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("CGST");
            entity.Property(e => e.CgstledgerId).HasColumnName("CGSTLedgerId");
            entity.Property(e => e.DiscountPerc)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.DiscountValue)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.Igst)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("IGST");
            entity.Property(e => e.IgstledgerId).HasColumnName("IGSTLedgerId");
            entity.Property(e => e.PodtlIdNo).HasColumnName("PODtlIdNo");
            entity.Property(e => e.Rate).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.Sgst)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("SGST");
            entity.Property(e => e.SgstledgerId).HasColumnName("SGSTLedgerId");
            entity.Property(e => e.TotalAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");

            entity.HasOne(d => d.Bill).WithMany(p => p.PurBillDtls)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurBillDt__BillI__7FEAFD3E");

            entity.HasOne(d => d.Cgstledger).WithMany(p => p.PurBillDtlCgstledgers)
                .HasForeignKey(d => d.CgstledgerId)
                .HasConstraintName("FK__PurBillDt__CGSTL__01D345B0");

            entity.HasOne(d => d.DiscountLedger).WithMany(p => p.PurBillDtlDiscountLedgers)
                .HasForeignKey(d => d.DiscountLedgerId)
                .HasConstraintName("FK__PurBillDt__Disco__078C1F06");

            entity.HasOne(d => d.Igstledger).WithMany(p => p.PurBillDtlIgstledgers)
                .HasForeignKey(d => d.IgstledgerId)
                .HasConstraintName("FK__PurBillDt__IGSTL__03BB8E22");

            entity.HasOne(d => d.Item).WithMany(p => p.PurBillDtls)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurBillDt__ItemI__00DF2177");

            entity.HasOne(d => d.Order).WithMany(p => p.PurBillDtls)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__PurBillDt__Order__2CBDA3B5");

            entity.HasOne(d => d.PodtlIdNoNavigation).WithMany(p => p.PurBillDtls)
                .HasForeignKey(d => d.PodtlIdNo)
                .HasConstraintName("FK__PurBillDt__PODtl__0C50D423");

            entity.HasOne(d => d.Sgstledger).WithMany(p => p.PurBillDtlSgstledgers)
                .HasForeignKey(d => d.SgstledgerId)
                .HasConstraintName("FK__PurBillDt__SGSTL__02C769E9");
        });

        modelBuilder.Entity<PurBillHead>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__PurBillH__11F2FC6A78DB80E6");

            entity.ToTable("PurBillHead");

            entity.HasIndex(e => e.BillNo, "UQ__PurBillH__11F2841831749111").IsUnique();

            entity.Property(e => e.AccountsAdjAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.BillNo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NetAmount).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithMany(p => p.PurBillHeadAccounts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurBillHe__Accou__56E8E7AB");

            entity.HasOne(d => d.PartyCodeNavigation).WithMany(p => p.PurBillHeads)
                .HasForeignKey(d => d.PartyCode)
                .HasConstraintName("FK__PurBillHe__Party__55F4C372");

            entity.HasOne(d => d.PurAccount).WithMany(p => p.PurBillHeadPurAccounts)
                .HasForeignKey(d => d.PurAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurBillHe__PurAc__57DD0BE4");
        });

        modelBuilder.Entity<PurOrderDtl>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__PurOrder__B773C999259B9FBB");

            entity.ToTable("PurOrderDtl");

            entity.Property(e => e.Amount).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.OrderNo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Rate).HasColumnType("decimal(20, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.PurOrderDtls)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurOrderD__ItemI__46B27FE2");

            entity.HasOne(d => d.Order).WithMany(p => p.PurOrderDtls)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__PurOrderD__Order__45BE5BA9");
        });

        modelBuilder.Entity<PurOrderHead>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__PurOrder__C3905BCF86626C3D");

            entity.ToTable("PurOrderHead");

            entity.HasIndex(e => e.OrderNo, "UQ__PurOrder__C3907C7558FF5BCD").IsUnique();

            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderNo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(20, 2)");

            entity.HasOne(d => d.PartyCodeNavigation).WithMany(p => p.PurOrderHeads)
                .HasForeignKey(d => d.PartyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurOrderH__Party__41EDCAC5");
        });

        modelBuilder.Entity<StockDtl>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__StockDtl__B773C999458825B0");

            entity.ToTable("StockDtl");

            entity.Property(e => e.Amount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.DocNo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InQty).HasDefaultValue(0);
            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Mrp)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("MRP");
            entity.Property(e => e.OutQty).HasDefaultValue(0);
            entity.Property(e => e.Rate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.StockDtls)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__StockDtl__ItemId__07C12930");
        });

        modelBuilder.Entity<SystemVariable>(entity =>
        {
            entity.HasKey(e => e.VariableName).HasName("PK__SystemVa__6E717C36E056CF22");

            entity.ToTable("SystemVariable");

            entity.Property(e => e.VariableName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.VariableValue)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__Units__44F5ECB5ECC1DA00");

            entity.HasIndex(e => e.UnitName, "UQ__Units__B5EE6678CC2E0A15").IsUnique();

            entity.Property(e => e.UnitName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdNo).HasName("PK__Users__B773C99954357D71");

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F284565E18D532").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsAdmin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
