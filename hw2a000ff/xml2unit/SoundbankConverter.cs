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
	public class SoundMetadata
	{
		public bool m_2d = false;
		public bool m_looping = false;
	}

	public static class SoundbankConverter
	{
		public static Dictionary<string, SoundMetadata> s_dicSoundMetadata = new Dictionary<string, SoundMetadata>();

		public static void Convert(XmlFile xml, StreamWriter writer, string sbnkName)
		{
			var root = xml.Root["soundbank"];

			writer.WriteLine("<soundbank>");

			var sounds = root.FindTagsByName("sound");
			foreach (var sound in sounds) {
				string name = sound.Attributes["name"];
				float volume = 1.0f;
				float pitchVar = 0.0f;

				if (sound.Attributes.ContainsKey("volume")) {
					float.TryParse(sound.Attributes["volume"], out volume);
				}
				if (sound.Attributes.ContainsKey("pitch-var")) {
					float.TryParse(sound.Attributes["pitch-var"], out pitchVar);
				}

				bool is3D = true;
				bool isLooping = false;

				SoundMetadata dicMeta;
				if (s_dicSoundMetadata.TryGetValue("sound/" + sbnkName + ".sbnk:" + name, out dicMeta)) {
					is3D = !dicMeta.m_2d;
					isLooping = dicMeta.m_looping;
				}

				writer.WriteLine("  <sound category=\"sfx\" name=\"{0}\" volume=\"{1}\" pitch-var=\"{2}\" is3d=\"{3}\" looping=\"{4}\">",
					name, volume, pitchVar, is3D.ToString().ToLower(), isLooping.ToString().ToLower());

				var resources = sound.FindTagsByName("source");
				foreach (var res in resources) {
					if (res.Attributes.ContainsKey("res")) {
						var fnm = res.Attributes["res"];
						Program.CopyAsset(fnm);
						writer.WriteLine("    <source res=\"{0}\" />", Settings.OutputPrefix + fnm);
					} else {
						writer.WriteLine("    <source />");
					}
				}

				writer.WriteLine("  </sound>");
			}

			var musics = root.FindTagsByName("music");
			foreach (var music in musics) {
				string name = music.Attributes["name"];
				float volume = 1.0f;

				if (music.Attributes.ContainsKey("volume")) {
					float.TryParse(music.Attributes["volume"], out volume);
				}

				writer.WriteLine("  <sound category=\"music\" name=\"{0}\" volume=\"{1}\" is3d=\"false\" looping=\"true\">",
						name, volume);

				var resources = music.FindTagsByName("source");
				foreach (var res in resources) {
					var fnm = res.Attributes["res"];
					Program.CopyAsset(fnm);
					writer.WriteLine("    <source res=\"{0}\" />", Settings.OutputPrefix + fnm);
				}

				writer.WriteLine("  </sound>");
			}

			writer.WriteLine("</soundbank>");
		}
	}
}
