using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProductDataAccess.ViewModels;

namespace ProductDataAccess.Models;

public partial class ProductCategoryContext : DbContext
{
    public ProductCategoryContext()
    {
    }

    public ProductCategoryContext(DbContextOptions<ProductCategoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderVoucher> OrderVouchers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<VoucherAssignment> VoucherAssignments { get; set; }

    public virtual DbSet<VoucherCampaign> VoucherCampaigns { get; set; }

    public virtual DbSet<VoucherUser> VoucherUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=SHITORU\\SQLEXPRESS;Initial Catalog=Product_Category;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B121F43B0");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Categories_ParentId");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF52E821F7");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(200);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber).HasMaxLength(12);
            entity.Property(e => e.ReceverName).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserId__68487DD7");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06819917D78E");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__6C190EBB");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__OrderItem__Produ__6D0D32F4");
        });

        modelBuilder.Entity<OrderVoucher>(entity =>
        {
            entity.HasKey(e => e.OrderVoucherId).HasName("PK__OrderVou__5B3AFEF4AC4E1E35");

            entity.ToTable("OrderVoucher");

            entity.Property(e => e.OrderVoucherId).HasColumnName("OrderVoucherID");
            entity.Property(e => e.DiscountApplied).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.VoucherId).HasColumnName("VoucherID");

            entity.HasOne(d => d.Voucher).WithMany(p => p.OrderVouchers)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__OrderVouc__Vouch__151B244E");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD751209AB");

            entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImgName).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Stock).HasDefaultValue(0);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__2B3F6F97");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A75C904F7");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F6D87809CB").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C9FA369D5");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateLastUpdated"));

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4A0C2453D").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534DEB8913B").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Group).WithMany(p => p.Users)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Users_UserGroups");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__UserGrou__149AF36AC52A853D");

            entity.HasIndex(e => e.GroupName, "UQ__UserGrou__6EFCD434A03BE200").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.GroupName).HasMaxLength(20);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Voucher__3AEE79C14A5D1580");

            entity.ToTable("Voucher");

            entity.HasIndex(e => e.Code, "UQ__Voucher__A25C5AA7CE2B9C42").IsUnique();

            entity.Property(e => e.VoucherId).HasColumnName("VoucherID");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Conditions).HasColumnType("text");
            entity.Property(e => e.DiscountType).HasMaxLength(20);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active");
            entity.Property(e => e.UsedCount).HasDefaultValue(0);
            entity.Property(e => e.VoucherType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VoucherAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__VoucherA__32499E57E653DC9E");

            entity.ToTable("VoucherAssignment");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.CampaignId).HasColumnName("CampaignID");
            entity.Property(e => e.VoucherId).HasColumnName("VoucherID");

            entity.HasOne(d => d.Campaign).WithMany(p => p.VoucherAssignments)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("FK__VoucherAs__Campa__123EB7A3");

            entity.HasOne(d => d.Voucher).WithMany(p => p.VoucherAssignments)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__VoucherAs__Vouch__114A936A");
        });

        modelBuilder.Entity<VoucherCampaign>(entity =>
        {
            entity.HasKey(e => e.CampaignId).HasName("PK__VoucherC__3F5E8D798A4CF447");

            entity.ToTable("VoucherCampaign");

            entity.Property(e => e.CampaignId).HasColumnName("CampaignID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active");
            entity.Property(e => e.TargetAudience).HasMaxLength(255);
        });

        modelBuilder.Entity<VoucherUser>(entity =>
        {
            entity.HasKey(e => e.VoucherUserId).HasName("PK__VoucherU__3E27916E856D8694");

            entity.Property(e => e.DateAssigned)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Voucher).WithMany(p => p.VoucherUsers)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VoucherUs__Vouch__41EDCAC5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
