using Microsoft.Extensions.DependencyInjection;
using name_sorter.Services;
using name_sorter.Interfaces;

namespace name_sorter
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            try
            {
                var app = serviceProvider.GetRequiredService<NameSorter>();
                await app.Run(args);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"An error occurred: {ex.Message}");
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // core services
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<INameParser, NameParser>();
            services.AddSingleton<INameSorter, NameSorterService>();

            // entrypoint
            services.AddSingleton<NameSorter>();
            return services.BuildServiceProvider();
        }
    }
}