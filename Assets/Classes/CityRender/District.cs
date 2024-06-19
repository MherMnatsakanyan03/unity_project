using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using CityData;
using UnityEngine;

namespace CityRender
{
    public class District : MonoBehaviour
    {
        [SerializeField]
        private GameObject street_original,
            street_edge_original;

        private List<GameObject> houses = new List<GameObject>();
        public string district_type = "Warehouse";

        public Vector3 position;
        public float width;
        public float height;
        public float street_width;
        public float districtSquareMeterSize = 40f;

        public Facility facility_data;
        public double maxEUI;

        public void create_district()
        {
            List<int> houses_area = new List<int>
                {
                (int)facility_data.GetThreshoulds(0) / 10000,
                (int)facility_data.GetThreshoulds(1) / 10000,
                (int)facility_data.GetThreshoulds(2) / 10000
                };
            List<int> number_houses = new List<int>
                {
                (int)facility_data.GetCountPart(0),
                (int)facility_data.GetCountPart(1),
                (int)facility_data.GetCountPart(2)
                };

            Grid grid = new Grid(
                    gameObject.transform,
                    1,
                    0,
                    0,
                    houses_area,
                    number_houses,
                    district_type,
                    facility_data.GetHouses(),
                    maxEUI
                );


            this.street_width = street_original.GetComponent<Renderer>().bounds.size.x;
            this.width = grid.width + street_width;
            this.height = grid.height + street_width;

            if(districtSquareMeterSize != 0)
            {
                //Create Streets
                (List<Matrix4x4> streetMatricesN, List<Matrix4x4> edgeMatricesN) =
                    StreetPositionCalculator.createStreet_Matrix4x4(
                        gameObject,
                        street_original,
                        street_edge_original,
                        grid.width,
                        grid.height
                    );
                for (int i = 0; i < streetMatricesN.Count; i++)
                {
                    Matrix4x4 matrix = streetMatricesN[i];
                    GameObject street = Instantiate(street_original);
                    street.transform.SetParent(gameObject.transform);
                    StreetPositionCalculator.SetTransformFromMatrix(street.transform, ref matrix);
                }
                for (int i = 0; i < edgeMatricesN.Count; i++)
                {
                    Matrix4x4 matrix = edgeMatricesN[i];
                    GameObject street = Instantiate(street_edge_original);
                    street.transform.SetParent(gameObject.transform);
                    StreetPositionCalculator.SetTransformFromMatrix(street.transform, ref matrix);
                }

                //Create Houses
                //get_house_modells(district_type);

                
                //grid.drawOutlines();

                EventListener.current.enableBoxColliderDistrict += enableBoxCollider;
                EventListener.current.disableBoxColliderDistrict += disableBoxCollider;
            }
            
        }

        private void enableBoxCollider()
        {
           gameObject.GetComponent<BoxCollider>().enabled = true;
        }

        private void disableBoxCollider()
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }

        private void get_house_modells(string house_type_name)
        {
            string directoryPath = "Assets/Resources/House/" + house_type_name;
            string[] directoryContents = Directory.GetFiles(directoryPath);
            foreach (string file in directoryContents)
            {
                string substring = Path.GetFileName(file);
                GameObject loadedObject = Resources.Load<GameObject>(
                    "House/" + house_type_name + "/" + substring.Substring(0, substring.Length - 7)
                );
                if (loadedObject != null)
                {
                    houses.Add(loadedObject);
                }
            }
        }
    }
}
