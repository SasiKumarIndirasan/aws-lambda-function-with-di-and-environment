using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

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
}