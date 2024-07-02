using System;
using System.Collections;
using System.Collections.Generic;
using CityData;
using RectpackSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CityRender
{
    public class Grid : MonoBehaviour
    {
        private int shift_x;
        private int shift_y;
        private float cellSize;
        private int[,] gridArray;

        public int width;
        public int height;
        private int area;

        private List<int> houses_area = new List<int>();
        private List<int> number_houses = new List<int>();
        private List<int> number_houses_cum_sum = new List<int>();
        private List<float> house_width = new List<float>();
        private List<float> house_height = new List<float>();
        private List<float> house_x;
        private List<float> house_y;
        private string house_type;
        private string district_type;
        private List<string> house_types = new List<string>();

        private Transform parent;
        private GameObject house_copy;
        private GameObject cube;
        private int counter = 0;
        public float[] shares;
        public int spacing = 2;

        private List<CityData.House> houses;
        private double maxEUI;

        private Dictionary<string, string> color_map = new();

        public Grid(
            Transform parent,
            float cellSize,
            int shift_x,
            int shift_y,
            List<int> houses_area,
            List<int> number_houses,
            string house_type,
            List<CityData.House> houses,
            double maxEUI,
            GameObject cube
        )
        {
            this.shift_x = shift_x;
            this.shift_y = shift_y;
            this.cellSize = cellSize;

            this.houses = houses;
            this.houses_area = houses_area; //Wie gro� ist welches Haus (klein,mittel,gro�)
            this.number_houses = number_houses; //Anzahl der H�user in den jeweiligen Klassen (klein,mittel,gro�)
            this.number_houses_cum_sum = cum_sum(number_houses);
            this.house_type = house_type;
            this.district_type = house_type;
            this.house_types.Add(house_type + "_small");
            this.house_types.Add(house_type + "_middle");
            this.house_types.Add(house_type + "_big");
            this.parent = parent;
            this.cube = cube;
            this.cube.SetActive(false);
            this.maxEUI = maxEUI;

            calculate_house_dimensions();
            PackingRectangle[] small_rectangles = new PackingRectangle[this.number_houses[0]];
            PackingRectangle[] middle_rectangles = new PackingRectangle[this.number_houses[1]];
            PackingRectangle[] large_rectangles = new PackingRectangle[this.number_houses[2]];
            int id = 0;
            for (int i = 0; i < small_rectangles.Length; i++)
            {
                small_rectangles[i].Width = (uint)this.house_width[0] + 2;
                small_rectangles[i].Height = (uint)this.house_height[0] + 2;
                small_rectangles[i].Id = id;
                id++;
            }

            for (int i = 0; i < middle_rectangles.Length; i++)
            {
                middle_rectangles[i].Width = (uint)this.house_width[1] + 2;
                middle_rectangles[i].Height = (uint)this.house_height[1] + 2;
                middle_rectangles[i].Id = id;
                id++;
            }

            for (int i = 0; i < large_rectangles.Length; i++)
            {
                large_rectangles[i].Width = (uint)this.house_width[2] + 2;
                large_rectangles[i].Height = (uint)this.house_height[2] + 2;
                large_rectangles[i].Id = id;
                id++;
            }

            Packing house_positions = new Packing(
                small_rectangles,
                middle_rectangles,
                large_rectangles
            );
            house_positions.pack();

            house_x = new List<float>(house_positions.rectangles.Length);
            house_y = new List<float>(house_positions.rectangles.Length);

            for (int i = 0; i < house_positions.rectangles.Length; i++)
            {
                house_x.Add(house_positions.rectangles[i].X);
                house_y.Add(house_positions.rectangles[i].Y);
            }
            this.width = (int)house_positions.bounds.Width;
            this.height = (int)house_positions.bounds.Height;
            this.shift_x = -this.height / 2;
            this.area = this.width * this.height;
            place_houses(house_x, house_y);

            this.gridArray = new int[this.width, this.height];
            fill_array(this.gridArray);
        }

        private List<int> cum_sum(List<int> list)
        {
            List<int> cumulativeSum = new List<int>();

            int sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
                cumulativeSum.Add(sum);
            }
            return cumulativeSum;
        }

        public void calculate_house_dimensions()
        {
            foreach (var area in this.houses_area)
            {
                var a = (float)Math.Sqrt((double)area);
                this.house_width.Add(a*2);
                this.house_height.Add(a*2);
            }
        }

        public Color pick_color()
        {
            color_map.TryGetValue(this.district_type, out string color);
            color ??= "#FFFFFF";

            int r = Convert.ToInt32(color.Substring(1, 2), 16);
            int g = Convert.ToInt32(color.Substring(3, 2), 16);
            int b = Convert.ToInt32(color.Substring(5, 2), 16);

            return new Color(r / 255f, g / 255f, b / 255f);
        }

        public void SetColorMap(Dictionary<string, string> color_map)
        {
            this.color_map = color_map;
        }

        public void drawOutlines()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.GetComponent<BoxCollider>().enabled = false;
                        cube.transform.localScale = new Vector3(2,0.5f,2);
                        cube.transform.parent = parent;
                        this.house_copy = cube;
                        cube.GetComponent<Renderer>().material.color = pick_color();
                        cube.transform.position = GetWorldPosition(y, x, 1);
                    }
                }
            }
        }

        public void place_houses(List<float> house_x, List<float> house_y)
        {
            for (int i = 0; i < house_x.Count; i++)
            {
                this.house_copy = choose_house(i);
                House house = new House(
                    GetWorldPosition((int)house_x[i], (int)house_y[i]),
                    this.house_copy,
                    this.houses[i],
                    this.maxEUI
                );
                house.house_type = house_type;
                GameObject t = house.create_house();
                t.SetActive(true);
                t.transform.SetParent(parent);
                //t.tag = house_type;
            }
        }

        private float choose_house_dimension_width(int i)
        {
            this.counter = 0;
            for (int j = 0; j < 2; j++)
            {
                if (i >= this.number_houses_cum_sum[this.counter] && this.counter < 2)
                {
                    this.counter++;
                }
            }

            return this.house_width[counter];
        }

        private float choose_house_dimension_height(int i)
        {
            this.counter = 0;
            for (int j = 0; j < 2; j++)
            {
                if (i >= this.number_houses_cum_sum[this.counter] && this.counter < 2)
                {
                    this.counter++;
                }
            }
            return this.house_height[counter];
        }

        private GameObject choose_house(int i)
        {
            this.counter = 0;
            for (int j = 0; j < 2; j++)
            {
                if (i >= this.number_houses_cum_sum[this.counter] && this.counter < 2)
                {
                    this.counter++;
                }
            }

            var x = (int)Math.Sqrt((double)this.houses_area[counter]);
            this.cube.transform.localScale = new Vector3(x*2, x*2, x*2);

            house_type = house_types[counter];
            return this.cube;
        }

        private void destroy_gameobjects()
        {
            GameObject[] testObjects = GameObject.FindGameObjectsWithTag(this.house_type);

            foreach (GameObject obj in testObjects)
            {
                Destroy(obj);
            }
        }

        private Vector3 GetWorldPosition(int x, int y, int z = 0)
        {
            var house_width = this.house_copy.GetComponent<Renderer>().bounds.size.x;
            var house_height = this.house_copy.GetComponent<Renderer>().bounds.size.y;
            var size = this.house_copy.GetComponent<Renderer>().bounds.size.z;
            return new Vector3(
                    x + this.shift_x + house_width / 2,
                    size / 2 - z,
                    y + this.shift_y + house_height / 2
                ) * cellSize;
        }

        private void fill_array(int[,] array)
        {
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if (j < 2)
                    {
                        this.gridArray[i, j] = 1;
                    }
                    else
                    {
                        this.gridArray[i, j] = 0;
                    }
                }
            }
        }
    }
}
