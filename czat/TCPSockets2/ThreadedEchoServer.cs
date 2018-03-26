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
            
            NetworkStream ns; // strumien sieciowy "na gniezdzie"
            StreamReader sr;  // strumien do odbierania danych "na s.sieciowym"
            StreamWriter sw;  // strumien do wysylania danych "na s.sieciowym"
            ThreadedEchoServer server;
           
            
            string login;


            public string Login { get { return login; } }

            public bool SocketIsConnected(Socket s)
            {
                try
                {
                    bool part1 = s.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (s.Available == 0);
                    if (part1 && part2)
                        return false;
                    else
                        return true;
                }
                catch (Exception)
                {
                    return false;
                }
              
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

                bool continuation = true;
                string message = string.Empty;

                // czy ktores operacje tu wykonywane moga spowodowac wyjatek?
                sw.WriteLine("Client thread started. Enter login:");
                //sw.Flush();
                DateTime dt = DateTime.Now;
                string tempLogin = sr.ReadLine();
                if (server.LoginExists(tempLogin) == false)
                {
                    login = tempLogin;
                    server.SendMessage("SERVER", string.Format("Witamy uzytkownika {0}", login));
                    sw.WriteLine("aby zobaczyc dostepne opcje wpisz: /option");
                    do 
                    {
                        bool check = SocketIsConnected(socket);
                        if (check==true)
                        {                           
                        try
                        {
                                message = sr.ReadLine();
                                if (message == "/option") server.Option(login);
                            if (message=="/priv")

                            {
                                    string recipient = sr.ReadLine();
                                    do
                                    {
                                        
                                        message = sr.ReadLine();
                                        if (message != string.Empty) server.PrivSendMessage(login, recipient, message);
                                        if (message == "QUIT!")
                                        {
                                            server.PrivSendMessage(login,recipient,message);
                                            continuation = false;
                                            message = "/stop";
                                        }
                                    }
                                    while (message != "/stop");
                                    
                                    

                                }




                            else 
                            {
                                    if (message != string.Empty) Console.WriteLine(string.Format("Client @ {0} says: {1}", login, message));                              
                                 if(message!=string.Empty && message!="/option") server.SendMessage(login, message);                       
                             



                            }



                        }
                        catch (IOException)
                        {
                          Disconnect();
                         server.RemoveClient(this);
                        }   
                        }



                        if (message == "QUIT!")
                        {
                            
                            continuation = false;
                        }
                      



                    }
                    while (continuation);
                }
                else
                {
                    sw.WriteLine("Ten login jest zajety, sprobuj ponownie.");
                    ProcessCommunication();
                }

            }


           
          


            public void SendMessage(string msg)
            {
                sw.WriteLine(msg);
            }
          

            internal void Disconnect()
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
                
                try {
                    Console.WriteLine("Server @ {0} waits for a client...", ipEndPoint);
                    Socket clientSocket = listeningSocket.Accept(); // czy to moze zglosic wyjatek?
                    Console.WriteLine("Server @ {0} client connected @ {1}.", ipEndPoint, clientSocket.RemoteEndPoint);
                    Console.WriteLine("Server @ {0} starting client thread.", ipEndPoint, clientSocket.RemoteEndPoint);
                    ClientHelper ch = new ClientHelper(clientSocket, this);
                    activeClients.Add(ch);
                   Thread t = new Thread(ch.ProcessCommunication);
                    t.Start();
                }
                catch (Exception)
                {
                    flaga = false;                 
                    
                }

                    
                       
                
            }
            while (flaga);
        }


      
        void SendMessage(string from, string msg)
        {
            DateTime dt = DateTime.Now;
            string message;
            if (msg == "QUIT!")
            {
                message = string.Format("[{0}] Opuscil czat\n {1}", from,dt.ToString());
                foreach (ClientHelper item in activeClients)
                {
                    if (item.Login != from)
                        item.SendMessage(message);
                }
            }
            else
            {
            message = string.Format("[{0}] mowi: {1}\n {2}", from, msg,dt.ToString());
            foreach (ClientHelper item in activeClients)
            {                           
                if (item.Login != from)
                    item.SendMessage(message);
            }
            }
            
        }

            
        public void Closing()
        {
            DateTime dt = DateTime.Now;
            string message = string.Format("SERVER IS CLOSED\n{0}",dt.ToString());
            foreach (ClientHelper item in activeClients)
            {
                
                    item.SendMessage(message);
            }



        }

        public void Option(string from)
        {
            string message1 = "1. /priv + [ENTER] + wprowadzenie nazwy odbiorcy - rozmowa prywatna";
            string message2 = "2. /stop - zakonczenie rozmowy prywatnej";
            string message3 = "3. QUIT! wyjście z czatu";
            foreach (ClientHelper item in activeClients)
            {
                if (item.Login == from)
                {
                    item.SendMessage(message1);
                    item.SendMessage(message2);
                    item.SendMessage(message3);
                }
            }
        }




        void PrivSendMessage(string from, string to,string message)
        {

            DateTime dt = DateTime.Now;

            if (message != "/stop")
            {
                if (message == "QUIT!")
                {
                    foreach (ClientHelper item in activeClients)
                    {
                        if (item.Login == from)
                        {
                            foreach (ClientHelper item2 in activeClients)
                            {
                                if (item2.Login == to)
                                {
                                    item2.SendMessage(string.Format("[{0}] opuścił czat\n{1}",item.Login,dt.ToString()));
                                    
                                }
                               

                            }
                           
                        }
                    }
                }
                else
                {
                message = string.Format("[{0}] przesyła wiadomość prywatną: {1}\n{2}", from, message,dt.ToString());
              foreach (ClientHelper item in activeClients)
                  {                
                    if (item.Login == from)
                {
                    foreach (ClientHelper item2 in activeClients)
                    {
                        if (item2.Login == to)
                        {
                            item2.SendMessage(message);
                        }
                        
                    }
                }
            }
                }
           
                    
         }
            else
            {
                foreach (ClientHelper item in activeClients)
                {
                    if (item.Login == from)
                    {
                        foreach (ClientHelper item2 in activeClients)
                        {
                            if (item2.Login == to)
                            {
                                item2.SendMessage(string.Format("{0} zakończył rozmowe prywatna\n{1}",from,dt.ToString()));
                            }

                        }
                    }
                }
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
            foreach (var item in activeClients)
            {
                item.Disconnect();

            }
            listeningSocket.Close();
           

            // a co z ew. klientami?
        }

       





    } // class
} // namespace
