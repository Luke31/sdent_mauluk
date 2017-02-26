using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
	public GameObject levelRoot;

	public Texture2D spriteSheetTexture;
	public GameObject baseTilePrefab;
	public TextAsset tiledFile;


	public Vector3 offset;
	public float tileSize;

	private Sprite[] tileSprites;

	public void Generate (TiledMap map)
	{
		foreach(TiledLayer layer in map.Layer) {
			string[] data = Regex.Replace(layer.Data.Text, @"\r\n|\n", "").Split (',');

			for (int x = 0; x < layer.Width; x++) {
				for (int y = 0; y < layer.Height; y++) {
					int dataValue = int.Parse (data [y * layer.Width + x]) - 1;
					if (dataValue >= 0) {
						GameObject instance = Instantiate (baseTilePrefab,
							new Vector3 (x * tileSize + offset.x, -y * tileSize + offset.y, 0),
							Quaternion.identity);
						
						instance.transform.SetParent (transform);
						instance.transform.localScale *= tileSize;

						instance.GetComponentInChildren<SpriteRenderer> ().sprite = tileSprites [dataValue];
					}
				}
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		tileSprites = Resources.LoadAll<Sprite>(spriteSheetTexture.name);

		XmlSerializer serializer = new XmlSerializer (typeof(TiledMap));
		TiledMap map = (TiledMap) serializer.Deserialize (new MemoryStream (tiledFile.bytes));

		Generate (map);
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
