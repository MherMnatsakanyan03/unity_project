using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace CityRender
{
    public class EventListener : MonoBehaviour
    {
        public static EventListener current;
        public event Action show_energy_star;
        public event Action show_year_build;
        public event Action show_eui;

        public void Awake()
        {
            current = this;
        }

        public void execute_show_energy_star()
        {
            show_energy_star.Invoke();
        }
        public void execute_show_year_build()
        {
            show_year_build.Invoke();
        }
        public void execute_show_eui()
        {
            show_eui.Invoke();
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
}
