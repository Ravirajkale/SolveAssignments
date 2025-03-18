using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq_Practice
{
    class Department
    {
        public int dId { get; set; }
        public string deptName { get; set; }

        public Department(int dId, string deptName)
        {
            this.dId = dId;
            this.deptName = deptName;
        }

        public Department()
        {
            this.dId = 0;
            this.deptName = "";
        }
    }
}
