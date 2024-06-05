using System.Collections.Generic;

namespace CityData
{
    public class Year
    {
        public int year;
        private readonly List<int> city_ids = new();
        private readonly List<CityObj> cities = new();

        public Year(int year)
        {
            this.year = year;
        }

        public Year(string year)
        {
            this.year = int.Parse(year);
        }

        public void AddCity(CityObj city)
        {
            cities.Add(city);
        }

        public void AddCityId(int id)
        {
            city_ids.Add(id);
        }

        public void PrintYear()
        {
            UnityEngine.Debug.Log("Current Year: " + year);
        }

        public List<CityObj> GetCities()
        {
            return cities;
        }

        public List<int> GetCityIds()
        {
            return city_ids;
        }
    }
}
