using hw2a000ff;
using Nimble.XML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2unit
{
	partial class LevelLoader
	{
		public string m_levelName;

		public Dictionary<string, TileSet> m_tilesets = new Dictionary<string, TileSet>();

		public int m_unitIDCounter = 1;

		public Dictionary<string, UnitType> m_unitTypes = new Dictionary<string, UnitType>();
		public List<WorldScript> m_worldScripts = new List<WorldScript>();
		public List<CollisionArea> m_collisionAreas = new List<CollisionArea>();

		public List<OldUnitID> m_oldIDs = new List<OldUnitID>();

		public LevelLoader(string levelName)
		{
			m_levelName = levelName;
		}

		private void PrepareUnitType(string type)
		{
			if (!m_unitTypes.ContainsKey(type)) {
				m_unitTypes.Add(type, new UnitType());
			}
		}

		public void LoadTileData(XmlTag tileDict)
		{
			int pos_x = int.Parse(tileDict["int[name=x]"].Value);
			int pos_y = int.Parse(tileDict["int[name=y]"].Value);

			var sets = tileDict["array[name=datasets]"];

			foreach (var set in sets.Children) {
				if (set.Name != "dictionary") {
					continue;
				}

				var tagFilename = set["string[name=tileset]"];
				string filename = Path.ChangeExtension(tagFilename.Value, "tileset");
				int tileSize = 16; // reasons

				TileSet ts = null;
				if (m_tilesets.ContainsKey(filename)) {
					ts = m_tilesets[filename];
				} else {
					ts = new TileSet() {
						m_set = filename,
						m_size = tileSize
					};
					m_tilesets[filename] = ts;
				}

				var data = set["int-arr[name=data-t]"];
				string[] parse = data.Value.Split(' ');
				int side = (int)Math.Sqrt(parse.Length);
				Debug.Assert(side == 20); // should always be 20x20

				for (int y = 0; y < side; y++) {
					for (int x = 0; x < side; x++) {
						ts.m_tiles.Add(new Tile() {
							m_set = ts,
							m_originX = pos_x,
							m_originY = pos_y,
							m_x = x - side / 2,
							m_y = y - side / 2,
							m_i = byte.Parse(parse[y * side + x])
						});
					}
				}
			}
		}

		public void LoadDoodads(XmlTag doodadsDict)
		{
			var doodads = doodadsDict["array[name=doodads]"]; // why?
			foreach (var doodad in doodads.Children) {
				if (doodad.Name != "dictionary") {
					continue;
				}

				var id = int.Parse(doodad["int[name=id]"].Value);
				var type = doodad["string[name=type]"].Value;
				var pos = doodad["vec2[name=pos]"].Value.Split(' ');
				var layer = doodad["int[name=layer]"];

				PrepareUnitType(type);

				var newUnit = new Unit();
				newUnit.id = m_unitIDCounter++;
				newUnit.id_old = id;
				float.TryParse(pos[0], out newUnit.x);
				float.TryParse(pos[1], out newUnit.y);

				if (layer != null) {
					newUnit.layer = int.Parse(layer.Value);
				} else {
					newUnit.layer = null;
				}

				m_unitTypes[type].m_units.Add(newUnit);
			}
		}

		public void LoadActors(XmlTag actorsDict)
		{
			foreach (var item in actorsDict.Children) {
				if (item.Name != "array") {
					continue;
				}
				var type = item.Attributes["name"];
				foreach (var actor in item.Children) {
					if (actor.Name != "array") {
						continue;
					}

					var id = int.Parse(actor.Children[0].Value);
					var pos = actor.Children[1].Value.Split(' ');

					PrepareUnitType(type);

					var newActor = new Unit();
					newActor.id = m_unitIDCounter++;
					newActor.id_old = id;
					float.TryParse(pos[0], out newActor.x);
					float.TryParse(pos[1], out newActor.y);

					m_unitTypes[type].m_units.Add(newActor);
				}
			}
		}

		public void LoadItems(XmlTag itemsDict)
		{
			foreach (var item in itemsDict.Children) {
				if (item.Name != "array") {
					continue;
				}

				var type = item.Attributes["name"];

				foreach (var arr in item.Children) {
					if (arr.Name != "array") {
						continue;
					}

					int id = int.Parse(arr.Children[0].Value);
					var pos = arr.Children[1].Value.Split(' ');

					if (!m_unitTypes.ContainsKey(type)) {
						m_unitTypes.Add(type, new UnitType());
					}

					var newUnit = new Unit();
					newUnit.id = m_unitIDCounter++;
					newUnit.id_old = id;
					float.TryParse(pos[0], out newUnit.x);
					float.TryParse(pos[1], out newUnit.y);

					m_unitTypes[type].m_units.Add(newUnit);
				}
			}
		}

		public void LoadLights(XmlTag lightsDict)
		{
			var lights = lightsDict["array[name=lights]"];
			foreach (var light in lights.Children) {
				if (light.Name != "dictionary") {
					continue;
				}

				var type = "system/light_L.png";

				var id = int.Parse(light["int[name=id]"].Value);
				var pos = light["vec2[name=pos]"].Value.Split(' ');
				var color = light["int-arr[name=mulColor1]"].Value.Split(' ');
				var range = float.Parse(light["float[name=mulRange]"].Value);

				if (!m_unitTypes.ContainsKey(type)) {
					m_unitTypes.Add(type, new UnitType() {
						m_mutateFilename = false
					});
				}

				var newLight = new Light();
				newLight.id = m_unitIDCounter++;
				newLight.id_old = id;
				float.TryParse(pos[0], out newLight.x);
				float.TryParse(pos[1], out newLight.y);
				int.TryParse(color[0], out newLight.m_r);
				int.TryParse(color[1], out newLight.m_g);
				int.TryParse(color[2], out newLight.m_b);
				newLight.m_size = range;

				m_unitTypes[type].m_units.Add(newLight);
			}

			string[] colorAmbient = lightsDict["int-arr[name=ambient-color]"]?.Value.Split(' ');
			string colorShadow = lightsDict["int-arr[name=shadow-color]"]?.Value;

			int car, cag, cab;
			int.TryParse(colorAmbient[0], out car);
			int.TryParse(colorAmbient[1], out cag);
			int.TryParse(colorAmbient[2], out cab);

			car = Math.Min(255, (int)(Light.srgb_to_linear(car / 255.0f * 1.55f) * 255.0f));
			cag = Math.Min(255, (int)(Light.srgb_to_linear(cag / 255.0f * 1.55f) * 255.0f));
			cab = Math.Min(255, (int)(Light.srgb_to_linear(cab / 255.0f * 1.55f) * 255.0f));

			//string[] colorAdd = lightsDict["int-arr[name=add-color]"]?.Value.Split(' ');
			Program.Prepare("env", m_levelName + ".env");
			using (var envWriter = new StreamWriter(File.Create(Settings.OutputPath + "env/" + m_levelName + ".env"))) {
				envWriter.WriteLine("<environment>");

				if (colorShadow != "255 255 255 255") {
					envWriter.WriteLine("  <ambient value=\"{0} {1} {2} 0\" />", 0, 0, 0);
					envWriter.WriteLine("  <lights>");
					envWriter.WriteLine("    <light color=\"{0} {1} {2} 0\" dir=\"1.5 -2.5\" shadow-length=\"50\" />", car, cag, cab);
					envWriter.WriteLine("  </lights>");
				} else {
					envWriter.WriteLine("  <ambient value=\"{0} {1} {2} 0\" />", car, cag, cab);
				}

				envWriter.WriteLine("</environment>");
			}
		}

		public void PrepareWriting()
		{
			// Do a pre-connection pass to update all old IDs
			foreach (var oldID in m_oldIDs) {
				bool found = false;

				foreach (var coll in m_collisionAreas) {
					if (coll.id_old == oldID.m_oldID) {
						oldID.m_id = coll.id;
						found = true;
						break;
					}
				}

				if (found) continue;

				foreach (var ws in m_worldScripts) {
					if (ws.id_old == oldID.m_oldID) {
						oldID.m_id = ws.id;
						found = true;
						break;
					}
				}

				if (found) continue;

				foreach (var type in m_unitTypes) {
					foreach (var unit in type.Value.m_units) {
						if (unit.id_old == oldID.m_oldID) {
							oldID.m_id = unit.id;
							break;
						}
					}
				}
			}

			// Prepare the script writing
			PrepareScriptWriting();
		}
	}
}
