using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class EventListener : MonoBehaviour
{
    public static EventListener current;
    public event Action show_energy_star;

    public void Awake()
    {
        current = this;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            show_energy_star.Invoke();
        }
        if (Input.GetKeyDown("a"))
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.1f;
            //RenderSettings.fogColor = Color.gray;

        }
    }
}
