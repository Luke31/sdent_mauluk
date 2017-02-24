using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour {
	string[][] readFile(string file){
		string text = System.IO.File.ReadAllText(file);
		string[] lines = Regex.Split(text, "\r\n");
		int rows = lines.Length;

		string[][] levelBase = new string[rows][];
		for (int i = 0; i < lines.Length; i++)  {
			string[] stringsOfLine = Regex.Split(lines[i], " ");
			levelBase[i] = stringsOfLine;
		}
		return levelBase;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
