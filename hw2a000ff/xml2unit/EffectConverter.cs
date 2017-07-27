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
	public static class EffectConverter
	{
		private static List<string> m_converted = new List<string>();

		public static string Convert(string scriptPath)
		{
			var parse = scriptPath.Split(new[] { ':' }, 2);
			var localFilename = Path.GetDirectoryName(parse[0]).Replace('\\', '/') + "/" + Path.GetFileNameWithoutExtension(parse[0]) + "_" + parse[1] + ".effect";

			// Avoid converting this needlessly
			if (m_converted.Contains(localFilename)) {
				return localFilename;
			}
			m_converted.Add(localFilename);

			Console.WriteLine("Effect: " + localFilename);

			string fnm = Settings.SourcePath + parse[0];
			if (!File.Exists(fnm)) {
				fnm = Settings.SourceFallbackPath + parse[0];
			}
			var xml = XmlFile.FromFile(fnm);
			var root = xml.Root.Children[0];
			foreach (var c in root.Children) {
				if (c.Name != "sprite") {
					continue;
				}
				if (parse[1] == "" || c.Attributes["name"] == parse[1]) {
					Program.Prepare(Path.GetDirectoryName(parse[0]), localFilename);
					using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + localFilename))) {
						writer.WriteLine("<effect>");
						SpriteConverter.Convert(c, writer, "effect", false);
						writer.WriteLine("</effect>");
					}
					break;
				}
			}

			return localFilename;
		}
	}
}
