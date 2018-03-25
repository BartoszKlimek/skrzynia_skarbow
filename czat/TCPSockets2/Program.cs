/*
 * Telemedycyna i Technologie Sieciowe
 * Laboratorium. Gniazda sieciowe cz.2: Wielowatkowa obsluga polaczen
 * Program glowny
 * v.0.1.a, 2018-03-12, Marcin.Rudzki@polsl.pl 
 */

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Net;

namespace TCPSockets2
{
    class Program
    {
        static IServer server;

        static void Main(string[] args)
        {
            // podlaczenie metody obslugi zdarzenia Ctrl-C
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            
            server = new ThreadedEchoServer("127.0.0.1", 11000);
            server.Start();
            server.Stop();
            // czy to sie kiedys wykona?
            Console.WriteLine("The server has been stopped. Hit [Enter]...");
            Console.ReadLine();
           
             }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Rozłączono wszystkich klientów");
            Console.WriteLine("Stopping the server...");
            server.Closing();
            Console.ReadKey();
            server.Stop();
            //Console.ReadLine();
            e.Cancel = true; // nie chcemy wymusic zamkniecia programu!
        }
    }
}
