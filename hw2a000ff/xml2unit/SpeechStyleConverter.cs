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
	public static class SpeechStyleConverter
	{
		public static void Convert(XmlFile xml, StreamWriter writer)
		{
			var root = xml.Root.Children[0];

			var font = root.Attributes["font"];
			var colorParse = root.Attributes["text-color"].Split(' ');
			string color = (float.Parse(colorParse[0]) / 255.0f) + " "
				+ (float.Parse(colorParse[1]) / 255.0f) + " "
				+ (float.Parse(colorParse[2]) / 255.0f);

			writer.WriteLine("<dict>");
			writer.WriteLine("  <string name=\"font\">{0}</string>", Settings.OutputPrefix + Path.ChangeExtension(font, "fnt"));
			writer.WriteLine("  <vec3 name=\"color\">{0}</vec3>", color);
			writer.WriteLine();
			writer.WriteLine("  <dict name=\"sprites\">");
			foreach (var child in root.Children) {
				if (child.Name != "sprite") {
					continue;
				}
				writer.WriteLine("    <array name=\"{0}\">", child.Attributes["name"]);
				string fnmTexture = child.FindTagByName("texture").Value;
				writer.WriteLine("      <string>{0}</string>", Settings.OutputPrefix + fnmTexture);
				Program.CopyAsset(fnmTexture);
				foreach (var frame in child.Children) {
					if (frame.Name != "frame") {
						continue;
					}
					int length = 100;
					if (frame.Attributes.ContainsKey("time")) {
						length = int.Parse(frame.Attributes["time"]);
					}
					writer.WriteLine("      <int>{0}</int><vec4>{1}</vec4>", length, frame.Value);
				}
				writer.WriteLine("    </array>");
				var name = child.Attributes["name"];
			}
			writer.WriteLine("  </dict>");
			writer.WriteLine("</dict>");
		}
	}
}
