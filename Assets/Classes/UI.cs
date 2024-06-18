using System.Collections;
using System.Collections.Generic;
using CityData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using City = CityRender.City;
using CSVReader = CityData.CSVReader;
using EventListener = CityRender.EventListener;
using House = CityRender.House;
using Year = CityData.Year;

public class UI : MonoBehaviour
{
    private VisualElement root;
    private Button nextYearButton,
        prevYearButton,
        nextCityButton,
        prevCityButton,
        yellowBtn,
        greenBtn,
        blueBtn;

    private IntegerField yearDisplay;

    private CSVReader csvReader;
    private List<Year> years;

    private int currentYearIndex = 0;
    private int currentCityIndex = 0;

    private int currentMode = 0;
    private const int YEAR_OFFSET = 2005;

    [SerializeField]
    private GameObject initCity;

    private List<List<GameObject>> citiesObject = new();

    // Placeholder Cubes, shoukd be replaced with actual city data
    private GameObject[] cubes;

    private EventListener el;

    void OnEnable()
    {
        LoadData();
        GetUIComponents();

        InitButtonFunctions();

        // Initialize
        SetYearDisplay(currentYearIndex);
        cubes = GameObject.FindGameObjectsWithTag("Cube");
        System.Array.Sort(
            cubes,
            (a, b) => a.transform.position.x.CompareTo(b.transform.position.x)
        );
        FocusCameraOnCity(currentCityIndex);
    }

    void Start()
    {
        CreateCitys();
    }

    /* ================================================ Private Functions =============================================== */

    private void CreateCitys()
    {
        int i = 0,
            j = 0;
        foreach (Year year in years)
        {
            if (i != 0)
            {
                break;
            }
            i++;
            citiesObject.Add(new List<GameObject>());

            foreach (CityObj city_obj in year.GetCities())
            {
                if (j != 0)
                {
                    break;
                }
                j++;
                GameObject new_city = Instantiate(initCity);
                City script = new_city.GetComponent<City>();
                Debug.Log(city_obj.city_id);
                script.city_data = city_obj;
                script.create_city();
                citiesObject[^1].Add(new_city);
            }
        }
        EventListener.current.execute_disableBoxColliderDistrict();
        EventListener.current.execute_enableBoxColliderHouse();
        Camera.main.transform.position = new Vector3(
            0,
            50,
            -citiesObject[0][0].GetComponent<City>().camera_distance - 50
        );
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // �berpr�fen, ob die linke Maustaste gedr�ckt wurde
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Erzeugen eines Strahls von der Mausposition
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // �berpr�fen, ob der Strahl ein GameObject getroffen hat
            {
                //hit.collider.gameObject.transform
            }
        }
    }

    /**
     * Move to the next year
     */
    private void NextYear()
    {
        if (currentYearIndex < years.Count - 1)
        {
            currentYearIndex++;
            years[currentYearIndex].PrintYear();
        }
    }

    /**
     * Move to the previous year
     */
    private void PrevYear()
    {
        if (currentYearIndex > 0)
        {
            currentYearIndex--;
            years[currentYearIndex].PrintYear();
        }
    }

    /**
     * Set the year display to the given count. Is offset by ${YEAR_OFFSET}
     * @param count The count to set the year display to
     */
    private void SetYearDisplay(int count)
    {
        yearDisplay.value = count + YEAR_OFFSET;
    }

    /**
     * Move to the next city. This is done by incrementing the currentCityIndex and setting the camera position to the new city
     */
    private void NextCity()
    {
        if (currentCityIndex < cubes.Length - 1)
        {
            currentCityIndex++;
            FocusCameraOnCity(currentCityIndex);
        }
    }

    /**
     * Move to the previous city. This is done by decrementing the currentCityIndex and setting the camera position to the new city
     */
    private void PrevCity()
    {
        if (currentCityIndex > 0)
        {
            currentCityIndex--;
            FocusCameraOnCity(currentCityIndex);
        }
    }

    /**
     * Focus the camera on the city at the given index
     * @param cityIndex The index of the city to focus on
     */
    private void FocusCameraOnCity(int cityIndex)
    {
        Camera.main.transform.position = new Vector3(
            cubes[cityIndex].transform.position.x,
            cubes[cityIndex].transform.position.y,
            -10
        );
    }

    /**
     * Load Data from CSV File
     */
    private void LoadData()
    {
        // Load Data
        csvReader = new CSVReader();
        years = csvReader.GetData();
    }

    /**
     * Get UI Components (Buttons, IntegerFields, etc.)
     */
    private void GetUIComponents()
    {
        // Get UI Elements
        root = GetComponent<UIDocument>().rootVisualElement;
        nextYearButton = root.Q<Button>("nextYear");
        prevYearButton = root.Q<Button>("prevYear");
        nextCityButton = root.Q<Button>("nextCity");
        prevCityButton = root.Q<Button>("prevCity");
        yearDisplay = root.Q<IntegerField>("yearDisplay");
        greenBtn = root.Q<Button>("btn1");
        yellowBtn = root.Q<Button>("btn2");
        blueBtn = root.Q<Button>("btn3");
    }

    /**
     * Initialize Button Click Events
     */
    private void InitButtonFunctions()
    {
        // Set Button Click Events
        nextYearButton.clicked += () =>
        {
            NextYear();
            SetYearDisplay(currentYearIndex);
        };
        prevYearButton.clicked += () =>
        {
            PrevYear();
            SetYearDisplay(currentYearIndex);
        };
        nextCityButton.clicked += NextCity;
        prevCityButton.clicked += PrevCity;
        greenBtn.clicked += show_energy_star;
        yellowBtn.clicked += show_year_build;
        blueBtn.clicked += show_eui;
    }

    /**
     * Set the color of the text in the cube-object
     * @param color The color-object to set the text to
     */
    private void SetCubeTextColor(Color color)
    {
        foreach (var cube in cubes)
        {
            // Set Text Mesh Pro color
            cube.GetComponent<TMPro.TextMeshPro>().color = color;
        }
    }

    private void show_energy_star()
    {
        EventListener.current.execute_show_energy_star();
    }

    private void show_year_build()
    {
        EventListener.current.execute_show_year_build();
    }

    private void show_eui()
    {
        EventListener.current.execute_show_eui();
    }
}
