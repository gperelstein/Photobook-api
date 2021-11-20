using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Photobook.Data.IdentityCustomDbContext;
using Photobook.Common.Identity;
using System;

namespace Photobook.Data
{
    public class PhotobookDbContext : KeyApiAuthorizationDbContext<PhotobookUser, PhotobookRole, Guid>
    {
        public PhotobookDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
