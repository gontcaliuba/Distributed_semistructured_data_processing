using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PAD_03_v1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            XMLWorker xmlWorker = new XMLWorker("1.txt");
            EmployeeRepository empRep = xmlWorker.readXML();
            JsonWorker jsWorker = new JsonWorker("5.txt");
            jsWorker.jsonWrite(empRep);
            EmployeeRepository empRep2 = jsWorker.jsonRead();
            Console.ReadLine();
             */

            /*XDocument doc1 = new XDocument(
                new XElement("EmployeeRepository", 
                    new XElement("employees", 
                        new XElement("Employee", 
                            new XElement("firstName", "content1"),
                            new XElement("lastName", "content2"),
                            new XElement("department", "content3")
                            )
                );*/

            XDocument xml = XDocument.Load("filteredList.txt");
            XDocument xsd = XDocument.Load("xsd.txt");
            bool isValid = Validator.Validate(xml, xsd);
            Console.ReadLine();
        }
    }
}
