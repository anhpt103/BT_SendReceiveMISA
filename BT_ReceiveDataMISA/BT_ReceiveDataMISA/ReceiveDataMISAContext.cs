using BT_ReceiveDataMISA.Entities;
using Microsoft.EntityFrameworkCore;

namespace BT_ReceiveDataMISA
{
    public class ReceiveDataMISAContext : DbContext
    {
        public ReceiveDataMISAContext(DbContextOptions<ReceiveDataMISAContext> options) : base(options) { }

        public DbSet<CauHinhDongBo> CauHinhDongBos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CauHinhDongBo>().ToTable("Tbl_CauHinhDongBo");

            // Composite key
            modelBuilder.Entity<CauHinhDongBo>().HasKey(a => new { a.Token });
        }
    }
}
