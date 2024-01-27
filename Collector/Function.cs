using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Register.Core.Entities;
using Register.Core.Enums;
using Register.Core.Helpers;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Collector;

public class Function
{
    private readonly IDynamoDBContext _context;
    public Function(IDynamoDBContext context) => _context = context;

    public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {

        foreach (var record in dynamoEvent.Records)
        {
            if (record.EventName == "INSERT")
            {
                var order = record.Dynamodb.NewImage.ToObject<Order>();
                order.Status = StatusOrder.Collected;

                try
                {
                    await ProcessOrderValue(order);
                    await AmazonQueues.AddQueue(EQueueSQS.order, order);
                    context.Logger.LogLine($"Order collected successfully, {order.Id}");
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine($"Error: '{ex.Message}'");
                    order.Reason = ex.Message;
                    order.Cancelled = true;
                    await AmazonQueues.AddQueue(EQueueSNS.fail, order);
                }

                await _context.SaveAsync(order);

            }
        }

    }

    private async Task ProcessOrderValue(Order order)
    {
        foreach (var product in order.Products)
        {
            var prodStock = await GetProductDynamoDBAsync(product.Id) ?? throw new InvalidOperationException($"Product not found! {product.Id}");

            product.Price = prodStock.Price;
            product.Title = prodStock.Title;
        }

        var totalPrice = order.Products.Sum(x => x.Price * x.Qty);
        if (order.TotalPrice != 0 && order.TotalPrice != totalPrice)
            throw new InvalidOperationException($"Valor esperado {order.TotalPrice}");

        order.TotalPrice = (decimal)totalPrice;
    }

    private async Task<Product> GetProductDynamoDBAsync(string id)
    {
        var clientDynamoDB = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        var request = new QueryRequest
        {
            TableName = "Stock",
            KeyConditionExpression = "Id = :v_id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":v_id", new AttributeValue { S = id } } }
        };
        var response = await clientDynamoDB.QueryAsync(request);

        var result = response.Items.FirstOrDefault();
        if (result is null)
            return null;

        return result.ToObject<Product>();
    }
}