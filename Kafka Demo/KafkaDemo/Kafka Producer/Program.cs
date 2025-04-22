using Confluent.Kafka;

namespace Kafka_Producer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092" // Make sure Kafka is running on this
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            Console.WriteLine("Kafka Producer started. Type messages to send (type 'exit' to quit):");

            while (true)
            {
                Console.Write(">> ");
                string? input = Console.ReadLine();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                    break;

                try
                {
                    var result = await producer.ProduceAsync("dotnet-demo-topic", new Message<Null, string> { Value = input });
                    Console.WriteLine($"✅ Sent to partition {result.Partition}, offset {result.Offset}");
                }
                catch (ProduceException<Null, string> ex)
                {
                    Console.WriteLine($"❌ Delivery failed: {ex.Message}");
                }
            }
        }
    }
}
