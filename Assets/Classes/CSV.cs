using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CSV : MonoBehaviour
{
    private void Start()
    {
        CSVReader reader = new();
        reader.PrintData();
    }

    public class Facility
    {
        public string facility_type;
        private double total_temp = 0;
        private double avg_temp = 0;
        private int count = 1;

        public Facility(string facility_type, double temp)
        {
            this.facility_type = facility_type;
            total_temp += temp;
        }

        public void IncrementCount()
        {
            count++;
        }

        public void AddTemp(double temp)
        {
            total_temp += temp;
            avg_temp = Math.Round(total_temp / count, 2);
        }

        public double GetAvgTemp()
        {
            return avg_temp;
        }

        public int GetCount()
        {
            return count;
        }
    }

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

    public class CSVReader
    {
        private readonly string path = "Data/data.csv";
        private List<string> headers = new();
        private readonly List<string> passed_years = new();

        // Indexes of the columns in the CSV file
        private int csv_index_year;
        private int csv_index_class;
        private int csv_index_facility;
        private int csv_index_avg_temp;

        private readonly List<Year> years = new();

        public CSVReader()
        {
            ReadCSV();
        }

        public void PrintData()
        {
            string log = "";
            foreach (var year in years)
            {
                log += "Year: " + year.year + "\n";
                foreach (var building_class in year.GetBuildingClasses())
                {
                    log += "\t" + building_class.class_name + "\n";
                    foreach (var facility in building_class.GetFacilities())
                    {
                        log +=
                            "\t\t"
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

            Debug.Log(log);
        }

        private void ReadCSV()
        {
            StreamReader reader = new(path);
            string line = reader.ReadLine();
            Debug.Log(line);
            headers = line.Split(',').OfType<string>().ToList();

            csv_index_year = headers.IndexOf("Year_Factor");
            csv_index_class = headers.IndexOf("building_class");
            csv_index_facility = headers.IndexOf("facility_type");
            csv_index_avg_temp = headers.IndexOf("avg_temp");

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] values = line.Split(',');

                var year = values[csv_index_year];
                var building_class = values[csv_index_class];
                var facility_type = values[csv_index_facility];
                var avg_temp = values[csv_index_avg_temp];

                int index_passed_year = passed_years.IndexOf(year);

                // If the year has been checked before
                if (index_passed_year != -1)
                {
                    var current_year_obj = years[index_passed_year];
                    var passed_classes = current_year_obj.GetBuildingClassesNames();
                    var index_passed_class = passed_classes.IndexOf(building_class);

                    // If the building class has been checked before
                    if (index_passed_class != -1)
                    {
                        var current_class_obj = current_year_obj.GetBuildingClasses()[
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
                        current_year_obj.AddBuildingClassName(building_class);
                        current_year_obj.AddBuildingClass(building_class_obj);
                    }
                }
                // Else, add the year to the array and add the building class and facility
                // (if no year, then no calss and facility)
                else
                {
                    passed_years.Add(year.ToString());
                    var year_obj = new Year(year);
                    var building_class_obj = new Building_Class(building_class);
                    var facility_obj = new Facility(facility_type, ParseTemp(avg_temp));

                    building_class_obj.AddFacilityName(facility_type);
                    building_class_obj.AddFacility(facility_obj);
                    year_obj.AddBuildingClassName(building_class);
                    year_obj.AddBuildingClass(building_class_obj);

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
