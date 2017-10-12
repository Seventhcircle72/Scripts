using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// What I need:
// A generator that generates interesting simple polygons on a 2d cartesian grid.
// With doors linking adjacent rooms, some being locked (Door & key, elementally locked, etc...)
// Difficulty of each room is given by difficulty modifier as well as size of room as compared to average.
// (In other words, have a list of rooms, and if selected room size is greater than average room size, then difficulty increases)
// Populate with Traps, Treasure, Breakables, Unbreakables, Quest (Items and Events).
// Spawn enemies and tag difficult rooms to spawn Champions(Mini-bosses)
// On each floor, add a boss room and a boss to fight with 10% chance to spawn 'Ancient' boss.
// Have a simple mini-map that shows location in NodeGrid
// 

// Task list:
// 1. Make tilegrid [x]
// 2. Spawn room in grid [x]
// 3. Load grid on to node-grid [x]
// 4. Randomly generate dungeon layout using loaded nodes [], Bonus: Add keyboard traversal []
// 5. If currNode has nextNode, add a door to the same Direction for all nodes []
// 6. Make spawn-point in center of starting room []
// 7. Make spawn-point relative when traversing nodes [] (IE: If player walks East into door, spawn point should be on West door)

public enum Tiletype                // Represents the tile type of current grid position.
{
    Perimeter, Door, Corridor, Floor, Boundary,
}

public enum Celltype                // Represents what kind of cell room to build.
{
    Room, LRoom, CrossRoom, Hallway, OctagonalRoom, 
}

public class RoomGenerator : MonoBehaviour {

    public int roomTarget;                          // How many rooms to make.
    [Range(20, 100)]
    public int gridSize = 20;                       // The x-length and y-width of the a grid.
    public int minRoomSize = 3, maxRoomSize = 10;   // Minimum and maximum sizes a room can be.
    public int failThreshold = 5;
	public int failures = 0;
	public GameObject minimapContainer;
	public GameObject boardContainer;
    public GameObject boundaryTile;                 // Tile set for boundary in grid.
    public GameObject floorTile;                    // Tile set for floors in grid.
	public GameObject homeTile;

    private IntRange roomSize;              // Int range of sizes.
    private int averageSize = 0;            // Average size of all rooms.
    private Node currNode;              // The current grid node that is being rendered.
    private List<Node> nodes;
	private Node[,] nodeGrid;
	private Dictionary<int[], Node> nodeTable;
	private int localX = 0, localY = 0;

	void Start ()
	{
		GenerateNodeGrid();
		RenderGridMaps(currNode);
	}

	private void Update()
	{

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			if (CheckForNode((int)Direction.West))
			{
				if (localX > 0)
				{
					MoveCurrNode((int)Direction.West);
				} else
				{
					print("No node found that way...");
				}
			}
		} else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			if (CheckForNode((int)Direction.East))
			{
				if (localX > 0)
				{
					MoveCurrNode((int)Direction.East);
				}
				else
				{
					print("No node found that way...");
				}
			}
		} else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			if (CheckForNode((int)Direction.North))
			{
				if (localY > 0)
				{
					MoveCurrNode((int)Direction.North);
				}
				else
				{
					print("No node found that way...");
				}
			}
		} else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if (CheckForNode((int)Direction.South))
			{
				if (localY > 0)
				{
					MoveCurrNode((int)Direction.South);
				}
				else
				{
					print("No node found that way...");
				}
			}
		}
		RenderGridMaps(currNode);
	}

	public int[,] MakeGrid()
    {
        // Returns an instantiated 2d array representing a tile grid
        int[,] tileGrid = new int[gridSize, gridSize];
        tileGrid = InitializeGrid(tileGrid);
        return tileGrid;
    }

    public int[,] InitializeGrid(int[,] grid)
    {
        // Initializes and returns a 2d array setting all elements to a nullified tile
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize - 1; j++)
            {
                grid[i, j] = (int)Tiletype.Boundary;
            }
        }

        return grid;
    }

    public void RenderGrid(int[,] grid)
    {
        // Takes a 2d array and displays it depending on the tile type at each element.
        GameObject tile;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                switch(grid[i, j])
                {
                    case ((int) Tiletype.Boundary):
                        tile = Instantiate(boundaryTile, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
						tile.transform.parent = boardContainer.transform;
						break;
                    case ((int)Tiletype.Floor):
                        tile = Instantiate(floorTile, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
						tile.transform.parent = boardContainer.transform;
						break;

                }
            }
        }
    }

	public void RenderGridMaps(Node node)
	{
		GameObject.Destroy(boardContainer);
		boardContainer = new GameObject("Board Container");
		int[,] grid = node.RetrieveTileGrid();
		RenderGrid(grid);
		GameObject.Destroy(minimapContainer);
		minimapContainer = new GameObject("Minimap Container");
		RenderNodeGrid();
		minimapContainer.transform.position = new Vector3(gridSize + 1, 7, 0);
	}

	public void RenderNodeGrid()
	{
		GameObject tile;
		for (int i = 0; i < roomTarget; i++)
		{
			for (int j = 0; j < roomTarget; j++)
			{
				if (nodeGrid[i, j].IsActive() == true)
				{
					if (nodeGrid[i, j].IsHome())
					{
						tile = Instantiate(homeTile, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
						tile.transform.parent = minimapContainer.transform;
					}
					else
					{
						tile = Instantiate(floorTile, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
						tile.transform.parent = minimapContainer.transform;
					}
				} 
				else
				{
					tile = Instantiate(boundaryTile, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
					tile.transform.parent = minimapContainer.transform;
				}
			}
		}
	}
	public void MoveCurrNode(int direction) 
	{
		Node prevNode = currNode;
		if (direction == (int) Direction.East)
		{
			localX++;
			currNode = nodeGrid[localX, localY];
		}
		else if (direction == (int)Direction.North)
		{
			localY++;
			currNode = nodeGrid[localX, localY];
		}
		else if (direction == (int)Direction.West)
		{
			localX--;
			currNode = nodeGrid[localX, localY];
		}
		else if (direction == (int)Direction.South)
		{
			localY--;
			currNode = nodeGrid[localX, localY];
		}
		prevNode.RemoveHome();
		currNode.SetHome();
	}

    public Room GenerateRoom(int[,] grid)
    {
        // Takes a 2d array and adds some kind of rooms to it
        int cell = ChooseCellType();
        Room room = null;
        cell = 0;                                                    // DEBUG!!!!
        switch(cell)
        {
            case (int)Celltype.Room:
                room = MakeRoom(grid);
                break;
            case (int)Celltype.LRoom:
                
                break;
        }
        return room;
    }

    public Room MakeRoom(int[,] grid)
    {
        // Mutates 2d array grid to add a room. Returns the room.
        Room room = GenerateRandomRoomSpecifics();
        int yPos = room.yStartPoint; int xPos = room.xStartPoint;
        for (int i = 0; i < room.width; i++)
        {
            for (int j = 0; j < room.height; j++)
            {
                grid[xPos, yPos] = (int)Tiletype.Floor;
                yPos++;
            }
            yPos = room.yStartPoint;
            xPos++;
        }
        return room;
    }

	public Room MakeBossRoom(int[,] grid)
	{
		return null; 
	}

    public Room GenerateRandomRoomSpecifics()
    {
        // Firstly, decides size of room...
        roomSize = new IntRange(minRoomSize, maxRoomSize);
        int height = roomSize.Random;
        int width = roomSize.Random;
        // Secondly, centers room with respect to grid...
        int xStart = ((gridSize / 2) - (height / 2));
        int yStart = ((gridSize / 2) - (width / 2));
        Room room = new Room(xStart, yStart, width, height);
        // Thirdly, returns room.
        room.type = (int)Celltype.Room;
        return room;
    }

    public int ChooseCellType()
    {
        // Selects a room type at random.
        IntRange random = new IntRange(0, 5);
        int result = random.Random;
        return result;
    }

	public Node ChooseNode()
	{
		// Selects a node from the node list at random.
		IntRange random = new IntRange(0, nodes.Count);
		int result = random.Random;
		return nodes[result];
	}

	public int chooseDirection()
	{
		// Chooses a random direction from the Direction enum
		// North = 0, East = 1, West = 2, South = 3
		IntRange random = new IntRange(0, 4);
		return random.Random;
	}

	public Node[,] LoadNewNodeGrid()
	{
		// Creates and initializes every element of a 2d node array.
		Node[,] grid = new Node[roomTarget, roomTarget];
		for (int i = 0; i < roomTarget; i++)
		{
			for (int j = 0; j < roomTarget; j ++)
			{
				grid[i, j] = new Node();
			}
		}
		return grid;
	}

	public void GenerateNodeGrid()
	{
		// Generates a dungeon layout
		// Instantiate necessary variables
		// Loop until reached room target or fail out
		// Choose center of grid for start point
		// Choose node from node list
		// Choose direction
		// Check if there is a node there
		// If there is, then 
		nodeGrid = LoadNewNodeGrid();
		localX = roomTarget / 2;
		localY = localX;
		nodes = new List<Node>();
		Node nextNode;
		Node chosenNode;
		currNode = GenerateNode();
		PutNode(currNode, localX, localY);
		currNode.SetHome();
		int target = 1;
		int failures = 0;
		int direction = 0;
		int nextX = localX, nextY = localY;
		while ((target < roomTarget) && (failures < failThreshold))
		{
			// While we don't have enough rooms and we haven't failed out
			// Choose a node
			chosenNode = ChooseNode();
			nextX = chosenNode.GetXCoordinate();
			nextY = chosenNode.GetYCoordinate();
			// Choose a direction
			direction = chooseDirection();
			// If there is a node there then increment failures
			nextNode = GenerateNode();
			nextX = IncrementX(direction, nextX);
			nextY = IncrementY(direction, nextY);
			if (HasNode(nextX, nextY))
			{
				failures++;
			}
			else
			{
				print(nextX + ", " + nextY);
				PutNode(nextNode, nextX, nextY);
				target++;
			}
			// Otherwise, add a node
			
		}
		//AddBossNode();
	}

	public void AddBossNode()
	{
		// Adds a boss node to the current dungeon layout.
		Node chosenNode;
		Node nextNode;
		int nextX, nextY;
		bool putBoss = true; //DEBUG
		int direction = 0;
		// Until we place a boss node...
		while (putBoss == false)
		{
			// Check for breach of fail threshold 
			if (failures < failThreshold + 3)
			{
				chosenNode = ChooseNode();
				nextX = chosenNode.GetXCoordinate();
				nextY = chosenNode.GetYCoordinate();
				// Choose a direction
				direction = chooseDirection();
				nextNode = GenerateBossNode();
				nextX = IncrementX(direction, nextX);
				nextY = IncrementY(direction, nextY);
			}
		}
	}

	public Node GenerateBossNode()
	{
		return null;
	}

	public bool InboundTileGrid(int target)
	{
		bool result = false;
		if (target < gridSize)
		{
			result = true;
		}
		return result;
	}

	public bool InboundNodeGrid(int target)
	{
		bool result = false;
		if (target < roomTarget)
		{
			result = true;
		}
		return result;
	}

	public bool HasNode(int currX, int currY)
	{
		// Checks if there exists a node in given x-y coordinates.
		// Assumes nodeGrid is instantiated.
		// Check if given coordinates are within bounds.
		bool has = false;
		if (((currX < roomTarget)&&(currX >= 0))&&((currY < roomTarget)&&(currY >= 0))) 
		{
			if (nodeGrid[currX, currY].IsActive() == true)
			{
				has = true;
			}
		}
		return has;
	}

	public bool CheckForNode(int direction)
	{
		// Takes the current node and checks for neighbouring nodes based on int direction.
		int nodeX = currNode.GetXCoordinate();
		int nodeY = currNode.GetYCoordinate();
		bool nodeExists = false;
		if (direction == (int) Direction.North)
		{
			nodeExists = HasNode(nodeX, nodeY + 1);
		}
		if (direction == (int)Direction.East)
		{
			nodeExists = HasNode(nodeX + 1, nodeY);
		}
		if (direction == (int)Direction.South)
		{
			nodeExists = HasNode(nodeX, nodeY - 1);
		}
		if (direction == (int)Direction.West)
		{
			nodeExists = HasNode(nodeX - 1, nodeY);
		}
		return nodeExists;
	}

	public int IncrementY(int direction, int currY)
	{
		// Increments the y-coordinate based on incoming direction
		if (direction == (int)Direction.North)
		{
			currY++;
			if (currY >= gridSize)
			{
				currY = gridSize - 1;
			}
		}
		else if (direction == (int)Direction.South)
		{
			currY--;
			if (currY < 0)
			{
				currY = 0;
			}
		}
		return currY;
	}

	public int IncrementX(int direction, int currX)
	{
		// Increments the x-coordinate based on incoming direction
		if (direction == (int)Direction.East)
		{
			currX++;
			if (currX >= gridSize)
			{
				currX = gridSize - 1;
			}
		}
		else if (direction == (int)Direction.West)
		{
			currX--;
			if (currX < 0)
			{
				currX = 0;
			}
		}
		return currX;
	}

	public void PutNode(Node node, int xLocal, int yLocal)
	{
		// Places node on grid and adds it to node list
		if ((InboundNodeGrid(xLocal)) && (InboundNodeGrid(yLocal)))
		{
			nodeGrid[xLocal, yLocal] = node;
			node.SetCoordinates(xLocal, yLocal);
			nodes.Add(node);
		}
	}

	public Node GenerateNode()
	{
		// Make a node object and load it with a room
		Node node = new Node();
		int[,] newGrid = MakeGrid();
		Room room = GenerateRoom(newGrid);
		node.SetGrid(newGrid);
		node.SetRoom(room);
		return node;
	}
}
