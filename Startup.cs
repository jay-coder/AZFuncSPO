using AZFuncSPO.SharePoint;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(AZFuncSPO.Startup))]

namespace AZFuncSPO
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Build Configuration
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environment))
            {
                environment = "local";
            }

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            var config = configBuilder.Build();

            builder.Services.Add(
                new ServiceDescriptor(
                    typeof(IConfiguration),
                    provider => config,
                    ServiceLifetime.Singleton
                )
            );

            var authBuilder = new SharePointAuthenticationBuilder(config);
            builder.Services.AddHttpClient("SharePoint").ConfigureHttpMessageHandlerBuilder((b) =>
                    authBuilder.BuildHttpMessageHandler(b)
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            builder.Services.AddSingleton<ISharePointTermStore, SharePointTermStore>();
            builder.Services.AddTransient<ISharePointList, SharePointList>();
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                // HttpRequestException, 5XX and 408  
                .HandleTransientHttpError()
                // 404  
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                // Retry two times after delay  
                .WaitAndRetryAsync(2, 
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        }
    }
}