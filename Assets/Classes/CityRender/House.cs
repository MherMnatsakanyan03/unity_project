using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public Vector3 position;
    public GameObject house;
    public GameObject house_copy;
    private Gradient gradient;

    public float eui = 0f;
    public int year_build = 2000;
    public int energy_star = 50;
    public int colling_degree_days = 50;
    public int warming_degree_days = 50;
    public float relativ_eui = 5000;
    public int relativ_year_build = 2000;

    public string house_type;

    public House(Vector3 position, GameObject copy, float eui, int year_build, int energy_star, int colling_degree_days, int warming_degree_days)
    {
        this.gradient = new Gradient();
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.red, 1.0f);
        colors[1] = new GradientColorKey(Color.blue, 0.0f);

        // Blend alpha from opaque at 0% to transparent at 100%
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(0.0f, 1.0f);
        this.gradient.SetKeys(colors, alphas);

        this.position = position;

        this.house_copy = copy;

        this.eui = eui;
        this.year_build = year_build;
        this.energy_star = energy_star;
        this.colling_degree_days = colling_degree_days;
        this.warming_degree_days = warming_degree_days;
        Debug.Log(EventListener.current);
        EventListener.current.show_energy_star += show_energy_star;
    }

    public GameObject create_house()
    {
        this.house = Instantiate(this.house_copy, this.position, Quaternion.identity);
        return this.house;
    }

    public void show_energy_star()
    {
        this.house.GetComponent<Renderer>().material.SetColor("_Color", this.gradient.Evaluate(this.energy_star / 100f));
    }

    public void show_year_build()
    {
        this.house.GetComponent<Renderer>().material.SetColor("_Color", this.gradient.Evaluate(this.year_build / (float)this.relativ_year_build));
    }

    public void show_eui()
    {
        this.house.GetComponent<Renderer>().material.SetColor("_Color", this.gradient.Evaluate(this.eui / (float)this.relativ_year_build));
    }
}
