using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Register.Core.Entities;
using Register.Core.Enums;
using Register.Core.Helpers;
using System.Text.Json;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Booker;

public class Function
{
    private AmazonDynamoDBClient dynamoDBClient { get; }

    private readonly IDynamoDBContext _context;
    public Function(IDynamoDBContext context) => _context = context;

    public Function()
    {
        dynamoDBClient = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
    }

    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {

        if (evnt.Records.Any())
            throw new InvalidOperationException("Somente 1 mensagem por vez");

        var message = evnt.Records.FirstOrDefault();
        if (message is null) return;
        await ProcessMessageAsync(message, context);


    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed message {message.Body}");
        var failOnQueue = false;

        var order = JsonSerializer.Deserialize<Order>(message.Body);
        order.Status = StatusOrder.Reserved;

        foreach (var product in order.Products)
        {
            try
            {
                await RemoveFromStock(product.Id, product.Qty);
                product.Reserved = true;
                context.Logger.LogLine($"Product removed from stock {product.Id} - {product.Title}");

            }
            catch (ConditionalCheckFailedException)
            {
                order.Reason = $"Product Unavailable - {product.Id}";
                order.Cancelled = true;
                context.Logger.LogLine($"Error: {order.Reason}");

                break;
            }
        }

        if (order.Cancelled)
        {
            foreach (var product in order.Products)
            {
                if (product.Reserved)
                    await ReverseToStock(product.Id, product.Qty);

                product.Reserved = false;
                context.Logger.LogLine($"Reverse to Stock: {product.Id}");

            }

            await AmazonQueues.AddQueue(EQueueSNS.fail, order).ConfigureAwait(false);
            await _context.SaveAsync(order);
        }
        else
        {
            await AmazonQueues.AddQueue(EQueueSQS.reserved, order).ConfigureAwait(false);
            await _context.SaveAsync(order);

        }
    }

    private async Task RemoveFromStock(string id, byte qty)
    {
        var request = new UpdateItemRequest
        {
            TableName = "Stock",
            ReturnValues = "NONE",
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            },
            UpdateExpression = "SET Qty = (Qty - :qtyOrder)",
            ConditionExpression = "Qty >= :qtyOrder",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":qtyOrder", new AttributeValue{N = qty.ToString() } }
            }

        };

        using var amazonBDClient = new AmazonDynamoDBClient();

        await amazonBDClient.UpdateItemAsync(request).ConfigureAwait(false);

    }

    private async Task ReverseToStock(string id, byte qty)
    {
        var request = new UpdateItemRequest
        {
            TableName = "Stock",
            ReturnValues = "NONE",
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            },
            UpdateExpression = "SET Qty = (Qty + :qtyOrder)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":qtyOrder", new AttributeValue{N = qty.ToString() } }
            }

        };

        using var amazonBDClient = new AmazonDynamoDBClient();

        await amazonBDClient.UpdateItemAsync(request).ConfigureAwait(false);
    }
}