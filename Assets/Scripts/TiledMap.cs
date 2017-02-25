using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;


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
	[XmlElement (ElementName = "data")]
	public Data Data { get; set; }

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

[XmlRoot (ElementName = "map")]
public class TiledMap
{
	[XmlElement (ElementName = "tileset")]
	public Tileset Tileset { get; set; }

	[XmlElement (ElementName = "layer")]
	public List<TiledLayer> Layer { get; set; }

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
