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
            client.stworzklienta();
            Console.ReadKey();

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






        }
        class SimpleClient
        {
            private Socket serwer;

            public SimpleClient(Socket serwer)
            {
                this.serwer = serwer;
            }

            public void stworzklienta()
            {



                Socket client = serwer.Accept();
                do
                {
                                       

                    Console.WriteLine("Podlaczyl sie klient @ {0}", client.RemoteEndPoint);
                    Console.WriteLine("Czekam na dane od klienta...");
                    NetworkStream stream = new NetworkStream(client);
                    StreamReader sr = new StreamReader(stream);
                    StreamWriter sw = new StreamWriter(stream);
                    string dataReceived;
                    do
                    {


                        Console.WriteLine(dataReceived = string.Concat("otrzymano: ",sr.ReadLine()));                       

                        string dataToSend = Console.ReadLine();                     

                        sw.WriteLine(dataToSend);
                        sw.Flush();


                    }
                    while (dataReceived != "QUIT");

                    Console.WriteLine("Zamykam polaczenia...");
                    // zamykanie polaczenia
                    sw.Close();
                    sr.Close();
                    client.Close();                  

                }
                while (true);
                serwer.Close();
            }


        }


    }
}
