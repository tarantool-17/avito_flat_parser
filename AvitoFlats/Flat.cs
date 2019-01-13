using System;
using System.Globalization;
using System.Linq;

namespace AvitoFlats
{
    public class Flat
    {
        public string Header;
        public string Price;
        public string Commission;
        public string Address;
        public string Owner;
        public string Url;

        private readonly int _defaultDistance = 10000;

        public double MetroDistance
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MetroDistanceStr))
                    return _defaultDistance;

                var number = MetroDistanceStr.Split(" ")
                    .First(x => !string.IsNullOrWhiteSpace(x));

                if(!Double.TryParse(number, NumberStyles.AllowDecimalPoint,  CultureInfo.InvariantCulture, out var distance))
                    return _defaultDistance;

                if (MetroDistanceStr.Contains("км"))
                    distance *= 1000;

                return distance;
            }
        }
        
        public string MetroDistanceStr;
        
        public override string ToString()
        {
            return $"{Price} {Header.Clean()} \n {Address.Clean()} \n {Owner.Clean()} {Commission.Clean()} \n https://www.avito.ru{Url}";
        }

        public string ToHtml()
        {
            return $"<span>{Address}</span> <br /> <span class=\"price\">{Price}</span> <br /> <span class=\"owner\">{Owner} {Commission}</span> <br /> <span>{Header}</span> <br /> <span>https://www.avito.ru{Url}</span> <br />";
        }
    }
}