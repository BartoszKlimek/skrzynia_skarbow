using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Butterworth_Filter__low_pass_
{
    public partial class Form1 : Form
    {



        double[] data = new double[5000];
        double[] data2;
        string cutOffs;
        double cutOff;
        string pathdata;
         
        public Form1()
        {
            InitializeComponent();
            //this.data = data;
            //this.data2 = data2;
            //rysuj_default();
        }

        public double[] Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        public double[] Data2
        {
            get
            {
                return data2;
            }

            set
            {
                data2 = value;
            }
        }

        public string CutOff
        {
            get
            {
                return CutOff;
            }

            set
            {
                CutOff = value;
            }
        }

        public double CutOff1
        {
            get
            {
                return cutOff;
            }

            set
            {
                cutOff = value;
            }
        }

        public string Pathdata
        {
            get
            {
                return pathdata;
            }

            set
            {
                pathdata = value;
            }
        }

      
        private void rysuj()
        {

            chart1.Series[0].Points.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                chart1.Series[0].Points.AddY(data[i]);
            }


            chart2.Series[0].Points.Clear();
            bool IsDouble = Double.TryParse(cutOffs, out cutOff);
            if (IsDouble)
            {
                double[] data1 = new double[5000];
                Array.Copy(data, data1, data.Length);
                this.data2 = Butterworth(data1, 0.002, cutOff);
                for (int i = 0; i < data2.Length; i++)
                {
                    chart2.Series[0].Points.AddY(data2[i]);

                }
            }
            else MessageBox.Show("Należy wprowadzić Liczbę !!!");

        

}
        private void rysuj_default()
        {
            StreamReader sr = new StreamReader(pathdata);
           

            for (int i = 0; i < 5000; i++)
            {
                this.data[i] = double.Parse(sr.ReadLine());
            }
            sr.Close();          
            
            chart1.Series[0].Points.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                chart1.Series[0].Points.AddY(data[i]);
            }

        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }



        public static double[] Butterworth(double[] indata, double deltaTimeinsec, double CutOff)
        {
            if (indata == null) return null;
            if (CutOff == 0) return indata;

            double Samplingrate = 1 / deltaTimeinsec;
            long dF2 = indata.Length - 1;        // The data range is set with dF2
            double[] Dat2 = new double[dF2 + 4]; // Array with 4 extra points front and back
            double[] data = indata; // Ptr., changes passed data

            // Copy indata to Dat2
            for (long r = 0; r < dF2; r++)
            {
                Dat2[2 + r] = indata[r];
            }
            Dat2[1] = Dat2[0] = indata[0];
            Dat2[dF2 + 3] = Dat2[dF2 + 2] = indata[dF2];

            const double pi = 3.14159265358979;
            double wc = Math.Tan(CutOff * pi / Samplingrate);
            double k1 = 1.414213562 * wc; // Sqrt(2) * wc
            double k2 = wc * wc;
            double a = k2 / (1 + k1 + k2);
            double b = 2 * a;
            double c = a;
            double k3 = b / k2;
            double d = -2 * a + k3;
            double e = 1 - (2 * a) - k3;

            // RECURSIVE TRIGGERS - ENABLE filter is performed (first, last points constant)
            double[] DatYt = new double[dF2 + 4];
            DatYt[1] = DatYt[0] = indata[0];
            for (long s = 2; s < dF2 + 2; s++)
            {
                DatYt[s] = a * Dat2[s] + b * Dat2[s - 1] + c * Dat2[s - 2]
                           + d * DatYt[s - 1] + e * DatYt[s - 2];
            }
            DatYt[dF2 + 3] = DatYt[dF2 + 2] = DatYt[dF2 + 1];

            // FORWARD filter
            double[] DatZt = new double[dF2 + 2];
            DatZt[dF2] = DatYt[dF2 + 2];
            DatZt[dF2 + 1] = DatYt[dF2 + 3];
            for (long t = -dF2 + 1; t <= 0; t++)
            {
                DatZt[-t] = a * DatYt[-t + 2] + b * DatYt[-t + 3] + c * DatYt[-t + 4]
                            + d * DatZt[-t + 1] + e * DatZt[-t + 2];
            }

            // Calculated points copied for return
            for (long p = 0; p < dF2; p++)
            {
                data[p] = DatZt[p];
            }

            return data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.cutOffs = textBox1.Text;
            rysuj();
           
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fbd = new OpenFileDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.pathdata = fbd.FileName;
                    rysuj_default();
                }
            }
            
        }

        
    }
}
