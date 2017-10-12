using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapNodeType
{
	Ocean, Grassland, Desert, Tundra, Swamp, Mountain, Beach,
}

public class MapNode : MonoBehaviour {

	public int type = (int)MapNodeType.Ocean;
	public int xPos, yPos;

	private bool passable = true;

	public MapNode(int mapType, int xCoord, int yCoord)
	{
		type = mapType;
		xPos = xCoord;
		yPos = yCoord;
	}

	public int GetMapType()
	{
		return type;
	}

	public void SetMapType(int typeToSet)
	{
		type = typeToSet;
	}

	public bool isPassable()
	{
		return passable;
	}

	public void TogglePassable()
	{
		passable = !passable;
	}

	public int GetXPosition()
	{
		return xPos;
	}

	public int GetYPosition()
	{
		return yPos;
	}
}
