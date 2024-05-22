using System.Collections.Generic;

namespace CityData
{
    public class Year
    {
        public int year;
        private readonly List<string> building_classes_names = new();
        private readonly List<Building_Class> building_classes = new();

        public Year(int year)
        {
            this.year = year;
        }

        public Year(string year)
        {
            this.year = int.Parse(year);
        }

        public void AddBuildingClass(Building_Class building_class)
        {
            building_classes.Add(building_class);
        }

        public List<Building_Class> GetBuildingClasses()
        {
            return building_classes;
        }

        public List<string> GetBuildingClassesNames()
        {
            return building_classes_names;
        }

        public void AddBuildingClassName(string name)
        {
            building_classes_names.Add(name);
        }
    }
}
