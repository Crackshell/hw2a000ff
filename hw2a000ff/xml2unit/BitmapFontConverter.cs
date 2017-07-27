using hw2a000ff;
using Nimble.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace xml2unit
{
	public static class BitmapFontConverter
	{
		public static void Convert(XmlFile xml, StreamWriter writer)
		{
			var root = xml.Root.Children[0];

			LayoutTag(root.FindTagByName("info"), writer);
			LayoutTag(root.FindTagByName("common"), writer);

			var pages = root.FindTagsByName("page");
			foreach (var page in pages) {
				Program.CopyAsset(page.Attributes["file"]);

				var filename = Settings.OutputPrefix + page.Attributes["file"];
				page.Attributes["file"] = filename;
				LayoutTag(page, writer);
			}

			var chars = root.FindTagByName("chars");
			LayoutTag(chars, writer);

			foreach (var c in chars.Children) {
				if (c.Name != "char") {
					continue;
				}
				LayoutTag(c, writer);
			}
		}

		static void LayoutTag(XmlTag tag, StreamWriter writer)
		{
			writer.Write(tag.Name);
			foreach (var kp in tag.Attributes) {
				if (Regex.IsMatch(kp.Value, "^[0-9,\\-]+$")) {
					writer.Write(" {0}={1}", kp.Key, kp.Value);
				} else {
					writer.Write(" {0}=\"{1}\"", kp.Key, kp.Value);
				}
			}
			writer.WriteLine();
		}
	}
}
