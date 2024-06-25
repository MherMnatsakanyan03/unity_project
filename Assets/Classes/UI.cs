using System;
using System.Collections.Generic;
using CityData;
using Unity.VisualScripting;
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

    private VisualElement colorbar;
    private Label upper_limit_colorbar;
    private Label bottom_limit_colorbar;

    private ListView house_data_list;

    private CSVReader csvReader;
    private List<Year> years;

    private List<List<float>> offsets = new();

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
        CreateTest();
        //CreateCitys();
        FocusCameraOnCity();
        diable_colorbar();

        // Log all cities
        string log = "";

        for (int i = 0; i < citiesObject.Count; i++)
        {
            log += "Year " + (i + YEAR_OFFSET) + ":\n";
            for (int j = 0; j < citiesObject[i].Count; j++)
            {
                log +=
                    "\tCity "
                    + j
                    + ": "
                    + citiesObject[i][j].GetComponent<City>().city_data.city_id
                    + "\n";
                // Log coordinates
                log += "\t\tCoordinates: " + citiesObject[i][j].transform.position + "\n";
            }
        }

        Debug.Log(log);
    }

    /* ================================================ Private Functions =============================================== */

    private void CreateTest()
    {
        citiesObject.Add(new List<GameObject>());
        CityObj city_obj = years[0].GetCities()[0];
        GameObject new_city = Instantiate(initCity);
        City script = new_city.GetComponent<City>();
        script.city_data = city_obj;
        script.create_city();
        citiesObject[0].Add(new_city);
    }

    private void CreateCitys()
    {
        int i = 0;
        Vector2 init_pos = new Vector2(0, 0);
        foreach (Year year in years)
        {
            int j = 0;
            citiesObject.Add(new List<GameObject>());
            offsets.Add(new List<float>());

            offsets[i].Add(init_pos.x);
            foreach (CityObj city_obj in year.GetCities())
            {
                GameObject new_city = Instantiate(initCity);
                City script = new_city.GetComponent<City>();
                script.city_data = city_obj;
                script.create_city();

                new_city.transform.position = new Vector3(init_pos.x, 0, init_pos.y);

                var currentCity = new_city.GetComponent<City>();
                var city_width = (currentCity.size_x + currentCity.size_minus_x) * 3;
                var city_height = (currentCity.size_y + currentCity.size_minus_y) * 3;

                init_pos.x += Math.Max(city_height, city_width) + 100;

                if (i != 0)
                {
                    new_city.SetActive(false);
                }

                citiesObject[i].Add(new_city);

                if (j > 0)
                {
                    var prevCity = citiesObject[i][j - 1].GetComponent<City>();
                    var prevCity_width = (prevCity.size_x + prevCity.size_minus_x) * 3;
                    var prevCity_height = (prevCity.size_y + prevCity.size_minus_y) * 3;
                    init_pos.x += Math.Max(prevCity_height, prevCity_width) + 100;
                }

                offsets[i].Add(init_pos.x);

                j++;
            }

            init_pos.x = 0;
            init_pos.y += 1000;
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
                    // Set UI to show house data

                    var house_type = hit.collider.gameObject.GetComponent<House_data>().house_type;
                    var eui = hit.collider.gameObject.GetComponent<House_data>().eui;
                    var year = hit.collider.gameObject.GetComponent<House_data>().year_build;
                    var energy_star = hit
                        .collider.gameObject.GetComponent<House_data>()
                        .energy_star;

                    /* house_data_list.hierarchy.Add(new Label("House Type: " + house_type));
                    house_data_list.hierarchy.Add(new Label("Year Build: " + year));
                    house_data_list.hierarchy.Add(new Label("EUI: " + eui));
                    house_data_list.hierarchy.Add(new Label("Energy Star: " + energy_star)); */

                    List<string> items = new List<string>
                    {
                        "House Type: " + house_type,
                        "Year Build: " + year,
                        "EUI: " + eui,
                        "Energy Star: " + energy_star
                    };

                    // Define a function to create UI elements
                    VisualElement makeItem() => new Label();

                    // Bind data to UI elements
                    void bindItem(VisualElement e, int i) => (e as Label).text = items[i];

                    // Set the data source and callbacks
                    house_data_list.itemsSource = items;
                    house_data_list.makeItem = makeItem;
                    house_data_list.bindItem = bindItem;
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
        if (currentYearIndex < citiesObject.Count - 1 && currentMode == 0)
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
        if (currentYearIndex > 0 && currentMode == 0)
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
            diable_colorbar();
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
        if (currentCityIndex < citiesObject[currentYearIndex].Count - 1 && currentMode == 0)
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
        if (currentCityIndex > 0 && currentMode == 0)
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
        var city_width = Math.Max(currentCity.size_x + currentCity.size_minus_x, 100);
        var city_height = Math.Max(currentCity.size_y + currentCity.size_minus_y, 100);
        var a = Math.Max(city_width, city_height);
        var distance = 0.8f * a / Mathf.Tan(alpha * Mathf.Deg2Rad);

        distance = Math.Min(distance, 990);

        Camera.main.transform.position = new Vector3(
            currentGameObject.transform.position.x,
            distance,
            currentGameObject.transform.position.z + 30
        );
        Debug.Log(
            "New Pos: "
                + currentGameObject.transform.position.x
                + ", "
                + currentGameObject.transform.position.z
                + ", Distance: "
                + distance
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
        var middle = districtData.position;
        float width = districtData.width;
        float height = districtData.height;

        // Calculate the distance from the object for camera positioning
        var alpha = Camera.main.fieldOfView / 2;
        var a = Mathf.Max(width, height) / 2;
        var distance = a / Mathf.Tan(alpha * Mathf.Deg2Rad);

        distance = Math.Min(distance, 990);

        // Set the camera position directly above the object
        Camera.main.transform.position = new Vector3(
            middle.x + offsets[currentYearIndex][currentCityIndex],
            distance,
            middle.z + currentYearIndex * 1000 // Assuming 2D (X-Z plane)
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
        float width = Math.Max(colliderSize.x, 50);
        float height = Math.Max(colliderSize.z, 50);

        // Calculate the distance from the object for camera positioning
        var alpha = Camera.main.fieldOfView / 2;
        var a = Mathf.Max(width, height) / 2;
        var distance = a / Mathf.Tan(alpha * Mathf.Deg2Rad);

        distance = Math.Min(distance, 990);
        distance = Math.Max(distance, 50);

        // Set the camera position directly above the object
        Camera.main.transform.position = new Vector3(
            obj.transform.position.x,
            distance,
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
        house_data_list = root.Q<ListView>("house_data_list");

        upper_limit_colorbar = root.Q<Label>("UpperLimit");
        bottom_limit_colorbar = root.Q<Label>("BottomLimit");
        colorbar = root.Q<VisualElement>("Colorbar");
    }

    private void diable_colorbar()
    {
        upper_limit_colorbar.style.display = DisplayStyle.None;
        bottom_limit_colorbar.style.display = DisplayStyle.None;
        colorbar.style.display = DisplayStyle.None;
    }

    private void enable_colorbar()
    {
        upper_limit_colorbar.style.display = DisplayStyle.Flex;
        bottom_limit_colorbar.style.display = DisplayStyle.Flex;
        colorbar.style.display = DisplayStyle.Flex;
    }

    private void change_text(string up_text, string bot_text)
    {
        upper_limit_colorbar.text = up_text;
        bottom_limit_colorbar.text = bot_text;
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
        greenBtn.clicked += () =>
        {
            show_energy_star();
            enable_colorbar();
            change_text("100 %", "0");
        };
        yellowBtn.clicked += () =>
        {
            show_year_build();
            enable_colorbar();
        };
        blueBtn.clicked += () =>
        {
            show_eui();
            enable_colorbar();
        };
    }

    private void SetModeCity()
    {
        EventListener.current.execute_enableBoxColliderDistrict();
        EventListener.current.execute_disableBoxColliderHouse();

        house_data_list.itemsSource = null; // Reassign the itemsSource

        currentMode = 0;
    }

    private void SetModeDistrict()
    {
        EventListener.current.execute_disableBoxColliderDistrict();
        EventListener.current.execute_enableBoxColliderHouse();

        house_data_list.itemsSource = null; // Reassign the itemsSource

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
