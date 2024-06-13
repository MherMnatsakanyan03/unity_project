using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using UnityEngine;

public class District : MonoBehaviour
{
    [SerializeField]
    private GameObject street_original, street_edge_original;

    private List<GameObject> houses = new List<GameObject>();
    public string district_type = "Warehouse";

    public float width;
    public float height;
    public float street_width;
    public float districtSquareMeterSize = 40f;

    public void create_district()
    {
        
        float randomNumber = 0.5f;
        var b = (int)Mathf.Sqrt(districtSquareMeterSize / randomNumber) * 10;
        var a = (int)(randomNumber * b);

        this.street_width = street_original.GetComponent<Renderer>().bounds.size.x;
        this.width = a + street_width;
        this.height = b + street_width;

        //Create Streets
        (List<Matrix4x4> streetMatricesN, List<Matrix4x4> edgeMatricesN) = StreetPositionCalculator.createStreet_Matrix4x4(gameObject, street_original, street_edge_original,a,b);
        for (int i = 0; i < streetMatricesN.Count; i++)
        {
            Matrix4x4 matrix = streetMatricesN[i];
            GameObject street = Instantiate(street_original);
            street.transform.SetParent(gameObject.transform);
            StreetPositionCalculator.SetTransformFromMatrix(street.transform,ref matrix);
        }
        for(int i = 0;i < edgeMatricesN.Count; i++)
        {
            Matrix4x4 matrix = edgeMatricesN[i];
            GameObject street = Instantiate(street_edge_original);
            street.transform.SetParent(gameObject.transform);
            StreetPositionCalculator.SetTransformFromMatrix(street.transform, ref matrix);
        }

        //Create Houses
        get_house_modells(district_type);
        List<int> houses_area = new List<int> { 1, 4, 9 };
        List<int> number_houses = new List<int> { 2, 5, 3 };
        Grid grid = new Grid(gameObject.transform, (int)(b), (int)(a), 1, -(int)(a), 0, houses_area, number_houses, district_type);
        //grid.drawOutlines();
    }

    private void get_house_modells(string house_type_name)
    {
        string directoryPath = "Assets/Resources/House/" + house_type_name;
        string[] directoryContents = Directory.GetFiles(directoryPath);
        foreach (string file in directoryContents)
        {
            string substring = Path.GetFileName(file);
            GameObject loadedObject = Resources.Load<GameObject>("House/" + house_type_name + "/" + substring.Substring(0, substring.Length - 7));
            if (loadedObject != null)
            {
                houses.Add(loadedObject);
            }
        }
    }

    void Start()
    {
        //create_district();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Überprüfen, ob die linke Maustaste gedrückt wurde
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // Erzeugen eines Strahls von der Mausposition
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))  // Überprüfen, ob der Strahl ein GameObject getroffen hat
            {
                if (hit.collider.gameObject == this.gameObject)  // Überprüfen, ob das getroffene GameObject das gewünschte ist
                {
                    Debug.Log(this.gameObject.transform.position - new Vector3(0,0,this.width / 2 - this.street_width / 2));
                }
            }
        }
    }
}
