using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;


[XmlRoot (ElementName = "image")]
public class Image
{
	[XmlAttribute (AttributeName = "source")]
	public string Source { get; set; }

	[XmlAttribute (AttributeName = "width")]
	public int Width { get; set; }

	[XmlAttribute (AttributeName = "height")]
	public int Height { get; set; }
}

[XmlRoot (ElementName = "tileset")]
public class Tileset
{
	[XmlElement (ElementName = "image")]
	public Image Image { get; set; }

	[XmlAttribute (AttributeName = "firstgid")]
	public string Firstgid { get; set; }

	[XmlAttribute (AttributeName = "name")]
	public string Name { get; set; }

	[XmlAttribute (AttributeName = "tilewidth")]
	public int Tilewidth { get; set; }

	[XmlAttribute (AttributeName = "tileheight")]
	public int Tileheight { get; set; }

	[XmlAttribute (AttributeName = "tilecount")]
	public int Tilecount { get; set; }

	[XmlAttribute (AttributeName = "columns")]
	public int Columns { get; set; }
}

[XmlRoot (ElementName = "data")]
public class Data
{
	[XmlAttribute (AttributeName = "encoding")]
	public string Encoding { get; set; }

	[XmlText]
	public string Text { get; set; }
}

[XmlRoot (ElementName = "layer")]
public class TiledLayer
{
	private int[,] map = null;

	[XmlElement (ElementName = "properties")]
	public Properties Properties { get; set; }

	[XmlElement (ElementName = "data")]
	public Data Data { get; set; }

	public int[,] DataMap {
		get {
			if (map == null) {
				string[] data = Regex.Replace (Data.Text, @"\r\n|\n", "").Split (',');
				map = new int[Width, Height];

				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						int dataValue = int.Parse (data [y * Width + x]) - 1;
						map [x, y] = dataValue;
					}
				}
			}

			return map;
		}
	}

	[XmlAttribute (AttributeName = "name")]
	public string Name { get; set; }

	[XmlAttribute (AttributeName = "width")]
	public int Width { get; set; }

	[XmlAttribute (AttributeName = "height")]
	public int Height { get; set; }

	[XmlAttribute (AttributeName = "offsetx")]
	public float Offsetx { get; set; }

	[XmlAttribute (AttributeName = "offsety")]
	public float Offsety { get; set; }
}


[XmlRoot (ElementName = "property")]
public class Property : IEquatable<Property>
{
	[XmlAttribute (AttributeName = "name")]
	public string Name { get; set; }

	[XmlAttribute (AttributeName = "type")]
	public string Type { get; set; }

	[XmlAttribute (AttributeName = "value")]
	public string Value { get; set; }

	public bool Equals (Property other)
	{
		return other != null && this.Name.Equals (other.Name);	
	}
}

[XmlRoot (ElementName = "properties")]
public class Properties
{
	[XmlElement (ElementName = "property")]
	public List<Property> PropertyList { get; set; }
}


[XmlRoot (ElementName = "object")]
public class Object
{
	[XmlAttribute (AttributeName = "id")]
	public string Id { get; set; }

	[XmlAttribute (AttributeName = "name")]
	public string Name { get; set; }

	[XmlAttribute (AttributeName = "x")]
	public string X { get; set; }

	[XmlAttribute (AttributeName = "y")]
	public string Y { get; set; }

	[XmlAttribute (AttributeName = "width")]
	public string Width { get; set; }

	[XmlAttribute (AttributeName = "height")]
	public string Height { get; set; }

	[XmlAttribute (AttributeName = "rotation")]
	public string Rotation { get; set; }

	[XmlElement (ElementName = "properties")]
	public Properties Properties { get; set; }

	[XmlElement (ElementName = "ellipse")]
	public string Ellipse { get; set; }
}

[XmlRoot (ElementName = "objectgroup")]
public class Objectgroup
{
	[XmlElement (ElementName = "object")]
	public List<Object> Object { get; set; }

	[XmlAttribute (AttributeName = "name")]
	public string Name { get; set; }
}

[XmlRoot (ElementName = "map")]
public class TiledMap
{
	[XmlElement (ElementName = "tileset")]
	public Tileset Tileset { get; set; }

	[XmlElement (ElementName = "layer")]
	public List<TiledLayer> Layer { get; set; }

	[XmlElement (ElementName = "objectgroup")]
	public List<Objectgroup> Objectgroup { get; set; }

	[XmlAttribute (AttributeName = "version")]
	public string Version { get; set; }

	[XmlAttribute (AttributeName = "orientation")]
	public string Orientation { get; set; }

	[XmlAttribute (AttributeName = "renderorder")]
	public string Renderorder { get; set; }

	[XmlAttribute (AttributeName = "width")]
	public int Width { get; set; }

	[XmlAttribute (AttributeName = "height")]
	public int Height { get; set; }

	[XmlAttribute (AttributeName = "tilewidth")]
	public int Tilewidth { get; set; }

	[XmlAttribute (AttributeName = "tileheight")]
	public int Tileheight { get; set; }

	[XmlAttribute (AttributeName = "nextobjectid")]
	public string Nextobjectid { get; set; }
}
