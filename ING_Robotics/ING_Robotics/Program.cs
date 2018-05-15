using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using Microsoft.Office.Interop.Excel;
using ExcelApp=Microsoft.Office.Interop.Excel.Application;
using Excel = Microsoft.Office.Interop.Excel;



namespace ING_Robotics
{
    class Program
    {
        static void Main(string[] args)
        {    
            


              RemoteWebDriver web=  NewMethod2();
            web.Navigate().GoToUrl("http://google.pl/");
            ExcelApp excel = new ExcelApp();
            excel.Visible = true;
            Workbook wb = excel.Workbooks.Open("C:/Users/Bartosz/Desktop/ING.xlsx");
            Worksheet ws = wb.ActiveSheet;
        
            int wiersz = 1;
            int kolumna = 1;
            int valueOFcities = 0;
            while (excel.Cells[wiersz, kolumna].Value != null)
            {
                valueOFcities++;
                wiersz++;

            }

            string[] cities = new string[valueOFcities];
            int[] temp = new int[valueOFcities];


            for (int i = 0; i < valueOFcities; i++)
            {
                web.FindElementById("lst-ib").Clear();
                string miasto = excel.Cells[i + 1, 1].Value;

                web.FindElementById("lst-ib").SendKeys("pogoda " + miasto + Keys.Enter);
                cities[i] = miasto;

                //temperatura
                string temperatura = web.FindElementById("wob_tm").Text;
                temp[i] = int.Parse(temperatura);                              
                excel.Cells[i + 1, 2].Value = temperatura;

                //data
                string data;
                DateTime dat = DateTime.Now;
                data = dat.ToString();
                excel.Cells[i + 1, 4].Value = data;

                // pogoda
                string pogoda;
                pogoda = web.FindElementById("wob_dc").Text;
                excel.Cells[i + 1, 3].Value = pogoda;




            }

            int maxtemp = temp.Max();
            int mintemp = temp.Min();

            // wybor miast 
            for (int i = 0; i < valueOFcities; i++)
            {


                if (temp[i] == maxtemp)
                {
                    string cityofmaxtemp = cities[i];
                    excel.Cells[4, 8].Value = String.Format("najgorętszym miastem jest: " + cityofmaxtemp);
                }

                if (temp[i] == mintemp)
                {
                    string cityofmintemp = cities[i];
                    excel.Cells[5, 8].Value = String.Format("najchłodniejszym miastem jest: " + cityofmintemp);

                }



                

            }



            //rysowanie wykresu
            object misValue = System.Reflection.Missing.Value;
            Excel.Range chartRange;

            Excel.ChartObjects xlCharts = (Excel.ChartObjects)ws.ChartObjects(Type.Missing);
            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(200, 200, 200, 200);
            Excel.Chart chartPage = myChart.Chart;

            chartRange = ws.get_Range("A1", "B13");
            chartPage.SetSourceData(chartRange, misValue);
            chartPage.ChartType = Excel.XlChartType.xlColumnClustered;

            releaseObject(ws);
            releaseObject(wb);
            releaseObject(excel);




            web.Quit();
            //wb.Save();
            //excel.Quit();







        }

        private static RemoteWebDriver NewMethod2()
        {
            RemoteWebDriver web = new ChromeDriver();
            return web;
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine(ex);
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}

