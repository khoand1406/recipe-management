using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Persistence
{
    public partial class RecipeManagementContext : DbContext
    {
        public RecipeManagementContext()
        {
        }

        public RecipeManagementContext(DbContextOptions<RecipeManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Dish> Dishes { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<Ingredient> Ingredients { get; set; }

        public virtual DbSet<Comment> Comment { get; set; }

        public virtual DbSet<Bookmark> Bookmark { get; set; }

        public virtual DbSet<Following> Following { get; set; }

        public virtual DbSet<Rating> Ratings { get; set; }

        public virtual DbSet<Recipe> Recipes { get; set; }

        public virtual DbSet<RelatedRecipe> RelatedRecipes { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Step> Steps { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BB89254FB");

                entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E055772FB0").IsUnique();

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(255);
            });

            modelBuilder.Entity<Dish>(entity =>
            {
                entity.HasKey(e => e.DishId).HasName("PK__Dishes__18834F504EEDF949");

                entity.Property(e => e.DishName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Category).WithMany(p => p.Dishes)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Dishes__Category__2F10007B");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F70CAF793B1B");

                entity.Property(e => e.Caption).HasMaxLength(255);
                entity.Property(e => e.EntityType)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.UploadedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Bookmark>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookmarks)
                .HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Bookmark>()
                .HasOne(b => b.Recipe)
                .WithMany(r => r.Bookmarks)
                .HasForeignKey(b => b.RecipeId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Recipe)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RecipeId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Following>()
        .HasOne(f => f.Follower)
        .WithMany(u => u.FollowingUsers)
        .HasForeignKey(f => f.FollowerId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Following>()
                .HasOne(f => f.FollowingUser)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB25AF862F5EC");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Quantity).HasMaxLength(50);

                entity.HasOne(d => d.Recipe).WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__Ingredien__Recip__37A5467C");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.RatingId).HasName("PK__Ratings__FCCDF87CBB2A67F9");

                entity.Property(e => e.Comment).HasMaxLength(500);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Recipe).WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__Ratings__RecipeI__4222D4EF");

                entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Ratings__UserId__4316F928");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__FDD988B01750CBC5");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DifficultyLevel).HasMaxLength(50);
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Author).WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Recipes__AuthorI__34C8D9D1");

                entity.HasOne(d => d.Dish).WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.DishId)
                    .HasConstraintName("FK__Recipes__DishId__33D4B598");
            });

            modelBuilder.Entity<RelatedRecipe>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__RelatedR__3214EC07EE15C8BA");

                entity.HasOne(d => d.Recipe).WithMany(p => p.RelatedRecipeRecipes)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__RelatedRe__Recip__45F365D3");

                entity.HasOne(d => d.RelatedRecipeNavigation).WithMany(p => p.RelatedRecipeRelatedRecipeNavigations)
                    .HasForeignKey(d => d.RelatedRecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RelatedRe__Relat__46E78A0C");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1ABB3C72DB");

                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160937F43CB").IsUnique();

                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Step>(entity =>
            {
                entity.HasKey(e => e.StepId).HasName("PK__Steps__24343357099F3734");

                entity.Property(e => e.Instruction).IsRequired();

                entity.HasOne(d => d.Recipe).WithMany(p => p.Steps)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__Steps__RecipeId__3A81B327");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C6B0995CF");

                entity.HasIndex(e => e.Email, "UQ__Users__A9D10534617D2514").IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Users__RoleId__29572725");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(x => x.Token).IsUnique();

                entity.HasOne(x => x.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
