using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

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
        private double hz;
        private int page=0;
        private double[,] array = new double[4, 25];
        public void CreateGraph()
        {
            Graphics graph = graph1.CreateGraphics();

            Pen middle_pen = new Pen(Brushes.Blue, 2); //для осей
            Pen thin_pen = new Pen(Brushes.Black, 1);  //для сетки

            string num = textBox2.Text;
            string[] str = num.Split(' ');
            double n = double.Parse(str[0])-1;
            double m = double.Parse(str[1])+1;
            int cells; //количество клеток 
            int scale; //размер клетки
            Y_axis = 1;
            if (n >= 0 && m > 0)
            {
                cells = Convert.ToInt32(m) - Convert.ToInt32(n);
                scale = graph1.Height / cells;
                graph.DrawLine(middle_pen, new Point(1, 1), new Point(1, graph1.Height)); //Ось У
            }
            else if (n < 0 && m < 0)
            {
                cells = Math.Abs(Convert.ToInt32(n) - Convert.ToInt32(m));
                scale = graph1.Height / cells;
                graph.DrawLine(middle_pen, new Point(scale * cells, 0), new Point(scale * cells, graph1.Height)); //Ось У
                Y_axis = scale * cells;
            }
            else
            {
                cells = Convert.ToInt32(m) + Math.Abs(Convert.ToInt32(n));
                scale = graph1.Height / cells;
                graph.DrawLine(middle_pen, new Point(scale * Convert.ToInt32(Math.Abs(n)), 0), new Point(scale * Convert.ToInt32(Math.Abs(n)), graph1.Height)); //Ось У
                Y_axis = scale * Convert.ToInt32(Math.Abs(n));
            }

            graph.DrawLine(middle_pen, new Point(0, scale * (cells / 2)), new Point(graph1.Width, scale * (cells / 2))); //ось Х
            X_axis = scale * (cells / 2);
            hz = graph1.Height / cells;

            for (int i = 0; i <= graph1.Height; i++)                      //сеткa вдоль X
            {
                graph.DrawLine(thin_pen, new Point(0, i * scale), new Point(graph1.Width, i * scale));
            }
            for (int i = 0; i <= graph1.Width; i++)                       //сеткa вдоль Y
            {
                graph.DrawLine(thin_pen, new Point(i * scale, 0), new Point(i * scale, graph1.Height));
            }

            BuildFunc(graph, scale, n, cells);
            FindMinValueOnFunc(n+1, m-1);
        }
        public void BuildFunc(Graphics graph, int scale, double n, int cells)
        {
            Pen middle_pen2 = new Pen(Brushes.Red, 2);
            double[,] func = GetAllPoints(n, scale);
            for (int i = 0; i < graph1.Width - 1; i++)
            {
                Point num1 = new Point(i, Convert.ToInt32((graph1.Height / cells) * (cells / 2) - ((graph1.Height / cells) * func[1, i])));
                Point num2 = new Point(i + 1, Convert.ToInt32((graph1.Height / cells) * (cells / 2) - (graph1.Height / cells * func[1, i + 1])));
                graph.DrawLine(middle_pen2, num1, num2);
            }
        }
        public double[,] GetAllPoints(double n, int scale)
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
            array[2, 0] = u;
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
            //label5.Text = "Точка мінімуму на відрізку [" + Math.Round(n + 1, 4) + ";" + Math.Round(m - 1, 4) + "] знаходиться в точці х=" + Math.Round(min_cords[0], 4) + " і дорівнює " + min_cords[1];
            PrintAllSteps();
        }
        public double CalculateFunc(double x)
        {
            string str = textBox1.Text.Replace("x", x.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
            double fx = Convert.ToDouble(new DataTable().Compute(str, ""));
            return fx;
        }
        private void PrintAllSteps()
        {
            label6.Text = "a=" + Math.Round(array[0,page], 4) + "\nb=" + Math.Round(array[1,page], 4) + "\nu=" + Math.Round(array[2,page], 4) + "\nv=" + Math.Round(array[3,page], 4);
            Graphics pt1 = graph1.CreateGraphics();
            //pt1.Clear(Color.White);  Залишилось тільки стерти точки
            Drawpoints(pt1);
            pt1.Dispose();
        }
        private void Drawpoints(Graphics pt1)
        {
            SolidBrush for_point = new SolidBrush(Color.Green);
            SolidBrush for_axis = new SolidBrush(Color.Black);
            string[] text=new string[4] {"a","b","u","v" } ;
            for (int i=0;i<array.GetLength(0);i++)
            {
                RectangleF Drawpoint = new RectangleF(Convert.ToInt32(Y_axis+array[i,page]*hz),Convert.ToInt32(X_axis-CalculateFunc(array[i,page])*hz)-5, 8, 8);
                pt1.FillEllipse(for_point, Drawpoint);

                RectangleF x_axis = new RectangleF(Convert.ToInt32(Y_axis + array[i, page] * hz)-3, X_axis-5, 8, 8);
                RectangleF x_axis2 = new RectangleF(Convert.ToInt32(Y_axis + array[i, page] * hz)-3, X_axis + 4, 20, 15);
                pt1.FillEllipse(for_axis, x_axis);
                pt1.DrawString(text[i], label1.Font, for_axis, x_axis2);

                RectangleF y_axis = new RectangleF(Y_axis - 5, Convert.ToInt32(X_axis - CalculateFunc(array[i, page]) * hz)-5, 8, 8);
                RectangleF y_axis2 = new RectangleF(Y_axis + 5, Convert.ToInt32(X_axis - CalculateFunc(array[i, page]) * hz)+3, 30, 15);
                pt1.FillEllipse(for_axis, y_axis);
                string str = "f(" + text[i] + ")";
                pt1.DrawString(str, label1.Font, for_axis, y_axis2);
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            CreateGraph();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (page > 0)
            {
                page--;
            }
            PrintAllSteps();
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            if (page < array.GetLength(1))
            {
                page++;
            }
            PrintAllSteps();
        }
    }
}

