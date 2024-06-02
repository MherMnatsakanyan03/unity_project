using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CityData
{
    public class CSVReader : MonoBehaviour
    {
        private readonly string path = "Data/data.csv";
        private List<string> headers = new();
        private readonly List<string> passed_years = new();

        // Indexes of the columns in the CSV file
        private int csv_index_year;
        private int csv_index_city;
        private int csv_index_class;
        private int csv_index_facility;
        private int csv_index_avg_temp;

        private readonly List<Year> years = new();

        public CSVReader()
        {
            ReadCSV();
        }

        public void Start()
        {
            PrintData();
        }

        public List<Year> GetData()
        {
            return years;
        }

        public void PrintData()
        {
            string log = "";
            foreach (var year in years)
            {
                log += "Year: " + year.year + "\n";
                var cities = year.GetCities();
                foreach (var city in cities)
                {
                    log += "\tCity: " + city.city_id + "\n";
                    var building_classes = city.GetBuildingClasses();
                    foreach (var building_class in building_classes)
                    {
                        log += "\t\t" + building_class.class_name + "\n";
                        foreach (var facility in building_class.GetFacilities())
                        {
                            log +=
                                "\t\t\t"
                                + facility.facility_type
                                + (
                                    facility.facility_type.Length > 6
                                        ? ",\t\t avg Temp: "
                                        : ",\t\t\t avg Temp: "
                                )
                                + facility.GetAvgTemp().ToString()
                                + (
                                    facility.GetAvgTemp().ToString().Length > 3
                                        ? ",\t count: "
                                        : ",\t\t count: "
                                )
                                + facility.GetCount().ToString()
                                + "\n";
                        }
                    }
                }
            }
            UnityEngine.Debug.Log(log);
        }

        private void ReadCSV()
        {
            StreamReader reader = new(path);
            string line = reader.ReadLine();
            headers = line.Split(',').OfType<string>().ToList();

            csv_index_year = headers.IndexOf("Year_Factor");
            csv_index_city = headers.IndexOf("State_Factor");
            csv_index_class = headers.IndexOf("building_class");
            csv_index_facility = headers.IndexOf("facility_type");
            csv_index_avg_temp = headers.IndexOf("avg_temp");

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] values = line.Split(',');

                var year = values[csv_index_year];
                var city_id = int.Parse(values[csv_index_city].Split('_').Last()); // Get the last character of the string
                var building_class = values[csv_index_class];
                var facility_type = values[csv_index_facility];
                var avg_temp = values[csv_index_avg_temp];

                int index_passed_year = passed_years.IndexOf(year);

                // If the year has been checked before
                if (index_passed_year != -1)
                {
                    var current_year_obj = years[index_passed_year];
                    var passed_cities = current_year_obj.GetCityIds();
                    var index_passed_city = passed_cities.IndexOf(city_id);
                    // If the city has been checked before
                    if (index_passed_city != -1)
                    {
                        var current_city_obj = current_year_obj.GetCities()[index_passed_city];
                        var passed_classes = current_city_obj.GetBuildingClassesNames();
                        var index_passed_class = passed_classes.IndexOf(building_class);
                        // If the building class has been checked before
                        if (index_passed_class != -1)
                        {
                            var current_class_obj = current_city_obj.GetBuildingClasses()[
                                index_passed_class
                            ];
                            var passed_facilities = current_class_obj.GetFacilitiesNames();
                            var index_passed_facility = passed_facilities.IndexOf(facility_type);

                            // If the facility has been checked before
                            if (index_passed_facility != -1)
                            {
                                var current_facility_obj = current_class_obj.GetFacilities()[
                                    index_passed_facility
                                ];
                                current_facility_obj.IncrementCount();
                                current_facility_obj.AddTemp(ParseTemp(avg_temp));
                            }
                            // Else, add the facility to the existing building class
                            else
                            {
                                var facility_obj = new Facility(facility_type, ParseTemp(avg_temp));
                                current_class_obj.AddFacilityName(facility_type);
                                current_class_obj.AddFacility(facility_obj);
                            }
                        }
                        // Else, add the building class and facility to the existing year
                        else
                        {
                            var building_class_obj = new Building_Class(building_class);
                            var facility_obj = new Facility(facility_type, ParseTemp(avg_temp));

                            building_class_obj.AddFacilityName(facility_type);
                            building_class_obj.AddFacility(facility_obj);
                            current_city_obj.AddBuildingClassName(building_class);
                            current_city_obj.AddBuildingClass(building_class_obj);
                        }
                    }
                    else
                    {
                        var city_obj = new CityObj(city_id);
                        var building_class_obj = new Building_Class(building_class);
                        var facility_obj = new Facility(facility_type, ParseTemp(avg_temp));

                        building_class_obj.AddFacilityName(facility_type);
                        building_class_obj.AddFacility(facility_obj);
                        city_obj.AddBuildingClassName(building_class);
                        city_obj.AddBuildingClass(building_class_obj);
                        current_year_obj.AddCity(city_obj);
                        current_year_obj.AddCityId(city_id);
                    }
                }
                // Else, add the year to the array and add the building class and facility
                // (if no year, then no calss and facility)
                else
                {
                    passed_years.Add(year.ToString());
                    var year_obj = new Year(year);
                    var city_obj = new CityObj(city_id);
                    var building_class_obj = new Building_Class(building_class);
                    var facility_obj = new Facility(facility_type, ParseTemp(avg_temp));

                    building_class_obj.AddFacilityName(facility_type);
                    building_class_obj.AddFacility(facility_obj);
                    city_obj.AddBuildingClassName(building_class);
                    city_obj.AddBuildingClass(building_class_obj);
                    year_obj.AddCity(city_obj);
                    year_obj.AddCityId(city_id);
                    years.Add(year_obj);
                }
            }

            reader.Close();
        }

        private double ParseTemp(string temp)
        {
            return double.Parse(temp.Split('.')[0]);
        }
    }
}
