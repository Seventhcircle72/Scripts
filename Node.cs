using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
	Mob, Boss, Treasure, 
}

public class Node : MonoBehaviour {

	public int xPosition, yPosition;                    // Position relative to node grid
	public int nodeType = (int) NodeType.Mob;			// The type of node this is. Defaults to mob node.
	public bool active = false;                         // An indicator if this node is a home node
	public bool isHome = false;
	public List<int> doorSpace;                         // Indicator of door capacity

	private int[,] currGrid { get; set; }               // Tile-grid that is loaded into the node.
	private Room loadedRoom { get; set; }				// Room loaded into tile-grid.

	// Simple constructor which does not activate the node
	public Node()
	{
		currGrid = null;
		loadedRoom = null;
		InitializeGridVariables();
	}

	// Constrctor which activates the node
	public Node(int[,] grid, Room room, int xPos, int yPos)
	{
		currGrid = grid;
		active = true;
		loadedRoom = room;
		xPosition = xPos;
		yPosition = yPos;
		InitializeGridVariables();
	}

	public bool IsActive()
	{
		// Returns whether or not this node is active
		return active;
	}

	public void SetGrid(int[,] grid)
	{
		// Sets the grid of this node...
		// Also, sets this node active.
		currGrid = grid;
		active = true;
	}

	public void SetRoom(Room room)
	{
		loadedRoom = room;
	}

	public void SetCoordinates(int xLocal, int yLocal)
	{
		// Sets both coordinates, x & y.
		xPosition = xLocal;
		yPosition = yLocal;
	}

	public void SetBoss()
	{
		nodeType = (int)NodeType.Boss;
	}

	public void SetHome()
	{
		isHome = true;
	}
	public void RemoveHome()
	{
		isHome = false;
	}

	public int GetXCoordinate()
	{
		return xPosition;
	}

	public int GetYCoordinate()
	{
		return yPosition;
	}

	public Room GetRoom()
	{
		return loadedRoom;
	}

	public bool IsHome()
	{
		// Returns if this is a home node.
		return (isHome);
	}

	public int[,] RetrieveTileGrid()
	{
		return currGrid;
	}

	public void InitializeGridVariables()
	{
		// Initializes door capacity list for use.
		doorSpace = new List<int>();
		doorSpace.Add((int)Direction.North);
		doorSpace.Add((int)Direction.East);
		doorSpace.Add((int)Direction.South);
		doorSpace.Add((int)Direction.West);
	}

	public int chooseDirection()
	{
		// Chooses a random direction from the Direction enum
		// North = 0, East = 1, West = 2, South = 3
		IntRange random = new IntRange(0, 3);
		return random.Random;
	}
}
