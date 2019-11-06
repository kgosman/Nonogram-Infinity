using System;

namespace _5TSPGA
{
    public class City
    {
        public string id;
        public double xCoord;
        public double yCoord;
        public double distance;
        public City()
        {
            this.id = "0";
            this.xCoord = 0.0;
            this.yCoord = 0.0;
            this.distance = 0.0;
        }
        //Point to point distance formula
        public void DistanceToCity(City targetCity)
        {
            this.distance = Math.Sqrt(Math.Pow((targetCity.xCoord - this.xCoord), 2) + Math.Pow((targetCity.yCoord - this.yCoord), 2));
        }
        public void Clone(City city)
        {
            this.id = city.id;
            this.xCoord = city.xCoord;
            this.yCoord = city.yCoord;
        }
    }
}
