using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SpaceSim;
using System.Collections;

namespace Ovelse3
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"SpaceObjects.txt");
            string[] lines = System.IO.File.ReadAllLines(path);

            string[][] jaggedArray = lines.Select(line => line.Split(' ').ToArray()).ToArray();

            List<SpaceObject> solarSystem = new List<SpaceObject>();

  

            foreach (string [] line in jaggedArray)
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
            Console.Write("Time: ");
            int time = Convert.ToInt32(Console.ReadLine());
            Console.Write("Planet: ");
            string planet = Console.ReadLine();

            foreach (SpaceObject obj in solarSystem)
            {
                if (planet == "" && obj.Name.Equals("Sun") ||
                    obj.Parent.Name.Equals("Sun"))
                {
                    obj.Draw();
                    Console.WriteLine(obj.CalculatPos(time) + "\n");
                }
                else if (obj.Name.ToLower().Equals(planet.ToLower()) ||
                    (obj.Parent != null && obj.Parent.Name.ToLower().Equals(planet.ToLower())))
                {
                    obj.Draw();
                    Console.WriteLine(obj.CalculatPos(time) + "\n");
                }
            }
            Console.ReadLine();
        }
    }
}
