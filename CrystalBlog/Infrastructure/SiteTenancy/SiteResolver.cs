using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrystalBlog.Data;
using CrystalBlog.Entities.Sites;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;

namespace CrystalBlog.Infrastructure.SiteTenancy
{
    public class SiteResolver : MemoryCacheTenantResolver<Site>
    {
        private readonly CrystalDbContext _dbContext;

        public SiteResolver(IMemoryCache cache,
            ILoggerFactory loggerFactory,
            CrystalDbContext dbContext)
            : base(cache, loggerFactory)
        {
            _dbContext = dbContext;
        }

        protected override string GetContextIdentifier(HttpContext context) => context.Request.Host.Value.ToLower();

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<Site> context) => context.Tenant.Hostnames.Split(new[] { ';' });

        protected override Task<TenantContext<Site>> ResolveAsync(HttpContext context)
        {
            TenantContext<Site> tenantContext = null;

            var hostname = context.Request.Host.Value.ToLower();
            var tenant = _dbContext.Sites.FirstOrDefault(x => x.Hostnames.Contains(hostname));

            if (tenant != null)
            {
                tenantContext = new TenantContext<Site>(tenant);
            }

            return Task.FromResult(tenantContext);
        }
    }
}
