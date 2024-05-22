using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid: MonoBehaviour
{
    private int width;
    private int height;
    private int[,] gridArray;
    private float cellSize;
    private int shift_x;
    private int shift_y;
    private int house_width;
    private int house_height;
    private List<GameObject> folderObjects;
    private GameObject house;

    public Grid(int width, int height, float cellSize, int shift_x, int shift_y, List<GameObject> folderObjects)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.shift_x = shift_x;
        this.shift_y = shift_y;
        this.folderObjects = folderObjects;

        this.house = choose_house();
        this.house_width = (int)(Mathf.Round(house.GetComponent<Renderer>().bounds.size.x));
        this.house_height = (int)(Mathf.Round(house.GetComponent<Renderer>().bounds.size.y));

        this.gridArray = new int[width, height];
        update(width, height, shift_x, shift_y,0,0);

    }
    private bool space_available(int x, int y, int width, int height)
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                //Debug.Log("width: " + this.gridArray.GetLength(0) + " height: " this.gridArray.GetLength(1) +" "+ (y + j));
                if (this.gridArray.GetLength(0) > x+i && this.gridArray.GetLength(1) > y+i)
                {
                    if (this.gridArray[x + i, y + j] == 1)
                    {
                        return false;
                    }
                }else
                {
                    return false;
                }
                
            }
        }
        return true;
    }

    private GameObject choose_house()
    {
        int randomIndex = Random.Range(0, folderObjects.Count);
        return folderObjects[randomIndex];
    }

    public void update(int width, int height, int shift_x, int shift_y, float a, float b)
    {
        
        this.shift_x = shift_x;
        this.shift_y = shift_y;
        this.gridArray = new int[width, height];

        this.house_width = (int)(Mathf.Round(this.house.GetComponent<Renderer>().bounds.size.x));
        this.house_height = (int)(Mathf.Round(this.house.GetComponent<Renderer>().bounds.size.y));

        var x = 0;
        var y = 0;

        //North
        y = 0;
        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            x = i;
            if (space_available(x, y, house_width, house_height))
            {
                place_random(x, y);
            }
        }

        //East
        
        x = gridArray.GetLength(0)- house_width;
        for (int i = 0; i < gridArray.GetLength(1); i++)
        {
            y = i;
            if (space_available(x, y, house_width, house_height))
            {
                place_random(x, y);
            }
        }
        
        //South
        y = gridArray.GetLength(1)- house_height;
        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            x = i;
            if (space_available(x, y, house_width, house_height))
            {
                place_random(x, y);
            }
        }
        
        //West
        /*
        x = 0;
        for (int i = 0; i < gridArray.GetLength(1); i++)
        {
            y = i;
            if (space_available(x, y, house_width, house_height))
            {
                place_random(x, y);
            }
        }*/

    }

    private void place_random(int x, int y)
    {
        
        Instantiate(this.house, GetWorldPosition((int)(x+ this.house_width/2), (int)(y + this.house_height / 2)), Quaternion.identity);
        var space = 2;
        var x_border = (int)(Mathf.Max(0, x - space));
        var y_border = (int)(Mathf.Max(0, y - space));

        Debug.Log("place: " + " x: " + x + " y: " + y + " max: " + (int)(Mathf.Min(gridArray.GetLength(0) - this.house_width, x + this.house_width + space)));
        for (int i= x_border; i< (int)(Mathf.Min(gridArray.GetLength(0) - this.house_width+1, x + this.house_width + space)); i++)
        {
            for (int j = y_border; j < (int)(Mathf.Min(gridArray.GetLength(1) - this.house_height+1, y + this.house_height + space)); j++)
            {
                Debug.Log("x: " + x + " y: " + y + " j: " + j + "  "+ Mathf.Min(gridArray.GetLength(1) - this.house_height+1) + "  " + (y + this.house_height + space));
                this.gridArray[i, j] = 1;
            }
        }
    }



    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x- shift_x,0, y-shift_y) * cellSize;
    }
}
