using System;
using Confluent.Kafka;

namespace KafkaConsumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                GroupId = "dotnet-consumer-group",  // Consumer group name
                BootstrapServers = "localhost:9092", // Kafka broker address
                AutoOffsetReset = AutoOffsetReset.Earliest // Start from the earliest message if no offset is committed
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe("dotnet-demo-topic"); // Subscribe to the topic

            Console.WriteLine("Kafka Consumer started. Press any key to exit.");

            try
            {
                while (true)
                {
                    try
                    {
                        // Consume messages from the Kafka topic
                        var consumeResult = consumer.Consume();

                        Console.WriteLine($"✅ Message consumed: {consumeResult.Message.Value}");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"❌ Error consuming message: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation (e.g., when you press Ctrl+C)
                Console.WriteLine("Consumer canceled.");
            }
            finally
            {
                consumer.Close(); // Gracefully close the consumer
            }
        }
    }
}

