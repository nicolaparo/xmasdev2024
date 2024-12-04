using Azure.AI.OpenAI;
using Azure.Storage.Blobs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using System.ClientModel;
using XmasDev24.Application.Components;
using XmasDev24.Core;
using XmasDev24.Data;

namespace XmasDev24.Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddUserSecrets(typeof(Program).Assembly);

            var connectionString = builder.Configuration["SqlConnectionString"];
            var endpoint = builder.Configuration["AzureOpenAIEndpoint"];
            var apiKey = builder.Configuration["AzureOpenAIKey"];
            var storageConnectionString = builder.Configuration["StorageConnectionString"];

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDbContext<ChristmasContext>(b => b.UseSqlServer(connectionString));

            IChatClient CreateChatClient(string modelName) => new AzureOpenAIClient(new(endpoint), new ApiKeyCredential(apiKey)).AsChatClient(modelName);

            builder.Services.AddKeyedSingleton("imageToTextChatClient", CreateChatClient("gpt-4o"));
            builder.Services.AddKeyedSingleton("giftsExtractorChatClient", CreateChatClient("gpt-4o-mini"));
            builder.Services.AddSingleton<ChristmasLetterAIReader>();

            builder.Services.AddSingleton(new BlobServiceClient(storageConnectionString).GetBlobContainerClient("christmasletters"));

            builder.Services.AddQuickGridEntityFrameworkAdapter();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            await using (var scope = app.Services.CreateAsyncScope())
            {
                await scope.ServiceProvider.GetRequiredService<ChristmasContext>()
                    .Database.EnsureCreatedAsync();

                await scope.ServiceProvider.GetRequiredService<BlobContainerClient>()
                    .CreateIfNotExistsAsync();
            }

            app.Run();
        }
    }
}
