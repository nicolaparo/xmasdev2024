using Azure.Messaging.ServiceBus;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XmasDev24.Core;
using XmasDev24.Data;

namespace XmasDev24.Functions
{
    public class Functions(ILogger<Functions> log, StorageSharedKeyCredential storageCredentials, ChristmasLetterAIReader aiReader, ChristmasContext context)
    {
        [Function(nameof(ExtractTextFromImageAsync))]
        public async Task ExtractTextFromImageAsync(
            [ServiceBusTrigger("%ServiceBusQueueName%", AutoCompleteMessages = true, Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message, ServiceBusClient serviceBusClient
        )
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {message}");

            var storageEvent = message.Body.ToObjectFromJson<StorageEvent>();
            if (storageEvent is null)
                return;

            if (storageEvent.EventType is not "Microsoft.Storage.BlobCreated")
                return;

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(storageEvent.Data.Url);

            if (!Guid.TryParse(fileNameWithoutExtension, out var letterId))
                return;

            var letter = await context.ChristmasLetters.FirstOrDefaultAsync(l => l.Id == letterId);

            if (letter is null)
                return;

            var blobClient = new BlobClient(new(storageEvent.Data.Url), storageCredentials);
            using var fileContentStream = new MemoryStream();
            var response = await blobClient.DownloadToAsync(fileContentStream);
            var contentType = response.Headers.ContentType;

            fileContentStream.Position = 0;

            await aiReader.UpdateLetterAsync(fileContentStream, contentType, letter);

            await context.SaveChangesAsync();

            log.LogInformation($"Letter {letterId} updated with text extracted from image.");

        }
    }

}
