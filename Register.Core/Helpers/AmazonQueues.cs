using Amazon;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Amazon.SQS;
using Amazon.SQS.Model;
using Register.Core.Entities;
using Register.Core.Enums;
using System.Text.Json;
using Amazon.DynamoDBv2.DocumentModel;

namespace Register.Core.Helpers
{
    public static class AmazonQueues
    {
        public static async Task AddQueue(EQueueSQS queue, Order order)
        {
            var jsonString = JsonSerializer.Serialize(order);
            var clientSQS = new AmazonSQSClient(RegionEndpoint.USEast1);
            var request = new SendMessageRequest
            {
                QueueUrl = $"https://sqs.us-east-1.amazonaws.com/459094004332/{queue}",
                MessageBody = jsonString
            };

            await clientSQS.SendMessageAsync(request);
        }

        public static async Task AddQueue(EQueueSNS queue, Order order)
        {
            await Task.CompletedTask;
        }


        public static T ToObject<T>(this Dictionary<string, AttributeValue> dictionary)
        {
            var client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            var context = new DynamoDBContext(client);

            var doc = Document.FromAttributeMap(dictionary);

            return context.FromDocument<T>(doc);
        }
    }
}
