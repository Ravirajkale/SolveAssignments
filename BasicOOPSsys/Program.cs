using System.Runtime.CompilerServices;

namespace BasicOOPSsys
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu menu=new Menu();
            MenuHelper help = new();
            bool running = true;
            while(running)
            {
                string choice = menu.GetChoice();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        help.AddBook();
                        break;
                    case "2":
                        help.DisplayBooks();
                        break;
                    case "3":
                        help.AddMember();
                        break;
                    case "4":
                        help.DisplayMembers();
                        break;
                    case "5":
                        help.MemberBook();
                        break;
                    case "6":
                        help.RentBook();
                        break;
                    case "7":
                        help.FindBookByTitle();
                        break;
                    case "8":
                        help.FindBooksOlderThan();
                        break;
                    case "9":
                        Console.WriteLine("thank you for using the app");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Wrong Choice");
                        break;
                }
            }
        }
    }
}
