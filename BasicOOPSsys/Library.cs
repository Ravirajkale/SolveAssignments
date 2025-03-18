using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicOOPSsys
{
     class Library
    {
        public List<Book> Books { get; set; }
        public List<Member> Members { get; set; }

        public Library()
        {
            Books = new List<Book>();
            Members = new List<Member>();
        }

        public void addBook(Book book)
        {
            Books.Add(book);
        }
        public void removeBook(int bookIndex)
        {
            Books.RemoveAt(bookIndex);
        }
        public void addMember(Member member)
        {
            Members.Add(member);
        }
        public void removeMember(int memberIndex)
        {
            Members.RemoveAt(memberIndex);
        }

        //generic private method for displaying lists of the library
        private void displayItems<T>(List<T> items)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("List is Empty");

            }
            foreach(T item in items)
            {
                Console.WriteLine(item.ToString());
                Console.WriteLine();
            }
        }
        public void displayBooks()
        {
            displayItems(Books);
        }
        public void displayMembers()
        {
            displayItems(Members);
        }
        public Member FindMember(int mid)
        {
            foreach(Member m in Members)
            {
                if (m.MemberId == mid)
                {
                    return m;
                }
            }
            return null;
        }
        public Book FindBook(int bid)
        {
            foreach (Book b in Books)
            {
                if (b.ItemId == bid)
                {
                    return b;
                }
            }
            return null;
        }
        public void BooksOlderThan(int age)
        {
            var bookOlder = from book in Books let diff =DateTime.Now.Year- book.publishDate.Year where diff>age select book;

            if (bookOlder != null)
            {
                foreach(Book b in bookOlder)
                {
                    Console.WriteLine(b.ToString());
                }
            }
            else
            {
                Console.WriteLine("Books of that Doesn't Exist");
            }
        }
    }

    
}
