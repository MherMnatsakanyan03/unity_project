using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CSVReader = CityData.CSVReader;
using Year = CityData.Year;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement root;
    private Button nextYearButton,
        prevYearButton,
        nextCityButton,
        prevCityButton,
        yellowBtn,
        greenBtn,
        blueBtn;

    private CSVReader csvReader;
    private List<Year> years;

    private int currentYearIndex = 0;
    private const int YEAR_OFFSET = 2005;

    void OnEnable()
    {
        csvReader = new CSVReader();
        years = csvReader.GetYears();
        root = GetComponent<UIDocument>().rootVisualElement;

        nextYearButton = root.Q<Button>("nextYear");
        prevYearButton = root.Q<Button>("prevYear");
        nextCityButton = root.Q<Button>("nextCity");
        prevCityButton = root.Q<Button>("prevCity");
        greenBtn = root.Q<Button>("btn1");
        yellowBtn = root.Q<Button>("btn2");
        blueBtn = root.Q<Button>("btn3");

        nextYearButton.clicked += NextYear;
        prevYearButton.clicked += PrevYear;
        nextCityButton.clicked += () => Debug.Log("Next City");
        prevCityButton.clicked += () => Debug.Log("Previous City");
        greenBtn.clicked += () => Debug.Log("Green Button");
        yellowBtn.clicked += () => Debug.Log("Yellow Button");
        blueBtn.clicked += () => Debug.Log("Blue Button");
    }

    private void NextYear() {
        if (currentYearIndex < years.Count - 1) {
            currentYearIndex++;
            years[currentYearIndex].printYearData();
        }
    }

    private void PrevYear() {
        if (currentYearIndex > 0) {
            currentYearIndex--;
            years[currentYearIndex].printYearData();
        }
    }
}
