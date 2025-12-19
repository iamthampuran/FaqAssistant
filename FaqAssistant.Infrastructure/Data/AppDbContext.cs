using FaqAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaqAssistant.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Faq> Faqs => Set<Faq>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<User> Users => Set<User>();
        public DbSet<FaqTag> FaqTags => Set<FaqTag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configure Faq entity
            modelBuilder.Entity<Faq>(entity =>
            {
                entity.ToTable("Faqs");
                entity.HasKey(f => f.Id);
                
                entity.Property(f => f.Question)
                    .IsRequired();
                
                entity.Property(f => f.Answer)
                    .IsRequired();
                
                entity.Property(f => f.Rating)
                    .IsRequired();
                
                entity.Property(f => f.CategoryId)
                    .IsRequired();
                
                entity.Property(f => f.UserId)
                    .IsRequired();
                
                entity.Property(f => f.CreatedAt)
                    .IsRequired();
                
                entity.Property(f => f.LastUpdatedAt)
                    .IsRequired();
                
                entity.Property(f => f.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                entity.HasOne(f => f.Category)
                    .WithMany(c => c.Faqs)
                    .HasForeignKey(f => f.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(f => f.User)
                    .WithMany(u => u.Faqs)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(f => f.Tags)
                    .WithOne(ft => ft.Faq)
                    .HasForeignKey(ft => ft.FaqId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);
                
                entity.Property(c => c.Name)
                    .IsRequired();
                
                entity.Property(c => c.CreatedAt)
                    .IsRequired();
                
                entity.Property(c => c.LastUpdatedAt)
                    .IsRequired();
                
                entity.Property(c => c.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                entity.HasMany(c => c.Faqs)
                    .WithOne(f => f.Category)
                    .HasForeignKey(f => f.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Tag entity
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags");
                entity.HasKey(t => t.Id);
                
                entity.Property(t => t.Name)
                    .IsRequired();
                
                entity.Property(t => t.CreatedAt)
                    .IsRequired();
                
                entity.Property(t => t.LastUpdatedAt)
                    .IsRequired();
                
                entity.Property(t => t.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                entity.HasMany(t => t.Faqs)
                    .WithOne(ft => ft.Tag)
                    .HasForeignKey(ft => ft.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                
                entity.Property(u => u.Username)
                    .IsRequired();
                
                entity.Property(u => u.Email)
                    .IsRequired();
                
                entity.Property(u => u.PasswordHash)
                    .IsRequired();
                
                entity.Property(u => u.CreatedAt)
                    .IsRequired();
                
                entity.Property(u => u.LastUpdatedAt)
                    .IsRequired();
                
                entity.Property(u => u.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                entity.HasMany(u => u.Faqs)
                    .WithOne(f => f.User)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure FaqTag entity (junction table)
            modelBuilder.Entity<FaqTag>(entity =>
            {
                entity.ToTable("FaqTag"); // Keep original table name to avoid migration issues
                
                entity.HasKey(ft => ft.Id); // Use Id as primary key, not composite
                
                entity.Property(ft => ft.FaqId)
                    .IsRequired();
                
                entity.Property(ft => ft.TagId)
                    .IsRequired();
                
                entity.Property(ft => ft.CreatedAt)
                    .IsRequired();
                
                entity.Property(ft => ft.LastUpdatedAt)
                    .IsRequired();
                
                entity.Property(ft => ft.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                // Add composite index for performance
                entity.HasIndex(ft => new { ft.FaqId, ft.TagId })
                    .HasDatabaseName("IX_FaqTag_FaqId_TagId");
                
                entity.HasOne(ft => ft.Faq)
                    .WithMany(f => f.Tags)
                    .HasForeignKey(ft => ft.FaqId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(ft => ft.Tag)
                    .WithMany(t => t.Faqs)
                    .HasForeignKey(ft => ft.TagId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
