using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rapier.Client.Descriptive;
using Rapier.Client.HttpClients;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rapier.Client
{
    public class Program
    {
        private static async Task DebugDelayAsync()
        {
#if DEBUG
            await Task.Delay(3000);
#endif
        }
        public static async Task Main(string[] args)
        {
            await DebugDelayAsync();
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            var uri = builder.Configuration.GetSection(ApiConfiguration.ApiSettings)[ApiConfiguration.RapierApiUrl];
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddHttpClient("Api", (p, client) =>
            {
                client.BaseAddress = new Uri("https://localhost:5020");
            });

            builder.Services.AddScoped<IRapierClient>(sp =>
            {
                var api = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api");
                return new RapierClient(api.BaseAddress.ToString(), api);
            });

            await builder.Build().RunAsync();
        }
    }
}
