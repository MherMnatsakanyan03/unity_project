using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CityRender
{
    public class House : MonoBehaviour
    {
        public Vector3 position;
        public GameObject house;
        public GameObject house_copy;
        private Gradient gradient;

        public float eui = 0f;
        public int year_build = 2000;
        public int energy_star = 50;
        public int colling_degree_days = 50;
        public int warming_degree_days = 50;
        public float relativ_eui = 5000;
        public int relativ_year_build = 2000;

        public string house_type;

        public House(Vector3 position, GameObject copy, CityData.House house)
        {
            this.gradient = new Gradient();
            var colors = new GradientColorKey[2];
            colors[0] = new GradientColorKey(Color.red, 1.0f);
            colors[1] = new GradientColorKey(Color.blue, 0.0f);

            // Blend alpha from opaque at 0% to transparent at 100%
            var alphas = new GradientAlphaKey[2];
            alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphas[1] = new GradientAlphaKey(0.0f, 1.0f);
            this.gradient.SetKeys(colors, alphas);

            this.position = position;

            this.house_copy = copy;

            this.eui = (float)house.GetSiteEUI();
            this.year_build = house.GetYearBuilt();
            this.energy_star = house.GetEnergyStarRating();
            //this.colling_degree_days = house.;
            //this.warming_degree_days = warming_degree_days;

            EventListener.current.enableBoxColliderHouse += enableBoxCollider;
            EventListener.current.disableBoxColliderHouse += disableBoxCollider;

            EventListener.current.show_energy_star += show_energy_star;
            EventListener.current.show_year_build += show_year_build;
            EventListener.current.show_eui += show_eui;
        }

        private void enableBoxCollider()
        {
            //gameObject.GetComponent<BoxCollider>().enabled = true;
            this.house.GetComponent<BoxCollider>().enabled = true;
        }

        private void disableBoxCollider()
        {
            //gameObject.GetComponent<BoxCollider>().enabled = false;
            this.house.GetComponent<BoxCollider>().enabled = false;
        }

        public GameObject create_house()
        {
            this.house = Instantiate(this.house_copy, this.position, Quaternion.identity);
            this.house.AddComponent<CityRender.house_data>();
            house_data script = this.house.GetComponent<CityRender.house_data>();
            script.eui = this.eui;
            script.year_build = this.year_build;
            script.energy_star = this.energy_star;
            script.colling_degree_days = this.colling_degree_days;
            script.warming_degree_days = this.warming_degree_days;
            script.relativ_eui = this.relativ_eui;
            script.relativ_year_build = this.relativ_year_build;

            return this.house;
        }

        public void show_energy_star()
        {
            Debug.Log("clicked 2");
            this.house.GetComponent<Renderer>()
                .material.SetColor("_Color", this.gradient.Evaluate(this.energy_star / 100f));
        }

        public void show_year_build()
        {
            this.house.GetComponent<Renderer>()
                .material.SetColor(
                    "_Color",
                    this.gradient.Evaluate(this.year_build / (float)this.relativ_year_build)
                );
        }

        public void show_eui()
        {
            this.house.GetComponent<Renderer>()
                .material.SetColor(
                    "_Color",
                    this.gradient.Evaluate(this.eui / (float)this.relativ_year_build)
                );
        }
    }
}
