using System;
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
        private int csv_index_year,
            csv_index_city,
            csv_index_class,
            csv_index_facility,
            csv_index_avg_temp,
            csv_index_area,
            csv_index_house_id,
            csv_index_site_eui,
            csv_index_year_built,
            csv_index_energy_star_rating;

        private List<int> csv_index_monthly_temp = new();

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

        /**
         * Print the data in the console
         */
        public void PrintData()
        {
            string log = "";
            foreach (var year in years)
            {
                log += "Year: " + year.year + "\n";
                var cities = year.GetCities();
                foreach (var city in cities)
                {
                    log +=
                        "\tCity: "
                        + city.city_id
                        + ", Area: "
                        + city.GetArea().ToString()
                        + ", Buildings: "
                        + city.GetBuildingCount().ToString()
                        + ", Max EUI: "
                        + city.GetMaxEUI().ToString()
                        + "\n";
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
                                + "\t Total Area:"
                                + facility.GetTotalArea().ToString()
                                + "\t Small: "
                                + facility.GetCountPart(1).ToString()
                                + "\t Medium: "
                                + facility.GetCountPart(2).ToString()
                                + "\t Large: "
                                + facility.GetCountPart(3).ToString()
                                + "\t Size Thresholds: "
                                + facility.GetThreshoulds(0).ToString()
                                + ", "
                                + facility.GetThreshoulds(1).ToString()
                                + "\n";

                            log +=
                                "\t\t\t\tHouse ID: "
                                + facility.GetHouses()[0].ID
                                + ", Area: "
                                + facility.GetHouses()[0].GetArea().ToString()
                                + ", Site EUI: "
                                + facility.GetHouses()[0].GetSiteEUI().ToString()
                                + ", Year Built: "
                                + facility.GetHouses()[0].GetYearBuilt().ToString()
                                + ", Energy Star Rating: "
                                + facility.GetHouses()[0].GetEnergyStarRating().ToString()
                                + ", Monthly Temps: "
                                + string.Join(", ", facility.GetHouses()[0].GetTempsOverTime())
                                + "\n";
                        }
                    }
                }
            }
            Debug.Log(log);
        }

        /* ==================================================== Functions =================================================== */

        /**
         * Read the CSV file and store the data in the years, cities, building classes, and facilities
         * Includes some pre-processing to calculate things
         */
        private void ReadCSV()
        {
            // measure the time taken to read the CSV file
            System.Diagnostics.Stopwatch stopwatch = new();
            stopwatch.Start();

            StreamReader reader = new(path);
            string line = reader.ReadLine();
            headers = line.Split(',').ToList();

            csv_index_year = headers.IndexOf("Year_Factor");
            csv_index_city = headers.IndexOf("State_Factor");
            csv_index_class = headers.IndexOf("building_class");
            csv_index_facility = headers.IndexOf("facility_type");
            csv_index_house_id = headers.IndexOf("id");

            csv_index_avg_temp = headers.IndexOf("avg_temp");
            csv_index_area = headers.IndexOf("floor_area");
            csv_index_site_eui = headers.IndexOf("site_eui");
            csv_index_year_built = headers.IndexOf("year_built");
            csv_index_energy_star_rating = headers.IndexOf("energy_star_rating");

            csv_index_monthly_temp.Add(headers.IndexOf("january_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("february_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("march_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("april_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("may_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("june_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("july_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("august_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("september_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("october_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("november_avg_temp"));
            csv_index_monthly_temp.Add(headers.IndexOf("december_avg_temp"));

            // Initialize the years, cities, building classes, and facilities
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] values = line.Split(',');

                var year = values[csv_index_year];
                var city_id = int.Parse(values[csv_index_city].Split('_').Last()); // Get the last character of the string
                var building_class = values[csv_index_class];
                var facility_type = values[csv_index_facility];
                var house_id = int.Parse(values[csv_index_house_id]);

                var avg_temp = ParseDouble(values[csv_index_avg_temp]);
                var area = ParseArea(values[csv_index_area]);
                var site_eui = ParseDouble(values[csv_index_site_eui]);

                bool rating_not_empty = int.TryParse(
                    values[csv_index_energy_star_rating],
                    out int energy_star_rating
                );
                if (!rating_not_empty)
                {
                    energy_star_rating = -1;
                }

                bool year_not_empty = int.TryParse(
                    values[csv_index_year_built],
                    out int year_built
                );
                if (!year_not_empty)
                {
                    year_built = -1;
                }

                List<double> monthly_temps = new();
                foreach (int index in csv_index_monthly_temp)
                {
                    monthly_temps.Add(ParseDouble(values[index]));
                }

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
                        current_city_obj.AddArea(area);
                        current_city_obj.SetMaxEUI(site_eui);
                        current_city_obj.IncrementBuildingCount();
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
                                var house = new House(
                                    house_id,
                                    area,
                                    site_eui,
                                    year_built,
                                    energy_star_rating,
                                    monthly_temps
                                );

                                current_facility_obj.AddHouse(house);
                                current_facility_obj.IncrementCount();
                                current_facility_obj.AddTemp(avg_temp);
                                current_facility_obj.AddArea(area);
                            }
                            // Else, add the facility to the existing building class
                            else
                            {
                                var facility_obj = new Facility(facility_type, avg_temp, area);
                                var house = new House(
                                    house_id,
                                    area,
                                    site_eui,
                                    year_built,
                                    energy_star_rating,
                                    monthly_temps
                                );

                                facility_obj.AddHouse(house);
                                current_class_obj.AddFacilityName(facility_type);
                                current_class_obj.AddFacility(facility_obj);
                            }
                        }
                        // Else, add the building class and facility to the existing year
                        else
                        {
                            var building_class_obj = new Building_Class(building_class);
                            var facility_obj = new Facility(facility_type, avg_temp, area);
                            var house = new House(
                                house_id,
                                area,
                                site_eui,
                                year_built,
                                energy_star_rating,
                                monthly_temps
                            );

                            facility_obj.AddHouse(house);
                            building_class_obj.AddFacilityName(facility_type);
                            building_class_obj.AddFacility(facility_obj);
                            current_city_obj.AddBuildingClassName(building_class);
                            current_city_obj.AddBuildingClass(building_class_obj);
                        }
                    }
                    else
                    {
                        var city_obj = new CityObj(city_id, area);
                        var building_class_obj = new Building_Class(building_class);
                        var facility_obj = new Facility(facility_type, avg_temp, area);
                        var house = new House(
                            house_id,
                            area,
                            site_eui,
                            year_built,
                            energy_star_rating,
                            monthly_temps
                        );

                        facility_obj.AddHouse(house);
                        building_class_obj.AddFacilityName(facility_type);
                        building_class_obj.AddFacility(facility_obj);
                        city_obj.AddBuildingClassName(building_class);
                        city_obj.AddBuildingClass(building_class_obj);
                        city_obj.SetMaxEUI(site_eui);
                        current_year_obj.AddCity(city_obj);
                        current_year_obj.AddCityId(city_id);
                    }
                }
                // Else, add the year to the array and add the building class and facility
                // (if no year, then no class and facility)
                else
                {
                    passed_years.Add(year.ToString());
                    var year_obj = new Year(year);
                    var city_obj = new CityObj(city_id, area);
                    var building_class_obj = new Building_Class(building_class);
                    var facility_obj = new Facility(facility_type, avg_temp, area);
                    var house = new House(
                        house_id,
                        area,
                        site_eui,
                        year_built,
                        energy_star_rating,
                        monthly_temps
                    );

                    facility_obj.AddHouse(house);
                    building_class_obj.AddFacilityName(facility_type);
                    building_class_obj.AddFacility(facility_obj);
                    city_obj.AddBuildingClassName(building_class);
                    city_obj.AddBuildingClass(building_class_obj);
                    city_obj.SetMaxEUI(site_eui);
                    year_obj.AddCity(city_obj);
                    year_obj.AddCityId(city_id);
                    years.Add(year_obj);
                }
            }
            reader.Close();

            // Set the size thresholds for each facility and more
            foreach (Year year in years)
            {
                foreach (CityObj city in year.GetCities())
                {
                    foreach (Building_Class building_class in city.GetBuildingClasses())
                    {
                        foreach (Facility facility in building_class.GetFacilities())
                        {
                            facility.SetSizeThresholds();

                            foreach (House house in facility.GetHouses())
                            {
                                for (int type = 0; type < 3; type++)
                                {
                                    double size = facility.GetThreshoulds(type);
                                    if (house.GetArea() < size)
                                    {
                                        house.SetType(type);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            reader = new(path);
            reader.ReadLine(); // Skip the headers

            //  re-run to count the number of small, medium, and large facilities
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] values = line.Split(',');

                var year = values[csv_index_year];
                var city_id = int.Parse(values[csv_index_city].Split('_').Last()); // Get the last character of the string
                var building_class = values[csv_index_class];
                var facility_type = values[csv_index_facility];
                var house_id = int.Parse(values[csv_index_house_id]);
                var area = ParseArea(values[csv_index_area]);

                int index_passed_year = passed_years.IndexOf(year);
                var current_year_obj = years[index_passed_year];

                var passed_cities = current_year_obj.GetCityIds();
                var index_passed_city = passed_cities.IndexOf(city_id);
                var current_city_obj = current_year_obj.GetCities()[index_passed_city];

                var passed_classes = current_city_obj.GetBuildingClassesNames();
                var index_passed_class = passed_classes.IndexOf(building_class);
                var current_class_obj = current_city_obj.GetBuildingClasses()[index_passed_class];

                var passed_facilities = current_class_obj.GetFacilitiesNames();
                var index_passed_facility = passed_facilities.IndexOf(facility_type);
                var current_facility_obj = current_class_obj.GetFacilities()[index_passed_facility];

                var passed_houses = current_facility_obj.GetHouses();
                var current_house_obj = passed_houses.Find(house => house.ID == house_id);

                current_facility_obj.IncrementPartCount(area);
            }

            reader.Close();

            stopwatch.Stop();
            Debug.Log("Time taken to read the CSV file: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        /**
         * Parse the doubles from the CSV file
         */
        private double ParseDouble(string @double)
        {
            string[] temp_split = @double.Split('.');
            string parts = temp_split[0] + (temp_split.Length > 1 ?  "," + temp_split[1] : "");
            return Math.Round(double.Parse(parts), 2);
        }

        /**
         * Parse the area values from the CSV file
         */
        private ulong ParseArea(string area)
        {
            area = area.Split('.')[0];
            return ulong.Parse(area);
        }
    }
}
