using System;
using System.Collections.Generic;
using CityData;
using UnityEngine;
using UnityEngine.UIElements;
using City = CityRender.City;
using District = CityRender.District;
using EventListener = CityRender.EventListener;
using House_data = CityRender.House_data;

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

    void Start()
    {
        LoadData();
        GetUIComponents();

        InitButtonFunctions();

        // Initialize
        SetYearDisplay(currentYearIndex);
        CreateCitys();
        FocusCameraOnCity();
    }

    /* ================================================ Private Functions =============================================== */

    private void CreateCitys()
    {
        int i = 0;
        foreach (Year year in years)
        {
            int j = 0;
            citiesObject.Add(new List<GameObject>());

            Vector2 init_pos = new Vector2(0, 0);

            foreach (CityObj city_obj in year.GetCities())
            {
                GameObject new_city = Instantiate(initCity);
                City script = new_city.GetComponent<City>();
                Debug.Log(city_obj.city_id);
                script.city_data = city_obj;
                script.create_city();
                citiesObject[i].Add(new_city);

                if (i != 0)
                {
                    new_city.SetActive(false);
                }

                new_city.transform.position = new Vector3(init_pos.x, 0, init_pos.y);

                var currentCity = new_city.GetComponent<City>();
                var city_width = (currentCity.size_x + currentCity.size_minus_x) * 3;
                var city_height = (currentCity.size_y + currentCity.size_minus_y) * 3;

                init_pos.x += Math.Max(city_height, city_width) + 100;

                if (j > 0)
                {
                    var prevCity = citiesObject[i][j - 1].GetComponent<City>();
                    var prevCity_width = (prevCity.size_x + prevCity.size_minus_x) * 3;
                    var prevCity_height = (prevCity.size_y + prevCity.size_minus_y) * 3;
                    init_pos.x += Math.Max(prevCity_height, prevCity_width) + 100;
                }

                j++;
            }

            i++;
        }
        SetModeCity();
    }

    private GameObject currentDistrict = null;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // �berpr�fen, ob die linke Maustaste gedr�ckt wurde
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Erzeugen eines Strahls von der Mausposition

            if (Physics.Raycast(ray, out RaycastHit hit)) // �berpr�fen, ob der Strahl ein GameObject getroffen hat
            {
                // View of City
                if (currentMode == 0)
                {
                    SetModeDistrict();

                    FocusCameraOnDistrict(hit.collider.gameObject);
                    Debug.Log(
                        "Current District: "
                            + hit.collider.gameObject.GetComponent<District>().district_type
                    );
                    currentDistrict = hit.collider.gameObject;
                }
                // View of District
                else if (currentMode == 1)
                {
                    SetModeHouse();

                    FocusCameraOnObject(hit.collider.gameObject);
                    Debug.Log(
                        "Current House: "
                            + hit.collider.gameObject.GetComponent<House_data>().house_type
                    );
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // View of District
            if (currentMode == 1)
            {
                SetModeCity();

                FocusCameraOnCity();
            }
            // View of House
            else if (currentMode == 2)
            {
                SetModeDistrict();

                if (currentDistrict != null)
                {
                    Debug.Log(
                        "Current District: "
                            + currentDistrict.GetComponent<District>().district_type
                    );
                    FocusCameraOnDistrict(currentDistrict);
                }
            }
        }
    }

    /**
     * Move to the next year
     */
    private void NextYear()
    {
        if (currentYearIndex < citiesObject.Count - 1)
        {
            Debug.Log("Year: " + currentYearIndex + ",\tCity: " + currentCityIndex);
            var oldCity = citiesObject[currentYearIndex][currentCityIndex];

            citiesObject[currentYearIndex].ForEach(city => city.SetActive(false));
            currentYearIndex++;
            citiesObject[currentYearIndex].ForEach(city => city.SetActive(true));

            currentCityIndex = citiesObject[currentYearIndex]
                .FindIndex(city =>
                    city.GetComponent<City>().city_data.city_id
                    == oldCity.GetComponent<City>().city_data.city_id
                );

            if (currentCityIndex == -1)
            {
                currentCityIndex = 0;
            }
           
            SetModeCity();
            SetYearDisplay(currentYearIndex);
            FocusCameraOnCity();
        }
    }

    /**
     * Move to the previous year
     */
    private void PrevYear()
    {
        if (currentYearIndex > 0)
        {
            Debug.Log("Year: " + currentYearIndex + ",\tCity: " + currentCityIndex);

            var oldCity = citiesObject[currentYearIndex][currentCityIndex];

            citiesObject[currentYearIndex].ForEach(city => city.SetActive(false));
            currentYearIndex--;
            citiesObject[currentYearIndex].ForEach(city => city.SetActive(true));

            currentCityIndex = citiesObject[currentYearIndex]
                .FindIndex(city =>
                    city.GetComponent<City>().city_data.city_id
                    == oldCity.GetComponent<City>().city_data.city_id
                );

            if (currentCityIndex == -1)
            {
                currentCityIndex = 0;
            }
            SetModeCity();
            SetYearDisplay(currentYearIndex);
            FocusCameraOnCity();
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
        if (currentCityIndex < citiesObject[currentYearIndex].Count - 1)
        {
            currentCityIndex++;
            FocusCameraOnCity();
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
            FocusCameraOnCity();
        }
    }

    /**
     * Focus the camera on the city at the given index
     * @param cityIndex The index of the city to focus on
     */
    private void FocusCameraOnCity()
    {
        var currentGameObject = citiesObject[currentYearIndex][currentCityIndex];
        var currentCity = currentGameObject.GetComponent<City>();
        var alpha = Camera.main.fieldOfView / 2;
        var city_width = currentCity.size_x + currentCity.size_minus_x;
        var city_height = currentCity.size_y + currentCity.size_minus_y;
        var a = Math.Max(city_width, city_height) / 2;
        var distance = a / Mathf.Tan(alpha * Mathf.Deg2Rad);

        Camera.main.transform.position = new Vector3(
            currentGameObject.transform.position.x,
            distance,
            currentGameObject.transform.position.z
        );
    }

    private void FocusCameraOnDistrict(GameObject district)
    {
        // Get the collider attached to the object
        if (!district.TryGetComponent<Collider>(out var collider))
        {
            Debug.LogError("No collider found on the object!");
            return;
        }

        // Calculate the dimensions of the collider
        var districtData = district.GetComponent<District>();
        float width = districtData.width;
        float height = districtData.height;

        // Calculate the distance from the object for camera positioning
        var alpha = Camera.main.fieldOfView / 2;
        var a = Mathf.Max(width, height) / 2;
        var distance = a / Mathf.Tan(alpha * Mathf.Deg2Rad);

        // Set the camera position directly above the object
        Camera.main.transform.position = new Vector3(
            district.transform.position.x,
            distance + 20,
            district.transform.position.z // Assuming 2D (X-Z plane)
        );
    }

    private void FocusCameraOnObject(GameObject obj)
    {
        // Get the collider attached to the object
        if (!obj.TryGetComponent<Collider>(out var collider))
        {
            Debug.LogError("No collider found on the object!");
            return;
        }

        // Calculate the dimensions of the collider
        Vector3 colliderSize = collider.bounds.size;
        float width = colliderSize.x;
        float height = colliderSize.z;

        // Calculate the distance from the object for camera positioning
        var alpha = Camera.main.fieldOfView / 2;
        var a = Mathf.Max(width, height) / 2;
        var distance = a / Mathf.Tan(alpha * Mathf.Deg2Rad);

        // Set the camera position directly above the object
        Camera.main.transform.position = new Vector3(
            obj.transform.position.x,
            distance + 20,
            obj.transform.position.z // Assuming 2D (X-Z plane)
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

    private void SetModeCity()
    {
        EventListener.current.execute_enableBoxColliderDistrict();
        EventListener.current.execute_disableBoxColliderHouse();

        currentMode = 0;
    }

    private void SetModeDistrict()
    {
        EventListener.current.execute_disableBoxColliderDistrict();
        EventListener.current.execute_enableBoxColliderHouse();

        currentMode = 1;
    }

    private void SetModeHouse()
    {
        EventListener.current.execute_disableBoxColliderDistrict();
        EventListener.current.execute_disableBoxColliderHouse();

        currentMode = 2;
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
