using Microsoft.EntityFrameworkCore;
using ABCCarTraders.Models;

namespace ABCCarTraders.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets for all entities based on your model files
        public DbSet<User> Users { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarPart> CarParts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==============================================
            // USER ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.ToTable("Users");

                // Properties
                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(u => u.Address)
                    .HasColumnType("TEXT");

                entity.Property(u => u.City)
                    .HasMaxLength(100);

                entity.Property(u => u.Country)
                    .HasMaxLength(100);

                entity.Property(u => u.UserType)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(u => u.IsActive)
                    .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(u => u.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Indexes
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.UserType);
                entity.HasIndex(u => u.IsActive);
            });

            // ==============================================
            // BRAND ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(b => b.BrandId);
                entity.ToTable("Brands");

                entity.Property(b => b.BrandName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(b => b.Description)
                    .HasColumnType("TEXT");

                entity.Property(b => b.LogoUrl)
                    .HasMaxLength(255);

                entity.Property(b => b.IsActive)
                    .HasDefaultValue(true);

                entity.Property(b => b.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Navigation properties
                entity.HasMany(b => b.Cars)
                    .WithOne(c => c.Brand)
                    .HasForeignKey(c => c.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(b => b.CarParts)
                    .WithOne(cp => cp.Brand)
                    .HasForeignKey(cp => cp.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(b => b.BrandName);
                entity.HasIndex(b => b.IsActive);
            });

            // ==============================================
            // CATEGORY ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.ToTable("Categories");

                entity.Property(c => c.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasColumnType("TEXT");

                entity.Property(c => c.CategoryType)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(c => c.IsActive)
                    .HasDefaultValue(true);

                entity.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Navigation properties
                entity.HasMany(c => c.Cars)
                    .WithOne(car => car.Category)
                    .HasForeignKey(car => car.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.CarParts)
                    .WithOne(cp => cp.Category)
                    .HasForeignKey(cp => cp.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(c => c.CategoryName);
                entity.HasIndex(c => c.CategoryType);
                entity.HasIndex(c => c.IsActive);
            });

            // ==============================================
            // CAR ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(c => c.CarId);
                entity.ToTable("Cars");

                // Properties
                entity.Property(c => c.BrandId).IsRequired();
                entity.Property(c => c.CategoryId).IsRequired();

                entity.Property(c => c.Model)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Year)
                    .IsRequired();

                entity.Property(c => c.Color)
                    .HasMaxLength(50);

                entity.Property(c => c.Price)
                    .HasColumnType("decimal(15,2)")
                    .IsRequired();

                entity.Property(c => c.FuelType)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(c => c.Transmission)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(c => c.EngineCapacity)
                    .HasMaxLength(20);

                entity.Property(c => c.Description)
                    .HasColumnType("TEXT");

                entity.Property(c => c.Features)
                    .HasColumnType("TEXT");

                entity.Property(c => c.ImageUrl)
                    .HasMaxLength(255);

                entity.Property(c => c.StockQuantity)
                    .HasDefaultValue(1);

                entity.Property(c => c.IsAvailable)
                    .HasDefaultValue(true);

                entity.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(c => c.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Foreign key relationships are handled by navigation properties in Brand/Category

                // Indexes
                entity.HasIndex(c => c.BrandId);
                entity.HasIndex(c => c.CategoryId);
                entity.HasIndex(c => c.Model);
                entity.HasIndex(c => c.Year);
                entity.HasIndex(c => c.Price);
                entity.HasIndex(c => c.FuelType);
                entity.HasIndex(c => c.Transmission);
                entity.HasIndex(c => c.IsAvailable);
                entity.HasIndex(c => c.StockQuantity);
                entity.HasIndex(c => new { c.BrandId, c.CategoryId });
            });

            // ==============================================
            // CAR PART ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<CarPart>(entity =>
            {
                entity.HasKey(cp => cp.CarPartId);
                entity.ToTable("CarParts");

                // Properties
                entity.Property(cp => cp.BrandId).IsRequired();
                entity.Property(cp => cp.CategoryId).IsRequired();

                entity.Property(cp => cp.PartName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(cp => cp.PartNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(cp => cp.Price)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(cp => cp.Description)
                    .HasColumnType("TEXT");

                entity.Property(cp => cp.Compatibility)
                    .HasColumnType("TEXT");

                entity.Property(cp => cp.ImageUrl)
                    .HasMaxLength(255);

                entity.Property(cp => cp.StockQuantity)
                    .HasDefaultValue(0);

                entity.Property(cp => cp.IsAvailable)
                    .HasDefaultValue(true);

                entity.Property(cp => cp.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(cp => cp.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Indexes
                entity.HasIndex(cp => cp.PartNumber).IsUnique();
                entity.HasIndex(cp => cp.BrandId);
                entity.HasIndex(cp => cp.CategoryId);
                entity.HasIndex(cp => cp.PartName);
                entity.HasIndex(cp => cp.Price);
                entity.HasIndex(cp => cp.IsAvailable);
                entity.HasIndex(cp => cp.StockQuantity);
                entity.HasIndex(cp => new { cp.BrandId, cp.CategoryId });
            });

            // ==============================================
            // ORDER ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.ToTable("Orders");

                // Properties
                entity.Property(o => o.UserId).IsRequired();

                entity.Property(o => o.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(o => o.TotalAmount)
                    .HasColumnType("decimal(15,2)")
                    .IsRequired();

                entity.Property(o => o.OrderStatus)
                    .HasConversion<string>()
                    .HasDefaultValue(OrderStatus.Pending);

                entity.Property(o => o.PaymentStatus)
                    .HasConversion<string>()
                    .HasDefaultValue(PaymentStatus.Pending);

                entity.Property(o => o.PaymentMethod)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(o => o.ShippingAddress)
                    .HasColumnType("TEXT")
                    .IsRequired();

                entity.Property(o => o.Notes)
                    .HasColumnType("TEXT");

                entity.Property(o => o.OrderDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(o => o.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Foreign key relationship
                entity.HasOne(o => o.User)
                    .WithMany() // User doesn't have Orders navigation property in your model
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Navigation properties
                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(o => o.OrderNumber).IsUnique();
                entity.HasIndex(o => o.UserId);
                entity.HasIndex(o => o.OrderStatus);
                entity.HasIndex(o => o.PaymentStatus);
                entity.HasIndex(o => o.PaymentMethod);
                entity.HasIndex(o => o.OrderDate);
                entity.HasIndex(o => new { o.UserId, o.OrderStatus });
            });

            // ==============================================
            // ORDER ITEM ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.OrderItemId);
                entity.ToTable("OrderItems");

                // Properties
                entity.Property(oi => oi.OrderId).IsRequired();

                entity.Property(oi => oi.ItemType)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(oi => oi.ItemId).IsRequired();

                entity.Property(oi => oi.Quantity).IsRequired();

                entity.Property(oi => oi.UnitPrice)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(oi => oi.TotalPrice)
                    .HasColumnType("decimal(15,2)")
                    .IsRequired();

                entity.Property(oi => oi.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Foreign key relationship is handled by Order navigation property

                // Indexes
                entity.HasIndex(oi => oi.OrderId);
                entity.HasIndex(oi => oi.ItemType);
                entity.HasIndex(oi => oi.ItemId);
                entity.HasIndex(oi => new { oi.ItemType, oi.ItemId });
                entity.HasIndex(oi => new { oi.OrderId, oi.ItemType });
            });

            // ==============================================
            // CART ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.CartId);
                entity.ToTable("Cart");

                // Properties
                entity.Property(c => c.UserId).IsRequired();

                entity.Property(c => c.ItemType)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(c => c.ItemId).IsRequired();

                entity.Property(c => c.Quantity).IsRequired();

                entity.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Foreign key relationship
                entity.HasOne(c => c.User)
                    .WithMany() // User doesn't have Cart navigation property in your model
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint - one item per user
                entity.HasIndex(c => new { c.UserId, c.ItemType, c.ItemId }).IsUnique();

                // Other indexes
                entity.HasIndex(c => c.UserId);
                entity.HasIndex(c => c.ItemType);
                entity.HasIndex(c => c.ItemId);
            });

            // ==============================================
            // CONTACT MESSAGE ENTITY CONFIGURATION
            // ==============================================
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(cm => cm.MessageId);
                entity.ToTable("ContactMessages");

                // Properties
                entity.Property(cm => cm.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(cm => cm.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(cm => cm.Subject)
                    .HasMaxLength(200);

                entity.Property(cm => cm.Message)
                    .HasColumnType("TEXT")
                    .IsRequired();

                entity.Property(cm => cm.IsRead)
                    .HasDefaultValue(false);

                entity.Property(cm => cm.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes
                entity.HasIndex(cm => cm.Email);
                entity.HasIndex(cm => cm.IsRead);
                entity.HasIndex(cm => cm.CreatedAt);
            });

            // ==============================================
            // GLOBAL CONFIGURATIONS
            // ==============================================

            // Configure decimal precision globally for any missed decimals
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        if (string.IsNullOrEmpty(property.GetColumnType()))
                        {
                            // Default decimal precision for properties not explicitly configured
                            property.SetColumnType("decimal(18,2)");
                        }
                    }
                }
            }

            // Configure string length defaults for any missed strings
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && property.GetMaxLength() == null && property.GetColumnType() != "TEXT")
                    {
                        property.SetMaxLength(255);
                    }
                }
            }
        }

        // ==============================================
        // OVERRIDE SAVE CHANGES FOR AUTOMATIC TIMESTAMPS
        // ==============================================
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                // Auto-update UpdatedAt timestamp for entities that have it
                if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                // Auto-update UpdatedAt timestamp for entities that have it
                if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}