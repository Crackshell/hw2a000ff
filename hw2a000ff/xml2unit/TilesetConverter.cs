using hw2a000ff;
using Nimble.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2unit
{
	public static class TilesetConverter
	{
		static int m_size = 0;

		public static void Convert(XmlFile xml, StreamWriter writer)
		{
			var root = xml.Root.Children[0];
			if (root.Name != "tileset") {
				return;
			}

			string texture = "";
			m_size = 0;

			var sprites = root.FindTagsByName("sprite");
			foreach (var sprite in sprites) {
				var tagTexture = sprite.FindTagByName("texture");
				if (tagTexture != null) {
					if (texture == "") {
						texture = tagTexture.Value;
					} else if (texture != tagTexture.Value) {
						// This is never the case in Hammerwatch, but report it anyway.
						Console.WriteLine("ERROR: Multiple textures!");
					}
				}

				var tagFrame = sprite.FindTagByName("frame");
				if (tagFrame != null) {
					string[] parse = tagFrame.Value.Split(' ');
					if (parse.Length == 4) {
						int w = int.Parse(parse[2]);
						int h = int.Parse(parse[3]);
						//m_size = Math.Max(m_size, Math.Max(w, h));
						m_size = 16;
					}
				}
			}

			int layer = -1100; // Ridiculously high value because Hammerwatch uses ridiculously high level values.
			if (root.Attributes.ContainsKey("level")) {
				layer += int.Parse(root.Attributes["level"]);
			}

			Program.CopyAsset(texture);

			writer.WriteLine("<tileset texture=\"{0}\" layer=\"{1}\" size=\"{2}\" material=\"{3}system/hammerwatch.mats:floor\">", Settings.OutputPrefix + texture, layer, m_size, Settings.OutputPrefix);

			foreach (var tag in root.Children) {
				if (tag.Name == "sprite") {
					var tagFrame = tag.FindTagByName("frame");
					if (tagFrame == null) {
						continue;
					}
					string szComment = "";
					string sz = ProperSize(tagFrame.Value, out szComment);
					writer.WriteLine("  <tile>{0}</tile>{1}", sz, szComment);
				}

				if (tag.Name == "borders") {
					writer.WriteLine("  <borders>");
					foreach (var tagBorder in tag.Children) {
						if (tagBorder.Name != "sprite") {
							continue;
						}
						var name = tagBorder.Attributes["name"];
						var tagFrame = tagBorder.FindTagByName("frame");
						if (tagFrame == null) {
							continue;
						}
						string szComment = "";
						string sz = ProperSize(tagFrame.Value, out szComment);
						writer.WriteLine("    <tile name=\"{0}\">{1}</tile>{2}", name, sz, szComment);
					}
					writer.WriteLine("  </borders>");
				}
			}

			writer.WriteLine("</tileset>");
		}

		static string ProperSize(string s, out string comment)
		{
			string[] parse = s.Split(' ');
			if (parse.Length != 4) {
				comment = "";
				return s;
			}

			int w = int.Parse(parse[2]);
			int h = int.Parse(parse[3]);

			if (w == m_size && h == m_size) {
				comment = "";
				return s;
			}
			comment = string.Format(" <!-- {0} {1} -->", w, h);
			return string.Format("{0} {1} {2} {2}", parse[0], parse[1], m_size);
		}
	}
}
