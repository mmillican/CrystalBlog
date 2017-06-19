using System.Text.RegularExpressions;

namespace CrystalBlog
{
    public static class ExtensionMethods
    {
        public static string Clean(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var regex = new Regex("[^a-z0-9\\-_]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            input = input.Replace(" ", "-");
            var cleaned = regex.Replace(input, "").ToLower();

            while (cleaned.Contains("--"))
            {
                cleaned = cleaned.Replace("--", "-");
            }

            return cleaned;
        }

        public static int GetPageCountForResults(this int resultCount, int pageSize)
        {
            var pageCount = resultCount % pageSize == 0
                ? resultCount / pageSize
                : resultCount / pageSize + 1;

            return pageCount;
        }
        public static int CalculateRecordSkip(int pageSize, int currentPage)
        {
            return pageSize * (currentPage - 1);
        }
    }
}
