using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;
using Photobook.Common.Services.CurrentUser;
using Photobook.Common.Services.DateTimeUtil;
using Photobook.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photobook.Tests.Helpers
{
    public static class ContextHelper
    {
        public static PhotobookDbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<PhotobookDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var operationalStoreOptions = new Mock<IOptions<OperationalStoreOptions>>();
            operationalStoreOptions.Setup(x => x.Value)
                .Returns(new OperationalStoreOptions());

            var currentUserService = new Mock<ICurrentUserService>();
            var dateTimeService = new Mock<IDateTimeService>();

            return new PhotobookDbContext(optionsBuilder.Options,
                operationalStoreOptions.Object,
                currentUserService.Object,
                dateTimeService.Object);
        }

        public static Mock<PhotobookDbContext> CreateMockContext()
        {
            return new Mock<PhotobookDbContext>(new DbContextOptions<PhotobookDbContext>(), null, null, null);
        }
    }
}
