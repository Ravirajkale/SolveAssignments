using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicOOPSsys
{
    //Extension class for the Books
     static class BookExtensions
    {
        //Extension Method for Finding Book by Title
        //extends library s book list
        public static Book FindByTitle(this List<Book> blist,string title)
        {
            
            if (blist.Count == 0 )
            {
                return null;
            }

            //Searches Book By Title and Returns it
            return blist.FirstOrDefault(b => b.Title.Equals(title));
        }
    }

}
