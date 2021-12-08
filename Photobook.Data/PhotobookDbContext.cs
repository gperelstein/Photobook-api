using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Photobook.Data.IdentityCustomDbContext;
using Photobook.Common.Identity;
using System;
using Photobook.Common.Models;
using System.Linq;
using Photobook.Common.Models.Base;

namespace Photobook.Data
{
    public class PhotobookDbContext : KeyApiAuthorizationDbContext<PhotobookUser, PhotobookRole, Guid>
    {
        public virtual DbSet<Profile> Profiles { get; set; }

        public PhotobookDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            BaseEntityConfiguration(builder);
        }

        private static void BaseEntityConfiguration(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes()
                .Where(t => t.ClrType.IsSubclassOf(typeof(BaseEntity))))
            {
                builder.Entity(
                    entityType.Name,
                    x =>
                    {
                        x.HasKey("Id");
                        x.Property("Id").HasDefaultValueSql("NEWID()");
                    });
            }
        }
    }
}
