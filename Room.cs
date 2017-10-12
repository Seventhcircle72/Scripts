using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : Cell {

    public int difficultyModifier;          // How difficult the room will be. IE: How many enemies to spawn/Traps
    public int type {get; set;}             // Whether it is a room, lroom, bossroom, etc...
    public bool safeZone = false;           // Whether enemies/traps will spawn in it
    public List<int> doorSpace;             // Available spaces for a door

    public Room(int xStart, int yStart, int cellWidth, int cellHeight) : base(xStart, yStart, cellWidth, cellHeight)
    {
        doorSpace = new List<int>();
        doorSpace.Add((int)Direction.North);
        doorSpace.Add((int)Direction.East);
        doorSpace.Add((int)Direction.South);
        doorSpace.Add((int)Direction.West);
    }

}
