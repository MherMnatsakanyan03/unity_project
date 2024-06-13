using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace CityData
{
    public class Facility
    {
        public string facility_type;

        private List<House> houses = new List<House>();
        private double total_temp = 0;
        private double avg_temp = 0;
        private ulong total_area = 0;
        private int count = 1;

        private int count_small = 0, count_medium = 0, count_large = 0;
        private ulong upper_size_to_be_small = 0, upper_size_to_be_medium = 0, upper_size_to_be_large = 0;

        public Facility(string facility_type, double temp, ulong area)
        {
            this.facility_type = facility_type;
            total_temp += temp;
            avg_temp = total_temp;
            total_area = area;
        }

        public void AddHouse(House house)
        {
            houses.Add(house);
        }

        public List<House> GetHouses()
        {
            return houses;
        }

        public void IncrementCount()
        {
            count++;
        }

        public void IncrementPartCount(ulong area)
        {
            if (area < upper_size_to_be_small)
            {
                count_small++;
            }
            else if (area < upper_size_to_be_medium)
            {
                count_medium++;
            }
            else
            {
                count_large++;
            }
        }

        public void AddTemp(double temp)
        {
            total_temp += temp;
            avg_temp = Math.Round(total_temp / count, 2);
        }

        public void AddArea(ulong area)
        {
            total_area += area;
        }

        public void SetSizeThresholds()
        {
            var average_area = total_area / (ulong)count;
            upper_size_to_be_small = (ulong)(average_area * 0.5);
            upper_size_to_be_medium = (ulong)(average_area * 3);
            upper_size_to_be_large = (ulong)(average_area * 6);
        }

        public double GetAvgTemp()
        {
            return avg_temp;
        }

        public ulong GetTotalArea()
        {
            return total_area;
        }

        public int GetCount()
        {
            return count;
        }

        public int GetCountPart(int part)
        {
            switch (part)
            {
                case 1:
                    return count_small;
                case 2:
                    return count_medium;
                case 3:
                    return count_large;
                default:
                    return 0;
            }
        }

        public ulong GetThreshoulds(int type)
        {
            switch (type)
            {
                case 0:
                    return upper_size_to_be_small;
                case 1:
                    return upper_size_to_be_medium;
                case 2:
                    return upper_size_to_be_large;
                default:
                    return 0;
            }
        }
    }
}
