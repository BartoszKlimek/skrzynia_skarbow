/*
 * Telemedycyna i Technologie Sieciowe
 * Laboratorium. Gniazda sieciowe cz.2: Wielowatkowa obsluga polaczen
 * Klasa serwera echo (wielowatkowy)
 * v.0.1.a, 2018-03-12, Marcin.Rudzki@polsl.pl 
 */

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPSockets2
{
    class ThreadedEchoServer : IServer
    {


        // klasa obslugujaca polaczenie z jednym klientem
        class ClientHelper
        {

            Socket socket;    // otwarte gniazdo polaczenia
            Socket prywatny;
            NetworkStream ns; // strumien sieciowy "na gniezdzie"
            StreamReader sr;  // strumien do odbierania danych "na s.sieciowym"
            StreamWriter sw;  // strumien do wysylania danych "na s.sieciowym"
            StreamWriter priv;
            StreamReader srp;
            ThreadedEchoServer server;
            ThreadedEchoServer serverpriv;
            string login;


            public string Login { get { return login; } }

            public bool SocketIsConnected(Socket s)
            {
                bool part1 = s.Poll(1000, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            }




            public ClientHelper(Socket socket, ThreadedEchoServer server)
            {
                this.socket = socket;
                ns = new NetworkStream(this.socket);
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                sw.AutoFlush = true;
                this.server = server;
            }





            public void ProcessCommunication()
            {


                string message;

                // czy ktores operacje tu wykonywane moga spowodowac wyjatek?
                sw.WriteLine("Client thread started. Enter login:");
                //sw.Flush();
                string tempLogin = sr.ReadLine();
                if (server.LoginExists(tempLogin) == false)
                {
                    login = tempLogin;
                    server.SendMessage("SERVER", string.Format("Witamy uzytkownika {0} na czacie", login));
                    do
                    {
                        message = sr.ReadLine();
                        bool check = SocketIsConnected(socket);
                        Console.WriteLine(string.Format("Client @ {0} says: {1}", login, message));
                        if (check == true)
                        {
                            server.SendMessage(login, message);


                            if (message == "priv")
                            {
                                privProcessCommunication(login);
                            }

                        }
                        else
                        {
                            message = "QUIT!";
                            server.RemoveClient(this);
                            Disconnect();
                        }
                    }
                    while (message != "QUIT!");
                }
                else
                {
                    sw.WriteLine("Ten login jest zajety, sprobuj ponownie.");
                }

                Disconnect();
                server.RemoveClient(this);
            }



            void privProcessCommunication(string login)
            {

            }


            public void SendMessage(string msg)
            {
                sw.WriteLine(msg);
            }
            public void privmessage(string msg)
            {
                priv.WriteLine(msg);
            }

            void Disconnect()
            {
                // to nie sprawdza czy strumien jest juz Disposed...
                if (sw != null) sw.Close();
                if (sr != null) sr.Close();
                if (ns != null) ns.Close();
                if (socket != null) socket.Close();
            }


        }       

      


        IPEndPoint ipEndPoint;
        Socket listeningSocket;
        private List<ClientHelper> activeClients;
       
        private List<ClientHelper> ActiveClients
        {
            get
            {
                return activeClients;
            }

            set
            {
                activeClients = value;
            }
        }
        
        public ThreadedEchoServer(string ipAddress, int ipPort)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), ipPort);
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Console.WriteLine("Server created.");
            ActiveClients = new List<ClientHelper>();
            
        }
        public bool SocketIsConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }



        public void Start()
        {
            listeningSocket.Bind(ipEndPoint);
            listeningSocket.Listen(1);

            Console.WriteLine("Server @ {0} started.", ipEndPoint);
           
            bool flaga = true;
            do
            {
                          
                        Console.WriteLine("Server @ {0} waits for a client...", ipEndPoint);                         
                            Socket clientSocket = listeningSocket.Accept(); // czy to moze zglosic wyjatek?
                        Console.WriteLine("Server @ {0} client connected @ {1}.", ipEndPoint, clientSocket.RemoteEndPoint);
                        Console.WriteLine("Server @ {0} starting client thread.", ipEndPoint, clientSocket.RemoteEndPoint);                       
                        ClientHelper ch = new ClientHelper(clientSocket, this);
                        activeClients.Add(ch);                        
                        Thread t = new Thread(ch.ProcessCommunication);               
                        t.Start();
                       
                
            }
            while (flaga);
        }


      
        void SendMessage(string from, string msg)
        {
            string message = string.Format("[{0}] mowi: {1}", from, msg);
            foreach (ClientHelper item in activeClients)
            {                           
                if (item.Login != from)
                    item.SendMessage(message);
            }
        }

        void privmessage(string login, string msg)
        {            
            foreach (ClientHelper item in activeClients)
            {
                if (item.Login == login)
                    item.privmessage(msg);
            }
        }






   

        void RemoveClient(ClientHelper ch)
        {
            ActiveClients.Remove(ch);
            
        }

        bool LoginExists(string login)
        {
            foreach (var item in ActiveClients)
            {
                if (item.Login == login)
                    return true;
            }
            return false;
        }

        public void Stop()
        {
            listeningSocket.Close();
            // a co z ew. klientami?
        }

       





    } // class
} // namespace
