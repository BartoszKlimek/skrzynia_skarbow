using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LiczbyNaSlowaNET;

namespace liczby_slownie
{
    class Program
    {
        static void Main(string[] args)
        {

            //StreamReader sr = new StreamReader(@"C:\Users\Bartosz\Desktop\liczby.txt");
            //string[] liczby = new string[2501];
            //int i = 0;
            //do
            //{

            //    liczby[i] = sr.ReadLine();
            //    i++;
            //}
            //while (sr != null&&i<=2500);


            //do
            //{
            //    Console.WriteLine("podaj liczbe:");
            //    int a = int.Parse(Console.ReadLine());
            //    Console.WriteLine(liczby[a]);
            //    Console.ReadKey();

            //}
            //while (true);

            //sr.Close();

            var options = new NumberToTextOptions
            {
                Stems = true,
                Currency = Currency.PLN,
            };

            string wynik = NumberToText.Convert(2311, options);
            Console.WriteLine(wynik);
            Console.ReadLine();















        }
    }
}