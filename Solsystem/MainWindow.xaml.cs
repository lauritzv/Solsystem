using System;
using System.Collections.Generic;
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
using SpaceSim;
using System.Reflection;
using System.IO;
using System.Windows.Threading;

namespace Solsystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double sizeOfSaturn = 60268;
        private List<SpaceObject> solarSystem;
        private double origox;
        private double origoy;
        private double time = 0;
        private const double MOVESPEED = 0.3;
        private const long TIMERFREQUENCY = 20000;
        private List<double> times = new List<double>();

        //Tilfeldige startposisjoner?
        private const bool RANDOMSTARTPOS = true;



        public MainWindow()
        {
            InitializeComponent();

            solarSystem = InitSolarSystem();
            origox = spaceWindow.Width / 2;
            origoy = spaceWindow.Height / 2;

            Random random = new Random(DateTime.Now.Second);

            for (int i = 0; i < solarSystem.Count; i++)
            {
                Ellipse el;
                double posx = 0;
                double posy = 0;
                el = new Ellipse();
                el.Name = solarSystem[i].Name;
                el.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(solarSystem[i].objectColor));

                if (el.Name.Contains("Sun"))
                    el.Width = 3 * Scale(solarSystem[i].objectRadius, sizeOfSaturn, 35.0);
                else
                    el.Width = Scale(solarSystem[i].objectRadius, sizeOfSaturn, 35.0);
                el.Height = el.Width;


                Tuple<double, double> pos;

                if (RANDOMSTARTPOS)
                {
                    times.Add(Math.Pow(random.NextDouble() * 365.0, 3));
                    pos = solarSystem[i].CalculatPos(times[i]);
                    if (el.Name.Contains("Moon") || el.Name == "Luna")
                    {
                        int earthIndex = solarSystem.IndexOf(solarSystem[i].Parent);
                        times[i] = times[earthIndex];
                    }
                }
                else
                    pos = solarSystem[i].CalculatPos(time);

                posx = origox + pos.Item1 * 100 - el.Width * 0.5;
                posy = origoy + pos.Item2 * 100 - el.Height * 0.5;
                Canvas.SetTop(el, posy);
                Canvas.SetLeft(el, posx);

                el.MouseDown += new MouseButtonEventHandler(el_MouseDown);

                spaceFrame.Children.Add(el);
            }

            //Timer:
            DispatcherTimer timer;
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(TIMERFREQUENCY)
            };
            timer.Tick += t_Tick;
            timer.Start();
        }


        private void t_Tick(object sender, EventArgs e)
        {
            if (RANDOMSTARTPOS)
            {
                for (int i = 0; i < times.Count; i++)
                {
                    times[i] += MOVESPEED;
                }
            }
            else
                time += MOVESPEED;


            UpdatePositions();
        }


        private void UpdatePositions()
        {
            double posx = 0;
            double posy = 0;
            for (int i = 0; i < solarSystem.Count; i++)
            {
                Ellipse e = (Ellipse)VisualTreeHelper.GetChild(spaceFrame, i);
                Tuple<double, double> pos;
                if (RANDOMSTARTPOS)
                    pos = solarSystem[i].CalculatPos(times[i]);
                else
                    pos = solarSystem[i].CalculatPos(time);

                posx = origox - e.Width * 0.5 + pos.Item1 * 100;
                posy = origoy - e.Height * 0.5 + pos.Item2 * 100;

                Canvas.SetTop(e, posy);
                Canvas.SetLeft(e, posx);
            }

        }

        public void el_MouseDown(object sender, MouseEventArgs e)
        {
            Shape shape = (Shape)e.OriginalSource;
            String name = shape.Name;
            MessageBox.Show(name + " clicked!");
            e.Handled = true;
        }

        public static double Scale(double value, double maxInputValue, double maxOutputValue)
        {
            if (value <= 1.0)
                return 0.0; // log is undefined for 0, log(1) = 0
            return maxOutputValue * Math.Log(value) / Math.Log(maxInputValue);
        }

        public List<SpaceObject> InitSolarSystem()
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"SpaceObjects.txt");
            string[] lines = System.IO.File.ReadAllLines(path);

            string[][] jaggedArray = lines.Select(line => line.Split(' ').ToArray()).ToArray();

            List<SpaceObject> solarSystem = new List<SpaceObject>();

            foreach (string[] line in jaggedArray)
            {
                string obj = line[0];
                string name = line[1];
                double orbRad = Convert.ToDouble(line[2]);
                int orbPeriod = Convert.ToInt32(line[3]);
                int objRad = Convert.ToInt32(line[4]);
                double rotPeriod = Convert.ToDouble(line[5]);
                string color = line[6];

                switch (obj)
                {
                    case "Star":
                        solarSystem.Add(new Star(name, orbRad, orbPeriod, objRad, rotPeriod, color));
                        break;
                    case "Planet":
                        solarSystem.Add(new Planet(name, orbRad, orbPeriod, objRad, rotPeriod, color));
                        break;
                    case "Moon":
                        solarSystem.Add(new Moon(name, orbRad, orbPeriod, objRad, rotPeriod, color));
                        break;
                }

            }

            //Set parent objects:
            foreach (SpaceObject obj in solarSystem)
            {
                switch (obj.Name.ToLower())
                {
                    case "sun":
                        break;

                    case "themoon":                     // proceed to case of "Luna"
                    case "luna":                        // luna orbits around terra/earth
                        SpaceObject earth = solarSystem.First(x => x.Name.ToLower() == "terra" || x.Name.ToLower().Contains("earth"));
                        if (earth != null)
                            obj.Parent = earth;
                        break;

                    default:                            //orbit around sun
                        SpaceObject sun = solarSystem.First(x => x.Name.ToLower() == "sun");
                        if (sun != null)
                            obj.Parent = sun;
                        break;
                }

            }


            return solarSystem;
        }


    }

}

