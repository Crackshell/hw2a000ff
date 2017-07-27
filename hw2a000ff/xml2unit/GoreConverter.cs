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
	public static class GoreConverter
	{
		private static List<string> m_converted = new List<string>();

		public static string ConvertParticle(string particleEffect)
		{
			var index = particleEffect.IndexOf(':');
			if (index < 0)
				return null;

			var particleFile = particleEffect.Substring(0, index);
			var particle = particleEffect.Substring(index + 1);

			var file = particleEffect.Replace(".xml:", "_") + "_gib";
			var localFilename = file + ".sval";

			// Avoid converting this needlessly
			if (m_converted.Contains(localFilename))
				return localFilename;

			m_converted.Add(localFilename);


			string fnm = Settings.SourcePath + particleFile;
			if (!File.Exists(fnm)) {
				fnm = Settings.SourceFallbackPath + particleFile;
			}


			Console.WriteLine("Gore particle: " + localFilename);
			Program.Prepare(Path.GetDirectoryName(localFilename), localFilename);

			using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + localFilename))) {
				writer.WriteLine("<dict>");
				writer.WriteLine("  <array name=\"death-gib\">");
				writer.WriteLine("    <float>0</float>");

				writer.WriteLine("    <dict>");
				writer.WriteLine("      <int name=\"min\">6</int>");
				writer.WriteLine("      <int name=\"max\">8</int>");
				writer.WriteLine("      <float name=\"force\">0.5</float>");
				writer.WriteLine("      <string name=\"unit\">{0}</string>", Settings.OutputPrefix + localFilename + ".unit");
				writer.WriteLine("      <string name=\"anim\">{0}</string>", particle);
				writer.WriteLine("      <bool name=\"stay\">true</bool>");
				writer.WriteLine("    </dict>");

				writer.WriteLine("    <dict>");
				writer.WriteLine("      <int name=\"min\">8</int>");
				writer.WriteLine("      <int name=\"max\">12</int>");
				writer.WriteLine("      <float name=\"force\">0.75</float>");
				writer.WriteLine("      <string name=\"unit\">{0}</string>", Settings.OutputPrefix + localFilename + ".unit");
				writer.WriteLine("      <string name=\"anim\">{0}</string>", particle);
				writer.WriteLine("      <bool name=\"stay\">false</bool>");
				writer.WriteLine("    </dict>");

				writer.WriteLine("  </array>");
				writer.WriteLine("</dict>");
			}

			Program.Prepare(Path.GetDirectoryName(localFilename), localFilename + ".unit");
			using (StreamWriter writerUnit = new StreamWriter(File.Create(Settings.OutputPath + localFilename + ".unit"))) {
				var xml = XmlFile.FromFile(fnm);
				var sprite = xml["sprite[name=breakable_wood]"];


				writerUnit.WriteLine("<unit netsync=\"none\" save=\"false\">");
				writerUnit.WriteLine("  <scenes>");

				writerUnit.WriteLine("    <scene name=\"{0}\" random-start=\"true\">", particle);
				SpriteConverter.Convert(sprite, writerUnit, "gib");
				writerUnit.WriteLine("    </scene>");

				writerUnit.WriteLine("  </scenes>");
				writerUnit.WriteLine("</unit>");
			}

			return localFilename;
		}


		public static string Convert(string scriptPath)
		{
			string fnm = Settings.SourcePath + scriptPath;
			if (!File.Exists(fnm)) {
				fnm = Settings.SourceFallbackPath + scriptPath;
			}
			var xml = XmlFile.FromFile(fnm);
			var root = xml.Root.Children[0];

			var localFilename = Path.ChangeExtension(scriptPath, "sval");

			// Avoid converting this needlessly
			if (m_converted.Contains(localFilename)) {
				return localFilename;
			}
			m_converted.Add(localFilename);

			Console.WriteLine("Gore: " + localFilename);

			Program.Prepare(Path.GetDirectoryName(localFilename), localFilename);

			using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + localFilename))) {
				float force = 0.5f;
				string animFile = "";
				string anim = "none";

				if (root.Attributes.ContainsKey("speed")) {
					force *= float.Parse(root.Attributes["speed"]);
				}

				if (root.Attributes.ContainsKey("particle")) {
					animFile = ParticleConverter.Convert(root.Attributes["particle"]);
					anim = root.Attributes["particle"].Split(':').Last();
				}

				writer.WriteLine("<dict>");
				writer.WriteLine("  <array name=\"death-gib\">");
				writer.WriteLine("    <float>0</float>");

				writer.WriteLine("    <dict>");
				writer.WriteLine("      <int name=\"min\">1</int>");
				writer.WriteLine("      <int name=\"max\">1</int>");
				writer.WriteLine("      <float name=\"force\">{0}</float>", force);
				writer.WriteLine("      <string name=\"unit\">{0}</string>", animFile);
				writer.WriteLine("      <string name=\"anim\">{0}</string>", anim);
				writer.WriteLine("      <bool name=\"stay\">false</bool>");
				writer.WriteLine("    </dict>");

				// We need to make a new file for the additional gore units
				if (root.Children.Count > 0) {
					Program.Prepare(Path.GetDirectoryName(localFilename), localFilename + ".unit");
					using (StreamWriter writerUnit = new StreamWriter(File.Create(Settings.OutputPath + localFilename + ".unit"))) {
						writerUnit.WriteLine("<unit netsync=\"none\" save=\"false\">");
						writerUnit.WriteLine("  <scenes>");

						int n = 0;
						foreach (var sprite in root.Children) {
							if (sprite.Name != "sprite") {
								continue;
							}
							writerUnit.WriteLine("    <scene name=\"{0}\" random-start=\"true\">", "hwport_" + n);
							SpriteConverter.Convert(sprite, writerUnit, "gib");
							writerUnit.WriteLine("    </scene>");

							writer.WriteLine("    <dict>");
							writer.WriteLine("      <int name=\"min\">0</int>");
							writer.WriteLine("      <int name=\"max\">1</int>");
							writer.WriteLine("      <float name=\"force\">{0}</float>", force);
							writer.WriteLine("      <string name=\"unit\">{0}</string>", Settings.OutputPrefix + localFilename + ".unit");
							writer.WriteLine("      <string name=\"anim\">{0}</string>", "hwport_" + n);
							writer.WriteLine("      <bool name=\"stay\">false</bool>");
							writer.WriteLine("    </dict>");

							n++;
						}

						writerUnit.WriteLine("  </scenes>");
						writerUnit.WriteLine("</unit>");
					}
				}

				writer.WriteLine("  </array>");
				writer.WriteLine("</dict>");
			}

			return localFilename;
		}
	}
}
