using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PAD_03_v1;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Node
{
    class NodeProgram
    {
        public static float getSalaryList(string txtName)
        {
            JsonWorker jsWorker = new JsonWorker(txtName);
            EmployeeRepository empRep = jsWorker.jsonRead();
            float sumSal = 0;
            for (int i = 0; i < empRep.employees.Count(); i++)
            {
                sumSal += empRep.employees[i].salary;
            }
            return sumSal;
        }
        public static int getEmpNum(string txtName)
        {
            JsonWorker jsWorker = new JsonWorker(txtName);
            EmployeeRepository empRep = jsWorker.jsonRead();
            int empNum = empRep.employees.Count();
            return empNum;
        }
        static void Main(string[] args)
        { 
            Initializator init = new Initializator(args);
            Console.WriteLine(init.isMaven());
            Console.WriteLine(init.isWhite());
            Console.WriteLine(init.mavensPort());
            Console.WriteLine(init.nodesPort());
            Console.WriteLine(init.txtName());


            Task t1 = Task.Factory.StartNew(async () =>
            {
                UDPSenderReceiver udp1 = new UDPSenderReceiver();

                UdpReceiveResult res = await udp1.receiveBytesAsync(init.nodesPort());
                string str = Encoding.UTF8.GetString(res.Buffer);
                Console.WriteLine(str);

                if (init.isMaven() == true)
                {
                    //UDP соединение с клиентом для отклика на Who is Maven?
                    Console.WriteLine("I am Maven!");
                    UDPSenderReceiver udp2 = new UDPSenderReceiver();
                    int mavensPort = init.nodesPort();
                    byte[] bytesSended = BitConverter.GetBytes(mavensPort);
                    await udp2.sendUnicastBytesAsync(bytesSended, 30000);

                    //Готовим данные, которые будут отправлены клиенту
                    string txtName = init.txtName();
                    int empNum = getEmpNum(txtName);
                    EmployeeRepository empRepRes = new EmployeeRepository();
                    AvgSalary avgSal = new AvgSalary(getEmpNum(txtName), getSalaryList(txtName));
                    float avgSalaryVal = avgSal.getAvgSalary();
                    int avgSalaryNum = 1;

                    //Заносим данные Maven'a
                    JsonWorker jsWorker = new JsonWorker(txtName);
                    EmployeeRepository empRep = jsWorker.jsonRead();
                    empRepRes.employees.AddRange(empRep.employees);                    

                    //Получаем данные с узлов
                    int nodeNum = init.getNodeNumber();
                    Console.WriteLine();
                    Console.WriteLine("Received information:");
                    for (int i = 0; i < nodeNum; i++)
                    {
                        TCPServer connectorToNode = new TCPServer(IPAddress.Parse("127.0.0.1"), mavensPort + i + 1);
                        byte[] receivedBytes = connectorToNode.TCPReceive();
                        BinaryWorker bw = new BinaryWorker();
                        Message msg = bw.bytesToMessage(receivedBytes);
                        avgSal = msg.s;
                        avgSalaryVal += avgSal.getAvgSalary();
                        avgSalaryNum ++;
                        empRep = msg.empRep;
                        empRepRes.employees.AddRange(empRep.employees);

                        Console.WriteLine(avgSal.getAvgSalary());
                        for (int j = 0; j < empRep.employees.Count(); j++)
                        {
                            Console.WriteLine(empRep.employees[j]);
                        }
                    }

                    //Обрабатываем информацию для отправки клиенту
                    AvgSalary avgSalary = new AvgSalary(avgSalaryNum, avgSalaryVal);
                    Calculator calc = new Calculator(empRepRes);
                    List<EmployeeRepository> groupedList = calc.groupBy();
                    EmployeeRepository sortedList = calc.sortBy();
                    EmployeeRepository filteredList = calc.filterBy(avgSalary.getAvgSalary());

                    //Готовим данные для отправки
                    MessageCalc msgCalc = new MessageCalc(sortedList, filteredList, groupedList);
                    BinaryWorker bwForCalc = new BinaryWorker();
                    byte[] bytesForClient = bwForCalc.messageCalcToBytes(msgCalc);

                    //Отправляем клиенту
                    TCPClient connectorToClient = new TCPClient(IPAddress.Parse("127.0.0.1"), 30000);
                    connectorToClient.TCPSend(bytesForClient);
                }

                else if (init.isWhite() == true)
                {
                    string jsName = init.txtName();
                    float salarySum = getSalaryList(jsName);
                    int empNum = getEmpNum(jsName);

                    //Считываем данные с xml
                    //XMLWorker xmlWorker = new XMLWorker(xmlName);
                    //EmployeeRepository empRep = xmlWorker.readXML();

                    //Считываем данные с JSON
                    JsonWorker jsWorker = new JsonWorker(jsName);
                    EmployeeRepository empRep = jsWorker.jsonRead();

                    //Готовим данные для отправки Maven'у
                    Message msg = new Message(new AvgSalary(empNum, salarySum), empRep);
                    BinaryWorker bw = new BinaryWorker();
                    byte[] bytesToSend = bw.messageToBytes(msg);

                    //Отправляем
                    TCPClient senderToMaven = new TCPClient(IPAddress.Parse("127.0.0.1"), init.nodesPort());
                    senderToMaven.TCPSend(bytesToSend);
                }

                else if (init.isWhite() == false)
                {
                    string jsName = init.txtName();
                    float salarySum = getSalaryList(jsName);
                    int empNum = getEmpNum(jsName);

                    //Готовим данные для отправки
                    Message msg = new Message(new AvgSalary(empNum, salarySum), new EmployeeRepository());
                    BinaryWorker bw = new BinaryWorker();
                    byte[] bytes = bw.messageToBytes(msg);

                    //Отправляем
                    TCPClient senderToMaven = new TCPClient(IPAddress.Parse("127.0.0.1"), init.nodesPort());
                    senderToMaven.TCPSend(bytes);
                }
            });
            t1.Wait();


            Console.ReadLine();
        }
    }
}
