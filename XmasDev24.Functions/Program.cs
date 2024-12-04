using Azure.AI.OpenAI;
using Azure.Storage;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ClientModel;
using XmasDev24.Core;
using XmasDev24.Data;

namespace XmasDev24.Functions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = FunctionsApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

            var connectionString = builder.Configuration["SqlConnectionString"];
            var endpoint = builder.Configuration["AzureOpenAIEndpoint"];
            var apiKey = builder.Configuration["AzureOpenAIKey"];
            var storageAccountName = builder.Configuration["StorageAccountName"];
            var storageAccountKey = builder.Configuration["StorageAccountKey"];
            var serviceBusConnectionString = builder.Configuration["ServiceBusConnectionString"];

            builder.Services.AddDbContext<ChristmasContext>(b => b.UseSqlServer(connectionString));

            IChatClient CreateChatClient(string modelName) => new AzureOpenAIClient(new(endpoint), new ApiKeyCredential(apiKey)).AsChatClient(modelName);

            builder.Services.AddKeyedSingleton("imageToTextChatClient", CreateChatClient("gpt-4o"));
            builder.Services.AddKeyedSingleton("giftsExtractorChatClient", CreateChatClient("gpt-4o-mini"));
            builder.Services.AddSingleton<ChristmasLetterAIReader>();

            builder.Services.AddScoped(_ => new StorageSharedKeyCredential(storageAccountName, storageAccountKey));

            var host = builder.Build();

            host.Run();
        }
    }
}