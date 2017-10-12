using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction                       // Enum representing directions.
{
    North, East, West, South,
}

public class Cell : MonoBehaviour {

    

    public int xStartPoint { get; set; }    // Cell grid starting point on the x-axis
    public int yStartPoint { get; set; }    // Cell grid starting point on the y-axis
    public int xEndPoint { get; set; }      // Cell grid ending point on the x-axis
    public int yEndPoint { get; set; }      // Cell grid ending point on the y-axis
    public int width, height;               // Cell dimensions in terms of columns and rows

    // Constructor
    public Cell(int xStart, int yStart, int cellWidth, int cellHeight)
    {
        this.xStartPoint = xStart;
        this.yStartPoint = yStart;
        this.width = cellWidth;
        this.height = cellHeight;
        setEndPointsFromDimensions();
    }

    public void setEndPointsFromDimensions()
    {
        // Store x,y start points and loop through width and height.
        int xCurr = xStartPoint;
        int yCurr = yStartPoint;
        int i = 1;
        int j = 1;
        while (i < width)
        {
            while(j < height)
            {
                yCurr++;
                j++;
            }
            xCurr++;
            i++;
        }
        // Store last x,y entry as end points.
        this.xEndPoint = xCurr;
        this.yEndPoint = yCurr;
    }

    public int[] ChooseWall(int direction)
    {
        // Based on direction choose a random wall tile (Within boundary - 2)
        IntRange random;
        int xMin, xMax, yMin, yMax;
        // Returning value is a list of coordinates. Format is [x, y].
        int[] coords = new int[2];
        // Depending on the direction, select a tile from non-corner range.
        switch(direction)
        {
            case (int)Direction.North:              // Select from top wall
                // Make the ranges to be boundary - corners (Because we can't put a door on a corner!).
                xMin = xStartPoint + 1;
                xMax = xEndPoint - 1;
                // Select random x, and y-startpoint as our coordinates. 
                random = new IntRange(xMin, xMax);
                coords[0] = random.Random;
                coords[1] = yStartPoint;
                break;
            case (int)Direction.East:               // Select from right wall
                // ... Similarly,
                yMin = yEndPoint + 1;
                yMax = yStartPoint - 1;
                random = new IntRange(yMin, yMax);
                coords[0] = xEndPoint;
                coords[1] = random.Random;
                break;
            case (int)Direction.South:              // Select from bottom wall
                xMin = xStartPoint + 1;
                xMax = xEndPoint - 1;
                random = new IntRange(xMin, xMax);
                coords[0] = random.Random;
                coords[1] = yEndPoint;
                break;
            case (int)Direction.West:               // Select from left wall
                yMin = yEndPoint + 1;
                yMax = yStartPoint - 1;
                random = new IntRange(yMin, yMax);
                coords[0] = xStartPoint;
                coords[1] = random.Random;
                break;
        }
        
        return coords;
    }
}
