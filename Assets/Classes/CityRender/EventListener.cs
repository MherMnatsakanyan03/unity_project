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
        public event Action reset_color;

        public event Action enableBoxColliderHouse, disableBoxColliderHouse;

        public event Action enableBoxColliderDistrict, disableBoxColliderDistrict;

        public void Awake()
        {
            current = this;
        }

        public void execute_enableBoxColliderHouse()
        {
            enableBoxColliderHouse.Invoke();
        }

        public void execute_disableBoxColliderHouse()
        {
            disableBoxColliderHouse.Invoke();
        }

        public void execute_enableBoxColliderDistrict()
        {
            enableBoxColliderDistrict.Invoke();
        }

        public void execute_disableBoxColliderDistrict()
        {
            disableBoxColliderDistrict.Invoke();
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

        internal void execute_reset_color()
        {
            reset_color.Invoke();
        }

    }
}
