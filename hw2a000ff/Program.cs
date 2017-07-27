using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace hw2a000ff
{
	public static class Program
	{
		public static FormMain MainForm;
		public static Dictionary<string, string> LevelKeys = new Dictionary<string, string>();

		public static void Prepare(string dir, string filenameUnit)
		{
			string outputDir = Settings.OutputPath;

			if (!Directory.Exists(outputDir + dir)) {
				Directory.CreateDirectory(outputDir + dir);
			}

			if (File.Exists(outputDir + filenameUnit)) {
				File.Delete(outputDir + filenameUnit);
			}
		}

		public static string ChangeExtension(string filename, string ext)
		{
			if (filename.Contains(":")) {
				var parse = filename.Split(new[] { ':' }, 2);
				return Path.ChangeExtension(parse[0], ext) + ":" + parse[1];
			}
			return Path.ChangeExtension(filename, ext);
		}

		public static void CopyAsset(string fnm)
		{
			string outputDir = Settings.OutputPath;

			string assetDir = outputDir + Path.GetDirectoryName(fnm);
			if (!Directory.Exists(assetDir)) {
				Directory.CreateDirectory(assetDir);
			}

			if (!File.Exists(outputDir + fnm)) {
				string real_fnm = Settings.SourcePath + fnm;
				string fallbackDir = Settings.SourceFallbackPath;
				if (!File.Exists(real_fnm) && fallbackDir != "") {
					real_fnm = fallbackDir + fnm;
				}
				if (!File.Exists(real_fnm)) {
					Console.WriteLine("WARNING: Couldn't locate asset '{0}'", fnm);
					return;
				}

				// For textures, we re-save them to make sure they always have 32bpp.
				if (Path.GetExtension(fnm) == ".png") {
					using (var img = Image.FromFile(real_fnm)) {
						img.Save(outputDir + fnm, System.Drawing.Imaging.ImageFormat.Png);
					}
				} else {
					File.Copy(real_fnm, outputDir + fnm);
				}
			}
		}

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(MainForm = new FormMain());
		}
	}
}
