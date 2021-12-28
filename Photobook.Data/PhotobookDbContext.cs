using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Photobook.Data.IdentityCustomDbContext;
using Photobook.Common.Identity;
using System;
using Photobook.Common.Models;
using System.Linq;
using Photobook.Common.Models.Base;
using System.Threading.Tasks;
using Photobook.Common.Services.CurrentUser;
using System.Threading;
using Photobook.Common.Services.DateTimeUtil;

namespace Photobook.Data
{
    public class PhotobookDbContext : KeyApiAuthorizationDbContext<PhotobookUser, PhotobookRole, Guid>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public virtual DbSet<Profile> Profiles { get; set; }

        public PhotobookDbContext(DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService) : base(options, operationalStoreOptions)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyAuditableChanges(_currentUserService.GetUserId());

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        public async Task<int> SaveChangesAsync(Guid userId, CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyAuditableChanges(userId);

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        public void ApplyAuditableChanges(Guid? userId)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.CreatedDate = _dateTimeService.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModifiedDate = _dateTimeService.Now;
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            BaseEntityConfiguration(builder);

            AuditableConfiguration(builder);
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

        private static void AuditableConfiguration(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(IAuditable).IsAssignableFrom(t.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditable.CreatedDate))
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                modelBuilder.Entity(entityType.ClrType)
                        .HasOne(typeof(PhotobookUser))
                        .WithMany()
                        .HasForeignKey(nameof(IAuditable.CreatedBy));

                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(PhotobookUser))
                    .WithMany()
                    .HasForeignKey(nameof(IAuditable.LastModifiedBy))
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
