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
		private string[] m_supportedScripts = new[] {
			"LevelStart", "LevelExitArea", "LevelExit", "ScriptLink", "SpawnObject", "RectangleShape", "CircleShape", "Random", "PlaySound",
			"ChangeDoodadState", "AnnounceText", "DestroyObject", "ToggleElement", "AreaTrigger", "GlobalEventTrigger", "Counter",
			"IncrementCounter", "ObjectEventTrigger", "DangerArea", "TimerTrigger", "ProjectileShooter", "PlayMusic", "ChangeDoodadLayer",
			"HideObject", "SetGlobalFlag", "CheckGlobalFlag", "PlayEffect", "TogglePhysics", "DifficultyFilter", "PlayAttachedEffect",
			"ToggleImmortality", "ShowSpeechBubble", "HideSpeechBubble", "StopSound", "ProjectileSpewer", "ShopArea", "PathNode"
		};

		public void LoadScripts(XmlTag scriptDict)
		{
			var scriptArr = scriptDict["array[name=nodes]"];
			foreach (var dict in scriptArr.Children) {
				var typeHw = dict["string[name=type]"].Value;
				var type = typeHw;
				if (!m_supportedScripts.Contains(type)) {
					type = "UnknownScriptLink";
					Console.WriteLine("WARNING: Unsupported script {0}, loading as ScriptLink", typeHw);
				}

				if (type == "SpawnObject") {
					type = "SpawnUnit";
				} else if (type == "LevelExitArea") {
					type = "LevelExit";
				} else if (type == "Random") {
					type = "RandomCount";
				} else if (type == "ChangeDoodadState") {
					type = "SetUnitScene";
				} else if (type == "DestroyObject") {
					type = "DestroyUnits";
				} else if (type == "ToggleElement") {
					type = "ToggleScripts";
				} else if (type == "ChangeDoodadLayer") {
					type = "SetUnitLayer";
				} else if (type == "HideObject") {
					type = "HideUnit";
				} else if (type == "CheckGlobalFlag") {
					type = "CheckFlag";
				} else if (type == "PlayEffect") {
					type = "SpawnEffect";
				} else if (type == "TogglePhysics") {
					type = "ToggleCollision";
				}


				int id = int.Parse(dict["int[name=id]"].Value);
				bool enabled = bool.Parse(dict["bool[name=enabled]"].Value);
				int triggerTimes = int.Parse(dict["int[name=trigger-times]"].Value);
				var pos = dict["vec2[name=pos]"].Value.Split(' ');
				var args = dict.FindTagByAttribute("name", "parameters");
				bool executeOnStart = false;

				// There could be a GlobalEventTrigger with LevelLoaded as its event, which we can use for execute on startup
				if (type == "GlobalEventTrigger") {
					if (args.Value == "LevelLoaded") {
						type = "ScriptLink";
						executeOnStart = true;
					} else {
						//TODO: Support other events
						continue;
					}
				} else if (type == "ObjectEventTrigger") {
					string ev = args["string[name=event]"].Value;
					if (ev == "Destroyed") {
						type = "UnitDestroyedTrigger";
					} else if (ev == "Hit") {
						type = "UnitDamagedTrigger";
					} else {
						//TODO: Support other events
						continue;
					}
				}

				// AnnounceText and floating texts
				if (type == "AnnounceText") {
					if (args["int[name=type]"].Value == "3") {
						type = "ShowFloatingText";
					}
				}

				// Some scripts are actually collision areas
				if (type == "RectangleShape") {
					var newColl = new RectangleShape();
					newColl.id = m_unitIDCounter++;
					newColl.id_old = id;
					float.TryParse(pos[0], out newColl.x);
					float.TryParse(pos[1], out newColl.y);
					float.TryParse(args["float[name=w]"].Value, out newColl.m_w);
					float.TryParse(args["float[name=h]"].Value, out newColl.m_h);

					m_collisionAreas.Add(newColl);
					continue;

				} else if (type == "CircleShape") {
					var newColl = new CircleShape();
					newColl.id = m_unitIDCounter++;
					newColl.id_old = id;
					float.TryParse(pos[0], out newColl.x);
					float.TryParse(pos[1], out newColl.y);
					float.TryParse(args["float[name=diameter]"].Value, out newColl.m_diameter);

					m_collisionAreas.Add(newColl);
					continue;
				}

				var ws = new WorldScript();
				ws.m_tag = dict;
				ws.id = m_unitIDCounter++;
				ws.id_old = id;
				float.TryParse(pos[0], out ws.x);
				float.TryParse(pos[1], out ws.y);
				ws.m_type = type;
				ws.m_enabled = enabled;
				ws.m_triggerTimes = triggerTimes;
				ws.m_executeOnStart = executeOnStart;

				if (args != null) {
					// Most scripts have more than 1 parameter in a dictionary
					if (args.Name == "dictionary") {
						string soundFile = "";
						foreach (var param in args.Children) {
							object obj = null;

							string name = param.Attributes["name"];
							switch (typeHw) {
								case "LevelStart":
									if (name == "id") {
										if (param.Value != "0") {
											name = "StartID";
											obj = param.Value;
										}
									}
									break;

								case "LevelExitArea":
								case "LevelExit": // The same, except doesn't have 'shape'
									if (name == "level") {
										name = "Level";
										if (Program.LevelKeys.ContainsKey(param.Value)) {
											obj = Program.LevelKeys[param.Value];
										} else {
											obj = "HWPORT.UNKNOWN.LVL";
											Console.WriteLine("WARNING: Unknown level ID '{0}' referenced in LevelExit worldscript", param.Value);
										}
									} else if (name == "start id") {
										if (param.Value != "0") {
											name = "StartID";
											obj = param.Value;
										}
									} else if (name == "shape") {
										// Special case: this script points to a shape to have something happen in this script
										// This doesn't work in SSBD, so we make an intermediate AreaTrigger script
										var arrayTag = param["int-arr"];
										if (arrayTag != null) {
											var newTrigger = new WorldScript();
											newTrigger.id = m_unitIDCounter++;
											newTrigger.id_old = -1;
											newTrigger.x = ws.x + 4;
											newTrigger.y = ws.y;
											newTrigger.m_type = "AreaTrigger";
											newTrigger.m_enabled = true;
											newTrigger.m_triggerTimes = ws.m_triggerTimes;
											newTrigger.m_connections.Add(new Tuple<int, int>(ws.id, 0));
											var parse = arrayTag.Value.Split(' ');
											var arr = new OldUnitID[parse.Length];
											for (int i = 0; i < parse.Length; i++) {
												arr[i] = new OldUnitID(int.Parse(parse[i]));
												m_oldIDs.Add(arr[i]);
											}
											newTrigger.m_params.Add("Areas", arr);
											m_worldScripts.Add(newTrigger);
										}
									}
									break;

								case "Random":
									if (name == "nodes") {
										UnitConnections(param, ws, "ToExecute");
									} else if (name == "run-one") {
										if (!bool.Parse(param.Value)) {
											// If not run-one, we want a RandomChance script instead of RandomCount
											ws.m_type = "RandomChance";
										}
									}
									break;

								case "PlaySound":
									if (name == "sound") {
										name = "Sound";
										soundFile = Program.ChangeExtension(param.Value, "sbnk");
										obj = Settings.OutputPrefix + soundFile;
									} else if (name == "loop") {
										name = "Looping";
										obj = bool.Parse(param.Value);

										SoundMetadata meta;
										if (!SoundbankConverter.s_dicSoundMetadata.TryGetValue(soundFile, out meta)) {
											meta = new SoundMetadata();
											SoundbankConverter.s_dicSoundMetadata[soundFile] = meta;
										}
										meta.m_looping = (bool)obj;
									} else if (name == "play3d") {
										name = "PlayAs3D";
										obj = bool.Parse(param.Value);

										SoundMetadata meta;
										if (!SoundbankConverter.s_dicSoundMetadata.TryGetValue(soundFile, out meta)) {
											meta = new SoundMetadata();
											SoundbankConverter.s_dicSoundMetadata[soundFile] = meta;
										}
										meta.m_2d = !(bool)obj;
									}
									break;

								case "ChangeDoodadState":
									if (name == "state") {
										name = "State";
										obj = param.Value;
									} else if (name == "object") {
										UnitConnections(param, ws, "Units");
									}
									break;

								case "AnnounceText":
									if (name == "text") {
										name = "Text";
										if (param.Value.StartsWith("ig.")) {
											obj = "." + Settings.StringsKeyPrefix + param.Value;
										} else {
											obj = param.Value;
										}
									} else if (name == "time") {
										name = "Time";
										obj = int.Parse(param.Value);
									} else if (name == "type") {
										if (param.Value == "0") {
											// Title
											name = "AnchorY";
											obj = 0.2f;
										} else if (param.Value == "1") {
											// Subtitle
											name = "AnchorY";
											obj = 0.2f;
											ws.m_params.Add("Font", "system/system_small.fnt");
										} else if (param.Value == "2") {
											// Text
											name = "Font";
											obj = "system/system_small.fnt";
										}
									}
									break;

								case "DestroyObject":
									//NOTE: This is *technically* a single-param, but it's still a dict, so this is fine
									if (name == "static") {
										UnitConnectionsStatic(param, ws, "Units");
									} else if (name == "dynamic") {
										UnitConnectionsDynamic(param, ws, "Units");
									}
									break;

								case "ToggleElement":
									if (name == "state") {
										name = "State";
										obj = int.Parse(param.Value) + 1;
									} else if (name == "element") {
										UnitConnections(param, ws, "Scripts");
									}
									break;

								case "AreaTrigger":
									if (name == "event") {
										name = "Event";
										obj = int.Parse(param.Value) + 1;
									} else if (name == "types") {
										// Hammerwatch:
										//   1: Player
										//   2: Enemy
										//   4: Allied
										//   8: Neutral

										// SSBD:
										//   1: NeutralActor
										//   2: PlayerActor
										//   4: EnemyActor
										//   8: PlayerProjectile
										//  16: EnemyProjectile
										//  32: TrapProjectile
										//  64: Other
										uint flags = uint.Parse(param.Value);
										uint newFlags = 0;

										if ((flags & 1) != 0) { newFlags |= 2; }
										if ((flags & 2) != 0) { newFlags |= 4; }
										if ((flags & 4) != 0) { newFlags |= 2; } // NOTE: Actually PlayerActor
										if ((flags & 8) != 0) { newFlags |= 1; }

										name = "Filter";
										obj = (int)newFlags;
									} else if (name == "shape") {
										UnitConnections(param, ws, "Areas");
									}
									break;

								case "Counter":
									if (name == "count") {
										name = "Count";
										obj = int.Parse(param.Value);
									} else if (name == "execute") {
										UnitConnections(param, ws, "ToExecute");
									}
									break;

								case "ObjectEventTrigger":
									if (type == "UnitDestroyedTrigger") {
										if (name == "object") {
											UnitConnections(param, ws, "Units");
										}

									} else if (type == "UnitDamagedTrigger") {
										if (name == "object") {
											UnitConnections(param, ws, "Units");
										}
									}
									break;

								case "DangerArea":
									if (name == "damage") {
										name = "Damage";
										obj = int.Parse(param.Value);
									} else if (name == "shape") {
										UnitConnections(param, ws, "Areas");
									} else if (name == "freq") {
										name = "Frequency";
										obj = int.Parse(param.Value);
									}
									//TODO: Buffs ("buff")
									break;

								case "ProjectileShooter":
								case "ProjectileSpewer":
									if (name == "projectile") {
										name = "Projectile";
										obj = Settings.OutputPrefix + Program.ChangeExtension(param.Value, "unit");
									} else if (name == "direction") {
										name = "Direction";
										int dir = int.Parse(param.Value);
										switch (dir) {
											case 0: obj = 270; break;
											case 1: obj = 90; break;
											case 2: obj = 180; break;
											case 3: obj = 0; break;
										}
									} else if (name == "spread") {
										name = "Spread";
										obj = (float)Math.PI * float.Parse(param.Value) / 180.0f;
									} else if (name == "spawn-rate") {
										name = "Frequency";
										obj = int.Parse(param.Value);
									}
									break;

								case "PlayMusic":
									if (name == "sound") {
										name = "Music";
										obj = Settings.OutputPrefix + Program.ChangeExtension(param.Value, "sbnk");
									}
									break;

								case "ChangeDoodadLayer":
									if (name == "layer") {
										name = "Layer";
										obj = int.Parse(param.Value);
									} else if (name == "objects") {
										UnitConnections(param, ws, "Units");
									}
									break;

								case "HideObject":
									if (name == "state") {
										int state = int.Parse(param.Value);
										if (state == 0) {
											state = 2; // Show
										} else if (state == 2) {
											state = 3; // Toggle
										}
										name = "State";
										obj = state;
									} else if (name == "object") {
										UnitConnections(param, ws, "Units");
									}
									break;

								case "SetGlobalFlag":
									if (name == "flag") {
										name = "Flag";
										obj = param.Value;
									} else if (name == "state") {
										name = "State";
										obj = int.Parse(param.Value);
									}
									break;

								case "CheckGlobalFlag":
									if (name == "flag") {
										name = "Flag";
										obj = param.Value;
									} else if (name == "on-true") {
										UnitConnections(param, ws, "OnTrue");
									} else if (name == "on-false") {
										UnitConnections(param, ws, "OnFalse");
									}
									break;

								case "PlayEffect":
									if (name == "effect") {
										name = "Effect";
										obj = Settings.OutputPrefix + EffectConverter.Convert(param.Value);
									}
									break;

								case "TogglePhysics":
									if (name == "state") {
										name = "State";

										int state = int.Parse(param.Value);
										if (state == 2)
											obj = 0;
										else
											obj = state;

									} else if (name == "doodad") {
										UnitConnections(param, ws, "Units");
									}
									break;

								case "DifficultyFilter":
									if (name == "easy") {
										UnitConnections(param, ws, "Easy");
									} else if (name == "medium") {
										UnitConnections(param, ws, "Medium");
									} else if (name == "hard") {
										UnitConnections(param, ws, "Hard");
									}
									break;

								case "PlayAttachedEffect":
									if (name == "effect") {
										name = "Effect";
										obj = Settings.OutputPrefix + EffectConverter.Convert(param.Value);
									} else if (name == "layer") {
										name = "Layer";
										obj = int.Parse(param.Value);
									} else if (name == "objects") {
										UnitConnections(param, ws, "Objects");
									} else if (name == "y-offset") {
										name = "OffsetY";
										obj = (int)(float.Parse(param.Value) * 16.0f);
									}
									break;

								case "ToggleImmortality":
									if (name == "state") {
										int state = int.Parse(param.Value);
										if (state == 0) {
											state = 2; // Show
										} else if (state == 2) {
											state = 3; // Toggle
										}
										name = "State";
										obj = state;
									} else if (name == "element") {
										UnitConnections(param, ws, "Units");
									}
									break;

								case "ShowSpeechBubble":
									if (name == "style") {
										name = "Style";
										obj = Settings.OutputPrefix + Path.ChangeExtension(param.Value, "sval");
									} else if (name == "text") {
										name = "Text";
										obj = "." + Settings.StringsKeyPrefix + param.Value;
									} else if (name == "objects") {
										UnitConnections(param, ws, "Objects");
									} else if (name == "x-offset") {
										name = "OffsetX";
										obj = (int)(float.Parse(param.Value) * 16.0f);
									} else if (name == "y-offset") {
										name = "OffsetY";
										obj = (int)(float.Parse(param.Value) * 16.0f);
									} else if (name == "width") {
										name = "Width";
										obj = int.Parse(param.Value);
									} else if (name == "layer") {
										name = "Layer";
										obj = int.Parse(param.Value);
									} else if (name == "time") {
										name = "Time";
										obj = int.Parse(param.Value);
									} else if (name == "execute") {
										UnitConnections(param, ws, "OnFinished");
									}
									break;

								case "StopSound":
									if (name == "sound") {
										UnitConnections(param, ws, "Sounds");
									}
									break;

								case "ShopArea":
									if (name == "cats") {
										name = "Categories";
										obj = param.Value;
									} else if (name == "shape") {
										UnitConnections(param, ws, "Areas");
									}
									break;

								case "PathNode":
									if (name == "next") {
										UnitConnections(param, ws, "NextPath");
									} else if (name == "spread") {
										name = "Spread";
										obj = float.Parse(param.Value);
									}
									break;
							}

							if (obj != null) {
								ws.m_params.Add(name, obj);
							}
						}
					} else {
						// Some scripts have only a single parameter, which is packed as a single value
						object obj = null;

						string name = "";
						switch (typeHw) {
							case "SpawnObject":
								name = "UnitType";
								obj = Settings.OutputPrefix + Path.ChangeExtension(args.Value, "unit");
								break;

							case "TimerTrigger":
								name = "Frequency";
								obj = int.Parse(args.Value);
								break;
						}

						if (obj != null) {
							ws.m_params.Add(name, obj);
						}
					}
				}

				m_worldScripts.Add(ws);
			}
		}

		private void UnitConnectionsStatic(XmlTag tag, WorldScript ws, string name)
		{
			if (tag == null) {
				return;
			}
			var ids = tag.Value.Split(' ');
			if (ids == null) {
				return;
			}

			var arr = new OldUnitID[ids.Length];
			for (int i = 0; i < ids.Length; i++) {
				arr[i] = new OldUnitID(int.Parse(ids[i]));
				m_oldIDs.Add(arr[i]);
			}
			ws.m_params.Add(name, arr);
		}

		private void UnitConnectionsDynamic(XmlTag tag, WorldScript ws, string name)
		{
			if (tag == null) {
				return;
			}
			var ids = tag.Value.Split(' ');
			if (ids == null) {
				return;
			}

			var arr = new OldUnitID[ids.Length / 2];
			for (int i = 0; i * 2 < ids.Length; i++) {
				int n = i * 2;
				arr[i] = new OldUnitID(int.Parse(ids[n]));
				m_oldIDs.Add(arr[i]);
				//NOTE: We don't need to store [n+1], since it's always 0!
				Debug.Assert(ids[n + 1] == "0");
			}
			ws.m_params.Add("#" + name, arr);
		}

		private void UnitConnections(XmlTag param, WorldScript ws, string name)
		{
			if (param.Children.Count == 0) {
				return;
			}

			UnitConnectionsStatic(param["int-arr[name=static]"], ws, name);
			UnitConnectionsDynamic(param["int-arr[name=dynamic]"], ws, name);
		}

		public void PrepareScriptWriting()
		{
			// Do a pass for (additional) script connections
			foreach (var ws in m_worldScripts) {
				if (ws.m_tag == null) {
					continue;
				}

				var conns = ws.m_tag["int-arr[name=connections]"]?.Value.Split(' ');
				var connDelays = ws.m_tag["int-arr[name=connection-delays]"]?.Value.Split(' ');

				if (conns == null) {
					continue;
				}

				for (int i = 0; i < conns.Length; i++) {
					int id_old = int.Parse(conns[i]);
					int delay = (connDelays == null ? 0 : int.Parse(connDelays[i]));

					var connScript = m_worldScripts.Find(w => w.id_old == id_old);
					if (connScript == null) {
						continue;
					}

					ws.m_connections.Add(new Tuple<int, int>(connScript.id, delay));
				}
			}
		}
	}
}
