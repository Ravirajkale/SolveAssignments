using System.Xml.Linq;

namespace PubSubDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello, World!");
            //Publishers
            Publisher publisher1 = new Publisher();
            Publisher publisher2 = new Publisher();


            //sms subscriber
            SmsSubsriber smsSubsriber1 = new SmsSubsriber();
            smsSubsriber1.Name="Ravi";
            SmsSubsriber smsSubsriber2 = new SmsSubsriber();
            smsSubsriber2.Name = "avi";

            //email subscriber
            EmailSubscriber emailSubscriber = new EmailSubscriber();
            emailSubscriber.Name = "Raj";

            //publisher 1 subscribers
            publisher1.AddSubscriber(emailSubscriber);
            publisher1.AddSubscriber(smsSubsriber1);

            //publisher 2 subscribers
            publisher2.AddSubscriber(smsSubsriber1);
            publisher2.AddSubscriber(smsSubsriber2);

            publisher1.notify(" published by raviraj");
            publisher2.notify("published by aviraj");
            Console.ReadLine();
        }
    }

    public class Publisher
    {
        List<ISubscriber> subscribers;

        public Publisher()
        {
            subscribers = new List<ISubscriber>();
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void notify(string message)
        {
            foreach(ISubscriber subscriber in subscribers)
            {
                subscriber.Subscribe(message);
            }
        }
    }
    public interface ISubscriber
    {
        void Subscribe(string message);
    }

    public class EmailSubscriber : ISubscriber
    {
        public string Name { get; set; }
        public void Subscribe(string message)
        {
            Console.WriteLine("Email Subscriber:-"+Name+"  ||  published message:-"+message);
        }
    }
    public class SmsSubsriber : ISubscriber
    {
        public string Name { get; set; }
        public void Subscribe(string message)
        {
            Console.WriteLine("sms Subscriber:-" + Name + "  || published message:-" + message);
        }
    }
}
