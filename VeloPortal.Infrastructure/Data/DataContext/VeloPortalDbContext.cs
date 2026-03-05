using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Entities.Documentation;
using VeloPortal.Domain.Entities.FacilityManagement;
using VeloPortal.Domain.Entities.SystemConfig;
using VeloPortal.Domain.Entities.Vendor;

namespace VeloPortal.Infrastructure.Data.DataContext
{
    public class VeloPortalDbContext: DbContext, IDataProtectionKeyContext
    {
        public VeloPortalDbContext(DbContextOptions<VeloPortalDbContext> options) : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        #region System configuration
        public DbSet<ResCodeInf> ResCodeInf { get; set; }
        public DbSet<SysGenInf> SysGenInf { get; set; }
        public DbSet<Industries> Industries { get; set; }
        #endregion
        #region Authentication
        public DbSet<PassRecovery> PassRecovery { get; set; }
        public DbSet<VendorProfile> VendorProfile { get; set; }
        public DbSet<SupportUser> SupportUsers { get; set; }
        #endregion

        #region Common       

        #endregion
        #region FMS
        public DbSet<ServReqInf> ServReqInf { get; set; }
        #endregion
        #region Documentation
        public DbSet<DocInfDet> DocInfDet { get; set; }
        #endregion

        #region Vendor
        public DbSet<VendorSuply> VendorSuply { get; set; }
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
