using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {

    public enum Tiletype
    {
        Room, Cell, Outer, Wall, Roof, Corner,
    }

    public int cellTarget = 0;         // Number of cells/rooms to generate
    public int roomTarget = 3;
    public int rows = 100;              // Number of rows in the grid
    public int columns = 100;           // Number of columns in the grid
    public int cellSizeMin = 4, cellSizeMax = 10;       // Min and Max room sizes for width and height
    //public GameObject[] floors;         // Collection of floor tiles  NOTE: WE WANT THESE AS ATTRIBUTES OF CHUNK (AKA: Current player map position)
    //public GameObject[] walls;          // Collection of wall tiles   TO BE REMOVED 
    //public GameObject[] outer;          // Collection of outer tiles  TO BE REMOVED
    public GameObject roomQuad;
    public GameObject cellQuad;
    public GameObject outerQuad;
    public GameObject wallQuad;
    public GameObject roofQuad;
    public GameObject corridorQuad;
    public GameObject cornerQuad;

    private Cell[] cells;           // Array of cells (Side rooms)
    private Room[] rooms;           // Array of important rooms
    private Hallway[] paths;           // Array of import pathways
    private Corridor[] corridors;       // Array of side corridors
    private int cellCounter = 0;
    private int[,] tileGrid;             
    private GameObject container;
    private IntRange cellSize;
    private IntRange startingPoint;

    void Start()
    {
        // 1. Create a grid and starting point near center of grid
        InitializeGrid();
        GenerateTargets();
        // 2. Add cells, rooms, pathways and corridors
        // Task 1) Make main pathway to end point
        // precond: From difficulty level and distance from home, calculate density of dungeon
        // a. Create starting room in grid
        // b. Choose wall of room
        // c. Check if there's sufficient space to add pathway
        // d. If c. is true, then add a pathway otherwise return to b.
        // e. At the end of pathway add important room
        // f. While number of important rooms doesn't meet precalculated threshold return to b.
        MakeEntry();
        ReachRoomTarget();
            // Task 2) Add siderooms and corridors
            // precond: Choose cell target (Will have one less corridor)
            // a. Choose wall of any pathway, room, cell, or corridor 
            // b. Create corridor
            // c. At end of corridor, choose and create cell improvement
            // d. Return to a. until cell target reached
        // 3. Populate with traps, loot, events, enemies.
        RenderGrid();
    }

    public void ReachRoomTarget()
    {
        // Loop until we have sufficient rooms,
        // first choose wall of any room,
        // then check space for and add a hallway
        while (cellCounter < roomTarget)
        {
            // Select room from room list
            // Select wall of room
            // Check for space
            // Add hallway
            int sampleSpace = rooms.Length;
            int chosenIndex = 0;
            IntRange rand;
            if (sampleSpace > 1)
            {
                rand = new IntRange(0, sampleSpace - 1);
                chosenIndex = rand.Random;
                
            }
            Room chosenRoom = rooms[chosenIndex];
            // Select wall of room
            rand = new IntRange(0, 3);
            chosenIndex = rand.Random;
            // 0 = North, 1 = East, 2 = West, 3 = South
            int[] coords = chosenRoom.ChooseWall(chosenIndex);
        }
    }

    public bool CheckSpace(int width, int height, int[] startingCoords)
    {
        // Check if parameters are within boundary of grid
        // Also check if there is enough space to add given component

        return false; 
    }

    public void InitializeGrid()
    {
        // Initialize grid array 
        tileGrid = new int[columns, rows];
        // ...and loop through to fill background 
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows - 1; j++)
            {
                tileGrid[i, j] = (int) Tiletype.Outer;
            }
        }
        // Instantiate container and array to store cells
        container = new GameObject("Container");
    }

    public void GenerateTargets()
    {
        // Generate targets based on difficulty modifier
        // Use targets to create lists for storing of rectangles
        cells = new Cell[cellTarget];
        rooms = new Room[roomTarget];
        int hallTarget = 0;
        if (roomTarget - 1 > 0)
        {
            hallTarget = roomTarget - 1;
        }
        paths = new Hallway[hallTarget];
        int corrTarget = 0;
        if (cellTarget - 1 > 0)
        {
            corrTarget = cellTarget - 1;
        }
        corridors = new Corridor[corrTarget];
    }

    public void RenderGrid()
    { 
        // Iterate through grid and at each sequence check for weight
        // Null/0 = "Black" for Null areas, 1 = "Blue" for Cells, 2 = "Red" for Rooms
        for (int i = 0; i < columns - 1; i++)
        {
            for (int j = 0; j < rows - 1; j++)
            {
                if (tileGrid[i, j] == (int)Tiletype.Cell)
                {
                    GameObject tile = Instantiate(cellQuad, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                    tile.transform.parent = container.transform;
                } else if (tileGrid[i, j] == (int) Tiletype.Outer)
                {
                    GameObject tile = Instantiate(outerQuad, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                    tile.transform.parent = container.transform;
                } else if (tileGrid[i, j] == (int) Tiletype.Room)
                {
                    GameObject tile = Instantiate(roomQuad, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                    tile.transform.parent = container.transform;
                } else if (tileGrid[i, j] == (int)Tiletype.Wall)
                {
                    GameObject tile = Instantiate(wallQuad, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                    tile.transform.parent = container.transform;
                }
            }
        }
        print("Rendered " + cells.Length + " Cells");           // DEBUG *****************************
    }

    public void MakeEntry()
    {
        // Choose entry size room
        cellSize = new IntRange(cellSizeMax - 3, cellSizeMax);
        int roomSize = cellSize.Random;
        // Choose center of grid
        int xGridHalf = (columns / 2); int yGridHalf = (rows / 2);
        int xStartPoint = xGridHalf;
        int yStartPoint = yGridHalf;
        // Check if there's space in grid
        if (xGridHalf > (roomSize / 2))
        {
            xStartPoint = (xGridHalf - (roomSize / 2));
        }
        if (yGridHalf > (roomSize / 2))
        {
            yStartPoint = (yGridHalf - (roomSize / 2));
        }
        // Make cell object and add to important list
        CreateRoom(xStartPoint, yStartPoint, roomSize, roomSize);
    }

    public void CreateRoom(int xStart, int yStart, int roomWidth, int roomLength)
    {
        // Create Cell object 
        Room room = new Room(xStart, yStart, roomWidth, roomLength);
        // Fill grid with cell 
        int yPos = yStart; int xPos = xStart;
        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomLength; j++)
            {
                print("xPos = " + xPos + "\nyPos = " + yPos + "\ni, j = " + i + ", " + j + "/nRw, Rh = " + roomWidth + ", " + roomLength); // DEBUG *******
                tileGrid[xPos, yPos] = (int)Tiletype.Room;
                yPos++;
            }
            yPos = yStart;
            xPos++;
        }
        rooms[cellCounter] = room;
        cellCounter++;
    }

    public void SetCellPerimeter(Room room)
    {
        // Get end points
        int xPerimeterStart = room.xStartPoint - 1;
        int yPerimeterStart = room.yStartPoint - 1;
        int xPerimeterEnd = room.xEndPoint + 1;
        int yPerimeterEnd = room.yEndPoint + 1;
        // Iterate right
        int xCurr = xPerimeterStart;
        while (xCurr < xPerimeterEnd)
        {
            tileGrid[xCurr, yPerimeterStart] = (int)Tiletype.Wall;
            xCurr++;
        }
        // Iterate down
        int yCurr = yPerimeterStart;
        while (yCurr < yPerimeterEnd)
        {
            tileGrid[xCurr, yCurr] = (int)Tiletype.Wall;
            yCurr++;
        }
        // Iterate left
        while (xCurr > xPerimeterStart)
        {
            tileGrid[xCurr, yCurr] = (int)Tiletype.Wall;
            xCurr--;
        }
        // Iterate up
        while (yCurr > yPerimeterStart)
        {
            tileGrid[xPerimeterStart, yCurr] = (int)Tiletype.Wall;
            yCurr--;
        }
        // Iterate and add roof
        // Get grid point of top left corner
        // Iterate right 
    }

    public void SelectStartPoint()
    {

    } 
}
