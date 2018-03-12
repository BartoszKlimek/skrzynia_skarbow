using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SimpleServer
{
    class Program
    {
        static void Main(string[] args)

        {

            SimpleServer server = new SimpleServer("127.0.0.1", 11000);
              Socket serwer = server.stworzsocket();            
            SimpleClient client = new SimpleClient(serwer);
            Socket cl= client.stworzklienta();
            server.zamknijserwer(cl, serwer);
          

        }
        class SimpleServer
        {
            private string IP;
            private int port;        

           

            public SimpleServer(string IP, int port)
            {
                this.IP = IP;
                this.port = port;

            }

            public Socket stworzsocket()
            {
                // numer IP, na ktorym ma nasluchiwac serwer
                IPAddress ipAddress = IPAddress.Parse(IP);
                // numer portu, na ktorym ma nasluchiwac serwer          

                IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);

                Console.WriteLine("===== SERVER v.0.1 =====");
                Console.WriteLine("Serwer @ {0}", ipEndpoint);

                // stworzenie gniazda - tu sie podaje tylko rodzaj gniazda
                Socket listening = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                // przypisanie mu numeru IP i portu
                listening.Bind(ipEndpoint);

                // otwarcie w trybie nasluchiwania, 
                listening.Listen(1); // arg. informuje ile nieobsluzonych klientow moze oczekiwac
                                     
                 Console.WriteLine("Oczekuje na polaczenie...");         

                
                    return listening;




            }
            public void zamknijserwer(Socket klient, Socket serwer)
            {
                Console.WriteLine("Zamykam polaczenia...");
                klient.Close();
                serwer.Close();
                
            }





        }
        class SimpleClient
        {
            private Socket serwer;

            public SimpleClient(Socket serwer)
            {
                this.serwer = serwer;
            }

            public Socket stworzklienta()
            {
                Socket client = serwer.Accept();
                
                                       

                    Console.WriteLine("Podlaczyl sie klient @ {0}", client.RemoteEndPoint);
                    Console.WriteLine("Czekam na dane od klienta...");
                    NetworkStream stream = new NetworkStream(client);
                    StreamReader sr = new StreamReader(stream);
                    StreamWriter sw = new StreamWriter(stream);
                    string dataReceived;
                    do
                    {

                         dataReceived = sr.ReadLine();
                        Console.WriteLine(dataReceived);                      

                        string dataToSend = Console.ReadLine();                   

                        sw.WriteLine(dataToSend);
                        sw.Flush();

                    }
                    while (dataReceived != "QUIT");
                
                    sw.Close();
                    sr.Close();
                    return client;   

               
            }


        }


    }
}
