using Confluent.Kafka;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stl.Fusion;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.HostedService
{
    public class ProductInventoryKafkaConsumerService : BackgroundService
    {
        private readonly string topic = "adventureworks.Production.ProductInventory";
        private readonly string groupId = "backend";
        private readonly string bootstrapServers = "localhost:9092";

        private readonly IProductInventoryService _productInventoryService;

        public ProductInventoryKafkaConsumerService(IServiceProvider serviceProvider)
        {
            _productInventoryService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IProductInventoryService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumerBuilder.Subscribe(topic);
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume(cancelToken.Token);
                            var productEvent = JsonSerializer.Deserialize<ProductInventoryChangeEvent>(consumer.Message.Value)!;

                            var productId = 0;
                            switch (productEvent.Payload.Op)
                            {
                                // Update
                                case "u":
                                // Insert
                                case "i":

                                    break;
                                // Delete
                                case "d":
                                    using (Computed.Invalidate())
                                    {
                                        _ = _productInventoryService.GetInventoriesForProduct(productEvent.Payload.Before.ProductID);
                                        _ = _productInventoryService.ProductGetTotalStock(productEvent.Payload.Before.ProductID);
                                    }
                                    break;
                                
                                    using (Computed.Invalidate())
                                    {
                                        _ = _productService.TryGetProduct(productEvent.Payload.After.ProductID);
                                        _ = _productService.GetAll();
                                    }
                                    break;


                            }

                            using (Computed.Invalidate())
                            {
                                _ = _productInventoryService.GetInventoriesForProduct(productEvent.Payload.After.ProductID);
                                _ = _productInventoryService.ProductGetTotalStock(productEvent.Payload.After.ProductID);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
