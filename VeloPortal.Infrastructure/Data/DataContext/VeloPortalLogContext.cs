using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeloPortal.Domain.Entities.Authentication;

namespace VeloPortal.Infrastructure.Data.DataContext
{
    public class VeloPortalLogContext : DbContext, IDataProtectionKeyContext
    {
        public VeloPortalLogContext(DbContextOptions<VeloPortalLogContext> options) : base(options)
        {
        }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }


        #region Authentication
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<LoginLogs> LoginLogs { get; set; }
        #endregion

    }
}
