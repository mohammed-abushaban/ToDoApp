using Microsoft.EntityFrameworkCore;
using ToDo.DbModel.Models;


namespace ToDo.DbModel.Data
{
    public partial class ToDoContext : DbContext
    {
        public ToDoContext()
        {
        }
        public bool IgnoreFilter { get; set; }
        public bool IgnoreIsRead { get; set; }
        public ToDoContext(DbContextOptions<ToDoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Models.ToDo> ToDo { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(a => !a.IsArchived || IgnoreFilter);
            modelBuilder.Entity<Models.ToDo>().HasQueryFilter(a => !a.IsArchived || IgnoreFilter);

            modelBuilder.Entity<Models.ToDo>().HasQueryFilter(a => !a.IsRead || IgnoreFilter || IgnoreIsRead);

            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Models.ToDo>(entity =>
            {
                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("date");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.ToDo)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ToDo_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}