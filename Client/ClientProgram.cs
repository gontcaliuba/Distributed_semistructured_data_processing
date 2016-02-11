using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PAD_03_v1;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;

namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            // получаем количество узлов
            int node_number = 0;
            if (args.Count() > 0)
            {
                try { node_number = Int32.Parse(args[0]); }
                catch { node_number = 0; }
            }

            // ждем ответа от Maven
            Task t1 = Task.Factory.StartNew(async () =>
            {
                UDPSenderReceiver client = new UDPSenderReceiver();
                UdpReceiveResult res = await client.receiveBytesAsync(30000);
                int mavensPort = BitConverter.ToInt32(res.Buffer, 0);
                Console.WriteLine(mavensPort);

                //Соединение с Maven'ом
                TCPServer tcpServer = new TCPServer(IPAddress.Parse("127.0.0.1"), 30000);
                byte[] bytesFromMaven = tcpServer.TCPReceive();
                if (bytesFromMaven == null) Console.WriteLine("Byte's array is empty");

                BinaryWorker bw = new BinaryWorker();
                MessageCalc msgCalc = bw.bytesToMessageCalc(bytesFromMaven);
                if (msgCalc == null) Console.WriteLine("MsgCalc is empty");

                //Вывод полученной информации
                else
                {
                    EmployeeRepository filteredList = msgCalc.filteredEl;
                    EmployeeRepository sortedList = msgCalc.sortedEl;
                    List<EmployeeRepository> groupedList = msgCalc.groupedEl;
                    EmployeeRepository groupedEmpRep = new EmployeeRepository();

                    for (int i = 0; i < groupedList.Count(); i++)
                    {
                        List<Employee> empList = groupedList[i].employees;
                        for (int j = 0; j < empList.Count(); j++)
                        {
                            groupedEmpRep.Add(empList[j]);
                            //Console.WriteLine(empList[j]);
                        }
                    }

                    //Записываем в xml
                    string xmlNameFiltered = "filteredList.txt";
                    string xmlNameSorted = "sortedList.txt";
                    string xmlNameGrouped = "groupedList.txt";
                    XMLWorker xmlWorker1 = new XMLWorker(xmlNameFiltered);
                    xmlWorker1.writeXML(filteredList);
                    XMLWorker xmlWorker2 = new XMLWorker(xmlNameSorted);
                    xmlWorker2.writeXML(sortedList);
                    XMLWorker xmlWorker3 = new XMLWorker(xmlNameGrouped);
                    xmlWorker3.writeXML(groupedEmpRep);

                    //Подготавливаем документы для валидации
                    XDocument xmlFiltered = XDocument.Load(xmlNameFiltered);
                    XDocument xmlSorted = XDocument.Load(xmlNameSorted);
                    XDocument xmlGrouped = XDocument.Load(xmlNameGrouped);
                    XDocument xsd = XDocument.Load("xsd.txt");

                    //Если документ прошел валидацию, выводим инфу
                    if (Validator.Validate(xmlFiltered, xsd) == true)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Filtered list: ");
                        for (int i = 0; i < filteredList.employees.Count(); i++)
                        {
                            Console.WriteLine(filteredList.employees[i]);
                        }
                    }
                    else Console.WriteLine("Filtered XML didn't pass validation!");

                    if (Validator.Validate(xmlSorted, xsd) == true)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Sorted list: ");
                        for (int i = 0; i < sortedList.employees.Count(); i++)
                        {
                            Console.WriteLine(sortedList.employees[i]);
                        }
                    }
                    else Console.WriteLine("Sorted XML didn't pass validation!");

                    if (Validator.Validate(xmlSorted, xsd) == true)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Grouped list: ");
                        for (int i = 0; i < groupedEmpRep.employees.Count(); i++)
                        {
                            Console.WriteLine(groupedEmpRep.employees[i]);
                        }
                    }
                    else Console.WriteLine("Grouped XML didn't pass validation!");
                }

            });
            t1.Wait();

            // Шлем узлам запрос на Maven
            Task t2 = Task.Factory.StartNew(async () =>
            {
                UDPSenderReceiver client = new UDPSenderReceiver();

                Console.WriteLine("Client. Press enter key to send!");
                Console.ReadLine();
                byte[] bytes = Encoding.UTF8.GetBytes("Who is Maven?");

                for (int i = 0; i < node_number; i++)
                {
                    await client.sendBroadcastBytesAsync(bytes, 30000 + i + 1);
                }
                
            });
            t2.Wait();




            Console.ReadLine();
        }
    }
}
