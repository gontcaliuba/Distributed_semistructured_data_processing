using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAD_03_v1
{
    [Serializable]
    public class EmployeeRepository
    {
        public List<Employee> employees = new List<Employee>();
        
        public Employee Extract(int number)
        {
            if (employees[number] == null) return null;
            return employees[number];
        }
        public void Add(Employee emp)
        {
            if (emp == null) return;
            employees.Add(emp);
        }
    }
}
