using System.Collections.Generic;
using System.Linq;
using CrystalBlog.Entities.Sites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;

namespace CrystalBlog.Infrastructure.Views
{
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        private const string ThemeKey = "theme";
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string theme = null;

            if (context.Values.TryGetValue(ThemeKey, out theme))
            {
                viewLocations = new[]
                {
                    $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                    $"/Themes/{theme}/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context) => context.Values[ThemeKey] = context.ActionContext.HttpContext.GetTenant<Site>()?.Theme;
    }
}
