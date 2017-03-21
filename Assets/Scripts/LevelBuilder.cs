using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
	public GameObject levelRoot;

	public Texture2D spriteSheetTexture;
	public GameObject baseTilePrefab;
	public TextAsset tiledFile;

	public Vector3 offset;
	public float tileSize;

	public GameObject collisionModelRoot;

	private Sprite[] tileSprites;
	private readonly Property collisionPropComp = new Property { Name = "ComputeCollision" };
	private readonly Property depthPropComp = new Property { Name = "Depth" };



	public void Generate (TiledMap map)
	{
		foreach (TiledLayer layer in map.Layer) {
			string[] data = Regex.Replace (layer.Data.Text, @"\r\n|\n", "").Split (',');
			int[,] dataMap = new int[layer.Width, layer.Height];

			float depth = 0;

			Property depthProp = layer.Properties.PropertyList.Find (x => x.Equals (depthPropComp));
			if (depthProp != null) {
				depth = float.Parse (depthProp.Value);
			}

			for (int x = 0; x < layer.Width; x++) {
				for (int y = 0; y < layer.Height; y++) {
					int dataValue = int.Parse (data [y * layer.Width + x]) - 1;
					dataMap [x, y] = dataValue;
					if (dataValue >= 0) {
						GameObject instance = Instantiate (baseTilePrefab,
							                      new Vector3 ((x + layer.Offsetx) * tileSize + offset.x,
								                      -(y + layer.Offsety) * tileSize + offset.y, depth),
							                      Quaternion.identity);

						instance.transform.SetParent (transform);
						instance.transform.localScale *= tileSize;

						instance.GetComponentInChildren<SpriteRenderer> ().sprite = tileSprites [dataValue];
					}
				}
			}


			Property collisionProp = layer.Properties.PropertyList.Find (x => x.Equals (collisionPropComp));
			if (collisionProp != null && collisionProp.Value == "true") {
				WorldCollision (layer, dataMap);
			}

		}
	}


	private void WorldCollision (TiledLayer layer, int[,] dataMap)
	{
		bool[,] colliderMap = new bool[layer.Width, layer.Height];

		BoxCollider2D box = null;
		// Create collision model
		for (int y = 0; y < layer.Height; y++) {
			for (int x = 0; x < layer.Width; x++) {
				if (dataMap [x, y] >= 0 && !colliderMap [x, y]) {
					box = collisionModelRoot.AddComponent<BoxCollider2D> ();
					box.size = new Vector2 (tileSize, tileSize);
					box.offset = new Vector2 ((x + layer.Offsetx) * tileSize + offset.x,
						-(y + layer.Offsety) * tileSize + offset.y);

					colliderMap [x, y] = true;

					// grow box horizontally
					int rectWidth = 1;
					for (int xx = x + 1; xx < layer.Width; xx++) {
						if (dataMap [xx, y] >= 0) {
							box.size += new Vector2 (tileSize, 0);
							box.offset += new Vector2 (tileSize / 2f, 0);
							rectWidth++;

							colliderMap [xx, y] = true;
						} else {
							break;
						}
					}

					// grow box vertically
					for (int yy = y + 1; yy < layer.Height; yy++) {
						bool sameWidth = true;
						for (int xx = x; xx < x + rectWidth; xx++) {
							if (dataMap [xx, yy] < 0) {
								sameWidth = false;
								break;
							}
						}
						if (!sameWidth) {
							break;
						} else {
							box.size += new Vector2 (0, tileSize);
							box.offset -= new Vector2 (0, tileSize / 2f);

							for (int xx = x; xx < x + rectWidth; xx++) {
								colliderMap [xx, yy] = true;
							}
						}
					}
				}
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		tileSprites = Resources.LoadAll<Sprite> (spriteSheetTexture.name);

		XmlSerializer serializer = new XmlSerializer (typeof(TiledMap));
		TiledMap map = (TiledMap)serializer.Deserialize (new MemoryStream (tiledFile.bytes));

		Generate (map);
	}

	// Update is called once per frame
	void Update ()
	{
	}
}
