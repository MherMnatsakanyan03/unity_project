using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using CityData;
using Unity.VisualScripting;
using UnityEngine;

namespace CityRender
{
    public class City : MonoBehaviour
    {

        public float width = 0f;
        public float height = 0f;
        public Vector3 position = Vector3.zero;

        public float camera_distance = 0;
        public CityObj city_data;
        private Building_Class building_class;

        [SerializeField]
        private GameObject district;

        private List<GameObject> copiedObject = new List<GameObject>();

        public double maxEUI;
        

        public void create_city(Dictionary<string, string> color_map)
        {
            building_class = city_data.GetBuildingClasses()[0];

            maxEUI = city_data.GetMaxEUI();

            List<string> district_names = building_class.GetFacilitiesNames();
            System.Random random = new System.Random();

            for (int i = 0; i < district_names.Count; i++)
            {
                GameObject new_district = Instantiate(district);
                District script = new_district.GetComponent<District>();
                script.facility_data = building_class.GetFacilities()[i];
                script.district_type = district_names[i];
                script.maxEUI = maxEUI;
                new_district.transform.SetParent(transform);
                copiedObject.Add(new_district);
            }
            arange_city(color_map);
        }

        private void arange_city(Dictionary<string, string> color_map)
        {
            float new_postion_x = 0;
            float shift_x = 0;
            float sum_height = 0;
            float offset = 0;
            float postion_y = 0;

            int i = 0;
            foreach (GameObject district in copiedObject)
            {
                District script = district.GetComponent<District>();

                script.create_district(color_map);
                district.GetComponent<BoxCollider>().size = new Vector3(
                    script.height,
                    1,
                    script.width
                );
                district.GetComponent<BoxCollider>().center = new Vector3(
                    0,
                    1,
                    script.width / 2 - script.street_width / 2
                );

                var width = -script.width/2 + district.GetComponent<BoxCollider>().center.z;
                var height = -script.height / 2;
                if (script.width > this.height) { this.height = script.width; }
                if (script.width/2 - script.street_width / 2 > postion_y) { postion_y = script.width/2 -script.street_width / 2; }

                new_postion_x = sum_height;
                shift_x = new_postion_x - height;
                if (i == 0) { shift_x = 0; offset = -script.height/2; }
                this.width += script.height;
                district.transform.position = new Vector3(0 + shift_x, 0, 0);
                script.position = district.transform.position + new Vector3(0, 0, district.GetComponent<BoxCollider>().center.z);
                

                if (i == 0)
                {
                    sum_height += script.height / 2;
                }
                else
                {
                    sum_height += script.height;
                }

                i++;
            }
            position = new Vector3 (width/2+offset, height/2, postion_y);
        }

        void Update()
        {
            //arange_city();
        }
    }
}
