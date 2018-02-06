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


namespace Solsystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<SpaceObject> solarSystem = InitSolarSystem();


            for (int i = 0; i < solarSystem.Count-1; i++) {
                Ellipse el;
                el = new Ellipse();
                el.Name = solarSystem[i].Name;
                el.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(solarSystem[i].objectColor));
                el.Width = 50;
                el.Height = 50;

                spaceFrame.Children.Add(el);
            }
            


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
                int orbRad = Convert.ToInt32(line[2]);
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

            foreach (SpaceObject obj in solarSystem)
            {
                switch (obj.Name)
                {
                    case "Sun":
                        break;
                    case "The Moon":
                        obj.Parent = solarSystem[3];
                        break;
                    default:
                        obj.Parent = solarSystem[0];
                        break;
                }

            }


            return solarSystem;
        }


    }

}

    