using Nimble.XML;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xml2unit;

namespace hw2a000ff
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();

			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

#if DEBUG
			textSourcePath.Text = @"g:\Steam\steamapps\common\Hammerwatch\assets\";
			textLevelsXmlPath.Text = @"g:\Steam\steamapps\common\Hammerwatch\assets_campaign\campaign\levels.xml";
			textOutputPath.Text = @"g:\Steam\steamapps\common\Hammerwatch\assets_hwr\";
#endif
		}

		private void buttonConvertUnit_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.Title = "Browse Hammerwatch unit file";
			ofd.Filter = "Hammerwatch units (*.xml)|*.xml";
			if (ofd.ShowDialog() != DialogResult.OK) {
				return;
			}
			var fnm = ofd.FileName;

			XmlFile xml = null;
			try {
				xml = XmlFile.FromFile(fnm);
			} catch (Exception ex) {
				MessageBox.Show("The given unit file has some problems: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string slot = null;

			XmlTag root = null;
			try {
				root = xml.Root.Children[0];
			} catch {
				MessageBox.Show("The given unit file is not supported. It has no root element.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			switch (root.Name) {
				case "actor":
				case "projectile":
				case "doodad":
					slot = root.Name;
					break;

				case "item":
					string behavior = null;
					try {
						behavior = root.Attributes["behavior"];
					} catch {
						MessageBox.Show("The given unit file is not supported. This is an item without a behavior.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					switch (behavior) {
						case "food":
						case "money":
						case "key":
						case "collectable":
						case "mana":
						case "life":
						case "potion":
						case "present": // uhh
						case "upgrade":
							slot = "item";
							break;

						case "breakable":
						case "door":
						case "bomb":
						case "checkpoint": //TODO: This needs a behavior
							slot = "doodad";
							break;

						default:
							MessageBox.Show("The given unit file uses an unsupported behavior. It seems to be using '" + behavior + "'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
					}
					break;
			}

			if (slot == null) {
				MessageBox.Show("The given unit file is not supported. It seems to be a '" + root.Name + "'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var sfd = new SaveFileDialog();
			sfd.Title = "Save to A000FF unit";
			sfd.Filter = "A000FF units (*.unit)|*.unit";
			if (sfd.ShowDialog() != DialogResult.OK) {
				return;
			}
			var fnmOutput = sfd.FileName;

			using (var writer = new StreamWriter(fnmOutput)) {
				try {
					UnitConverter.Convert(xml, writer, slot, Path.GetFileNameWithoutExtension(fnmOutput), fnmOutput);
					MessageBox.Show("File converted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				} catch (Exception ex) {
					MessageBox.Show("Conversion failed: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void buttonBrowseLevelsXml_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.Title = "Locate levels.xml";
			ofd.Filter = "Levels file|levels.xml";
			if (ofd.ShowDialog() != DialogResult.OK) {
				return;
			}
			textLevelsXmlPath.Text = ofd.FileName;
		}

		private void buttonConvert_Click(object sender, EventArgs e)
		{
			if (textSourcePath.Text == "") {
				MessageBox.Show("You did not provide a source path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (textOutputPath.Text == "") {
				MessageBox.Show("You did not provide an output path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (textLevelsXmlPath.Text != "" && !textLevelsXmlPath.Text.EndsWith("levels.xml")) {
				MessageBox.Show("You picked an invalid path for the levels.xml file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (!textSourcePath.Text.EndsWith("\\")) {
				textSourcePath.Text += "\\";
			}
			if (textSourceFallbackPath.Text != "" && !textSourceFallbackPath.Text.EndsWith("\\")) {
				textSourceFallbackPath.Text += "\\";
			}
			if (!textOutputPath.Text.EndsWith("\\")) {
				textOutputPath.Text += "\\";
			}

			var hwPath = textSourceFallbackPath.Text;
			if (hwPath == "") {
				hwPath = textSourcePath.Text;
			}

			if (!File.Exists(hwPath + "..\\Hammerwatch.exe")) {
				MessageBox.Show("Your game's base assets path seems to be set to the wrong path. (Couldn't find Hammerwatch.exe in parent directory of source or fallback path)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Settings.SourcePath = textSourcePath.Text;
			Settings.SourceFallbackPath = textSourceFallbackPath.Text;
			Settings.LevelsPath = textLevelsXmlPath.Text;
			Settings.OutputPath = textOutputPath.Text;
			Settings.OutputPrefix = textOutputPrefix.Text;

			Settings.ConvertActors = checkConvertActors.Checked;
			Settings.ConvertProjectiles = checkConvertProjectiles.Checked;
			Settings.ConvertDoodads = checkConvertDoodads.Checked;
			Settings.ConvertTilesets = checkConvertTilesets.Checked;
			Settings.ConvertItems = checkConvertItems.Checked;
			Settings.ConvertStrings = checkConvertStrings.Checked;
			Settings.ConvertSpeechStyles = checkConvertSpeechStyles.Checked;
			Settings.ConvertFonts = checkConvertFonts.Checked;
			Settings.ConvertLoot = checkConvertLoot.Checked;
			Settings.ConvertLevels = checkConvertLevels.Checked;
			Settings.ConvertSounds = checkConvertSounds.Checked;

			Settings.HealthScale = trackActorHealthScale.Value / 100.0f;
			Settings.RangeScale = trackRangeScale.Value / 100.0f;
			Settings.DamageScale = trackDamageScale.Value / 100.0f;
			Settings.SpeedScale = trackActorSpeedScale.Value / 100.0f;

			Settings.ModifyWallCollision = checkModifyWallCollision.Checked;

			Settings.StringsKeyPrefix = textStringsKeyPrefix.Text;

			var waiting = new FormWaiting();

			var thread = new Thread(() => {
				int fileCount = 0;
				var tmStart = DateTime.Now;

				try {
					var files = Directory.GetFiles(Settings.SourcePath, "*.xml", SearchOption.AllDirectories);

					foreach (var fnm in files) {
						string filename = fnm.Substring(Settings.SourcePath.Length).Replace('\\', '/');
						string dir = Path.GetDirectoryName(filename);

						var xml = XmlFile.FromFile(fnm);
						var root = xml.Root.Children[0];

						if (Settings.ConvertActors && root.Name == "actor") {
							// Actor units
							string filenameUnit = Path.ChangeExtension(filename, "unit");
							waiting.SetStatus("Actor: " + filenameUnit);
							fileCount++;

							Program.Prepare(dir, filenameUnit);

							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameUnit))) {
								UnitConverter.Convert(xml, writer, "actor", Path.GetFileNameWithoutExtension(filenameUnit), filenameUnit);
							}

						} else if (Settings.ConvertProjectiles && root.Name == "projectile") {
							// Projectile units
							string filenameUnit = Path.ChangeExtension(filename, "unit");
							waiting.SetStatus("Projectile: " + filenameUnit);
							fileCount++;

							Program.Prepare(dir, filenameUnit);

							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameUnit))) {
								UnitConverter.Convert(xml, writer, "projectile", Path.GetFileNameWithoutExtension(filenameUnit), filenameUnit);
							}

						} else if (Settings.ConvertDoodads && root.Name == "doodad") {
							// Doodad units
							string filenameUnit = Path.ChangeExtension(filename, "unit");
							waiting.SetStatus("Doodad: " + filenameUnit);
							fileCount++;

							Program.Prepare(dir, filenameUnit);

							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameUnit))) {
								UnitConverter.Convert(xml, writer, "doodad", Path.GetFileNameWithoutExtension(filenameUnit), filenameUnit);
							}

						} else if (Settings.ConvertTilesets && root.Name == "tileset") {
							// Tilesets
							string filenameTileset = Path.ChangeExtension(filename, "tileset");
							waiting.SetStatus("Tilemap: " + filenameTileset);
							fileCount++;

							Program.Prepare(dir, filenameTileset);

							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameTileset))) {
								TilesetConverter.Convert(xml, writer);
							}

						} else if (Settings.ConvertItems && root.Name == "item") {
							// Items (actually just units with behaviors)
							string filenameUnit = Path.ChangeExtension(filename, "unit");
							waiting.SetStatus("Item: " + filenameUnit);
							fileCount++;

							Program.Prepare(dir, filenameUnit);

							var slot = "";

							var behavior = root.Attributes["behavior"];
							switch (behavior) {
								case "food":
								case "money":
								case "key":
								case "collectable":
								case "mana":
								case "life":
								case "potion":
								case "present": // uhh
								case "upgrade":
									slot = "item";
									break;

								case "breakable":
								case "door":
								case "bomb":
								case "checkpoint": //TODO: This needs a behavior
									slot = "doodad";
									break;

								default:
									Console.WriteLine("WARNING: Unknown behavior '{0}'", behavior);
									continue;
							}

							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameUnit))) {
								UnitConverter.Convert(xml, writer, slot, "", filenameUnit);
							}

						} else if (Settings.ConvertStrings && filename.StartsWith("language\\") && root.Name == "dictionary") {
							// String files (almost the same format, but we want to append our language prefix)
							string filenameLang = Path.ChangeExtension(filename, "lang");
							waiting.SetStatus("Strings: " + filenameLang);
							fileCount++;

							Program.Prepare(dir, filenameLang);

							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameLang))) {
								StringConverter.Convert(xml, writer);
							}

						} else if (Settings.ConvertSpeechStyles && root.Name == "speech") {
							// Speech styles
							string filenameUnit = Path.ChangeExtension(filename, "sval");
							waiting.SetStatus("Speech style: " + filenameUnit);
							fileCount++;

							Program.Prepare(dir, filenameUnit);

							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameUnit))) {
								SpeechStyleConverter.Convert(xml, writer);
							}

						} else if (Settings.ConvertFonts && root.Name == "font") {
							// Fonts
							string filenameLocal = Path.ChangeExtension(filename, "fnt");
							waiting.SetStatus("Font: " + filenameLocal);
							fileCount++;

							Program.Prepare(dir, filenameLocal);

							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameLocal))) {
								BitmapFontConverter.Convert(xml, writer);
							}

						}
					}

					if (Settings.ConvertLoot) {
						if (fileCount == 0) {
							waiting.SetStatus("WARNING: Can't convert loot if no units are being converted.");
						} else {
							string[] slots = LootConverter.GetSlots();
							foreach (var slot in slots) {
								string filenameLocal = slot + ".sval";
								waiting.SetStatus("Loot: " + filenameLocal);
								fileCount++;

								Program.Prepare("loot", filenameLocal);

								using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + "loot/" + filenameLocal))) {
									LootConverter.Convert(slot, writer);
								}
							}
						}
					}

					var levelsPath = Settings.LevelsPath;

					if (Settings.ConvertLevels && levelsPath != "" && File.Exists(levelsPath)) {
						levelsPath = Path.GetDirectoryName(levelsPath) + "\\";

						//TODO: Properly convert this into a scenario xml
						var levelsXml = XmlFile.FromFile(levelsPath + "levels.xml");
						var levelsTags = levelsXml.Root.FindTagsByName("level");
						foreach (var level in levelsTags) {
							Program.LevelKeys.Add(level.Attributes["id"], Path.ChangeExtension(level.Attributes["res"], "lvl"));
						}

						var levels = Directory.GetFiles(levelsPath + "levels\\", "*.xml", SearchOption.AllDirectories);
						foreach (var level in levels) {
							string filenameLocal = Path.ChangeExtension(level.Substring(levelsPath.Length + "levels\\".Length), "lvl");
							waiting.SetStatus("Level: " + filenameLocal);
							fileCount++;

							Program.Prepare("levels/" + Path.GetDirectoryName(filenameLocal), filenameLocal);

							XmlFile xmlLevel = XmlFile.FromFile(level);
							using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + "levels/" + filenameLocal))) {
								LevelConverter.Convert(xmlLevel, writer, filenameLocal);
							}
						}
					}

					if (Settings.ConvertSounds) {
						var filesSbnks = Directory.GetFiles(Settings.SourcePath + "/sound", "*.xml", SearchOption.AllDirectories);

						foreach (var fnm in filesSbnks) {
							string filename = fnm.Substring(Settings.SourcePath.Length);
							string dir = Path.GetDirectoryName(filename);

							var xml = XmlFile.FromFile(fnm);
							var root = xml.Root.Children[0];

							if (root.Name == "soundbank") {
								// Soundbanks (are the *exact* same format, but we're parsing it anyway, we want to copy the sound files!)
								string filenameSbnk = Path.ChangeExtension(filename, "sbnk");
								waiting.SetStatus("Soundbank: " + filenameSbnk);
								fileCount++;

								Program.Prepare(dir, filenameSbnk);

								using (StreamWriter writer = new StreamWriter(File.Create(Settings.OutputPath + filenameSbnk))) {
									SoundbankConverter.Convert(xml, writer, Path.GetFileNameWithoutExtension(filenameSbnk));
								}
							}
						}
					}
				} catch (Exception ex) {
					BeginInvoke(new Action(() => {
						MessageBox.Show("Conversion has thrown an exception! Conversion will be stopped.\n\n" + ex.ToString(), "Converter", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Enabled = true;
						waiting.Close();
					}));
					return;
				}

				Invoke(new Action(() => {
					Enabled = true;
					waiting.Close();
					MessageBox.Show("Done! Processed " + fileCount + " files in " + (DateTime.Now - tmStart).TotalSeconds.ToString("0.000") + " seconds.", "Converter", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}));
			});

			Enabled = false;
			waiting.Show(this);

			thread.Start();
		}
	}
}
