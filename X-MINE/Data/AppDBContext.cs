using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace X_MINE.Data
{
    public class AppDBContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AppDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<Models.vw_t_user_kategori> VwTUserKategoris { get; set; }
        public DbSet<Models.tbl_r_menu> tbl_r_menu { get; set; }
        public DbSet<Models.tbl_r_kategori_user> tbl_r_kategori_user { get; set; }
        public DbSet<Models.tbl_r_dept> tbl_r_dept { get; set; }
        public DbSet<Models.tbl_m_user_login> tbl_m_user_login { get; set; }
        public DbSet<Models.Dokumen> dokumens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Models.vw_t_user_kategori>()
                .HasNoKey()
                .ToView("vw_t_user_kategori");

            modelBuilder.Entity<Models.tbl_r_kategori_user>(entity =>
            {
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<Models.tbl_r_dept>(entity =>
            {
                entity.HasKey(e => e.id);
            });
            modelBuilder.Entity<Models.tbl_r_menu>(entity =>
            {
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<Models.tbl_m_user_login>(entity =>
            {
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<Models.Dokumen>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}
