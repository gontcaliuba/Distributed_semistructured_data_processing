using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAD_03_v1
{
[Serializable]
    public class Message
    {
        public AvgSalary s;
        public EmployeeRepository empRep;
        public Message(AvgSalary s, EmployeeRepository empRep)
        {
            this.s = s;
            this.empRep = empRep;
        }
    }
}
