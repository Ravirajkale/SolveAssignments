using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



    public abstract class LibraryItem
    {
   

    public string Title { get; set; }
    public string Author { get; set; }

    public int ItemId { get; set; }

    public override string ToString()
    {
        return "id-" + ItemId + "\nTitle- " + Title + "\nAuthor-" + Author;
    }
    public LibraryItem(string title, string author, int itemId)
    {
        Title = title;
        Author = author;
        ItemId = itemId;
    }
}

