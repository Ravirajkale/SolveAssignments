using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BasicOOPSsys
{
   
    class Book : LibraryItem
    {
        public String BookName { set; get; }
        public DateOnly publishDate { get; set; }
        public Book(string bookName,string title, string author, int itemId, DateOnly publishDate) : base(title, author, itemId)
        {
            BookName = bookName;
            this.publishDate = publishDate;
        }
        public Book(): base("", "", 0)
        {
           BookName="";
        }

       
        public override string ToString()
        {
            return "Bookname:-" + BookName+"\npublish Date:-"+publishDate.ToString() +"\n"+ base.ToString();
        }

        public void AddBook()
        {
            Console.Write("\nGive item id:-");
            String id = Console.ReadLine();
            base.ItemId = Convert.ToInt32(id);

            Console.Write("\nEnter Book Name:-");
            BookName = Console.ReadLine();

            Console.Write("\nEnter Book publish Year:-");
            int year = Convert.ToInt32( Console.ReadLine());

            Console.Write("\nEnter Book publish Month:-");
            int month = Convert.ToInt32(Console.ReadLine());

            Console.Write("\nEnter Book publish day:-");
            int day = Convert.ToInt32(Console.ReadLine());

            publishDate = new DateOnly(year, month, day);

            Console.Write("\nGive title:-");
            base.Title=Console.ReadLine();
           
            Console.Write("\nGive author:-");
            base.Author = Console.ReadLine();

            Console.WriteLine();
        }
    }
}