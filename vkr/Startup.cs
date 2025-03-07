using vkr.Services;

namespace vkr
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddHttpClient();
            services.AddSingleton<YandexDiskService>();
        }
    }
}
