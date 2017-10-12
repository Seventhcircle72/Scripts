using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour {

	public Node currNode;
	public List<Node> nodes;

	private Node homeNode;

	public NodeGrid(Node home)
	{
		homeNode = home;
		currNode = home;
	}

	public void InsertUp(Node nodeToInsert)
	{
		// Takes a node to insert to the North of currNode
		
	}
    
    
}
