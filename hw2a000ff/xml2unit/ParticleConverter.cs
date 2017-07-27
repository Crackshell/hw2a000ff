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
	public static class ParticleConverter
	{
		private static List<string> m_converted = new List<string>();

		public static string Convert(string scriptPath)
		{
			var parse = scriptPath.Split(new[] { ':' }, 2);
			var localFilename = Path.ChangeExtension(parse[0], "unit");

			// Avoid converting this needlessly
			if (m_converted.Contains(localFilename)) {
				return localFilename;
			}
			m_converted.Add(localFilename);

			Console.WriteLine("Particles: " + localFilename);

			Program.Prepare(Path.GetDirectoryName(localFilename), localFilename);

			using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + localFilename))) {
				var xml = XmlFile.FromFile(Settings.SourcePath + parse[0]);
				var root = xml.Root.Children[0];

				writer.WriteLine("<unit slot=\"doodad\" netsync=\"none\">");
				writer.WriteLine("  <scenes>");

				foreach (var sprite in root.Children) {
					if (sprite.Name != "sprite") {
						continue;
					}
					writer.WriteLine("    <scene name=\"{0}\" random-start=\"true\">", sprite.Attributes["name"]);
					SpriteConverter.Convert(sprite, writer, "gib");
					writer.WriteLine("    </scene>");
				}

				writer.WriteLine("  </scenes>");
				writer.WriteLine("</unit>");
			}

			return localFilename;
		}
	}
}
