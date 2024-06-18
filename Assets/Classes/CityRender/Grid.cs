using System;
using System.Collections;
using System.Collections.Generic;
using CityData;
using RectpackSharp;
using UnityEditor.Experimental.GraphView;
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
        private List<string> house_types = new List<string>();

        private Transform parent;
        private GameObject house_copy;
        private GameObject cube;
        private int counter = 0;
        public float[] shares;
        public int spacing = 2;

        private List<CityData.House> houses;

        public Grid(
            Transform parent,
            int width,
            int height,
            float cellSize,
            int shift_x,
            int shift_y,
            List<int> houses_area,
            List<int> number_houses,
            string house_type,
            List<CityData.House> houses
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
            this.house_types.Add(house_type + "_small");
            this.house_types.Add(house_type + "_middle");
            this.house_types.Add(house_type + "_big");
            this.parent = parent;
            this.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            this.cube.SetActive(false);

            calculate_house_dimensions();
            PackingRectangle[] rectangles = new PackingRectangle[this.number_houses_cum_sum[2]];
            for (int i = 0; i < rectangles.Length; i++)
            {
                rectangles[i].Width = (uint)choose_house_dimension_width(i);
                rectangles[i].Height = (uint)choose_house_dimension_height(i);
                rectangles[i].Id = i;
            }

            RectanglePacker.Pack(rectangles, out PackingRectangle bounds);
            house_x = new List<float>(rectangles.Length);
            house_y = new List<float>(rectangles.Length);

            for (int i = 0; i < rectangles.Length; i++)
            {
                Debug.Log(
                    "x: "
                        + rectangles[i].X
                        + " y: "
                        + rectangles[i].Y
                        + " Height: "
                        + rectangles[i].Height
                        + " Width: "
                        + rectangles[i].Width
                        + " id: "
                        + rectangles[i].Id
                );
                house_x.Add(rectangles[i].X);
                house_y.Add(rectangles[i].Y);
            }
            this.width = (int)bounds.Width;
            this.height = (int)bounds.Height;
            this.area = width * height;
            place_houses(house_x, house_y);

            this.gridArray = new int[width, height];
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
                this.house_width.Add(a);
                this.house_height.Add(a);
            }
        }

        public void drawOutlines()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.parent = parent;
                        cube.transform.position =
                            parent.transform.position
                            + new Vector3(x * cellSize + shift_x, 0, y * cellSize + shift_y);
                    }
                }
            }
        }

        /*
        public void update_grid(int width, int height, int shift_x, int shift_y)
        {
            this.width = width;
            this.height = height;
            this.shift_x = shift_x;
            this.shift_y = shift_y;
            this.gridArray = new int[width, height];
            fill_array(this.gridArray);
            place_houses();
        }
        */

        public void place_houses(List<float> house_x, List<float> house_y)
        {
            for (int i = 0; i < house_x.Count; i++)
            {
                this.house_copy = choose_house(i);
                House house = new House(
                    GetWorldPosition((int)house_x[i], (int)house_y[i]),
                    this.house_copy,
                    this.houses[i]
                );
                house.house_type = house_type;
                GameObject t = house.create_house();
                t.SetActive(true);
                t.transform.SetParent(parent);
                t.tag = house_type;
            }
        }

        private float choose_house_dimension_width(int i)
        {
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
            this.cube.transform.localScale = new Vector3(x, x, x);
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

        private Vector3 GetWorldPosition(int x, int y)
        {
            var width = this.house_copy.GetComponent<Renderer>().bounds.size.x;
            var height = this.house_copy.GetComponent<Renderer>().bounds.size.y;
            var size = this.house_copy.GetComponent<Renderer>().bounds.size.z;
            return new Vector3(
                    x + this.shift_x + width / 2,
                    size / 2,
                    y + this.shift_y + height / 2
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
