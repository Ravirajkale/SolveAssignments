using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicOOPSsys
{
    class Menu
    {
        public string GetChoice()
        {
            DisplayMenu();
            return Console.ReadLine();
            
        }

        private void DisplayMenu()
        {
            Console.WriteLine("\nLibrary Menu:");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. Display Books");
            Console.WriteLine("3. Add Member");
            Console.WriteLine("4. Display Members");
            Console.WriteLine("5. Display Member's Borrowed Books");
            Console.WriteLine("6. Rent Book");
            Console.WriteLine("7. FindBookByTitle");
            Console.WriteLine("8. Find Book Older Than Given age");
            Console.WriteLine("9. Exit");
            Console.Write("Enter your choice: ");
        }
    }
}
