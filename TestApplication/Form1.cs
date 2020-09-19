using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CPHControl;

namespace TestApplication_
{
    public partial class Form1 : Form
    {
        protected Color[] colorlist;
        protected double actTime = 0;
        protected Stopwatch StpWatch;
        protected string[] lines;
        RollingVec2List[] points;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string[] lines = { "ThisIsan Awesome Line with lots of Text inside of it", "line2", "line3", "line4", "line5", "LineLineLine", "TheLine", "ThisLine", "ThatLine", "AnotherLine" };
            string[] axes = { "YAxisOne", "YAxisTwo", "YAxisThree", "YAxisFour", "YAxisFive" };
            Color[] colorlist = { Color.Blue, Color.LightGreen, Color.Red, Color.LightCoral, Color.Black, Color.Brown, Color.DarkSeaGreen, Color.LightSalmon, Color.Gold, Color.Orchid };
            points = new RollingVec2List[lines.Length];
            GraphPane myPane = cphControl1.GraphPane;
            int i = 0;
            foreach (string line in lines)
            {
                points[i] = new RollingVec2List(10000);
                myPane.AddCurve(line, points[i], colorlist[i]);
                if ((i / 2) * 2 == i)
                {
                    myPane.PYAxisList.Add(axes[i/2]);
                    myPane.PYAxisList[i / 2].Scale.Max = 200 * (i + 1);
                    myPane.PYAxisList[i / 2].Scale.Min = 0;
                }
                myPane.PCurveList[i].YAxisIndex = i / 2;
                i++;
            }

            Timer myTimer = new Timer();
            myTimer.Tick += MyTimer_Tick;
            myTimer.Interval = 20;

            myTimer.Start();
            StpWatch = new Stopwatch();
            StpWatch.Reset();
            StpWatch.Start();
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            double IntervalExact = (double)(StpWatch.ElapsedMilliseconds) / 1000.0;
            StpWatch.Restart();
            actTime += IntervalExact;

            double Val;

            for (int i = 0; i < 10; i++)
            {
                Val = 200 * (i + 1) / 2 + Math.Sin(actTime / (i + 1)) * 200 * (i + 1)/3;
                points[i].Add(actTime, Val);
            }
        }
    }
}
