using RectpackSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityRender
{
    public class Packing : MonoBehaviour
    {
        PackingRectangle[] small_rectangles = null;
        PackingRectangle[] middle_rectangles = null;
        PackingRectangle[] large_rectangles = null;
        public PackingRectangle[] rectangles = null;
        public PackingRectangle bounds = new PackingRectangle();
        int number_of_houses_in_row = 0;


        public Packing(PackingRectangle[] small_rectangles, PackingRectangle[] middle_rectangles, PackingRectangle[] large_rectangles)
        {
            this.small_rectangles = small_rectangles;
            this.middle_rectangles = middle_rectangles;
            this.large_rectangles = large_rectangles;
            this.bounds.Width = 0;
            this.bounds.Height = 0;

            int most_houses = 0;
            uint width = 0;
            if (small_rectangles.Length > most_houses)
            {
                most_houses = small_rectangles.Length;
            }
            if (middle_rectangles.Length > most_houses)
            {
                most_houses = middle_rectangles.Length;
            }
            if (large_rectangles.Length > most_houses)
            {
                most_houses = large_rectangles.Length;
            }

            this.rectangles = new PackingRectangle[small_rectangles.Length+ middle_rectangles.Length+ large_rectangles.Length];
            small_rectangles.CopyTo(rectangles, 0); middle_rectangles.CopyTo(rectangles, small_rectangles.Length); large_rectangles.CopyTo(rectangles, small_rectangles.Length + middle_rectangles.Length);

            this.number_of_houses_in_row = Mathf.RoundToInt(Mathf.Sqrt((float) most_houses));
            if(small_rectangles.Length > 0 && small_rectangles[0].Width* number_of_houses_in_row > width)
            {
                width = (uint)number_of_houses_in_row * small_rectangles[0].Width;
            }
            if (middle_rectangles.Length > 0 && middle_rectangles[0].Width * number_of_houses_in_row > width)
            {
                width = (uint)number_of_houses_in_row * middle_rectangles[0].Width;
            }
            if (large_rectangles.Length > 0 && large_rectangles[0].Width * number_of_houses_in_row > width)
            {
                width = (uint)number_of_houses_in_row * large_rectangles[0].Width;
            }
            bounds.Height = width;
        }

        public void pack()
        {
            if (this.number_of_houses_in_row > 0)
            {
                int count = 0;
                int y = 0;
                int offset = 0;
                for (int i = 0; i < small_rectangles.Length; ++i)
                {
                    rectangles[count].X = small_rectangles[i].Width* (uint)(i - offset);
                    rectangles[count].Y = (uint)y;
                    count++;
                    if (count % number_of_houses_in_row == 0) { y += (int)small_rectangles[i].Width;offset += number_of_houses_in_row; }
                }
                if((middle_rectangles.Length == 0 && large_rectangles.Length == 0) && small_rectangles.Length > 0 && count % number_of_houses_in_row == 0)
                {
                    y -= (int)small_rectangles[0].Width;
                }
                else if(small_rectangles.Length > 0 && count % number_of_houses_in_row != 0)
                {
                    y += (int)small_rectangles[0].Width;
                }

                offset = 0;
                for (int i = 0; i < middle_rectangles.Length; ++i)
                {
                    rectangles[count].X = middle_rectangles[i].Width * (uint)(i - offset);
                    rectangles[count].Y = (uint)y;
                    count++;
                    if (count % number_of_houses_in_row == 0) { y += (int)middle_rectangles[i].Width; offset += number_of_houses_in_row; }
                }
                if (large_rectangles.Length == 0 && middle_rectangles.Length > 0 && count % number_of_houses_in_row == 0)
                {
                    y -= (int)middle_rectangles[0].Width;
                }
                else if (middle_rectangles.Length > 0 && count % number_of_houses_in_row != 0)
                {
                    y += (int)middle_rectangles[0].Width;
                }

                offset = 0;
                for (int i = 0; i < large_rectangles.Length; ++i)
                {
                    Debug.Log("i: " + i + " offset: "+ offset);
                    rectangles[count].X = large_rectangles[i].Width * (uint)(i - offset);
                    rectangles[count].Y = (uint)y;
                    count++;
                    Debug.Log("count: " + count + " number_of_houses_in_row " + number_of_houses_in_row);
                    if (count % number_of_houses_in_row  == 0) { y += (int)large_rectangles[i].Width; offset += number_of_houses_in_row; }
                }
                if (large_rectangles.Length > 0 && count % number_of_houses_in_row == 0)
                {
                    y -= (int)large_rectangles[0].Width;
                }
                bounds.Width = (uint)y + rectangles[count-1].Width;
            }
        }
    }
}

