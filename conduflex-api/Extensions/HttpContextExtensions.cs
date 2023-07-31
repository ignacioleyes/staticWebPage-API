namespace conduflex_api.Extensions
{
    public static class HttpContextExtensions
    {
        public static void InsertPaginationParams(this HttpContext httpContext, string entityName, int rangeStart, int rangeEnd, int recordsAmount)
        {
            httpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range, Pages-Amount, X-Total-Count");
            httpContext.Response.Headers.Add("Content-Range", $"{entityName} {rangeStart}-{rangeEnd}/{recordsAmount}");
            httpContext.Response.Headers.Add("X-Total-Count", recordsAmount.ToString());
            float pagesAmount = (float)recordsAmount / (rangeEnd - rangeStart + 1);
            httpContext.Response.Headers.Add("Pages-Amount", Math.Ceiling(pagesAmount).ToString());
        }
    }
}
