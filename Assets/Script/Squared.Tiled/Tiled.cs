using UnityEngine;
using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Squared.Tiled
{
	public class Tileset
	{
		public class TilePropertyList : Dictionary<string,string>
		{
			
		}
		
		public string Name;
		public int FirstTileID;
		public int TileWidth;
		public int TileHeight;
		public Dictionary<int, TilePropertyList> TileProperties = new Dictionary<int, TilePropertyList>();
		
		internal static Tileset Load(XmlReader reader)
		{
			var result = new Tileset();
			
			if (reader.GetAttribute("name") != null)
				result.Name = reader.GetAttribute("name");
			if (reader.GetAttribute("tilewidth") != null)
				result.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
			if (reader.GetAttribute("tileheight") != null)
				result.TileHeight = int.Parse(reader.GetAttribute("tileheight"));
			if (reader.GetAttribute("firstgid") != null)
				result.FirstTileID = int.Parse(reader.GetAttribute("firstgid"));
			
			return result;
		}	
		
		// get TilePropertyList by index
		public TilePropertyList GetTileProperties(int index)
		{
			index -= FirstTileID;
			if (index <0)
				return null;
			
			TilePropertyList result = null;
			TileProperties.TryGetValue(index, out result);
			
			return result;
		}
	}
		
	// Layer
	public class Layer
	{
		public SortedList<string,string> Properties = new SortedList<string, string>();
		public string Name;
		public int Width, Height;
		public int[] Tiles;
	
		internal static Layer Load(XmlReader reader)
		{
			var result = new Layer();
			if (reader.GetAttribute("name") != null)
				result.Name = reader.GetAttribute("name");
			if (reader.GetAttribute("width") != null)
				result.Width = int.Parse(reader.GetAttribute("width"));
			if (reader.GetAttribute("height") != null)
				result.Height = int.Parse(reader.GetAttribute("height"));
			
			result.Tiles = new int[result.Width * result.Height];
			
			while (!reader.EOF)
			{
				var name = reader.Name;
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (name)
					{
					case "data":
						{
							if (reader.GetAttribute("encoding") != null)
							{
								var encoding = reader.GetAttribute("encoding");
								switch (encoding)
								{
									case "xml":
									{
										using (var st = reader.ReadSubtree())
		                                {
		                                    int i = 0;
		                                    while (!st.EOF)
		                                    {
		                                        switch (st.NodeType)
		                                        {
		                                            case XmlNodeType.Element:
		                                                if (st.Name == "tile")
		                                                {
		                                                    if(i < result.Tiles.Length)
		                                                    {
		                                                        result.Tiles[i] = int.Parse(st.GetAttribute("gid"));
		                                                        i++;
		                                                    }
		                                                }
		
		                                                break;
		                                            case XmlNodeType.EndElement:
		                                                break;
		                                        }
		
		                                        st.Read();
		                                    }
										}

										continue;
									} 
								
									default: 
										throw new Exception("Unrecognized encoding.");
								}
							}
							else 
							{
								using (var st = reader.ReadSubtree())
                                {
                                    int i = 0;
                                    while (!st.EOF)
                                    {
                                        switch (st.NodeType)
                                        {
                                            case XmlNodeType.Element:
                                                if (st.Name == "tile")
                                                {
                                                    if(i < result.Tiles.Length)
                                                    {
                                                        result.Tiles[i] = int.Parse(st.GetAttribute("gid"));
                                                        i++;
                                                    }
                                                }

                                                break;
                                            case XmlNodeType.EndElement:
                                                break;
                                        }

                                        st.Read();
                                    }
								}
							}

						} break;
						
	                   case "properties":
	                    	{
		                        using (var st = reader.ReadSubtree())
		                        {
		                            while (!st.EOF)
		                            {
		                                switch (st.NodeType)
		                                {
		                                    case XmlNodeType.Element:
		                                        if (st.Name == "property")
		                                        {
		                                            st.Read();
		                                            if (st.GetAttribute("name") != null)
		                                            {
		                                                result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
		                                            }
		                                        }
		
		                                        break;
		                                    case XmlNodeType.EndElement:
		                                        break;
		                                }
		
		                                st.Read();
		                            }
		                        }
                            } break;
					} 
					
					break;
					
 				}
				
				reader.Read();
			}
		
			return result;
		}
	}
		
	// Map
	public class Map
	{
		public SortedList<string,Tileset> Tilesets = new SortedList<string, Tileset>();
		public SortedList<string,Layer> Layers = new SortedList<string,Layer>();
		public SortedList<string,string> Properties = new SortedList<string, string>();
		
		public int Width, Height;
		public int TileWidth, TileHeight;
		
		public static Map LoadData(TextAsset ta)
		{
			var result = new Map();
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.DTD;
			settings.ProhibitDtd = false;
    		
			using (var stream = new MemoryStream(ta.bytes))
			using (var reader = XmlReader.Create(stream, settings))
					
			while (reader.Read())
			{
				var name = reader.Name;
				switch (reader.NodeType)
				{
				case XmlNodeType.DocumentType:
					if (name != "map")
					{
						throw new Exception("Invalid map format");
					}
					
					break;
					
				case XmlNodeType.Element:
					switch (name)
					{
					case "map":
						{
							if (reader.GetAttribute("width") != null)
								result.Width = int.Parse(reader.GetAttribute("width"));
							if (reader.GetAttribute("height") != null)
								result.Height = int.Parse(reader.GetAttribute("height"));
							if (reader.GetAttribute("tilewidth") != null)
								result.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
							if (reader.GetAttribute("tileheight") != null)
								result.TileHeight = int.Parse(reader.GetAttribute("tileheight"));
						} break;
						
					case "tileset":
						{
							using (var st = reader.ReadSubtree())
							{
								st.Read();
								Tileset tileset = Tileset.Load(st);
								result.Tilesets.Add(tileset.Name,tileset);
							}
						} break;
		
					case "layer":
						{
							using (var st = reader.ReadSubtree())
							{
								st.Read();
								var layer = Layer.Load(st);
								result.Layers.Add(layer.Name, layer);
							}
						} break;
						
					case "properties":
						{
							using (var st = reader.ReadSubtree())
							{
								
								while (!st.EOF)
								{
									switch (st.NodeType)
									{
										case XmlNodeType.Element:
											if (st.Name == "property")
											{
												st.Read();
												if (st.GetAttribute("name") != null)
												{
													result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
												}
											}
									
											break;
										case XmlNodeType.EndElement:
											break;
									}
								
									st.Read();
								}
							}
						} break;
					}						

					break;
					
				case XmlNodeType.EndElement:
					break;
				case XmlNodeType.Whitespace:
					break;
				}
			}
		
			// Finished!
			return result;
		}
	}
}


