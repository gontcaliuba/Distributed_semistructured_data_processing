using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace PAD_03_v1
{
    public class Validator
    {
        public static bool Validate(XDocument xml, XDocument xsd)
        {
            bool errors = false;

            var shemas = new XmlSchemaSet();

            shemas.Add(null, xsd.CreateReader());

            xml.Validate(shemas, (o, e) =>
            {
                Console.WriteLine("{0}", e.Message);
                errors = true;
            }); ;

            if (errors == false) return true;
            return false;
        }
    }
}
