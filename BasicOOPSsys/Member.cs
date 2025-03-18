using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicOOPSsys
{
    class Member
    {
        public int MemberId { get; set; }
        public String Name { get; set; }
        public  List<Book> BorrowedBooks { get; set; }

        public Member()
        {
           BorrowedBooks = new List<Book>();
        }
        public void purchaseBook(Book book)
        {
            BorrowedBooks.Add(book);
        }

        public override string ToString()
        {
            return "memberId:-"+MemberId+"\nName:-"+Name;
        }

        public void DisplayBooks()
        {
           foreach(Book b in BorrowedBooks)
            {
                Console.WriteLine(b.ToString());
            }
        }
        public void addMember()
        {
            Console.Write("\nEnter Member Id:");
            string id = Console.ReadLine();
            MemberId = Convert.ToInt32( id);
            
            Console.Write("\nEnter name:-");
            Name = Console.ReadLine();

            Console.WriteLine();
        }
        
    }
}
