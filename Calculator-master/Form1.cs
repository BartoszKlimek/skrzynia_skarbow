using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Calculator : Form
    {
        private double value = 0;
        private string operation = "";
        private bool operation_pressed = false;
        private bool dot_pressed = false;

        public Calculator()
        {
            InitializeComponent();                        
        }

        private void number_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if ((result.Text == "0") || operation_pressed || (result.Text == "Nie dziel przez zero !")) 
            {
                result.Text = b.Text;
                operation_pressed = false;
                ButtonEnabled();
            }
            else
            {
                result.Text += b.Text;
            }
        }

        private void dot_click(object sender, EventArgs e)
        {
            if (!dot_pressed)
            {
                Button b = (Button)sender;
                result.Text += b.Text;
                dot_pressed = true;
            }  
        }
        
        private void operator_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if (!operation_pressed)
            {
                if (value != 0)
                {
                    equal.PerformClick();
                    operation = b.Text;
                    operation_pressed = true;
                    label1.Text += value + " " + operation;
                }
                else
                {
                    value = double.Parse(result.Text);
                    operation_pressed = true;
                    operation = b.Text;
                    label1.Text = value.ToString() + " " + operation;
                    dot_pressed = false;
                }
            }
            else
            {
                operation_pressed = true;
                operation = b.Text;
                label1.Text = value.ToString() + " " + operation;
            }
        }

        private void equals_click(object sender, EventArgs e)
        {
            switch (operation)
            {
                case "+":
                    result.Text = (value + double.Parse(result.Text)).ToString();
                    break;
                case "-":
                    result.Text = (value - double.Parse(result.Text)).ToString();
                    break;
                case "/":
                    if (result.Text != "0")
                    {
                        result.Text = (value / double.Parse(result.Text)).ToString();
                    }
                    else
                    {
                        result.Text = "Nie dziel przez zero !";
                        ButtonDisabled();
                    }
                    break;
                case "*":
                    result.Text = (value * double.Parse(result.Text)).ToString();
                    break;
                default:
                    break;

            }//end of switch

            if (result.Text != "Nie dziel przez zero !")
            {
                value = double.Parse(result.Text);
            }
            else
            {
                value = 0;
            }
            label1.Text = "";
            operation = "";
            dot_pressed = false;
        }
                      
        private void CE_click(object sender, EventArgs e)
        {
            result.Text = "0";
            label1.Text = "";
            dot_pressed = false;
            value = 0;
            ButtonEnabled();
        }

        private void C_click(object sender, EventArgs e)
        {
            if (result.Text == "Nie dziel przez zero !")
            {

            }
            else if (result.Text.Length == 1)
            {
                result.Text = "0";
            }
            else
            {
                string temp = result.Text;
                result.Text = temp.Substring(0, temp.Length - 1);
            }
        }

        public void ButtonDisabled()
        {
            plus.Enabled = false;
            minus.Enabled = false;
            delete.Enabled = false;
            multi.Enabled = false;
            division.Enabled = false;
            equal.Enabled = false;
            dot.Enabled = false;
        }

        public void ButtonEnabled()
        {
            plus.Enabled = true;
            minus.Enabled = true;
            multi.Enabled = true;
            delete.Enabled = true;
            division.Enabled = true;
            equal.Enabled = true;
            dot.Enabled = true;
        }

        private void Calculator_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Escape)
            {
                Environment.Exit(0);
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                equal.PerformClick();
            }
            if (e.KeyChar == (char)Keys.Back)
            {
                delete.PerformClick();
            }
           
            switch (e.KeyChar.ToString().ToLower())
            {
                case "0":
                    zero.PerformClick();
                    break;
                case "1":
                    one.PerformClick();
                    break;
                case "2":
                    two.PerformClick();
                    break;
                case "3":
                    three.PerformClick();
                    break;
                case "4":
                    four.PerformClick();
                    break;
                case "5":
                    five.PerformClick();
                    break;
                case "6":
                    six.PerformClick();
                    break;
                case "7":
                    seven.PerformClick();
                    break;
                case "8":
                    eight.PerformClick();
                    break;
                case "9":
                    nine.PerformClick();
                    break;
                case "=":
                    equal.PerformClick();
                    break;
                case "*":
                    multi.PerformClick();
                    break;
                case "/":
                    division.PerformClick();
                    break;
                case "-":
                    minus.PerformClick();
                    break;
                case ",":
                    dot.PerformClick();
                    break;
                case "+":
                    plus.PerformClick();
                    break;
            }
        }
    }
}
