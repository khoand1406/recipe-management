namespace RecipeMgt.Api.Common.Extension
{
    public static class HttpContextExtension
    {
        public static int GetUserId(this HttpContext ctx)
        {
            return ctx.Items["UserId"] as int?
                ?? throw new UnauthorizedAccessException();
        }

        public static int? GetOptionalUserId(this HttpContext ctx)
        {
            return ctx.Items["UserId"] as int? ;
        }   
    }
}
