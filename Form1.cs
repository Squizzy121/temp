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
        public void CreateGraph()
        {
            Graphics graph = graph1.CreateGraphics();

            Pen middle_pen = new Pen(Brushes.Blue, 2); //для осей
            Pen thin_pen = new Pen(Brushes.Black, 1);  //для сетки

            string num = textBox2.Text;
            string[] str = num.Split(' ');
            double n = double.Parse(str[0]);
            double m = double.Parse(str[1]);
            int cells; //количество клеток 
            int scale; //размер клетки
            int Y_axis = 1;
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

            for (int i = 0; i <= graph1.Height; i++)                      //сеткa вдоль X
            {
                graph.DrawLine(thin_pen, new Point(0, i * scale), new Point(graph1.Width, i * scale));
            }
            for (int i = 0; i <= graph1.Width; i++)                       //сеткa вдоль Y
            {
                graph.DrawLine(thin_pen, new Point(i * scale, 0), new Point(i * scale, graph1.Height));
            }

            BuildFunc(graph, scale, n, cells);

            Point point = new Point(111, 320);
            MarkOnAxis(graph, point, Y_axis, scale * (cells / 2));

        }
        public void BuildFunc(Graphics graph, int scale, double n, int cells)
        {
            Pen middle_pen2 = new Pen(Brushes.Red, 2);
            double[,] func = GetAllPoints(n, scale);
            for (int i = 0; i < graph1.Width - 1; i++)
            {
                Point num1 = new Point(i, Convert.ToInt32((graph1.Height /cells)*(cells/2) -((graph1.Height / cells) * func[1, i])));
                Point num2 = new Point(i + 1, Convert.ToInt32((graph1.Height /cells)*(cells/2) -( graph1.Height / cells * func[1, i + 1])));
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
        public void MarkOnAxis(Graphics graph, Point point, int Y_axis, int X_axis)
        {
            Pen Mark = new Pen(Brushes.Green, 2);

            Rectangle rect_Y = new Rectangle(Y_axis - 5, point.Y, 10, 1);
            graph.DrawRectangle(Mark, rect_Y);

            Rectangle rect_X = new Rectangle(point.X, X_axis - 5, 1, 10);
            graph.DrawRectangle(Mark, rect_X);

            RectangleF Drawpoint = new RectangleF(point.X - 3, point.Y - 3, 5, 5);
            graph.DrawEllipse(Mark, Drawpoint);
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            CreateGraph();
        }
    }
}

