using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using OxyPlot.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet.Numerics.Interpolation;

namespace PerceptronSim
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            dgSimple.ItemsSource = finalList;
            oxyPlotModel = new OxyPlotModel();
            Plot1.DataContext = oxyPlotModel; // To pozwala połączyć kontrolki z polami klasy OxyPlotModel
        }

        private OxyPlotModel oxyPlotModel;

        List<List<PointInfo>> Values = new List<List<PointInfo>>();
        List<PointInfo> finalList = new List<PointInfo>();

        Random rand = new Random();
        double alfa,weight1,weight2,treshold,X2zero,X1zero,chartMaxX=0,chartMinX=0;
        OxyPlot.Series.ScatterSeries pointsSeries;

        private void BttnAddP_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(TBX1.Text, out double X1) && double.TryParse(TBX2.Text, out double X2) && pointsSeries!=null)
            {
                PointInfo r = new PointInfo();
                r.X1 = X1;
                r.X2 = X2;
                r.W1 = weight1;
                r.W2 = weight2;
                r.T = treshold;
                r.S = X1 * weight1 + X2 * weight2 - treshold;
                r.Epoch = "-";
                r.E = "-";
                if (r.S > 0)
                {
                    r.Y = 1;
                }
                else
                    r.Y = 0;
                if (X1 > chartMaxX)
                    chartMaxX = X1;
                if (X1 < chartMinX)
                    chartMinX = X1;

                finalList.Add(r);

                oxyPlotModel.PlotModel.Series.Clear();

                OxyPlot.Series.LineSeries line = new OxyPlot.Series.LineSeries();
                if (X1zero != 0 && X2zero != 0)
                {
                    // NevillePolynomialInterpolation stepInterpolation = new NevillePolynomialInterpolation(new double[] { X1zero,0 }, new double[] { 0, X2zero });
                    double b = X2zero;
                    double a = b / X1zero * -1;
                    //   line.Points.Add(new OxyPlot.DataPoint(chartMaxX, stepInterpolation.Interpolate(chartMaxX)));
                    //    line.Points.Add(new OxyPlot.DataPoint(chartMinX, stepInterpolation.Interpolate(chartMinX)));
                    line.Points.Add(new OxyPlot.DataPoint(chartMaxX, chartMaxX * a + b));
                    line.Points.Add(new OxyPlot.DataPoint(chartMinX, chartMinX * a + b));
                }
                else
                {
                    line.Points.Add(new OxyPlot.DataPoint(chartMaxX, chartMaxX));
                    line.Points.Add(new OxyPlot.DataPoint(chartMinX, chartMinX));
                }

                oxyPlotModel.PlotModel.Series.Add(line);
                pointsSeries.Points.Add(new OxyPlot.DataPoint(X1, X2));
                oxyPlotModel.PlotModel.Series.Add(pointsSeries);
                dgSimple.Items.Refresh();
            }

        }



        int numOfPoints = 4;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Values.Clear();
            bool end = false;
            bool parseSucces = true;
            string[] StrPointsX1 = { P1x1.Text, P2x1.Text, P3x1.Text, P4x1.Text };
            string[] StrPointsX2 = { P1x2.Text, P2x2.Text, P3x2.Text, P4x2.Text };
            string[] StrExpectedValues = { P1e.Text, P2e.Text, P3e.Text, P4e.Text };


            if (double.TryParse(W1.Text, out weight1) && double.TryParse(W2.Text, out weight2) && double.TryParse(T.Text, out treshold) && double.TryParse(A.Text, out alfa))
            {
                if (alfa > 10 || alfa < 0)
                    parseSucces = false;
            }
            else
            {
                parseSucces = false;
            }
            List<PointInfo> rArray = new List<PointInfo>();
            chartMaxX = 0;
            chartMinX = 0;
            for (int i = 0; i < numOfPoints; i++)
            {
                PointInfo r = new PointInfo();
                if (double.TryParse(StrPointsX1[i], out double X1))
                {
                    r.X1 = X1;
                    if(X1>chartMaxX)
                        chartMaxX = X1;
                    if (X1 < chartMinX)
                        chartMinX = X1;

                }
                else
                {
                    parseSucces = false;
                    break;
                }

                if (double.TryParse(StrPointsX2[i], out double X2))
                {
                    r.X2 = X2;
                }
                else
                {
                    parseSucces = false;
                    break;
                }
                if (int.TryParse(StrExpectedValues[i], out int ex))
                {
                    r.E = ex.ToString();
                }
                else
                {
                    parseSucces = false;
                    break;
                }
                r.W1 = weight1;
                r.W2 = weight2;
                r.T = treshold;
                r.Epoch = "1";
                rArray.Add(r);
            }


            if (parseSucces)
            {
                Values.Add(rArray);

                int i = 0;
                do
                {
                    end = true;
                    for (int j=0 ; j < numOfPoints; j++)
                    {
                        Values[i][j].S = Values[i][j].X1 * weight1 + Values[i][j].X2 * weight2 - treshold;
                        if(Values[i][j].S>0)
                        {
                            Values[i][j].Y = 1;
                        }
                        else
                            Values[i][j].Y = 0;

                        if (Convert.ToInt32(Values[i][j].E) != Values[i][j].Y)
                        {
                            end = false;
                            weight1 = weight1 - alfa * Values[i][j].X1 * (Values[i][j].Y - Convert.ToInt32(Values[i][j].E));
                            weight2 = weight2 - alfa * Values[i][j].X2 * (Values[i][j].Y - Convert.ToInt32(Values[i][j].E));
                            treshold = treshold + alfa * (Values[i][j].Y - Convert.ToInt32(Values[i][j].E));
                        }
                    }
                    if(!end)
                    {
                        i++;
                        List<PointInfo> rArrayCopy = new List<PointInfo>();
                        for (int j = 0; j < numOfPoints; j++)
                        {
                            rArrayCopy.Add(new PointInfo());

                            rArrayCopy[j].X1 = rArray[j].X1;
                            rArrayCopy[j].X2 = rArray[j].X2;
                            rArrayCopy[j].E = rArray[j].E;

                            rArrayCopy[j].T = treshold;
                            rArrayCopy[j].W1 = weight1;
                            rArrayCopy[j].W2 = weight2;
                            rArrayCopy[j].Epoch = (i+1).ToString();
                        }
                        Values.Add(rArrayCopy);

                    }

                }
                while(!end);

                finalList.Clear();
                foreach (List<PointInfo> listR in Values)
                {
                    foreach (PointInfo p in listR)
                    {
                        finalList.Add(p);
                    }
                }
                dgSimple.Items.Refresh();
                X2zero = (treshold / weight2);
                X1zero = (treshold / weight1);

                oxyPlotModel.PlotModel = new PlotModel();
                this.oxyPlotModel.PlotModel.Title = "Percepton points";
                oxyPlotModel.PlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis()
                {
                    Title = "X2",
                    Position = AxisPosition.Left,
                    // Magic Happens here we add the extra grid line on our Y Axis at zero
                    ExtraGridlines = new Double[] { 0 }
                });
                oxyPlotModel.PlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis()
                {
                    Title = "X1",
                    Position = AxisPosition.Bottom,
                    
                    // Magic Happens here we add the extra grid line on our Y Axis at zero
                    ExtraGridlines = new Double[] { 0 }
                });

                oxyPlotModel.PlotModel.Series.Clear();


                OxyPlot.Series.LineSeries line = new OxyPlot.Series.LineSeries();
                line.Points.Add(new OxyPlot.DataPoint(X1zero, 0));
                line.Points.Add(new OxyPlot.DataPoint(0, X2zero));
                if(X1zero != 0 && X2zero != 0 )
                {
                    // NevillePolynomialInterpolation stepInterpolation = new NevillePolynomialInterpolation(new double[] { X1zero,0 }, new double[] { 0, X2zero });
                    double b = X2zero;
                    double a = b / X1zero * -1;
                    //   line.Points.Add(new OxyPlot.DataPoint(chartMaxX, stepInterpolation.Interpolate(chartMaxX)));
                    //    line.Points.Add(new OxyPlot.DataPoint(chartMinX, stepInterpolation.Interpolate(chartMinX)));
                    line.Points.Add(new OxyPlot.DataPoint(chartMaxX, chartMaxX * a + b));
                    line.Points.Add(new OxyPlot.DataPoint(chartMinX, chartMinX * a + b));
                }
                else
                {
                    line.Points.Add(new OxyPlot.DataPoint(chartMaxX, chartMaxX));
                    line.Points.Add(new OxyPlot.DataPoint(chartMinX, chartMinX));
                }
                oxyPlotModel.PlotModel.Series.Add(line);
                pointsSeries = new OxyPlot.Series.ScatterSeries();
                pointsSeries.Points.Add(new OxyPlot.DataPoint(double.Parse(StrPointsX1[0]), double.Parse(StrPointsX2[0])));
                pointsSeries.Points.Add(new OxyPlot.DataPoint(double.Parse(StrPointsX1[1]), double.Parse(StrPointsX2[1])));
                pointsSeries.Points.Add(new OxyPlot.DataPoint(double.Parse(StrPointsX1[2]), double.Parse(StrPointsX2[2])));
                pointsSeries.Points.Add(new OxyPlot.DataPoint(double.Parse(StrPointsX1[3]), double.Parse(StrPointsX2[3])));
                pointsSeries.Points.Add(new OxyPlot.DataPoint(X1zero, 0));
                pointsSeries.Points.Add(new OxyPlot.DataPoint(0,X2zero ));

                pointsSeries.MarkerFill = OxyColors.SteelBlue;
                pointsSeries.MarkerType = MarkerType.Circle;

                oxyPlotModel.PlotModel.Series.Add(pointsSeries);
            }

        }

        private void BttnRand_Click(object sender, RoutedEventArgs e)
        {
            int n = rand.Next(0, 2);
            if (n == 0)
            {
                // W1.Text = Math.Round(rand.NextDouble(), 8).ToString();
                W1.Text = rand.NextDouble().ToString();
            }
            else
                W1.Text = (rand.NextDouble() - 1).ToString();
            n = rand.Next(0, 2);
            if (n == 0)
            {
                // W2.Text = Math.Round(rand.NextDouble(), 8).ToString();
                W2.Text = rand.NextDouble().ToString();
            }
            else
                W2.Text = (rand.NextDouble() - 1).ToString();
            //W2.Text = (Math.Round(rand.NextDouble(), 8) - 1).ToString();
            n = rand.Next(0, 2);
            if (n == 0)
            {
                // T.Text = Math.Round(rand.NextDouble(), 8).ToString();
                T.Text = rand.NextDouble().ToString();
            }
            else
                T.Text = (rand.NextDouble() - 1).ToString();

        }

        public class PointInfo
        {
            public double X1 { get; set; }
            public double X2 { get; set; }
            public string E { get; set; }
            public int Y { get; set; }
            public double S { get; set; }

            public double W1 { get; set; }
            public double W2 { get; set; }
            public double T { get; set; }
            public string Epoch { get; set; }

        }
        public class OxyPlotModel : INotifyPropertyChanged
        {

            private OxyPlot.PlotModel plotModel;
            public OxyPlot.PlotModel PlotModel
            {
                get
                {
                    return plotModel;
                }
                set
                {
                    plotModel = value;
                    OnPropertyChanged("PlotModel");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}
