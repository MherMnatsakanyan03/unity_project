using System.Collections.Generic;

namespace CityData
{
    public class CityObj
    {
        public int city_id;
        private ulong area = 0;
        private int count_buildings = 0;

        private double max_eui = 0;

        private readonly List<string> building_classes_names = new();
        private readonly List<Building_Class> building_classes = new();

        public CityObj(int id, ulong area)
        {
            city_id = id;
            this.area = area;
            count_buildings++;
        }

        public CityObj(string id, ulong area)
        {
            city_id = int.Parse(id);
            this.area = area;
            count_buildings++;
        }

        public void AddBuildingClass(Building_Class building_class)
        {
            building_classes.Add(building_class);
        }

        public void AddBuildingClassName(string name)
        {
            building_classes_names.Add(name);
        }

        public void AddArea(ulong area)
        {
            this.area += area;
        }

        public void IncrementBuildingCount()
        {
            count_buildings++;
        }

        public List<Building_Class> GetBuildingClasses()
        {
            return building_classes;
        }

        public List<string> GetBuildingClassesNames()
        {
            return building_classes_names;
        }

        public ulong GetArea()
        {
            return area;
        }

        public int GetBuildingCount()
        {
            return count_buildings;
        }

        public void SetMaxEUI(double eui)
        {
            if (eui > max_eui)
            {
                max_eui = eui;
            }
        }

        public double GetMaxEUI()
        {
            return max_eui;
        }
    }
}
