using System.Text.Json.Serialization;

namespace XmasDev24.Functions
{
    public class StorageEvent
    {
        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("data")]
        public BlobData Data { get; set; }

        [JsonPropertyName("dataVersion")]
        public string DataVersion { get; set; }

        [JsonPropertyName("metadataVersion")]
        public string MetadataVersion { get; set; }

        [JsonPropertyName("eventTime")]
        public DateTime EventTime { get; set; }


        public class BlobData
        {
            [JsonPropertyName("api")]
            public string Api { get; set; }

            [JsonPropertyName("clientRequestId")]
            public string ClientRequestId { get; set; }

            [JsonPropertyName("requestId")]
            public string RequestId { get; set; }

            [JsonPropertyName("eTag")]
            public string ETag { get; set; }

            [JsonPropertyName("contentType")]
            public string ContentType { get; set; }

            [JsonPropertyName("contentLength")]
            public long ContentLength { get; set; }

            [JsonPropertyName("blobType")]
            public string BlobType { get; set; }

            [JsonPropertyName("accessTier")]
            public string AccessTier { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("sequencer")]
            public string Sequencer { get; set; }

            [JsonPropertyName("storageDiagnostics")]
            public StorageDiagnostics StorageDiagnostics { get; set; }
        }

        public class StorageDiagnostics
        {
            [JsonPropertyName("batchId")]
            public string BatchId { get; set; }
        }
    }
}
