using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using CityData;

public class City : MonoBehaviour
{
    public CityData.CityObj city_data;
    private Building_Class building_class;

    [SerializeField]
    private GameObject district;

    private List<GameObject> copiedObject = new List<GameObject>();

    public void create_city()
    {
        building_class = city_data.GetBuildingClasses()[0];
        List<string> district_names = building_class.GetFacilitiesNames();
        System.Random random = new System.Random();

        for(int i=0;i< district_names.Count;i++)
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
        foreach(GameObject district in copiedObject)
        {
            
            District script = district.GetComponent<District>();
            
            script.districtSquareMeterSize = 50 + i * 10;
            script.create_district();
            district.GetComponent<BoxCollider>().size = new Vector3(script.height, 1, script.width);
            district.GetComponent<BoxCollider>().center = new Vector3(0, 1, script.width / 2 - script.street_width/2);

            var width = 0 - script.street_width / 2;
            var height = -script.height / 2;

            if (i == 0) { parten_district_script = script; }

                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        rotation = 0;
                        new_postion_x = -parten_district_script.width + script.street_width / 2;
                        new_postion_y = parten_district_script.height / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y  - width;

                        district.transform.RotateAround(new Vector3(rotate_x, 0, rotate_y), Vector3.up, rotation);
                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        parent_district_scripts.Add(script);
                        break;
                    case 2:
                        rotation = 90;
                        new_postion_x = parten_district_script.width - script.street_width / 2;
                        new_postion_y = parten_district_script.height / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(new Vector3(new_postion_x, 0, new_postion_y), Vector3.up, rotation);
                        parent_district_scripts.Add(script);
                        break;
                    case 3:
                        rotation = 180;
                        new_postion_x = parten_district_script.width - script.street_width / 2;
                        new_postion_y = 0 - script.street_width / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(new Vector3(new_postion_x, 0, new_postion_y), Vector3.up, rotation);
                        parent_district_scripts.Add(script);
                        break;
                    case 4:
                        rotation = 270;
                        new_postion_x = -parten_district_script.width + script.street_width / 2;
                        new_postion_y = 0 - script.street_width / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(new Vector3(new_postion_x, 0, new_postion_y), Vector3.up, rotation);
                        parent_district_scripts.Add(script);
                        break;
                    case 5:
                        rotation = 0;
                        new_postion_x = -parten_district_script.width + script.street_width / 2;
                        new_postion_y = parten_district_script.height / 2 + parent_district_scripts[0].width;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.RotateAround(new Vector3(rotate_x, 0, rotate_y), Vector3.up, rotation);
                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        break;
                    case 6:
                        rotation = 90;
                        new_postion_x = parten_district_script.width - script.street_width / 2 + parent_district_scripts[1].width;
                        new_postion_y = parten_district_script.height / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(new Vector3(new_postion_x, 0, new_postion_y), Vector3.up, rotation);
                        break;
                    case 7:
                        rotation = 180;
                        new_postion_x = parten_district_script.width - script.street_width / 2;
                        new_postion_y = 0 - script.street_width / 2 - parent_district_scripts[2].width;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(new Vector3(new_postion_x, 0, new_postion_y), Vector3.up, rotation);
                        break;
                    case 8:
                        rotation = 270;
                        new_postion_x = -parten_district_script.width + script.street_width / 2 - parent_district_scripts[3].width;
                        new_postion_y = 0 - script.street_width / 2;
                        shift_x = new_postion_x - height;
                        shift_y = new_postion_y - width;

                        district.transform.position = new Vector3(0 + shift_x, 0, 0 + shift_y);
                        district.transform.RotateAround(new Vector3(new_postion_x, 0, new_postion_y), Vector3.up, rotation);
                        parent_district_scripts.Add(script);
                        break;
                    default:
                        break;
                }

            i++;
        }
    }


    void Update()
    {
        //arange_city();
    }
}