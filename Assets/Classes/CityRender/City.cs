using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using CityData;
using UnityEngine;

namespace CityRender
{
    public class City : MonoBehaviour
    {

        public float size_x = 0f;
        public float size_minus_x = 0f;
        public float size_y = 0f;
        public float size_minus_y = 0f;

        public float buffer_size_x = 0f;
        public float buffer_size_minus_x = 0f;
        public float buffer_size_y = 0f;
        public float buffer_size_minus_y = 0f;

        public float camera_distance = 0;
        public CityObj city_data;
        private Building_Class building_class;

        [SerializeField]
        private GameObject district;

        private List<GameObject> copiedObject = new List<GameObject>();

        public void create_city()
        {
            building_class = city_data.GetBuildingClasses()[0];
            List<string> district_names = building_class.GetFacilitiesNames();
            System.Random random = new System.Random();

            for (int i = 0; i < district_names.Count; i++)
            {
                GameObject new_district = Instantiate(district);
                District script = new_district.GetComponent<District>();
                script.facility_data = building_class.GetFacilities()[i];
                script.district_type = district_names[i];
                new_district.transform.SetParent(transform);
                copiedObject.Add(new_district);
            }
            arange_city();
        }

        private void arange_city()
        {
            float new_postion_x = 0;
            float new_postion_y = 0;
            float shift_x = 0;
            float shift_y = 0;
            float rotate_x = 0;
            float rotate_y = 0;
            float rotation = 0;
            District parten_district_script = null;
            List<District> parent_district_scripts = new List<District>();
            int i = 0;
            foreach (GameObject district in copiedObject)
            {
                District script = district.GetComponent<District>();

                script.districtSquareMeterSize = 50 + i * 10;
                script.create_district();
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

                var width = 0 - script.street_width / 2;
                var height = -script.height / 2;

                if (i == 0)
                {
                    parten_district_script = script;
                }

                switch (i)
                {
                    case 0:
                        script.position = district.transform.position + new Vector3(0,0,district.GetComponent<BoxCollider>().center.z);
                        break;
                    case 1:
                        rotation = 0;
                        new_postion_x = -parten_district_script.width + script.street_width / 2;
                        new_postion_y = parten_district_script.height / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.RotateAround(
                            new Vector3(rotate_x, 0, rotate_y),
                            Vector3.up,
                            rotation
                        );
                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        parent_district_scripts.Add(script);
                        buffer_size_x += script.width;
                        if (script.height > buffer_size_y) { size_y = script.height; }
                        script.position = district.transform.position + new Vector3(0, 0, district.GetComponent<BoxCollider>().center.z);
                        break;
                    case 2:
                        rotation = 90;
                        new_postion_x = parten_district_script.width - script.street_width / 2;
                        new_postion_y = parten_district_script.height / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(
                            new Vector3(new_postion_x, 0, new_postion_y),
                            Vector3.up,
                            rotation
                        );
                        parent_district_scripts.Add(script);
                        buffer_size_y += script.width;
                        if (script.height > buffer_size_minus_y) { size_minus_y = script.height; }
                        script.position = district.transform.position + new Vector3(district.GetComponent<BoxCollider>().center.z, 0, 0);
                        break;
                    case 3:
                        rotation = 180;
                        new_postion_x = parten_district_script.width - script.street_width / 2;
                        new_postion_y = 0 - script.street_width / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(
                            new Vector3(new_postion_x, 0, new_postion_y),
                            Vector3.up,
                            rotation
                        );
                        parent_district_scripts.Add(script);
                        camera_distance += script.width;
                        buffer_size_minus_x += script.width;
                        if (script.height > buffer_size_minus_y) { size_minus_y = script.height; }
                        script.position = district.transform.position + new Vector3(0, 0, -district.GetComponent<BoxCollider>().center.z);
                        break;
                    case 4:
                        rotation = 270;
                        new_postion_x = -parten_district_script.width + script.street_width / 2;
                        new_postion_y = 0 - script.street_width / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(
                            new Vector3(new_postion_x, 0, new_postion_y),
                            Vector3.up,
                            rotation
                        );
                        parent_district_scripts.Add(script);
                        buffer_size_minus_y += script.width;
                        if (script.height > buffer_size_x) { size_x = script.height; }
                        script.position = district.transform.position + new Vector3(-district.GetComponent<BoxCollider>().center.z, 0, 0);
                        break;
                    case 5:
                        rotation = 0;
                        new_postion_x = -parten_district_script.width + script.street_width / 2;
                        new_postion_y =
                            parten_district_script.height / 2 + parent_district_scripts[0].width;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.RotateAround(
                            new Vector3(rotate_x, 0, rotate_y),
                            Vector3.up,
                            rotation
                        );
                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        buffer_size_x += script.width;
                        if (script.height > buffer_size_y) { size_y = script.height; }
                        script.position = district.transform.position + new Vector3(0, 0, district.GetComponent<BoxCollider>().center.z);
                        break;
                    case 6:
                        rotation = 90;
                        new_postion_x =
                            parten_district_script.width
                            - script.street_width / 2
                            + parent_district_scripts[1].width;
                        new_postion_y = parten_district_script.height / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(
                            new Vector3(new_postion_x, 0, new_postion_y),
                            Vector3.up,
                            rotation
                        );
                        buffer_size_y += script.width;
                        if (script.height > buffer_size_minus_y) { size_minus_y = script.height; }
                        script.position = district.transform.position + new Vector3(district.GetComponent<BoxCollider>().center.z, 0, 0);
                        break;
                    case 7:
                        rotation = 180;
                        new_postion_x = parten_district_script.width - script.street_width / 2;
                        new_postion_y =
                            0 - script.street_width / 2 - parent_district_scripts[2].width;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(
                            new Vector3(new_postion_x, 0, new_postion_y),
                            Vector3.up,
                            rotation
                        );
                        camera_distance += script.width;
                        buffer_size_minus_x += script.width;
                        if (script.height > buffer_size_minus_y) { size_minus_y = script.height; }
                        script.position = district.transform.position + new Vector3(0, 0, -district.GetComponent<BoxCollider>().center.z);
                        break;
                    case 8:
                        rotation = 270;
                        new_postion_x =
                            -parten_district_script.width
                            + script.street_width / 2
                            - parent_district_scripts[3].width;
                        new_postion_y = 0 - script.street_width / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(
                            new Vector3(new_postion_x, 0, new_postion_y),
                            Vector3.up,
                            rotation
                        );
                        parent_district_scripts.Add(script);
                        buffer_size_minus_y += script.width;
                        if (script.height > buffer_size_x) { size_x = script.height; }
                        script.position = district.transform.position + new Vector3(-district.GetComponent<BoxCollider>().center.z, 0, 0);
                        Debug.Log("pos:" + script.position);
                        break;
                    default:
                        break;
                }

                i++;
            }
            if (size_x < buffer_size_x) { size_x = buffer_size_x; }
            if (size_minus_x < buffer_size_minus_x) { size_minus_x = buffer_size_minus_x; }
            if (size_y < buffer_size_y) { size_y = buffer_size_y; }
            if (size_minus_y < buffer_size_minus_y) { size_minus_y = buffer_size_minus_y; }
        }

        void Update()
        {
            //arange_city();
        }
    }
}
