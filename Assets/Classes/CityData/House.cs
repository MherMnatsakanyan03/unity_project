using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityData
{
    public class House
    {
        private List<double> temps_over_time = new List<double>();

        public int ID;
        private ulong area;

        private int type;

        private double site_eui;

        private int year_built;

        private int energy_star_rating;

        public House(int id, ulong area, double site_eui, int year_built,int energy_star_rating, List<double> temps_over_time)
        {
            ID = id;
            this.area = area;
            this.site_eui = site_eui;
            this.year_built = year_built;
            this.energy_star_rating = energy_star_rating;
            this.temps_over_time = temps_over_time;
        }

        public void AddTemp(double temp)
        {
            temps_over_time.Add(temp);
        }

        public ulong GetArea()
        {
            return area;
        }

        public int GetID()
        {
            return ID;
        }

        public double GetSiteEUI()
        {
            return site_eui;
        }

        public int GetYearBuilt()
        {
            return year_built;
        }

        public int GetEnergyStarRating()
        {
            return energy_star_rating;
        }

        public List<double> GetTempsOverTime()
        {
            return temps_over_time;
        }

        public void SetType(int type)
        {
            this.type = type;
        }
    }
}
