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
            double[] min_cords=FindMinValueOnFunc(n+1, m-1);
            Point min = new Point(Convert.ToInt32(Y_axis+scale*min_cords[0]), Convert.ToInt32((graph1.Height / cells) * (cells / 2) - (graph1.Height / cells * min_cords[1])));
            DrawPoints(min,"x*");

            DrawMarksOnAxis(n + 1, graph1.Height / cells);
            DrawMarksOnAxis(m-1, graph1.Height / cells);
            label5.Text = "Точка мінімуму на відрізку [" + (n+1) + ";" + (m-1) + "] знаходиться в точці х=" + min_cords[0] + " і дорівнює " + min_cords[1];
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
        private void DrawPoints(Point point,string text)
        {
            Graphics graph11 = graph1.CreateGraphics();
            SolidBrush for_point = new SolidBrush(Color.Green);
            SolidBrush for_axis = new SolidBrush(Color.Black);

            RectangleF Drawpoint = new RectangleF(point.X - 3, point.Y - 5, 8, 8);
            graph11.FillEllipse(for_point, Drawpoint);

            RectangleF x_axis = new RectangleF(point.X - 3, X_axis - 5, 8, 8);
            RectangleF x_axis2 = new RectangleF(point.X - 3, X_axis +4, 20, 15);
            graph11.FillEllipse(for_axis, x_axis);
            graph11.DrawString(text, label1.Font, for_axis, x_axis2);

            RectangleF y_axis = new RectangleF(Y_axis - 5, point.Y - 5, 8, 8);
            RectangleF y_axis2 = new RectangleF(Y_axis +5, point.Y +3, 30, 15);
            graph11.FillEllipse(for_axis, y_axis);
            string str = "f("+text +")";
            graph11.DrawString(str, label1.Font, for_axis, y_axis2);
        }
        public double[] FindMinValueOnFunc(double n, double m)
        {
            double u = n + (3 - Math.Sqrt(5)) / 2 * (m - n);
            double v = n + m - u;
            double fu = CalculateFunc(u);
            double fv = CalculateFunc(v);
            DrawPoints(new Point(Convert.ToInt32(u * hz+Y_axis),Convert.ToInt32(X_axis-hz*fu)),"u");
            DrawPoints(new Point(Convert.ToInt32(v * hz + Y_axis), Convert.ToInt32(X_axis - hz * fv)), "v");
            double e = double.Parse(textBox3.Text);
            double x1, fx1;
            int index = 1;
            bool stop = true;
            while (m - n > e)
            {
                if (fu <= fv)
                {
                    m = v;
                    v = u;
                    fv = fu;
                    u = n + m - v;
                    fu = CalculateFunc(u);
                    if (stop)
                    {
                        DrawPoints(new Point(Convert.ToInt32(u * hz + Y_axis), Convert.ToInt32(X_axis - hz * fu)), "u" + index);
                        DrawPoints(new Point(Convert.ToInt32(v * hz + Y_axis), Convert.ToInt32(X_axis - hz * fv)), "v" + index);
                        index++;
                    }
                }
                else
                {
                    n = u;
                    u = v;
                    fu = fv;
                    v = n + m - u;
                    fv = CalculateFunc(v);
                    if (stop)
                    {
                        DrawPoints(new Point(Convert.ToInt32(u * hz + Y_axis), Convert.ToInt32(X_axis - hz * fu)), "u" + index);
                        DrawPoints(new Point(Convert.ToInt32(v * hz + Y_axis), Convert.ToInt32(X_axis - hz * fv)), "v" + index);
                        index++;
                    }
                }
                if (u > v)
                {
                    u = n + (3 - Math.Sqrt(5)) / 2 * (m - n);
                    v = n + m - u;
                    fu = CalculateFunc(u);
                    fv = CalculateFunc(v);
                    if (stop)
                    {
                        DrawPoints(new Point(Convert.ToInt32(u * hz + Y_axis), Convert.ToInt32(X_axis - hz * fu)), "u" + index);
                        DrawPoints(new Point(Convert.ToInt32(v * hz + Y_axis), Convert.ToInt32(X_axis - hz * fv)), "v" + index);
                        index++;
                    }
                }
                stop = false;
            }
            x1 = (n + m) / 2;
            fx1 = CalculateFunc(x1);
            double[] temp=new double[2] {x1,fx1 };
            return temp;
        }
        public double CalculateFunc(double x)
        {
            string str = textBox1.Text.Replace("x", x.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
            double fx = Convert.ToDouble(new DataTable().Compute(str, ""));
            return fx;
        }
        public void DrawMarksOnAxis(double num,double scale)
        {
            Graphics pt1 = graph1.CreateGraphics();
            SolidBrush idk = new SolidBrush(Color.Black);
            RectangleF rect = new RectangleF(Convert.ToInt32(scale*num+Y_axis),X_axis,20,15);
            pt1.DrawString(Convert.ToString(num), label1.Font, idk,rect);

            RectangleF x_axis = new RectangleF(Convert.ToInt32(scale * num + Y_axis-4), X_axis - 5, 8, 8);
            pt1.FillEllipse(idk, x_axis);
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            CreateGraph();
        }
    }
}

