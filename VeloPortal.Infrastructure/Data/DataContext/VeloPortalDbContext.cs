using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeloPortal.Domain.Entities.FacilityManagement;

namespace VeloPortal.Infrastructure.Data.DataContext
{
    public class VeloPortalDbContext: DbContext, IDataProtectionKeyContext
    {
        public VeloPortalDbContext(DbContextOptions<VeloPortalDbContext> options) : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        #region Common       

        #endregion
        #region FMS
        public DbSet<ServReqInf> ServReqInf { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global convention for all decimal properties using LINQ
            modelBuilder.Model.GetEntityTypes()
                .SelectMany(entityType => entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))
                    .Select(p => new { entityType.ClrType, p.Name }))
                .ToList()
                .ForEach(prop =>
                    modelBuilder.Entity(prop.ClrType)
                        .Property(prop.Name)
                        .HasPrecision(18, 6));
        }
    }
}
