using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace City
{
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
}
