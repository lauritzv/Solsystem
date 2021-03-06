﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SpaceSim;
using System.Reflection;
using System.Windows.Threading;

namespace Solsystem
{

    public partial class MainWindow : Window
    {
        private double sizeOfSaturn = 60268;
        private List<SpaceObject> solarSystem;
        private double origox;
        private double origoy;
        private List<Point> positions = new List<Point>();
        private double time = 0;
        private const double MOVESPEED = 0.3;
        private const long TIMERFREQUENCY = 20000;
        private List<double> times = new List<double>();

        //Tilfeldige startposisjoner?
        private const bool RANDOMSTARTPOS = true;

        private Random random = new Random(DateTime.Now.Second);

        public MainWindow()
        {
            InitializeComponent();

            solarSystem = InitSolarSystem();
            Loaded += delegate
            {
                origox = spaceWindow.ActualWidth / 2;
                origoy = spaceWindow.ActualHeight / 2;
                objectFrame.Height = spaceWindow.ActualHeight * 0.25;
                objectFrame.Width = objectFrame.Height;

                for (int i = 0; i < solarSystem.Count; i++)
                {
                    Ellipse el = CreateOverviewEllipse(i);
                    spaceFrame.Children.Add(el);
                    Ellipse orbit = CreateOverviewOrbit(i);
                    orbitFrame.Children.Add(orbit);
                    Label tl = CreateOverviewTextlabel(i);
                    labelFrame.Children.Add(tl);
                } 

            spaceWindow.MouseRightButtonDown += new MouseButtonEventHandler(spaceWindow_MouseRightButtonDown);

            //Timer:
            DispatcherTimer timer;
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(TIMERFREQUENCY)
            };
            timer.Tick += t_Tick;
            timer.Start();
            };
        }

        private Label CreateOverviewTextlabel(int index)
        {
            Label tl = new Label()
            {
                Content = solarSystem[index].Name,
                Foreground = Brushes.White
            };
            return tl;
        }

        private void UpdateLabelPositions()
        {
            for (int i = 0; i< labelFrame.Children.Count; i++)
            {
                Shape item = (Shape)spaceFrame.Children[i];

                Canvas.SetLeft(labelFrame.Children[i], positions[i].X + item.Width);
                Canvas.SetTop(labelFrame.Children[i], positions[i].Y);
            }
        }

        //oppretter planet, måne eller stjerne grafikken som skal tegnes i oversiktsbildet
        private Ellipse CreateOverviewEllipse(int i)
        {
            double posx = 0;
            double posy = 0;

            Ellipse el = new Ellipse();
            el.Name = solarSystem[i].Name;
            el.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(solarSystem[i].objectColor));

            if (el.Name.Contains("Sun"))
                el.Width = 3 * Scale(solarSystem[i].objectRadius, sizeOfSaturn, 35.0);
            else
                el.Width = Scale(solarSystem[i].objectRadius, sizeOfSaturn, 35.0);
            el.Height = el.Width;

            Tuple<double, double> pos = solarSystem[i].CalculatPos(time);

            if (RANDOMSTARTPOS)
            {
                times.Add(Math.Pow(random.NextDouble() * 365.0, 3));
                pos = solarSystem[i].CalculatPos(times[i]);
                if ( solarSystem[i].GetType() == typeof(Moon) )
                {
                    int parentIndex = solarSystem.IndexOf(solarSystem[i].Parent);
                    times[i] = times[parentIndex];
                }
            }

            posx = origox + pos.Item1 * 160 - el.Width * 0.5;
            posy = origoy + pos.Item2 * 85 - el.Height * 0.5;
            positions.Add(new Point(posx, posy));
            Canvas.SetTop(el, posy);
            Canvas.SetLeft(el, posx);

            el.MouseLeftButtonDown += new MouseButtonEventHandler(el_MouseLeftButtonDown);

            return el;
        }

        //oppretter grafikken til banen som skal tegnes i oversiktsbildet
        private Ellipse CreateOverviewOrbit(int i)
        {
            Ellipse orbit = new Ellipse();
            orbit.Height = 2 * solarSystem[i].orbitalRadius * 85;
            orbit.Width = orbit.Height / 85 * 160;
            Canvas.SetLeft(orbit, origox - orbit.Width * 0.5);
            Canvas.SetTop(orbit, origoy - orbit.Height * 0.5);
            orbit.Stroke = Brushes.White;
            orbit.StrokeThickness = 0.25;
            return orbit;
        }

        private void Orbits_Click(object sender, RoutedEventArgs e)
        {
            if (orbitFrame.Visibility == Visibility.Collapsed)
                orbitFrame.Visibility = Visibility.Visible;
            else orbitFrame.Visibility = Visibility.Collapsed;
        }
        private void Labels_Click(object sender, RoutedEventArgs e)
        {
            if (labelFrame.Visibility == Visibility.Collapsed)
                labelFrame.Visibility = Visibility.Visible;
            else labelFrame.Visibility = Visibility.Collapsed;
        }

        private void t_Tick(object sender, EventArgs e)
        {
            if (RANDOMSTARTPOS)
            {
                for (int i = 0; i < times.Count; i++)
                    times[i] += MOVESPEED;
            }
            else time += MOVESPEED;

            UpdatePositions();
            UpdateLabelPositions();
            updateBoxPositions();
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

                posx = origox - e.Width * 0.5 + pos.Item1 * 160;
                posy = origoy - e.Height * 0.5 + pos.Item2 * 85;
                positions[i] = new Point(posx, posy);

                Canvas.SetTop(e, posy);
                Canvas.SetLeft(e, posx);
            }
        }

        // oppdaterer posisjonene til evt. måner i infoboksen 
        private void updateBoxPositions()
        {
            if (objectFrame.Children.Count > 1)
            {
                Shape p = (Shape)objectFrame.Children[0];
                int parentindex = Stripletters(p.Name);
                Point parentpos = positions[parentindex];
                for (int i = 1; i < objectFrame.Children.Count; i++)
                {
                    if ((objectFrame.Children[i].GetType() == typeof(Ellipse)))
                    {
                        Shape c = (Shape)objectFrame.Children[i];
                        int index = Stripletters(c.Name);
                        Point childpos = positions[index];
                        double xoffset = parentpos.X - childpos.X;
                        double yoffset = parentpos.Y - childpos.Y;
                        Canvas.SetLeft(c, objectFrame.Width * 0.5 - c.Width * 0.5 - xoffset * 2.1);
                        Canvas.SetTop(c, objectFrame.Height * 0.1 + p.Height * 0.5 - c.Height * 0.5 - yoffset * 2.8);
                    }
                }
            }
        }

        // fjerner foregående bokstaver og returnerer den resterende int verdien fra en streng "abcabc123" -> 123
        private int Stripletters(string s)
        {
            while (!Char.IsNumber(s, 0) && s.Length > 0)
                s = s.Substring(1);
            
            if (s.Length > 0)
                return Convert.ToInt32(s);
            
            else return 0;
        }

        public void spaceWindow_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            objectFrame.Children.Clear();
            objectFrame.Visibility = Visibility.Collapsed;
        }


        public void el_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            objectFrame.Visibility = Visibility.Visible;
            objectFrame.Children.Clear();

            Shape shape = (Shape)e.OriginalSource;
            int index = spaceFrame.Children.IndexOf(shape);
            AddShapeToObjectFrame(shape, index);
            AddChildrenToObjectFrame(solarSystem[index]);

            AddDataText(solarSystem[index]);
            
            e.Handled = true;
        }

        private void AddDataText(SpaceObject target)
        {
            TextBlock name = new TextBlock();
            name.Text = target.GetType().ToString().Substring(9) + ": " + target.Name;
            if (target.orbitalRadius != 0)
                name.Text += "\nOrbit radius: " + target.orbitalRadius;
            if (target.orbitalPeriod != 0)
                name.Text += "\nOrbit period: " + target.orbitalPeriod + dayOrDays(target.orbitalPeriod);
            name.Text += "\nObject radius: " + target.objectRadius + " km";
            name.Text += "\nRotational period: " + target.rotationalPeriod + dayOrDays(target.rotationalPeriod);
            if (target.Parent != null)
                name.Text += "\nOrbits around " + target.Parent.Name;
            name.Margin = new Thickness(5, objectFrame.Height * 0.55, 0, 0);
            name.Foreground = Brushes.White;

            objectFrame.Children.Add(name);
        }

        // Returnerer riktig bruk av flertall eller entall for utskrift som gjelder dager
        private Func<double, string> dayOrDays = n => (n <= 1) ? " day" : " days";
        
        private void AddShapeToObjectFrame(Shape shape, int index)
        {
            Ellipse el = new Ellipse();
            el.Name = shape.Name + index.ToString();
            el.Fill = shape.Fill;
            el.Height = objectFrame.Height * 0.3;
            el.Width = el.Height;
            Canvas.SetTop(el, objectFrame.Height * 0.1);
            Canvas.SetLeft(el, objectFrame.Width * 0.5 - (el.Width * 0.5));
            objectFrame.Children.Add(el);
        }

        private void AddChildrenToObjectFrame(SpaceObject spOb)
        {
            List<SpaceObject> children = new List<SpaceObject>();
            if (!spOb.Name.ToLower().Equals("sun")) // hindrer at alt som orbiter solen tegnes i infoboksen
            {
                for (int i = 0; i < solarSystem.Count; i++)
                {
                    if (solarSystem[i].Parent == spOb)
                    {
                        children.Add(solarSystem[i]);
                    }
                }
                if (children.Count != 0)
                {
                    for (int j = 0; j < children.Count; j++)
                    {
                        Ellipse child = new Ellipse();
                        int ind = solarSystem.IndexOf(children[j]);
                        Ellipse temp = (Ellipse)spaceFrame.Children[ind];
                        child.Fill = temp.Fill;
                        child.Height = objectFrame.Height * 0.06;
                        child.Width = child.Height;
                        child.Name = temp.Name + ind;
                        objectFrame.Children.Add(child);
                    }
                }
            }
        }

        
        private List<SpaceObject> InitSolarSystem()
        {
            List<SpaceObject> solarSystem = new List<SpaceObject>();

            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"SpaceObjects.txt");
            string[] lines = System.IO.File.ReadAllLines(path);

            string[][] jaggedArray = lines.Select(line => line.Split(' ').ToArray()).ToArray();

            foreach (string[] line in jaggedArray)
            {
                string obj = line[0];
                string name = line[1];
                double orbRad = ParseDouble(line[2]);
                int orbPeriod = Convert.ToInt32(line[3]);
                int objRad = Convert.ToInt32(line[4]);
                double rotPeriod = ParseDouble(line[5]);
                string color = line[6];

                switch (obj)
                {
                    case "Star": // stjerner får ikke parent
                        solarSystem.Add(new Star(name, orbRad, orbPeriod, objRad, rotPeriod, color));
                        break;
                    case "Moon": // måne har et ekstra argument for parent
                        try
                        {
                            SpaceObject parent = solarSystem.First(x => x.Name.ToLower() == line[7].ToLower());
                            solarSystem.Add(new Moon(name, orbRad, orbPeriod, objRad, rotPeriod, color, parent));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Feil oppstod med parenting av måne. Har månen et gyldig 8ende argument?");
                        }
                        break;
                    default:    // defaulter til planet med sun som parent
                        solarSystem.Add(new Planet(name, orbRad, orbPeriod, objRad, rotPeriod, color, solarSystem.First(x => x.Name.ToLower() == "sun")));
                        break;
                }
            }

            return solarSystem;
        }

        //hjelpefunksjon for logaritmisk skalering. Benyttes for skalering av rom-elementene (log udefinert for 0.0. log1 = 0)
        private Func<double, double, double, double> Scale = (value, maxInputValue, maxOutputValue) =>
            (value <= 1.0) ? 0.0 : Math.Log(value) * maxOutputValue / Math.Log(maxInputValue);

        //hjelpefunksjon for parsing av strenger til double uavhengig av regionale settings (punktum og komma)
        private Func<string, double> ParseDouble = s =>
        {
            double.TryParse(s.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double value);
            return value;
        };

    }

}

