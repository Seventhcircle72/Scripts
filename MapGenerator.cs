using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public int minCells = 30;			// Arbitrary maximum number of cells to create
	public int maxCells = 60;           // Arbitrary minimum number of cells to create
	public int mapSize = 100;           // Arbitrary map size

	public GameObject oceanTile;
	public GameObject landTile;

	private GameObject boardContainer;	// A container to store all of the rendered tiles
	private IntRange numberOfCells;     // Random selection of how many cells to generate
	private MapNode[,] mapGrid;             // 2D grid which contains all of the nodes
	private List<MapNode> landNodes;			// A List of all non-ocean map nodes

	// Use this for initialization
	void Start () {
		CreateLoadedGrid();
		GenerateMap();
		RenderMapGrid();
	}

	public void RenderMapGrid()
	{
		// Takes a 2d array and displays it depending on the tile type at each element.
		GameObject tile;
		boardContainer = new GameObject("Map-Grid Container");
		for (int i = 0; i < mapSize; i++)
		{
			for (int j = 0; j < mapSize; j++)
			{
				if (mapGrid[i, j].GetMapType() == (int)MapNodeType.Ocean)
				{
					tile = Instantiate(oceanTile, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
					tile.transform.parent = boardContainer.transform;
				}
				else if (mapGrid[i, j].GetMapType() == (int)MapNodeType.Grassland)
				{
					tile = Instantiate(landTile, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
					tile.transform.parent = boardContainer.transform;
				}
			}
		}
	}

	public void CreateLoadedGrid()
	{
		// Loads or Refreshes the map grid in this object
		mapGrid = new MapNode[mapSize, mapSize];
		for (int i = 0; i < mapSize; i++)
		{
			for (int j = 0; j < mapSize; j++)
			{
				mapGrid[i, j] = new MapNode((int)MapNodeType.Ocean, i, j);		// Set to Ocean 'default' node	
			}
		}
	}
	
	public void GenerateMap()
	{
		// Generates an island in the map grid by applying cellular automata
		numberOfCells = new IntRange(minCells, maxCells);
		int cellCap = numberOfCells.Random;
		MapNode current = mapGrid[mapSize / 2, mapSize / 2];
		current.SetMapType((int)MapNodeType.Grassland);		// First node
		int iteration = 1;
		while (iteration < cellCap)
		{
			iteration++;
		}
	}

	public void ApplyRules(MapNode currNode)
	{
		// Get nodes in all directions from this node
		int nextX = currNode.GetXPosition();
		int nextY = currNode.GetYPosition();
		// Check if the nodes should be activated
	}
	
}
