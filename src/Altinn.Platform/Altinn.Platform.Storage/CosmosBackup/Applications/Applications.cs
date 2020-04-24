using System;
using System.Collections.Generic;

using Altinn.Platform.Storage.CosmosBackup;

using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CosmosBackup
{
    /// <summary>
    /// Azure Function class for handling tasks related to applications.
    /// </summary>
    public static class Applications
    {
        /// <summary>
        /// Backs up Cosmos DB application documents in Blob Storage.
        /// </summary>
        /// <param name="input">Application document.</param>
        /// <param name="context">Function context.</param>
        /// <param name="log">Logger.</param>
        [FunctionName("ApplicationsCollectionBackup")]
        public static async void ApplicationsCollectionBackup(
            [CosmosDBTrigger(
            databaseName: "Storage",
            collectionName: "applications",
            ConnectionStringSetting = "DBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
            ExecutionContext context,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                IConfiguration config = ConfigHelper.LoadConfig(context);
                string blobName = string.Empty;

                try
                {
                    dynamic data = JObject.Parse(input[0].ToString());
                    string id = input[0].Id;
                    string partitionKey = data.org;
                    blobName = $"{partitionKey}/{id}";

                    await BlobService.SaveBlob(config, $"applications/{blobName}", input[0].ToString());
                }
                catch (Exception e)
                {
                    log.LogError($"Exception occured when storing element {blobName}. Exception: {e}. Message: {e.Message}");
                }
            }
        }
    }
}
