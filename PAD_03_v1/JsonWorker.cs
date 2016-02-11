using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PAD_03_v1
{
    public class JsonWorker
    {
        string jsonName;
        public JsonWorker(string jsonName)
        {
            this.jsonName = jsonName;
        }

        public void jsonWrite(EmployeeRepository empRep)
        {
            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(empRep);
            File.WriteAllText(jsonName, json);
        }
        public EmployeeRepository jsonRead()
        {
            string json = File.ReadAllText(jsonName);
            EmployeeRepository empRep = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<EmployeeRepository>(json);
            return empRep;
        }
    }
}
