using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace N3
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int Y_axis;
        private int X_axis;
        private double scale;
        private int page=0;
        private double[,] array = new double[4, 25];
        private double[,] func;
        private double n;
        private int cells;
        Graphics graph;

        public void CreateGraph()
        {
            graph = graph1.CreateGraphics();

            Pen middle_pen = new Pen(Brushes.Blue, 2); //для осей
            Pen thin_pen = new Pen(Brushes.Black, 1);  //для сетки

            string num = textBox2.Text;
            string[] str = num.Split(' ');
            n = double.Parse(str[0])-1;
            double m = double.Parse(str[1])+1;
            Y_axis = 1;
            if (n >= 0 && m > 0)
            {
                cells = Convert.ToInt32(m) - Convert.ToInt32(n);
                scale = graph1.Height / cells;
            }
            else if (n < 0 && m < 0)
            {
                cells = Math.Abs(Convert.ToInt32(n) - Convert.ToInt32(m));
                scale = graph1.Height / cells;
                Y_axis = Convert.ToInt32(scale * cells);
            }
            else
            {
                cells = Convert.ToInt32(m) + Math.Abs(Convert.ToInt32(n));
                scale = graph1.Height / cells;
                Y_axis = Convert.ToInt32(scale * Math.Abs(n));
            }
            
            X_axis = Convert.ToInt32(scale * (cells / 2));
            func=GetAllPoints(n);
            FindMinValueOnFunc(n+1, m-1);
        }
        public void BuildFunc( double n)
        {
            Pen middle_pen2 = new Pen(Brushes.Red, 2);
            DrawAxis();
            for (int i = 0; i < graph1.Width - 1; i++)
            {
                Point num1 = new Point(i, Convert.ToInt32(scale * (cells / 2) - (scale * func[1, i])));
                Point num2 = new Point(i + 1, Convert.ToInt32(scale * (cells / 2) - (scale * func[1, i + 1])));
                graph.DrawLine(middle_pen2, num1, num2);
            }
        }
        public void DrawAxis()
        {
            Pen middle_pen = new Pen(Brushes.Blue, 2); 
            Pen thin_pen = new Pen(Brushes.Black, 1);  
            for (int i = 0; i <= graph1.Height; i++)                      //сеткa вдоль X
            {
                graph.DrawLine(thin_pen, new Point(0, Convert.ToInt32(i * scale)), new Point(graph1.Width, Convert.ToInt32(i * scale)));
            }
            for (int i = 0; i <= graph1.Width; i++)                       //сеткa вдоль Y
            {
                graph.DrawLine(thin_pen, new Point(Convert.ToInt32(i * scale), 0), new Point(Convert.ToInt32(i * scale), graph1.Height));
            }
            graph.DrawLine(middle_pen, new Point(0, Convert.ToInt32(scale * (cells / 2))), new Point(graph1.Width, Convert.ToInt32(scale * (cells / 2)))); //ось Х
            graph.DrawLine(middle_pen, new Point(Y_axis, 0), new Point(Y_axis, graph1.Height));
        }
        public double[,] GetAllPoints(double n)
        {

            double[,] array = new double[2, 500];
            double result;
            double x_cord = n;
            for (int i = 0; i < graph1.Width; i++)
            {
                string ins = textBox1.Text.Replace("x", x_cord.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
                result = Convert.ToDouble(new DataTable().Compute(ins, ""));
                array[0, i] = x_cord;
                array[1, i] = result;
                x_cord = x_cord + 1 / Convert.ToDouble(scale);
            }
            return array;
        }
        public void FindMinValueOnFunc(double n, double m)
        {
            double u = n + (3 - Math.Sqrt(5)) / 2 * (m - n);
            double v = n + m - u;
            double fu = CalculateFunc(u);
            double fv = CalculateFunc(v);
            double e = double.Parse(textBox3.Text);
            double x1, fx1;
            array[0, 0] = n;
            array[1, 0] = m;
            array[2, 0] = n + (3 - Math.Sqrt(5)) / 2 * (m - n);
            array[3, 0] = v;
            int index = 1;
            while (m - n > e)
            {
                if (fu <= fv)
                {
                    m = v;
                    v = u;
                    fv = fu;
                    u = n + m - v;
                    fu = CalculateFunc(u);
                    array[0, index] = n;
                    array[1, index] = m;
                    array[2, index] = u;
                    array[3, index] = v;
                    index++;
                }
                else
                {
                    n = u;
                    u = v;
                    fu = fv;
                    v = n + m - u;
                    fv = CalculateFunc(v);
                    array[0, index] = n;
                    array[1, index] = m;
                    array[2, index] = u;
                    array[3, index] = v;
                    index++;
                }
                if (u > v)
                {
                    u = n + (3 - Math.Sqrt(5)) / 2 * (m - n);
                    v = n + m - u;
                    fu = CalculateFunc(u);
                    fv = CalculateFunc(v);
                    array[0, index] = n;
                    array[1, index] = m;
                    array[2, index] = u;
                    array[3, index] = v;
                    index++;
                }
            }
            x1 = (n + m) / 2;
            fx1 = CalculateFunc(x1);
            PrintAllSteps();
            label5.Text = "Мінімум функції знаходиться в точці " + x1 + " і дорівнює " + fx1;
            Drawpoints();
        }
        public double CalculateFunc(double x)
        {
            string str = textBox1.Text.Replace("x", x.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
            double fx = Convert.ToDouble(new DataTable().Compute(str, ""));
            return fx;
        }
        private void PrintAllSteps()
        {
            label6.Text = "a=" + array[0,page]+ "\nb=" +array[1,page] + "\nu=" + array[2,page] + "\nv=" + array[3,page];
        }
        private void Drawpoints()
        {
            graph.Clear(graph1.BackColor);
            BuildFunc(n);
            SolidBrush for_point = new SolidBrush(Color.Green);
            SolidBrush for_axis = new SolidBrush(Color.Black);
            string[] text=new string[4] {"a","b","u","v" } ;
            for (int i=0;i<array.GetLength(0);i++)
            {
                RectangleF Drawpoint = new RectangleF(Convert.ToInt32(Y_axis+array[i,page]*scale-4),Convert.ToInt32(X_axis-CalculateFunc(array[i,page])*scale)-5, 8, 8);
                graph.FillEllipse(for_point, Drawpoint);
                RectangleF x_axis = new RectangleF(Convert.ToInt32(Y_axis + array[i, page] * scale)-3, X_axis-5, 8, 8);
                RectangleF x_axis2 = new RectangleF(Convert.ToInt32(Y_axis + array[i, page] * scale)-3, X_axis + 4, 20, 15);
                graph.FillEllipse(for_axis, x_axis);
                graph.DrawString(text[i], label1.Font, for_axis, x_axis2);

                RectangleF y_axis = new RectangleF(Y_axis - 5, Convert.ToInt32(X_axis - CalculateFunc(array[i, page]) * scale)-5, 8, 8);
                RectangleF y_axis2 = new RectangleF(Y_axis + 5, Convert.ToInt32(X_axis - CalculateFunc(array[i, page]) * scale)+3, 30, 15);
                graph.FillEllipse(for_axis, y_axis);
                string str = "f(" + text[i] + ")";
                graph.DrawString(str, label1.Font, for_axis, y_axis2);
                
            }
            
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            CreateGraph();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (page > 0 && array[0, page + 1] != 0 && array[1, page + 1] != 0)
            {
                page--;
            }
            PrintAllSteps();
            Drawpoints();
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            if (page < array.GetLength(1)&&array[0,page+1]!=0&&array[1,page+1]!=0)
            {
                page++;
            }
            PrintAllSteps();
            Drawpoints();
        }
    }
}

