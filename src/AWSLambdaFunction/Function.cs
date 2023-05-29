using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AWSLambdaFunction
{
    public class Function
    {
        readonly ServiceProvider _serviceProvider;
        public Function()
        {
            Console.WriteLine("Setting up the DI container");
            var serviceCollection = new ServiceCollection();
            Console.WriteLine("Adding a scoped service");
            serviceCollection.AddScoped<ISomeService, SomeService>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static async Task Main(string[] args)
        {

            using IHost host = Host.CreateDefaultBuilder(args).Build();

            // Ask the service provider for the configuration abstraction.
            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

            // Get values from the config given their key and their target type.
            string token = config.GetValue<string>("token");

            // Write the values to the console.
            Console.WriteLine($"Token = {token}");

            Func<string> handler = FunctionHandler;
            await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
                .Build()
                .RunAsync();
        }

        public static string FunctionHandler()
        {
            var env = Environment.GetEnvironmentVariable("ENV");
            var host = Environment.GetEnvironmentVariable("HOST");
            Console.WriteLine(" ENV  : " + env);
            Console.WriteLine(" HOST : " + host);

            string input = "welcome to aws lambda csharp";
            string upperCaseText;

            Function fn = new Function();

            using (var scope = fn._serviceProvider.CreateScope())
            {
                var someService = scope.ServiceProvider.GetRequiredService<ISomeService>();
                upperCaseText = someService.ToUpperCase(input);
            }

            return upperCaseText;
        }
    }

    public sealed class Settings
    {
        public required string token { get; set; }
    }
}