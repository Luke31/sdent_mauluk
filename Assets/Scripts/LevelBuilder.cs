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
    public GameObject player;

    public Texture2D spriteSheetTexture;
    public Texture2D spriteSheetNormal;
    private TextAsset tiledFile;
    public GameObject baseTilePrefab;
    public GameObject waterPrefab;
	public GameObject goalPrefab;
	public GameObject deathPrefab;
	public GameObject deathCirclePrefab;

	public Vector3 offset;
    public float tileSize;

    private Sprite[] tileSprites;
    private readonly Property computeCollisionComp = new Property { Name = "ComputeCollision" };
    private readonly Property rigidBodyComp = new Property { Name = "Rigidbody" };
    private readonly Property rigidBodyMassComp = new Property { Name = "RigidbodyMass" };
    private readonly Property unityLayersComp = new Property { Name = "UnityLayers" };
    private readonly Property depthPropComp = new Property { Name = "Depth" };

	public int StartLevel = 1;
	private static int CurrentLevel = 1;

	TextAsset GetCurrentLevel()
	{
		CurrentLevel = StartLevel;
		return (TextAsset)Resources.Load(string.Format("Levels\\level{1}", Path.DirectorySeparatorChar, CurrentLevel), typeof(TextAsset));
	}

	public void IncCurLevel()
	{
		CurrentLevel++;
	}

	public void Generate()
	{
		tiledFile = GetCurrentLevel();
		tileSprites = Resources.LoadAll<Sprite>(string.Format("Levels\\{0}",spriteSheetTexture.name));

		XmlSerializer serializer = new XmlSerializer(typeof(TiledMap));
		TiledMap map = (TiledMap)serializer.Deserialize(new MemoryStream(tiledFile.bytes));


		foreach (TiledLayer layer in map.Layer)
        {
            int[,] dataMap = layer.DataMap;
            bool[,] objectMap = new bool[layer.Width, layer.Height];
            float depth = 0;

            GameObject currentLayer = new GameObject(layer.Name);
            currentLayer.transform.SetParent(levelRoot.transform);
			
            GameObject currentObject = null;
            BoxCollider2D currentCollider = null;

            // Check layer properties
            Property computeCollisionProp = layer.Properties.PropertyList.Find(x => x.Equals(computeCollisionComp));
            bool computeCollision = computeCollisionProp != null;

            Property rigidbodyProp = layer.Properties.PropertyList.Find(x => x.Equals(rigidBodyComp));
            bool isRigidbody = rigidbodyProp != null;

            Property unityLayersProp = layer.Properties.PropertyList.Find(x => x.Equals(unityLayersComp));
            if (unityLayersProp != null)
            {
                currentLayer.layer = LayerMask.NameToLayer(unityLayersProp.Value);
            }

            Property depthProp = layer.Properties.PropertyList.Find(x => x.Equals(depthPropComp));
            if (depthProp != null)
            {
                depth = float.Parse(depthProp.Value);
            }

            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    if (dataMap[x, y] >= 0 && !objectMap[x, y])
                    {
                        // Create parent GameObject
                        currentObject = new GameObject(x + "_" + y);
                        currentObject.transform.SetParent(currentLayer.transform);
                        currentObject.layer = currentLayer.layer;

                        // Compute collision box
                        if (computeCollision)
                        {
                            currentCollider = currentObject.AddComponent<BoxCollider2D>();
                            currentCollider.size = new Vector2(tileSize, tileSize);
                            currentCollider.offset = new Vector2((x + layer.Offsetx) * tileSize + offset.x,
                                -(y + layer.Offsety) * tileSize + offset.y);
                        }

                        if (isRigidbody)
                        {
                            Rigidbody2D rigidbody = currentObject.AddComponent<Rigidbody2D>();

                            Property rigidbodyMass = layer.Properties.PropertyList.Find(c => c.Equals(rigidBodyMassComp));
                            if (rigidbodyMass != null)
                            {
                                rigidbody.mass = float.Parse(rigidbodyMass.Value);
                            }
                        }

                        objectMap[x, y] = true;

                        // grow box horizontally
                        int rectWidth = 1;
	                    int rectHeight = 1;
                        for (int xx = x + 1; xx < layer.Width; xx++)
                        {
                            if (dataMap[xx, y] >= 0)
                            {
                                if (computeCollision)
                                {
                                    currentCollider.size += new Vector2(tileSize, 0);
                                    currentCollider.offset += new Vector2(tileSize / 2f, 0);
                                }
                                rectWidth++;

                                objectMap[xx, y] = true;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // grow box vertically
                        for (int yy = y + 1; yy < layer.Height; yy++)
                        {
                            bool sameWidth = true;
                            for (int xx = x; xx < x + rectWidth; xx++)
                            {
                                if (dataMap[xx, yy] < 0)
                                {
                                    sameWidth = false;
                                    break;
                                }
                            }
                            if (!sameWidth)
                            {
                                break;
                            }
                            else
                            {
	                            rectHeight++;

								if (computeCollision)
                                {
                                    currentCollider.size += new Vector2(0, tileSize);
                                    currentCollider.offset -= new Vector2(0, tileSize / 2f);
                                }

                                for (int xx = x; xx < x + rectWidth; xx++)
                                {
                                    objectMap[xx, yy] = true;
                                }
                            }
                        }

						// Instantiate tiles per object
	                    for (int xx = x; xx < x + rectWidth; xx++)
	                    {
		                    for (int yy = y; yy < y + rectHeight; yy++)
		                    {
			                    if (dataMap[xx, yy] >= 0)
			                    {
				                    // Instantiate tile
				                    GameObject instance = Instantiate(baseTilePrefab,
					                    new Vector3((xx + layer.Offsetx) * tileSize + offset.x,
						                    -(yy + layer.Offsety) * tileSize + offset.y, depth),
					                    Quaternion.identity);

				                    instance.transform.SetParent(currentObject.transform);
				                    instance.transform.localScale *= tileSize;

				                    SpriteRenderer spriteRenderer = instance.GetComponentInChildren<SpriteRenderer>();
				                    spriteRenderer.sprite = tileSprites[dataMap[xx, yy]];
				                    spriteRenderer.material = Resources.Load<Material>("TileMaterial");
			                    }
		                    }
	                    }
                    }
                }
            }

        }

        foreach (Objectgroup group in map.Objectgroup)
        {
			GameObject instance;
            GameObject currentParent = new GameObject(group.Name);
            currentParent.transform.SetParent(levelRoot.transform);
            Rect helpRect = new Rect();
            foreach (TiledObject tiledObject in group.Object)
            {
                GameObject currentObject = new GameObject(tiledObject.Name);
                currentObject.transform.SetParent(currentParent.transform);

                helpRect.Set(
                    offset.x + (tiledObject.X / map.Tilewidth) * tileSize - tileSize / 2f,
                    offset.y - (tiledObject.Y / map.Tileheight) * tileSize + tileSize / 2f,
                    (tiledObject.Width / map.Tilewidth) * tileSize,
                    (tiledObject.Height / map.Tileheight) * tileSize);

				helpRect.y -= helpRect.height;

                switch (tiledObject.Type)
                {
                    case TiledObjectType.Spawn:
                        if (player != null)
                        {
                            Vector3 spawn = helpRect.center;
                            player.transform.position = spawn;
                        }
                        break;
                    case TiledObjectType.Goal:
						instance = Instantiate(goalPrefab,
						helpRect.center,
						Quaternion.identity);

						instance.transform.SetParent(currentObject.transform);
						instance.transform.localScale = new Vector3(helpRect.width, helpRect.height, 1);
                        break;
                    case TiledObjectType.Water:
                        instance = Instantiate(waterPrefab,
                            helpRect.center,
                            Quaternion.identity);

                        instance.transform.SetParent(currentObject.transform);
                        instance.transform.localScale = new Vector3(helpRect.width, helpRect.height, 1);
                        break;
					case TiledObjectType.Death:
						instance = Instantiate(deathPrefab,
						helpRect.center,
						Quaternion.identity);

						instance.transform.SetParent(currentObject.transform);
						instance.transform.localScale = new Vector3(helpRect.width, helpRect.height, 1);
						break;
					case TiledObjectType.DeathCircle:
						instance = Instantiate(deathCirclePrefab,
						helpRect.center,
						Quaternion.identity);

						instance.transform.SetParent(currentObject.transform);
						instance.transform.localScale = new Vector3(helpRect.width, helpRect.height, 1);
						break;
					case TiledObjectType.Gravity:
                        break;
                    default:
                        Destroy(currentObject);
                        break;
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
