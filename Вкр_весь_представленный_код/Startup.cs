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
            services.AddSingleton<DropboxService>();
            services.AddSingleton<GoogleDriveService>();
            services.AddSingleton<RAID5Service>();
        }
    }
}
