using Nimble.XML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2unit
{
	class Tile
	{
		public int m_originX;
		public int m_originY;
		public int m_x;
		public int m_y;
		public byte m_i;

		public TileSet m_set;

		public int m_worldX { get { return (m_originX + m_x) * m_set.m_size; } }
		public int m_worldY { get { return (m_originY + m_y) * m_set.m_size; } }
	}

	class TileSet
	{
		public string m_set;
		public int m_size;
		public List<Tile> m_tiles = new List<Tile>();
	}

	class Unit
	{
		public int id;
		public int id_old;

		public float x;
		public float y;

		public int? layer;


		public virtual void Write(StreamWriter writer)
		{
			if (!layer.HasValue) {
				writer.WriteLine("      <vec3>{0} {1} 0</vec3>", LevelConverter.ToPixels(x), LevelConverter.ToPixels(y));
				writer.WriteLine("      <int>{0}</int>", id);
			} else {
				writer.WriteLine("      <array>");
				writer.WriteLine("          <vec3>{0} {1} 0</vec3>", LevelConverter.ToPixels(x), LevelConverter.ToPixels(y));
				writer.WriteLine("          <int>{0}</int>", id);
				writer.WriteLine("          <dict><int name=\"layer\">{0}</int></dict>", layer.Value);
				writer.WriteLine("      </array>");
			}
		}
	}

	class Light : Unit
	{
		public int m_r;
		public int m_g;
		public int m_b;

		public float m_size;

		public override void Write(StreamWriter writer)
		{
			writer.WriteLine("      <array>");
			writer.WriteLine("        <vec3>{0} {1} 0</vec3>", LevelConverter.ToPixels(x), LevelConverter.ToPixels(y));
			writer.WriteLine("        <int>{0}</int>", id);
			writer.WriteLine("        <dict>");
			writer.WriteLine("          <bool name=\"cast-shadows\">f</bool>");
			writer.WriteLine("          <vec4 name=\"color\">{0} {1} {2} 0</vec4>", transform(m_r), transform(m_g), transform(m_b));
			writer.WriteLine("          <float name=\"size\">{0}</float>", m_size * 16.0f * 0.8f);
			writer.WriteLine("        </dict>");
			writer.WriteLine("      </array>");
		}

		static public float transform(int color)
		{
			return srgb_to_linear(color / 255.0f * 0.5f);
		}

		static public float srgb_to_linear(float f)
		{
			if (f <= 0.04045f)
				return f / 12.92f;
			else
				return (float)Math.Pow((f + 0.055f) / 1.055f, 2.4f);
		}
	}

	class WorldScript : Unit
	{
		public XmlTag m_tag;

		public string m_type;
		public bool m_enabled = true;
		public int m_triggerTimes = -1;
		public bool m_executeOnStart;
		public Dictionary<string, object> m_params = new Dictionary<string, object>();
		public List<Tuple<int, int>> m_connections = new List<Tuple<int, int>>();

		public override void Write(StreamWriter writer)
		{
			writer.WriteLine("    <array>");
			writer.WriteLine("      <string>{0}</string>", m_type);
			writer.WriteLine("      <int>{0}</int>", id);
			writer.WriteLine("      <vec3>{0} {1} 0</vec3>", LevelConverter.ToPixels(x), LevelConverter.ToPixels(y));
			writer.WriteLine("      <bool>{0}</bool>", m_enabled ? "t" : "f");
			writer.WriteLine("      <int>{0}</int>", m_triggerTimes);
			writer.WriteLine("      <bool>{0}</bool>", m_executeOnStart ? "t" : "f");

			if (m_params.Count > 0) {
				writer.WriteLine("      <dict>");
				foreach (var param in m_params) {
					string paramType = "string";
					string paramName = param.Key;

					if (param.Value is Array) {
						writer.WriteLine("        <array name=\"{0}\">", paramName);
						foreach (var o in (param.Value as Array)) {
							var obj = o;
							if (obj is int) {
								paramType = "int";
							} else if (obj is float) {
								paramType = "float";
							} else if (obj is OldUnitID) {
								paramType = "int";
							} else if (obj is bool) {
								paramType = "bool";
								obj = ((bool)obj ? "t" : "f");
							} else if (!(param.Value is string)) {
								Debug.Assert(false);
							}
							writer.WriteLine("          <{0}>{1}</{0}>", paramType, obj);
							//NOTE: The reason we can hack around this is because there are only 2 dynamic unit feeds in Hammerwatch! :)
							if (paramName[0] == '#') {
								writer.WriteLine("          <string>LastSpawned</string>");
							}
						}
						writer.WriteLine("        </array>");
					} else {
						string paramValue = param.Value.ToString();

						if (param.Value is int) {
							paramType = "int";
						} else if (param.Value is float) {
							paramType = "float";
						} else if (param.Value is OldUnitID) {
							paramType = "int";
						} else if (param.Value is bool) {
							paramType = "bool";
							paramValue = ((bool)param.Value ? "t" : "f");
						} else if (!(param.Value is string)) {
							Debug.Assert(false);
						}

						writer.WriteLine("        <{0} name=\"{1}\">{2}</{0}>", paramType, paramName, paramValue);
					}
				}
				writer.WriteLine("      </dict>");
			}

			if (m_connections.Count > 0) {
				writer.WriteLine("      <array>");
				foreach (var conn in m_connections) {
					int id = conn.Item1;
					int delay = conn.Item2;
					writer.WriteLine("        <int>{0}</int><int>{1}</int>", id, delay);
				}
				writer.WriteLine("      </array>");
			}

			writer.WriteLine("    </array>");
		}
	}

	abstract class CollisionArea : Unit
	{
	}

	class RectangleShape : CollisionArea
	{
		public float m_w;
		public float m_h;

		public override void Write(StreamWriter writer)
		{
			writer.WriteLine("      <array>");
			writer.WriteLine("        <vec3>{0} {1} 0</vec3>", LevelConverter.ToPixels(x), LevelConverter.ToPixels(y));
			writer.WriteLine("        <int>{0}</int>", id);
			writer.WriteLine("        <dict>");
			writer.WriteLine("          <bool name=\"sensor\">t</bool>");
			writer.WriteLine("          <bool name=\"shoot-through\">t</bool>");
			writer.WriteLine("          <vec2 name=\"size\">{0} {1}</vec2>", m_w * 16.0f, m_h * 16.0f);
			writer.WriteLine("        </dict>");
			writer.WriteLine("      </array>");
		}
	}

	class CircleShape : CollisionArea
	{
		public float m_diameter;

		public override void Write(StreamWriter writer)
		{
			writer.WriteLine("      <array>");
			writer.WriteLine("        <vec3>{0} {1} 0</vec3>", LevelConverter.ToPixels(x), LevelConverter.ToPixels(y));
			writer.WriteLine("        <int>{0}</int>", id);
			writer.WriteLine("        <dict>");
			writer.WriteLine("          <bool name=\"sensor\">t</bool>");
			writer.WriteLine("          <bool name=\"shoot-through\">t</bool>");
			writer.WriteLine("          <float name=\"radius\">{0}</float>", (m_diameter / 2.0f) * 16.0f);
			writer.WriteLine("        </dict>");
			writer.WriteLine("      </array>");
		}
	}

	class OldUnitID
	{
		public int m_oldID;
		public int m_id = -1;

		public OldUnitID(int id)
		{
			m_oldID = id;
		}

		public override string ToString()
		{
			return m_id.ToString();
		}
	}

	class UnitType
	{
		public bool m_mutateFilename = true;
		public List<Unit> m_units = new List<Unit>();
	}
}
