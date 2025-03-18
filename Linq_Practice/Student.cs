using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq_Practice
{
    class Student
    {
        public int id { get; set; }
        public string name { get; set; }

        public int deptId { get; set; }
        public List<int> marks { get; set; }

        public Student(int id, string name,int deptId)
        {
            this.id = id;
            this.name = name;
            this.deptId = deptId;
            marks = new List<int>();
        }

        public Student()
        {
            this.id = 0;
            this.name = "";

            marks = new List<int>();
        }
        public override string ToString()
        {
            return "id:-"+id+" name:-"+name;
        }
    }
}
