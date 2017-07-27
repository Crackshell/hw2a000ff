using hw2a000ff;
using Nimble.XML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2unit
{
	public static class LevelConverter
	{
		static bool m_visualize = false;

		public static float ToPixels(float u)
		{
			return u * 16.0f - 160.0f;
		}

		public static int MakeMultiple(int n, int mul, bool min)
		{
			if (n > 0 && !min) {
				return (int)(Math.Ceiling(n / (double)mul) * mul);
			}

			return (int)(Math.Floor(n / (double)mul) * mul);
		}

		public static void Convert(XmlFile xml, StreamWriter writer, string localFilename)
		{
			var root = xml.Root.Children[0];
			var levelName = Path.GetFileNameWithoutExtension(localFilename);
			var loader = new LevelLoader(levelName);

			writer.WriteLine("<dict>");
			writer.WriteLine("  <dict name=\"game-mode\">");
			writer.WriteLine("    <string name=\"Class\">HwCampaign</string>");
			writer.WriteLine("  </dict>");
			writer.WriteLine("  <dict name=\"lighting\">");
			writer.WriteLine("    <string name=\"environment\">{0}env/{1}.env</string>", Settings.OutputPrefix, levelName);
			writer.WriteLine("  </dict>");

			// Begin reading tilesets into our grid
			var tileData = root["dictionary[name=tilemap]"].Children[0];
			foreach (var tileDict in tileData.Children) {
				if (tileDict.Name != "dictionary") {
					continue;
				}

				loader.LoadTileData(tileDict);
			}

			writer.WriteLine("  <array name=\"tiles\">");
			foreach (string key in loader.m_tilesets.Keys) {
				var set = loader.m_tilesets[key];

				int min_unit_x = 99999;
				int min_unit_y = 99999;
				int min_x = 99999;
				int min_y = 99999;
				int max_x = -99999;
				int max_y = -99999;
				foreach (var t in set.m_tiles) {
					min_unit_x = Math.Min(min_unit_x, t.m_originX + t.m_x);
					min_unit_y = Math.Min(min_unit_y, t.m_originY + t.m_y);
					min_x = Math.Min(min_x, t.m_worldX);
					min_y = Math.Min(min_y, t.m_worldY);
					max_x = Math.Max(max_x, t.m_worldX);
					max_y = Math.Max(max_y, t.m_worldY);
				}

				int newSide = 512 / set.m_size;

				min_unit_x = MakeMultiple(min_unit_x, newSide, true);
				min_unit_y = MakeMultiple(min_unit_y, newSide, true);

				min_x = MakeMultiple(min_x, 512, true);
				min_y = MakeMultiple(min_y, 512, true);
				max_x = MakeMultiple(max_x, 512, false);
				max_y = MakeMultiple(max_y, 512, false);

				int newSideOffset = newSide - 20; // old side is always 20!
				int newCellCountW = (int)Math.Ceiling((-min_x + max_x + newSideOffset * set.m_size) / 512.0);
				int newCellCountH = (int)Math.Ceiling((-min_y + max_y + newSideOffset * set.m_size) / 512.0);
				int newCellCount = newCellCountW * newCellCountH;

				Tile[][][] cells = new Tile[newCellCount][][];
				for (int i = 0; i < newCellCount; i++) {
					cells[i] = new Tile[newSide][];
					for (int j = 0; j < newSide; j++) {
						cells[i][j] = new Tile[newSide];
					}
				}

				foreach (var t in set.m_tiles) {
					int x = -min_unit_x + t.m_originX + t.m_x + newSideOffset / 2;
					int y = -min_unit_y + t.m_originY + t.m_y + newSideOffset / 2;

					int cell_x = x / newSide;
					int cell_y = y / newSide;
					int cell = cell_y * newCellCountW + cell_x;

					x %= newSide;
					y %= newSide;

					try {
						cells[cell][y][x] = t;
					} catch { /* um yeah i dunno */ }
				}

				int w = !m_visualize ? 1 : (max_x + (min_x < 0 ? -min_x : 0));
				int h = !m_visualize ? 1 : (max_y + (min_y < 0 ? -min_y : 0));
				var bmp = new Bitmap(w, h);
				var g = Graphics.FromImage(bmp);

				for (int i = 0; i < newCellCount; i++) {
					int cell_y = i / newCellCountW;
					int cell_x = i % newCellCountW;

					var sb = new StringBuilder();

					for (int y = 0; y < newSide; y++) {
						for (int x = 0; x < newSide; x++) {
							var t = cells[i][y][x];
							if (t == null) {
								sb.Append("00");
								continue;
							}
							sb.Append(t.m_i.ToString("x02"));

							if (m_visualize) {
								int col = (t.m_i * 40) % 255;
								g.FillRectangle(new SolidBrush(Color.FromArgb(col, col, col)),
									(cell_x * newSide + x) * set.m_size,
									(cell_y * newSide + y) * set.m_size,
									set.m_size, set.m_size);
							}
						}
					}

					int center_x = cell_x * newSide + min_unit_x;
					int center_y = cell_y * newSide + min_unit_y;

					writer.WriteLine("    <dict>");
					writer.WriteLine("      <array name=\"datasets\">");
					writer.WriteLine("        <dict>");
					writer.WriteLine("          <bytes name=\"data\">{0}</bytes>", sb.ToString());
					writer.WriteLine("          <string name=\"tileset\">{0}</string>", Settings.OutputPrefix + set.m_set);
					writer.WriteLine("        </dict>");
					writer.WriteLine("      </array>");
					writer.WriteLine("      <vec2 name=\"pos\">{0} {1}</vec2>", center_x * set.m_size, center_y * set.m_size);
					writer.WriteLine("    </dict>");

					if (m_visualize) {
						g.DrawRectangle(Pens.Red,
							(cell_x * newSide) * set.m_size,
							(cell_y * newSide) * set.m_size,
							newSide * set.m_size, newSide * set.m_size);
					}
				}

				if (m_visualize) {
					var fnm = Path.GetFileNameWithoutExtension(localFilename) + "." + set.m_set.Replace('/', '_') + ".visualized.png";
					bmp.Save(fnm);
				}
				g.Dispose();
				bmp.Dispose();
			}
			writer.WriteLine("  </array>"); // name=tiles

			var doodadsDict = root["dictionary[name=doodads]"];
			if (doodadsDict != null) {
				loader.LoadDoodads(doodadsDict);
			}

			var actorsDict = root["dictionary[name=actors]"];
			if (actorsDict != null) {
				loader.LoadActors(actorsDict);
			}

			var itemsDict = root["dictionary[name=items]"];
			if (itemsDict != null) {
				loader.LoadItems(itemsDict);
			}

			var lightsDict = root["dictionary[name=lighting]"];
			if (lightsDict != null) {
				loader.LoadLights(lightsDict);
			}

			var scriptDict = root["dictionary[name=scripting]"];
			if (scriptDict != null) {
				loader.LoadScripts(scriptDict);
			} // if (scriptDict != null)

			// Prepare for writing (needs to be done before loading prefabs!)
			loader.PrepareWriting();

			// Import prefabs (can contain doodads, items, actors, and script nodes)
			var prefabsDict = root["dictionary[name=prefabs]"];
			if (prefabsDict != null) {
				foreach (var prefabTypeArray in prefabsDict.Children) {
					if (prefabTypeArray.Name != "array") {
						continue;
					}

					foreach (var prefabPos in prefabTypeArray.Children) {
						if (prefabPos.Name != "vec2") {
							continue;
						}
						var parse = prefabPos.Value.Split(' ');
						float offset_x = float.Parse(parse[0]);
						float offset_y = float.Parse(parse[1]);

						string fnm = Settings.SourcePath + prefabTypeArray.Attributes["name"];
						if (!File.Exists(fnm)) {
							fnm = Settings.SourceFallbackPath + prefabTypeArray.Attributes["name"];
						}
						var xmlPrefab = XmlFile.FromFile(fnm);
						var rootPrefab = xmlPrefab.Root.Children[0].Children[0]; // <prefab><dictionary>
						var prefabLoader = new LevelLoader("prefab");

						prefabLoader.m_unitIDCounter = loader.m_unitIDCounter;

						var prefabDoodadsDict = rootPrefab["dictionary[name=doodads]"];
						if (prefabDoodadsDict != null) {
							prefabLoader.LoadDoodads(prefabDoodadsDict);
						}

						var prefabActorsDict = rootPrefab["dictionary[name=actors]"];
						if (prefabActorsDict != null) {
							prefabLoader.LoadActors(prefabActorsDict);
						}

						var prefabItemsDict = rootPrefab["dictionary[name=items]"];
						if (prefabItemsDict != null) {
							prefabLoader.LoadItems(prefabItemsDict);
						}

						var prefabScriptingDict = rootPrefab["dictionary[name=scripting]"];
						if (prefabScriptingDict != null) {
							prefabLoader.LoadScripts(prefabScriptingDict);
						}

						prefabLoader.PrepareWriting();

						loader.m_unitIDCounter = prefabLoader.m_unitIDCounter;

						// Copy (actually reference) all units and scripts to our current loader
						foreach (var unitType in prefabLoader.m_unitTypes) {
							if (!loader.m_unitTypes.ContainsKey(unitType.Key)) {
								loader.m_unitTypes.Add(unitType.Key, new UnitType() {
									m_mutateFilename = unitType.Value.m_mutateFilename
								});
							}
							foreach (var unit in unitType.Value.m_units) {
								unit.x += offset_x;
								unit.y += offset_y;
								loader.m_unitTypes[unitType.Key].m_units.Add(unit);
							}
						}
						foreach (var script in prefabLoader.m_worldScripts) {
							script.x += offset_x;
							script.y += offset_y;
							loader.m_worldScripts.Add(script);
						}
						foreach (var coll in prefabLoader.m_collisionAreas) {
							coll.x += offset_x;
							coll.y += offset_y;
							loader.m_collisionAreas.Add(coll);
						}
					}
				}
			}

			writer.WriteLine("  <dict name=\"units\">");
			foreach (var ddtp in loader.m_unitTypes) {
				if (ddtp.Value.m_mutateFilename) {
					writer.WriteLine("    <array name=\"{0}\">", Settings.OutputPrefix + Path.ChangeExtension(ddtp.Key, "unit"));
				} else {
					writer.WriteLine("    <array name=\"{0}\">", Settings.OutputPrefix + ddtp.Key);
				}
				foreach (var unit in ddtp.Value.m_units) {
					unit.Write(writer);
				}
				writer.WriteLine("    </array>");
			}

			var collsRectangle = loader.m_collisionAreas.Where(c => c is RectangleShape);
			if (collsRectangle.Count() > 0) {
				writer.WriteLine("    <array name=\":Physics_Rectangle\">");
				foreach (var coll in collsRectangle) {
					coll.Write(writer);
				}
				writer.WriteLine("    </array>"); // name=:Physics_Rectangle
			}

			var collsCircle = loader.m_collisionAreas.Where(c => c is CircleShape);
			if (collsCircle.Count() > 0) {
				writer.WriteLine("    <array name=\":Physics_Circle\">");
				foreach (var coll in collsCircle) {
					coll.Write(writer);
				}
				writer.WriteLine("    </array>"); // name=:Physics_Circle
			}

			writer.WriteLine("  </dict>"); // name=units

			if (loader.m_worldScripts.Count > 0) {
				writer.WriteLine("  <array name=\"scripts\">");
				foreach (var ws in loader.m_worldScripts) {
					ws.Write(writer);
				}
				writer.WriteLine("  </array>"); // name=scripts
			}

			writer.WriteLine("  <int name=\"version\">1</int>");
			writer.WriteLine("</dict>"); // root
		}
	}
}
