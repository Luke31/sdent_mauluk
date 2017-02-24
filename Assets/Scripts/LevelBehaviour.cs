using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
	public GameObject levelRoot;
	public Vector3 offset;
	public float tileSize;

	public Transform[] tilePreFabs;

	public void generate (int[][] tileMap)
	{
		for (int x = 0; x < tileMap.Length; x++) {
			for (int y = 0; y < tileMap [x].Length; y++) {
				if (tileMap [x] [y] >= 0) {
					Instantiate (tilePreFabs [tileMap [x] [y]],
						new Vector3 (x * tileSize + offset.x, -y * tileSize + offset.y, 0),
						Quaternion.identity);
				}
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		int[][] tileMap = new int[][] {  
			new int[]{ 1, 1, 1, 0, 1, 1, 1 },
			new int[]{ 1, -1, -1, -1, -1, -1, 1 },
			new int[]{ 1, -1, -1, -1, -1, -1, 1 },
			new int[]{ 1, -1, -1, -1, -1, -1, 1 },
			new int[]{ 1, 1, 1, 1, 1, 1, 1 }
		};

		generate (tileMap);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
