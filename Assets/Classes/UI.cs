using System;
using System.Collections.Generic;
using CityData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using City = CityRender.City;
using District = CityRender.District;
using EventListener = CityRender.EventListener;
using House_data = CityRender.House_data;

public class UI : MonoBehaviour
{
    private float CAMERA_ANGLE = 40f;
    private float TAN_CAMERA_ANGLE;
    private float CAMERA_Z = 1000f;
    private float CAMERA_Z_OFFSET;
    private VisualElement root;
    private Button nextYearButton,
        prevYearButton,
        nextCityButton,
        prevCityButton,
        yellowBtn,
        greenBtn,
        blueBtn,
        resetBtn;

    private IntegerField yearDisplay;

    private VisualElement colorbar;
    private Label upper_limit_colorbar;
    private Label bottom_limit_colorbar;
    private Label cityDisplay;

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
    private Dictionary<string, string> color_dict = new();

    void Start()
    {
        TAN_CAMERA_ANGLE = Mathf.Tan(CAMERA_ANGLE * Mathf.Deg2Rad);
        CAMERA_Z_OFFSET = CAMERA_Z / TAN_CAMERA_ANGLE;

        LoadData();
        GetUIComponents();
        CreateColorLegend();

        InitButtonFunctions();

        // Initialize
        SetYearDisplay(currentYearIndex);
        //CreateTest();
        CreateCitys();
        FocusCameraOnCity();
        disable_colorbar();

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
        script.create_city(color_dict);
        citiesObject[0].Add(new_city);
    }

    private void CreateCitys()
    {
        int i = 0;
        foreach (Year year in years)
        {
            /* if (i == 2)
            {
                break;
            } */
            Vector2 init_pos = new(0, i * 1000);

            int j = 0;
            citiesObject.Add(new List<GameObject>());
            offsets.Add(new List<float>());

            offsets[i].Add(init_pos.x);
            foreach (CityObj city_obj in year.GetCities())
            {
                GameObject new_city = Instantiate(initCity);
                City script = new_city.GetComponent<City>();
                script.city_data = city_obj;
                script.create_city(color_dict);

                new_city.transform.position = new Vector3(init_pos.x, 0, init_pos.y);

                var currentCity = new_city.GetComponent<City>();
                var city_width = currentCity.height * 3;
                var city_height = currentCity.width * 3;

                init_pos.x += Math.Max(city_height, city_width) + 100;

                if (i != 0)
                {
                    new_city.SetActive(false);
                }

                citiesObject[i].Add(new_city);

                if (j > 0)
                {
                    var prevCity = citiesObject[i][j - 1].GetComponent<City>();
                    var prevCity_width = prevCity.height * 3;
                    var prevCity_height = prevCity.width * 3;
                    init_pos.x += Math.Max(prevCity_height, prevCity_width) + 100;
                }

                offsets[i].Add(init_pos.x);

                j++;
            }
            i++;
        }
        SetModeCity();
    }

    private Color ParseColor(string color)
    {
        color ??= "#FFFFFF";

        int r = Convert.ToInt32(color.Substring(1, 2), 16);
        int g = Convert.ToInt32(color.Substring(3, 2), 16);
        int b = Convert.ToInt32(color.Substring(5, 2), 16);

        return new Color(r / 255f, g / 255f, b / 255f);
    }

    private void CreateColorLegend()
    {
        VisualElement overlayContainer = new();
        overlayContainer.style.position = Position.Absolute;
        overlayContainer.style.top = 0;
        overlayContainer.style.right = 0;
        overlayContainer.BringToFront(); // Ensure this is higher than the existing UI elements

        root.Add(overlayContainer);

        foreach (var pair in color_dict)
        {
            VisualElement facilityElement = new();
            facilityElement.style.flexDirection = FlexDirection.Row;
            facilityElement.style.alignItems = Align.Center; // Align items vertically centered

            // Create a color box
            VisualElement colorBox = new();
            colorBox.style.width = 22;
            colorBox.style.height = 22;
            colorBox.style.backgroundColor = ParseColor(pair.Value);
            facilityElement.Add(colorBox);

            // Create a label for the facility name
            Label facilityLabel = new(pair.Key);
            facilityLabel.style.fontSize = 18;
            facilityLabel.style.height = 20; // Match the height of the color box
            facilityLabel.style.marginLeft = 5; // Add some space between the color box and the label
            facilityLabel.AddToClassList("labelStyle");
            facilityElement.Add(facilityLabel);

            // Add the facility element to the overlay container
            overlayContainer.Add(facilityElement);
        }
    }

    private GameObject currentDistrict = null;
    private Vector3 old_camera_cosition = Vector3.zero;
    private float old_camera_size = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // überprüfen, ob die linke Maustaste gedrückt wurde
        {
            Vector3 mousePosition = Input.mousePosition;
            float screenHeight = Screen.height;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Erzeugen eines Strahls von der Mausposition

            // Falls Objekt getroffen wurde und Mausposition nicht in den oberen 10% oder unteren 10% des Bildschirms
            if (
                Physics.Raycast(ray, out RaycastHit hit)
                && mousePosition.y > 0.1f * screenHeight
                && mousePosition.y < 0.9f * screenHeight
            ) // überprüfen, ob der Strahl ein GameObject getroffen hat
            {
                // View of City
                if (currentMode == 0)
                {
                    old_camera_cosition = Camera.main.transform.position;
                    old_camera_size = Camera.main.orthographicSize;
                    FocusCameraOnDistrict(hit.collider.gameObject);
                    Debug.Log(
                        "Current District: "
                            + hit.collider.gameObject.GetComponent<District>().district_type
                    );
                    currentDistrict = hit.collider.gameObject;
                    SetModeDistrict();
                }
                // View of District
                else if (currentMode == 1)
                {
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

                    List<string> items =
                        new()
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

                    SetModeHouse();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // View of District
            if (currentMode == 1)
            {
                FocusCameraOnCity();
                SetModeCity();
                Camera.main.transform.position = old_camera_cosition;
                Camera.main.orthographicSize = old_camera_size;
            }
            // View of House
            else if (currentMode == 2)
            {
                if (currentDistrict != null)
                {
                    Debug.Log(
                        "Current District: "
                            + currentDistrict.GetComponent<District>().district_type
                    );
                    FocusCameraOnDistrict(currentDistrict);
                }
                SetModeDistrict();
            }
        }
        if (currentMode == 0)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                var current_city = citiesObject[currentYearIndex]
                    [currentCityIndex]
                    .GetComponent<City>();
                var current_city_pos =
                    current_city.position.x + offsets[currentYearIndex][currentCityIndex];
                var left_bound = current_city_pos - current_city.width / 2;

                var width = current_city.width;
                var step = width / 100 * Camera.main.orthographicSize / current_city.height;

                Camera.main.transform.position = new Vector3(
                    Mathf.Max(left_bound, Camera.main.transform.position.x - step),
                    Camera.main.transform.position.y,
                    Camera.main.transform.position.z
                );
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                var current_city = citiesObject[currentYearIndex]
                    [currentCityIndex]
                    .GetComponent<City>();
                var current_city_pos =
                    current_city.position.x + offsets[currentYearIndex][currentCityIndex];
                var right_bound = current_city_pos + current_city.width / 2;

                var width = current_city.width;
                var step = width / 100 * Camera.main.orthographicSize / current_city.height;

                Camera.main.transform.position = new Vector3(
                    Mathf.Min(right_bound, Camera.main.transform.position.x + step),
                    Camera.main.transform.position.y,
                    Camera.main.transform.position.z
                );
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                var current_city = citiesObject[currentYearIndex]
                    [currentCityIndex]
                    .GetComponent<City>();

                float currentCamY = Camera.main.transform.position.y;

                var current_city_pos =
                    current_city.position.y
                    + currentYearIndex * 1000
                    - currentCamY / TAN_CAMERA_ANGLE;
                var upper_bound = current_city_pos + current_city.height / 2;

                var height = current_city.height;
                var step = 0.0001f * height * Camera.main.orthographicSize;

                Camera.main.transform.position = new Vector3(
                    Camera.main.transform.position.x,
                    Camera.main.transform.position.y,
                    Mathf.Min(upper_bound, Camera.main.transform.position.z + step)
                );
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                var current_city = citiesObject[currentYearIndex]
                    [currentCityIndex]
                    .GetComponent<City>();

                float currentCamY = Camera.main.transform.position.y;

                var current_city_pos =
                    current_city.position.y
                    + currentYearIndex * 1000
                    - currentCamY / TAN_CAMERA_ANGLE;
                var lower_bound = current_city_pos - current_city.height / 2;

                var height = current_city.height;
                var step = 0.0001f * height * Camera.main.orthographicSize;

                Camera.main.transform.position = new Vector3(
                    Camera.main.transform.position.x,
                    Camera.main.transform.position.y,
                    Mathf.Max(lower_bound, Camera.main.transform.position.z - step)
                );
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                var current_city = citiesObject[currentYearIndex]
                    [currentCityIndex]
                    .GetComponent<City>();

                var current_size = Camera.main.orthographicSize;

                if (
                    Camera.main.orthographicSize
                    <= Math.Max(current_city.width, current_city.height)
                )
                {
                    Camera.main.orthographicSize += current_size / 100;
                }
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                var current_city = citiesObject[currentYearIndex]
                    [currentCityIndex]
                    .GetComponent<City>();

                var current_size = Camera.main.orthographicSize;

                if (Camera.main.orthographicSize >= 10)
                {
                    Camera.main.orthographicSize -= current_size / 100;
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
            disable_colorbar();
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
            disable_colorbar();
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
            SetModeCity();
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
            SetModeCity();
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

        // If camera is Perspective
        /* var alpha = Camera.main.fieldOfView / 2;
        var a = Math.Max(currentCity.width, currentCity.height) / 2;
        var distance = 0.8f * a / Mathf.Tan(alpha * Mathf.Deg2Rad);

        Camera.main.transform.position = new Vector3(
            currentCity.postion.x + offsets[currentYearIndex][currentCityIndex],
            distance,
            currentCity.postion.y + currentYearIndex * 1000 + currentCity.height / 2
        ); */

        //If camera is Orthographic
        // Change the width and height parameters to the aspect ratio of the camera
        Camera.main.orthographic = true;
        var width = currentCity.width;
        var height = currentCity.height;

        Camera.main.orthographicSize = Math.Max(width, height) / 2;

        var newCamX = currentCity.position.x + offsets[currentYearIndex][currentCityIndex];
        var newCamY = CAMERA_Z;
        var newCamZ = currentCity.position.y + currentYearIndex * 1000 - CAMERA_Z_OFFSET;

        Camera.main.transform.SetPositionAndRotation(
            new Vector3(newCamX, newCamY, newCamZ),
            Quaternion.Euler(CAMERA_ANGLE, 0f, 0f)
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

        // Revert to top-down view
        Camera.main.orthographic = false;
        // Calculate the dimensions of the collider
        var districtData = district.GetComponent<District>();
        var middle = districtData.position;
        float width = districtData.width;
        float height = districtData.height;

        // Calculate the distance from the object for camera positioning
        var alpha = Camera.main.fieldOfView / 2;
        var a = Mathf.Max(width, height) / 2;
        var distance = a / Mathf.Tan(alpha * Mathf.Deg2Rad);

        // Set the camera position directly above the object
        Camera.main.transform.SetPositionAndRotation(
            new Vector3(
                middle.x + offsets[currentYearIndex][currentCityIndex],
                distance,
                middle.z + currentYearIndex * 1000 // Assuming 2D (X-Z plane)
            ),
            Quaternion.Euler(90f, 0f, 0f)
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

        // Revert to top-down view
        Camera.main.orthographic = false;
        Camera.main.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        // Calculate the dimensions of the collider
        Vector3 colliderSize = collider.bounds.size;
        float width = Math.Max(colliderSize.x, 50);
        float height = Math.Max(colliderSize.z, 50);

        // Calculate the distance from the object for camera positioning
        var alpha = Camera.main.fieldOfView / 2;
        var a = Mathf.Max(width, height) / 2;
        var distance = a / Mathf.Tan(alpha * Mathf.Deg2Rad);

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
        color_dict = csvReader.GetFacilityColorMap();
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
        resetBtn = root.Q<Button>("btn4");
        house_data_list = root.Q<ListView>("house_data_list");

        upper_limit_colorbar = root.Q<Label>("UpperLimit");
        bottom_limit_colorbar = root.Q<Label>("BottomLimit");
        colorbar = root.Q<VisualElement>("Colorbar");

        cityDisplay = root.Q<Label>("cityDisplay");
    }

    private void disable_colorbar()
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
            change_text("100 %", "0");
        };
        blueBtn.clicked += () =>
        {
            show_eui();
            enable_colorbar();
            change_text("100 %", "0");
        };
        resetBtn.clicked += () =>
        {
            reset_color();
            disable_colorbar();
        };
    }

    private void SetModeCity()
    {
        EventListener.current.execute_enableBoxColliderDistrict();
        EventListener.current.execute_disableBoxColliderHouse();

        house_data_list.itemsSource = null; // Reassign the itemsSource

        cityDisplay.text =
            "City: "
            + citiesObject[currentYearIndex]
                [currentCityIndex]
                .GetComponent<City>()
                .city_data.city_id;

        currentMode = 0;
    }

    private void SetModeDistrict()
    {
        EventListener.current.execute_disableBoxColliderDistrict();
        EventListener.current.execute_enableBoxColliderHouse();

        house_data_list.itemsSource = null; // Reassign the itemsSource

        cityDisplay.text = "District: " + currentDistrict.GetComponent<District>().district_type;

        currentMode = 1;
    }

    private void SetModeHouse()
    {
        EventListener.current.execute_disableBoxColliderDistrict();
        EventListener.current.execute_disableBoxColliderHouse();

        cityDisplay.text = "";

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

    private void reset_color()
    {
        EventListener.current.execute_reset_color();
    }
}
