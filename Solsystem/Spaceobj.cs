using System;




namespace SpaceSim
{
    public class SpaceObject
    {
        public string Name { get; set; }
        public double orbitalRadius { get; }
        protected int orbitalPeriod;
        public int objectRadius { get; }
        protected double rotationalPeriod;
        public string objectColor { get; }

        public SpaceObject Parent { get; set; }
        

        public SpaceObject(string name,double orbitalRadius,int orbitalPeriod,int objectRadius, double rotationalPeriod,string objectColor) 
        {
            this.Name = name;
            this.orbitalRadius = orbitalRadius;
            this.orbitalPeriod = orbitalPeriod;
            this.objectRadius = objectRadius;
            this.rotationalPeriod = rotationalPeriod;
            this.objectColor = objectColor;
        }

        public virtual void Draw()
        {
            Console.WriteLine(Name);
            Console.WriteLine("Orbital Radius: " + orbitalRadius);
            Console.WriteLine("Orbital Period: " + orbitalPeriod);
            Console.WriteLine("Object Radius: " + objectRadius);
            Console.WriteLine("Rotational period: " + rotationalPeriod);
            Console.WriteLine("Object color: " + objectColor);
            if (Parent != null)
                Console.WriteLine("Orbit around: " + Parent.Name);
            else
                Console.WriteLine("Orbit around: nothing");
          
        }

        public Tuple<double,double> CalculatPos(double time)
        {
            double x;
            double y;
            if (orbitalRadius != 0)
            {
                double rad = 2*Math.PI * (time/orbitalPeriod);

                x = orbitalRadius * Math.Cos(rad);
                y = orbitalRadius * Math.Sin(rad);

                if(Parent != null)
                {
                    Tuple<double, double> par = Parent.CalculatPos(time);
                    x += par.Item1;
                    y += par.Item2;
                }
            }
            else
            {
                x = 0;
                y = 0;
            }

            return Tuple.Create(x,y);
        }
    }

    public class Star : SpaceObject
    {
        public Star(string name, double orbitalRadius, int orbitalPeriod, int objectRadius, double rotationalPeriod, string objectColor)
            : base(name,orbitalRadius,orbitalPeriod,objectRadius,rotationalPeriod,objectColor) { }

        public override void Draw()
        {
            Console.Write("Star: ");
            base.Draw();
        }

    }

    public class Planet : SpaceObject
    {
        public Planet(string name, double orbitalRadius, int orbitalPeriod, int objectRadius, double rotationalPeriod, string objectColor)
            : base(name, orbitalRadius, orbitalPeriod, objectRadius, rotationalPeriod, objectColor) { }

        public override void Draw()
        {
            System.Console.Write("Planet: ");
            base.Draw();
        }
    }

    public class Moon : SpaceObject
    {
        public Moon(string name, double orbitalRadius, int orbitalPeriod, int objectRadius, double rotationalPeriod, string objectColor)
            : base(name, orbitalRadius, orbitalPeriod, objectRadius, rotationalPeriod, objectColor) { }
        public override void Draw()
        {
            Console.Write("Moon : ");
            base.Draw();
        }
    }

    public class Comet : SpaceObject
    {
        public Comet(string name, double orbitalRadius, int orbitalPeriod, int objectRadius, double rotationalPeriod, string objectColor)
            : base(name, orbitalRadius, orbitalPeriod, objectRadius, rotationalPeriod, objectColor) { }

        public override void Draw()
        {
            Console.Write("Comet: ");
            base.Draw();
        }
    }

    public class Astroid : SpaceObject
    {
        public Astroid(string name, double orbitalRadius, int orbitalPeriod, int objectRadius, double rotationalPeriod, string objectColor)
            : base(name, orbitalRadius, orbitalPeriod, objectRadius, rotationalPeriod, objectColor) { }

        public override void Draw()
        {
            Console.Write("Astroid: ");
            base.Draw();
        }
    }

    public class AsteroidBelt : SpaceObject
    {
        public AsteroidBelt(string name, double orbitalRadius, int orbitalPeriod, int objectRadius, double rotationalPeriod, string objectColor)
            : base(name, orbitalRadius, orbitalPeriod, objectRadius, rotationalPeriod, objectColor) { }

        public override void Draw()
        {
            Console.Write("Asteroid belt: ");
            base.Draw();
        }
    }

    public class DwarfPlanet : SpaceObject
    {
        public DwarfPlanet(string name, double orbitalRadius, int orbitalPeriod, int objectRadius, double rotationalPeriod, string objectColor)
            : base(name, orbitalRadius, orbitalPeriod, objectRadius, rotationalPeriod, objectColor) { }

        public override void Draw()
        {
            Console.Write("Dwarf planet: ");
            base.Draw();
        }

    }


}
