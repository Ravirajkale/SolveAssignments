using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicOOPSsys
{
    class MenuHelper
    {

        Library lib = new();
        //Helper Method To Add Book in Library
        public void AddBook()
        {
            Book book = new();
            book.AddBook();
            lib.addBook(book);
            Console.WriteLine("Book Added Succesfully");
            Console.WriteLine();
        }
        //Helper Method to Display Books From Library
        public void DisplayBooks()
        {
            lib.displayBooks();
            Console.WriteLine();
        }
        //Helper Method to ADD Member To Library
        public void AddMember()
        {
            Member m = new();
            m.addMember();
            lib.addMember(m);
            Console.WriteLine("Member Added Succesfully");
            Console.WriteLine();
        }
        //Helper method to Display Members From Library
        public void DisplayMembers()
        {
            lib.displayMembers();
        }
        //Helper method To display Borrowed books of Member
        public void MemberBook()
        {
            //Display Members To Choose From
            DisplayMembers();
            Console.WriteLine("Enter Member Id:");
            int mid = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();
           
            //Find Member By Given Id
            Member m = lib.FindMember(mid);

            //For Type safety
            if (m == null)
            {
                Console.WriteLine("invalid id");
                return;
            }
            if (m.BorrowedBooks.Count == 0)
            {
                Console.WriteLine("No books Borrowed");
                return;
            }

            //Display Books For That Member
            m.DisplayBooks();
            Console.WriteLine();
        }

        //Helper Method to Rent A Book
        public void RentBook()
        {
            //Display List Of Books to Rent
            DisplayBooks();
            Console.WriteLine("Enter Book Id:");
            int bid = Convert.ToInt32(Console.ReadLine());

            //Find that Book By Id
            Book b = lib.FindBook(bid);

            //Type Safety
            if (b == null)
            {
                Console.WriteLine("invalid id");
                return;
            }
            if (lib.Books.Count == 0)
            {
                Console.WriteLine("No books Added");
                return;
            }

            //Display Members To Choose Whom To Rent
            DisplayMembers();
            Console.WriteLine("Enter Member Id:");
            int mid = Convert.ToInt32(Console.ReadLine());

            //Find Member By Given Id
            Member m = lib.FindMember(mid);

            //type safety
            if (m == null)
            {
                Console.WriteLine("invalid id");
                return;
            }

            //add given Book to borrowed books list of that member
            m.purchaseBook(b);
            Console.WriteLine("Book Added Succesfully");
        }

        //Extension Method Caller
        public void FindBookByTitle()
        {
            //get the title to search the given Book
            Console.Write("Enter Title:");
            string title = Console.ReadLine();

            //call the extension Method
           Book b= lib.Books.FindByTitle(title);
            if (b == null)
            {
                Console.WriteLine("Either List is Empty or Book doesn't exist");
                return;
            }
            Console.WriteLine();
            Console.WriteLine(b.ToString());
        }

        public void FindBooksOlderThan()
        {
            Console.WriteLine("Enter age of book");
            int age = Convert.ToInt32(Console.ReadLine());
            lib.BooksOlderThan(age);
        }
    }
}
