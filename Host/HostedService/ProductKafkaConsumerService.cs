using Confluent.Kafka;
using Dipterv.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stl.Fusion;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.HostedService
{
    public class ProductKafkaConsumerService : BackgroundService
    {
        private readonly string topic = "adventureworks.Production.Product";
        private readonly string groupId = "backend";
        private readonly string bootstrapServers = "localhost:9092";

        private readonly IProductService _productService;

        public ProductKafkaConsumerService(IServiceProvider serviceProvider)
        {
            _productService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IProductService>();
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
                            var productEvent = JsonSerializer.Deserialize<ProductChangeEvent>(consumer.Message.Value)!;

                            switch (productEvent.Payload.Op)
                            {
                                // Update
                                case "u":
                                    using (Computed.Invalidate())
                                    {
                                        _ = _productService.TryGetProduct(productEvent.Payload.After.ProductID);
                                        _ = _productService.GetAll();
                                    }
                                    break;
                                // Delete
                                case "d":
                                    using (Computed.Invalidate())
                                    {
                                        _ = _productService.TryGetProduct(productEvent.Payload.Before.ProductID);
                                        _ = _productService.GetAll();
                                    }
                                    break;
                                // Insert
                                case "i":
                                    using (Computed.Invalidate())
                                    {
                                        _ = _productService.TryGetProduct(productEvent.Payload.After.ProductID);
                                        _ = _productService.GetAll();
                                    }
                                    break;
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
