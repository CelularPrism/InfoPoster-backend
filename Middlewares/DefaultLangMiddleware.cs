
using InfoPoster_backend.Tools;

namespace InfoPoster_backend.Middlewares
{
    public class DefaultLangMiddleware
    {
        private readonly RequestDelegate _next;
        public DefaultLangMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var langHeader = context?.Request.Headers["Accept-Language"];
            var lang = !string.IsNullOrEmpty(langHeader) ? langHeader.ToString().ToLower() : Constants.DefaultLang;
            if (!Constants.SystemLangs.Contains(lang)) lang = Constants.DefaultLang;
            context.Items[Constants.HTTP_ITEM_ClientLang] = lang;
            await _next(context);
        }
    }
}
