using System.Collections.Generic;

namespace City
{
    public class Building_Class
    {
        public string class_name;
        private readonly List<string> facilities_names = new();
        private readonly List<Facility> facilities = new();

        public Building_Class(string class_name)
        {
            this.class_name = class_name;
        }

        public void AddFacility(Facility facility)
        {
            facilities.Add(facility);
        }

        public List<Facility> GetFacilities()
        {
            return facilities;
        }

        public List<string> GetFacilitiesNames()
        {
            return facilities_names;
        }

        public void AddFacilityName(string name)
        {
            facilities_names.Add(name);
        }
    }
}
