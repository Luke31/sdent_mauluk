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

	private Sprite[] tileSprites;
	private readonly Property computeCollisionComp = new Property { Name = "ComputeCollision" };
	private readonly Property rigidBodyComp = new Property { Name = "Rigidbody" };
	private readonly Property rigidBodyMassComp = new Property { Name = "RigidbodyMass" };
	private readonly Property depthPropComp = new Property { Name = "Depth" };



	public void Generate (TiledMap map)
	{
		foreach (TiledLayer layer in map.Layer) {
			// Check layer properties
			Property computeCollisionProp = layer.Properties.PropertyList.Find (x => x.Equals (computeCollisionComp));
			bool computeCollision = computeCollisionProp != null;

			Property rigidbodyProp = layer.Properties.PropertyList.Find (x => x.Equals (rigidBodyComp));
			bool isRigidbody = rigidbodyProp != null;

			int[,] dataMap = layer.DataMap;
			bool[,] objectMap = new bool[layer.Width, layer.Height];

			GameObject currentLayer = new GameObject (layer.Name);
			currentLayer.transform.SetParent (levelRoot.transform);
			GameObject currentObject = null;
			BoxCollider2D currentCollider = null;


			float depth = 0;

			Property depthProp = layer.Properties.PropertyList.Find (x => x.Equals (depthPropComp));
			if (depthProp != null) {
				depth = float.Parse (depthProp.Value);
			}

			for (int y = 0; y < layer.Height; y++) {
				for (int x = 0; x < layer.Width; x++) {
					if (dataMap [x, y] >= 0 && !objectMap [x, y]) {
						// Create parent GameObject
						currentObject = new GameObject(x + "_" + y);
						currentObject.transform.SetParent (currentLayer.transform);

						// Compute collision box
						if (computeCollision) {
							currentCollider = currentObject.AddComponent<BoxCollider2D> ();
							currentCollider.size = new Vector2 (tileSize, tileSize);
							currentCollider.offset = new Vector2 ((x + layer.Offsetx) * tileSize + offset.x,
								-(y + layer.Offsety) * tileSize + offset.y);
						}

						if (isRigidbody) {
							Rigidbody2D rigidbody = currentObject.AddComponent<Rigidbody2D> ();

							Property rigidbodyMass = layer.Properties.PropertyList.Find (c => c.Equals (rigidBodyMassComp));
							if (rigidbodyMass != null) {
								rigidbody.mass = float.Parse(rigidbodyMass.Value);
							}
						}

						objectMap [x, y] = true;

						// grow box horizontally
						int rectWidth = 1;
						for (int xx = x + 1; xx < layer.Width; xx++) {
							if (dataMap [xx, y] >= 0) {
								if (computeCollision) {
									currentCollider.size += new Vector2 (tileSize, 0);
									currentCollider.offset += new Vector2 (tileSize / 2f, 0);
								}
								rectWidth++;

								objectMap [xx, y] = true;
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
								if (computeCollision) {
									currentCollider.size += new Vector2 (0, tileSize);
									currentCollider.offset -= new Vector2 (0, tileSize / 2f);
								}

								for (int xx = x; xx < x + rectWidth; xx++) {
									objectMap [xx, yy] = true;
								}
							}
						}
					}

					// Instantiate tile
					if (dataMap [x, y] >= 0) {
						// Instantiate tile
						GameObject instance = Instantiate (baseTilePrefab,
							new Vector3 ((x + layer.Offsetx) * tileSize + offset.x,
								-(y + layer.Offsety) * tileSize + offset.y, depth),
							Quaternion.identity);

						instance.transform.SetParent (currentObject.transform);
						instance.transform.localScale *= tileSize;

						instance.GetComponentInChildren<SpriteRenderer> ().sprite = tileSprites [dataMap[x,y]];
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
