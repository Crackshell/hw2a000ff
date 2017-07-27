using hw2a000ff;
using Nimble.XML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2unit
{
	public abstract class UnitSkill
	{
		public string m_doSnd;

		public abstract void Write(StreamWriter writer);
	}

	public static class UnitConverter
	{
		const float SPEED_ACTOR_MULT = 3.0f;
		const float SPEED_PROJECTILE_MULT = 4.0f;

		public static void Convert(XmlFile xml, StreamWriter writer, string slot, string unitName, string fullUnitName)
		{
			var root = xml.Root.Children[0];

			bool isActor = (slot == "actor");
			bool isActorResolve = true;
			bool isProjectile = (slot == "projectile");
			bool isItem = (slot == "item");
			bool isEditor = unitName.StartsWith("editor_");

			string netsync = "";
			if (isProjectile)
				netsync = " netsync=\"none\"";
			else if (isActor)
				netsync = " netsync=\"position\"";

			int layer = 20;
			if (root.Attributes.ContainsKey("defaultlayer")) {
				layer = int.Parse(root.Attributes["defaultlayer"]);
			}

			if (layer != 20) {
				if (isProjectile || isItem) {
					writer.WriteLine("<unit{1} layer=\"{0}\">", layer - 20, netsync);
				} else {
					writer.WriteLine("<unit{2} slot=\"{0}\" layer=\"{1}\">", slot, layer - 20, netsync);
				}
			} else {
				if (isProjectile) {
					writer.WriteLine("<unit{0}>", netsync);
				} else {
					writer.WriteLine("<unit{1} slot=\"{0}\">", slot, netsync);
				}
			}
			writer.WriteLine();

			bool sensorColliders = false;
			bool staticColl = true;
			float collRadius = 16.0f;

			// Animation states
			var tagStates = root.FindTagByName("states");

			// Get sprites
			var sprites = root.FindTagsByName("sprite");

			// Do some inspecting on the sprite list
			int spriteCount = 0;
			int spriteMostPixels = 0;
			int spriteMostWidth = 0;
			int spriteMostHeight = 0;

			foreach (var sprite in sprites) {
				// Skip hit-effect sprites if we're a projectile
				if (isProjectile && sprite.Attributes.ContainsKey("name")) {
					if (sprite.Attributes["name"].StartsWith("d")) {
						continue;
					}
				}

				// Get frame size information
				var frames = sprite.FindTagsByName("frame");
				foreach (var frame in frames) {
					var parse = frame.Value.Split(' ');
					if (parse.Length != 4) {
						continue;
					}
					int w = int.Parse(parse[2]);
					int h = int.Parse(parse[3]);
					int pixels = w * h;
					if (pixels > spriteMostPixels) { spriteMostPixels = pixels; }
					if (w > spriteMostWidth) { spriteMostWidth = w; }
					if (h > spriteMostHeight) { spriteMostHeight = h; }
				}

				// Increase counter
				spriteCount++;
			}

			// Behaviors
			string behavior = "";
			if (
				root.Attributes.ContainsKey("behavior") &&
				(root.Attributes["behavior"] != "neutral" && root.Attributes["behavior"] != "spray") // HACK: Some projectile use a couple different behaviors.. we don't really care about this here.
				) {
				behavior = root.Attributes["behavior"];
				var dict = root["behavior"]["dictionary"];

				switch (behavior) {
					case "bomb":
						if (root.Name == "item") {
							writer.WriteLine("  <behavior class=\"BombBehavior\">");
							string deathSound = dict["entry[name=death-snd]"]?["string"].Value ?? dict["entry[name=explode-snd]"]?["string"].Value ?? "";
							if (deathSound != "") {
								writer.WriteLine("    <string name=\"explode-sound\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(deathSound, "sbnk"));
							}
							writer.WriteLine("    <int name=\"delay\">{0}</int>", int.Parse(dict["entry[name=explode-delay]"]["int"].Value));


							writer.WriteLine("    <dict name=\"action\">");
							writer.WriteLine("      <string name=\"class\">Explode</string>");
							string explodeFx = dict["entry[name=explode-particle]"]?["string"].Value ?? "";
							if (explodeFx != "") {
								writer.WriteLine("      <string name=\"fx\">{0}</string>", Settings.OutputPrefix + EffectConverter.Convert(explodeFx));
							}
							float radius = float.Parse(dict["entry[name=splash]"]?["float"].Value ?? dict["entry[name=dmg-range]"]?["float"].Value ?? "0") * 16.0f;
							writer.WriteLine("      <int name=\"radius\">{0}</int>", radius);
							bool dmgAll = bool.Parse(dict["entry[name=dmg-all]"]?["bool"].Value ?? "false");
							writer.WriteLine("      <float name=\"team-dmg\">{0}</float>", dmgAll ? "1" : "0");
							writer.WriteLine("      <array name=\"effects\">");
							writer.WriteLine("        <dict>");
							writer.WriteLine("          <string name=\"class\">Damage</string>");
							writer.WriteLine("          <int name=\"dmg\">10</int>");
							writer.WriteLine("          <string name=\"dmg-type\">pierce</string>");
							writer.WriteLine("        </dict>");
							string buff = dict["entry[name=buff]"]?["string"].Value ?? "";
							if (buff != "") {
								writer.WriteLine("        <dict>");
								writer.WriteLine("          <string name=\"class\">ApplyBuff</string>");
								writer.WriteLine("          <string name=\"buff\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(buff, "sval"));
								writer.WriteLine("        </dict>");
							}
							writer.WriteLine("      </array>");
							writer.WriteLine("    </dict>");
							writer.WriteLine("  </behavior>");
							writer.WriteLine();

						} else if (root.Name == "actor") {
							writer.WriteLine("  <behavior class=\"BombActorBehavior\">");
							writer.WriteLine("    <int name=\"hp\">{0}</int>", int.Parse(dict["entry[name=hp]"]["int"].Value) * Settings.HealthScale);
							writer.WriteLine("    <int name=\"explode-delay\">{0}</int>", int.Parse(dict["entry[name=explode-delay]"]["int"].Value));
							writer.WriteLine("    <int name=\"min-range\">{0}</int>", (int)(float.Parse(dict["entry[name=min-range]"]["float"].Value) * 16.0f));
							writer.WriteLine("    <int name=\"max-range\">{0}</int>", (int)(float.Parse(dict["entry[name=max-range]"]["float"].Value) * 16.0f));
							writer.WriteLine("    <int name=\"dmg-range\">{0}</int>", (int)(float.Parse(dict["entry[name=dmg-range]"]["float"].Value) * 16.0f));
							writer.WriteLine("    <int name=\"pulse-states\">{0}</int>", int.Parse(dict["entry[name=pulse-states]"]["int"].Value));
							string deathSound = dict["entry[name=death-snd]"]?["string"].Value ?? "";
							if (deathSound != "") {
								writer.WriteLine("    <string name=\"death-snd\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(deathSound, "sbnk"));
							}
							writer.WriteLine();
							writer.WriteLine("    <array name=\"skills\">");
							string spawnOnDeath = dict["entry[name=spawn-on-death]"]?["string"].Value ?? "";
							if (spawnOnDeath != "") {
								writer.WriteLine("      <dict>");
								writer.WriteLine("        <string name=\"class\">CompositeActorTriggeredSkill</string>");
								writer.WriteLine("        <string name=\"trigger\">OnDeath</string>");
								writer.WriteLine("        <array name=\"actions\">");
								//TODO: This could be better.. oh well
								int num = int.Parse(dict["entry[name=spawn-on-death-count]"]?["int"].Value ?? "1");
								for (int i = 0; i < num; i++) {
									writer.WriteLine("          <dict>");
									writer.WriteLine("            <string name=\"class\">SpawnUnit</string>");
									writer.WriteLine("            <string name=\"unit\">{0}</string>", Path.ChangeExtension(spawnOnDeath, "unit"));
									writer.WriteLine("            <bool name=\"safe-spawn\">true</bool>");
									writer.WriteLine("            <int name=\"spawn-dist\">{0}</int>", (spriteMostWidth + spriteMostHeight) / 5.0f);
									writer.WriteLine("          </dict>");
								}
								writer.WriteLine("        </array>");
								writer.WriteLine("      </dict>");
							}
							string deathCorpse = dict["entry[name=corpse]"]?["string"].Value ?? "";
							if (deathCorpse != "") {
								writer.WriteLine("      <dict>");
								writer.WriteLine("        <string name=\"class\">CompositeActorTriggeredSkill</string>");
								writer.WriteLine("        <string name=\"trigger\">OnDeath</string>");
								writer.WriteLine("        <array name=\"actions\">");
								writer.WriteLine("          <dict>");
								writer.WriteLine("            <string name=\"class\">SpawnUnit</string>");
								writer.WriteLine("            <string name=\"unit\">{0}</string>", Settings.OutputPrefix + Path.ChangeExtension(deathCorpse, "unit"));
								writer.WriteLine("          </dict>");
								writer.WriteLine("        </array>");
								writer.WriteLine("      </dict>");
							}
							writer.WriteLine("    </array>");
							writer.WriteLine();
							writer.WriteLine("    <dict name=\"movement\">");
							writer.WriteLine("      <string name=\"class\">EmptyMovement</string>");
							writer.WriteLine("    </dict>");
							writer.WriteLine("  </behavior>");
							writer.WriteLine();
						}
						break;

					case "checkpoint":
						writer.WriteLine("  <behavior class=\"HwCheckpoint\">");
						writer.WriteLine("    <string name=\"sound\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(dict["entry[name=snd]"]["string"].Value, "sbnk"));
						writer.WriteLine("    <string name=\"anim-active\">active</string>"); //TODO: Is this correct in all cases?
						writer.WriteLine("  </behavior>");
						writer.WriteLine();
						break;

					case "spawner":
					case "melee":
					case "ranged":
					case "caster":
					case "composite":
					case "tower-flower":
					case "tower-nova":
						string movement = "";
						float speed = 0;
						float dist = 0;
						float range = 0;
						int damage = 0;
						string projectile = "";
						bool seekingProjectile = false;
						float seekingProjectileTurnSpeed = -1.0f;
						string corpse = "";
						int frequency = 2000;
						List<Tuple<int, string>> spawnUnitTypes = new List<Tuple<int, string>>();
						List<UnitSkill> compositeSkills = new List<UnitSkill>();
						int aggroRange = 0;
						int attackDelay = 0;
						string attackSound = "";
						int novaParts = 0;
						string doSnd = "";
						int spikeDamageDelay = 0;
						int spikeDamageRange = 0;

						bool isSpawner = (behavior == "spawner");

						if (root.Attributes.ContainsKey("collision")) {
							collRadius = float.Parse(root.Attributes["collision"]);
						}

						if (!isSpawner) {
							staticColl = false;
						}

						writer.WriteLine("  <behavior class=\"CompositeActorBehavior\">");
						foreach (var entry in dict.Children) {
							if (entry.IsComment || entry.IsTextNode) {
								continue;
							}
							if (entry.Children.Count == 0) {
								continue;
							}
							var entryValueTag = entry.Children[0];

							string propType = entryValueTag.Name;
							string propName = entry.Attributes["name"];
							string propValue = entryValueTag.Value;

							// Skip stuff we don't care about
							if (propName == "editor-range-marker" || propName == "hp-bar") {
								continue;
							}

							// Fix some prop types
							if (propType == "dictionary") {
								propType = "dict";
							}

							// Fix sound assets to their folder prefix
							if (propName.EndsWith("-snd")) {
								propValue = Settings.OutputPrefix + Program.ChangeExtension(propValue, "sbnk");
							}

							// Fix some mismatched props
							if (propName == "aggro-range" || propName == "max-range") {
								// Store this for skills
								if (propName == "aggro-range") {
									aggroRange = (int)(float.Parse(propValue) * 16.0f * Settings.RangeScale);
								}

								// This is float in HW, but needs to be int in SSBD
								propType = "int";
								propValue = ((int)(float.Parse(propValue) * 16.0f * Settings.RangeScale)).ToString();

							} else if (propName == "dmg-delay") {
								// Damage delay for tower flower spikes
								spikeDamageDelay = int.Parse(propValue);
								continue;

							} else if (propName == "dmg-range") {
								// Damage range for tower flower spikes
								spikeDamageRange = (int)(float.Parse(propValue) * 16.0f);
								continue;

							} else if (propName == "nova-parts") {
								// Nova tower parts
								novaParts = int.Parse(propValue);
								continue;

							} else if (propName == "sound" && behavior == "composite") {
								// Composite sound
								doSnd = propValue;
								continue;

							} else if (propName == "attack-snd") {
								// An attacking sound
								attackSound = propValue;
								continue;

							} else if (propName == "attack-delay") {
								// This is used for the tower flower
								attackDelay = int.Parse(propValue);
								continue;

							} else if (propName == "speed") {
								// This is used for movement speed, and is used in the movement dict
								speed = float.Parse(propValue) * SPEED_ACTOR_MULT * Settings.SpeedScale;
								continue;

							} else if (propName == "projectile") {
								// This is the used projectile for ranged behaviors, and is used in the actions of the skills
								projectile = propValue;
								continue;

							} else if (propName == "seeker") {
								// Seeking projectiles have a seeker dictionary
								seekingProjectile = true;
								projectile = entry["entry[name=projectile]"]?.Children[0].Value;
								if (!float.TryParse(entry["entry[name=ang-speed]"]?.Children[0].Value, out seekingProjectileTurnSpeed)) {
									seekingProjectileTurnSpeed = -1;
								}
								continue;

							} else if (propName == "range") {
								// This is used for ranged behaviors to defined their attach distance, and is used in the skills array
								range = float.Parse(propValue) * 16.0f * Settings.RangeScale;
								continue;

							} else if (propName == "movement") {
								// Used for composite movements
								string type = entry["string[name=type]"].Value;
								if (type == "melee") {
									movement = "MeleeMovement";
									speed = float.Parse(entry["float[name=speed]"].Value) * SPEED_ACTOR_MULT;
								} else if (type == "ranged") {
									movement = "RangedMovement";
									speed = float.Parse(entry["float[name=speed]"].Value) * SPEED_ACTOR_MULT;
									dist = float.Parse(entry["float[name=range]"].Value) * 16.0f * Settings.RangeScale;
								} else {
									Console.WriteLine("WARNING: Unknown movement type '{0}'", type);
								}
								continue;

							} else if (propName == "blink") {
								compositeSkills.Add(new UnitSkills.BlinkSkill() {
									m_cooldown = int.Parse(entry["entry[name=timer]"]?.Children[0].Value),
									m_range = float.Parse(entry["entry[name=range]"]?.Children[0].Value) * 16.0f,
									m_distance = float.Parse(entry["entry[name=dist]"]?.Children[0].Value) * 16.0f,
									m_doSnd = doSnd
								});
								continue;

							} else if (propName == "skills" || propName == "pskills" /* passive skills */) {
								// Sometimes composite actors might have skills
								foreach (var skill in entry.Children) {
									if (skill.IsTextNode || skill.IsComment) {
										continue;
									}

									var skillName = skill["string[name=type]"];

									// Blink skill
									if (skillName.Value == "blink") {
										compositeSkills.Add(new UnitSkills.BlinkSkill() {
											m_cooldown = int.Parse(skill["int[name=cooldown]"].Value),
											m_range = float.Parse(skill["float[name=range]"].Value) * 16.0f,
											m_distance = float.Parse(skill["float[name=dist]"].Value) * 16.0f,
											m_sound = skill["string[name=sound]"]?.Value,
											m_fx = skill["string[name=effect]"]?.Value,
											m_doSnd = doSnd
										});

									} else if (skillName.Value == "hit") {
										compositeSkills.Add(new UnitSkills.StrikeSkill() {
											m_anim = skill["string[name=anim-set]"].Value,
											m_cooldown = int.Parse(skill["int[name=cooldown]"].Value),
											m_range = float.Parse(skill["float[name=range]"].Value) * 16.0f,
											m_damage = int.Parse(skill["int[name=dmg]"].Value),
											m_doSnd = doSnd
										});

									} else if (skillName.Value == "spew") {
										compositeSkills.Add(new UnitSkills.SpewSkill() {
											m_anim = skill["string[name=chnl-anim-set]"]?.Value ?? skill["string[name=anim-set]"].Value, //TODO: I think anim-set might be a castpoint animation
											m_range = float.Parse(skill["float[name=range]"].Value) * 16.0f,
											m_duration = int.Parse(skill["int[name=duration]"].Value),
											m_cooldown = int.Parse(skill["int[name=cooldown]"].Value),
											m_projectile = skill["string[name=proj]"].Value,
											m_spread = float.Parse(skill["float[name=spread]"].Value),
											m_rate = int.Parse(skill["int[name=rate]"].Value),
											m_doSnd = doSnd
										});
										//TODO: Particles! ParticleConverter works, but is not properly handled yet:
										// - There needs to be a HwParticle behavior and giving it a ttl property, then using the particles unit to set a custom scene (Converted gore does this)
										// - Then, there needs to be a HwProjectile behavior that is able to fire particles for a specific duration that the projectile is alive (spawn-time-limit)

									}
								}
								continue;

							} else if (propName == "loot") {
								CreateLoot(slot, fullUnitName, entry);

								propType = "string";
								propName = "loot";
								propValue = Settings.OutputPrefix + "loot/" + slot + ".sval:" + fullUnitName;

							} else if (propName == "dmg") {
								// Damage defined for melee enemies
								damage = int.Parse(propValue);
								continue;

							} else if (propName == "corpse") {
								// Corpse unit for spawners
								corpse = propValue;
								continue;

							} else if (propName == "frequency") {
								// Spawning frequency for spawners
								frequency = int.Parse(propValue);
								continue;

							} else if (propName == "spawn-range") {
								// Range for spawners
								range = float.Parse(propValue) * 16.0f;
								continue;

							} else if (propName == "spawns") {
								// Spawn units for spawners
								for (int i = 0; i < entry.Children.Count; i += 2) {
									spawnUnitTypes.Add(new Tuple<int, string>(int.Parse(entry.Children[i].Value), entry.Children[i + 1].Value));
								}
								continue;

							} else if (propName == "hit-effect") {
								// Hit effect on enemies
								propName = "hit-fx";
								propValue = Settings.OutputPrefix + EffectConverter.Convert(propValue);

							} else if (propName == "gib") {
								// Gibs on enemies
								propName = "gore";
								propValue = Settings.OutputPrefix + GoreConverter.Convert(propValue);

							} else if (propName == "summon") {
								//TODO: Summon skill
								continue;
							} else if (propName == "nova") {
								//TODO: Nova skill
								continue;
							}


							writer.WriteLine("    <{0} name=\"{1}\">{2}</{0}>", propType, propName, propValue);
						}


						if (isSpawner) {
							writer.WriteLine("    <bool name=\"impenetrable\">true</bool>");
						}

						if (behavior == "tower-flower") {
							writer.WriteLine("    <bool name=\"must-see-target\">false</bool>");
						}


						// Add a movement
						writer.WriteLine();
						writer.WriteLine("    <dict name=\"movement\">");
						if (isSpawner || behavior.StartsWith("tower-")) {
							writer.WriteLine("      <string name=\"class\">PassiveMovement</string>");
							writer.WriteLine("      <string name=\"anim-idle\">default</string>");
						} else {
							if (movement == "") {
								writer.WriteLine("      <string name=\"class\">MeleeMovement</string>");
							} else {
								writer.WriteLine("      <string name=\"class\">{0}</string>", movement);
							}
							writer.WriteLine("      <string name=\"anim-idle\">idle 8</string>");
							writer.WriteLine("      <string name=\"anim-walk\">walk 8</string>");
							writer.WriteLine();
							writer.WriteLine("      <float name=\"speed\">{0}</float>", speed);
							if (dist > 0) {
								writer.WriteLine("      <int name=\"dist\">{0}</int>", (int)dist);
							} else if (behavior == "ranged") {
								writer.WriteLine("      <int name=\"dist\">{0}</int>", (int)range);
							}
						}
						writer.WriteLine("    </dict>");

						// Add some skills
						writer.WriteLine();
						writer.WriteLine("    <array name=\"skills\">");
						if (behavior == "ranged" || behavior == "caster") {
							var attackLen = FindAttackLength(root);
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">CompositeActorSkill</string>");
							writer.WriteLine();
							writer.WriteLine("        <string name=\"anim\">attack 8</string>");
							writer.WriteLine();
							writer.WriteLine("        <int name=\"cooldown\">0</int>");
							writer.WriteLine("        <int name=\"range\">{0}</int>", range);
							writer.WriteLine("        <int name=\"castpoint\">{0}</int>", attackLen);
							writer.WriteLine("        <string name=\"snd\">sound/enemies.sbnk:spider-melee</string>"); //TODO
							writer.WriteLine("        <array name=\"actions\">");
							if (projectile != "") {
								writer.WriteLine("          <dict>");
								writer.WriteLine("            <string name=\"class\">HwShootProjectile</string>");
								writer.WriteLine("            <string name=\"projectile\">{0}</string>", Settings.OutputPrefix + Path.ChangeExtension(projectile, "unit"));
								if (seekingProjectile) {
									writer.WriteLine("            <bool name=\"seeking\">true</bool>");
									if (seekingProjectileTurnSpeed >= 0) {
										writer.WriteLine("            <float name=\"seek-turnspeed\">{0}</float>", seekingProjectileTurnSpeed);
									}
								}
								writer.WriteLine("          </dict>");
							}
							writer.WriteLine("        </array>");
							writer.WriteLine("      </dict>");

						} else if (behavior == "melee") {
							var attackLen = FindAttackLength(root);
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">EnemyMeleeStrike</string>");
							writer.WriteLine();
							writer.WriteLine("        <string name=\"anim\">attack 8</string>");
							writer.WriteLine("        <string name=\"snd\">sound/enemies.sbnk:gnaar-melee</string>"); //TODO
							writer.WriteLine();
							writer.WriteLine("        <int name=\"cooldown\">0</int>");
							writer.WriteLine("        <int name=\"range\">{0}</int>", 20); // `range` is likely 0 for non-ranged enemies
							writer.WriteLine("        <int name=\"castpoint\">{0}</int>", attackLen);
							writer.WriteLine("        <int name=\"arc\">90</int>");
							writer.WriteLine("        <dict name=\"effect\">");
							writer.WriteLine("          <string name=\"class\">Damage</string>");
							writer.WriteLine("          <int name=\"dmg\">{0}</int>", damage * Settings.DamageScale);
							writer.WriteLine("          <string name=\"dmg-type\">pierce</string>");
							writer.WriteLine("        </dict>");
							writer.WriteLine("      </dict>");

						} else if (behavior == "spawner") {
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">CompositeActorSkill</string>");
							writer.WriteLine("        <bool name=\"must-see\">true</bool>");
							writer.WriteLine("        <int name=\"cooldown\">{0}</int>", frequency);
							writer.WriteLine("        <int name=\"range\">{0}</int>", (int)range);
							//TODO: Check if this is generally ok for spawners? Otherwise; new behavior
							writer.WriteLine("        <string name=\"anim\">{0}</string>", "default");
							if (spawnUnitTypes.Count > 0) {
								writer.WriteLine("        <array name=\"actions\">");
								writer.WriteLine("          <dict>");
								writer.WriteLine("            <string name=\"class\">HwSpawnUnit</string>");
								writer.WriteLine("            <array name=\"units\">");
								foreach (var spawnUnitType in spawnUnitTypes) {
									writer.WriteLine("              <int>{0}</int><string>{1}</string>", spawnUnitType.Item1, Settings.OutputPrefix + Path.ChangeExtension(spawnUnitType.Item2, "unit"));
								}
								writer.WriteLine("            </array>");
								writer.WriteLine("            <bool name=\"safe-spawn\">true</bool>");
								writer.WriteLine("            <int name=\"spawn-dist\">{0}</int>", (spriteMostWidth + spriteMostHeight) / 4.0f);
								writer.WriteLine("          </dict>");
								writer.WriteLine("        </array>");
							} else {
								Console.WriteLine("WARNING: Spawner without spawn definitions???");
							}
							writer.WriteLine("      </dict>");

						} else if (behavior == "tower-flower") {
							// We need to write a special spike unit with this
							Program.Prepare("actors/spikes", unitName + "_spike.unit");
							using (var writerSpike = new StreamWriter(File.Create(Settings.OutputPath + "actors/spikes/" + unitName + "_spike.unit"))) {
								writerSpike.WriteLine("<unit slot=\"projectile\">");
								writerSpike.WriteLine("  <behavior class=\"TowerFlowerSpike\">");
								writerSpike.WriteLine("    <string name=\"anim\"></string>");
								writerSpike.WriteLine("    <int name=\"hurt-delay\">{0}</int>", spikeDamageDelay);
								writerSpike.WriteLine("    <int name=\"hurt-delay-max\">{0}</int>", spikeDamageDelay + 100);
								writerSpike.WriteLine("    <int name=\"radius\">{0}</int>", spikeDamageRange);
								writerSpike.WriteLine("    <dict name=\"effect\">");
								writerSpike.WriteLine("      <string name=\"class\">Damage</string>");
								writerSpike.WriteLine("      <int name=\"dmg\">{0}</int>", damage * Settings.DamageScale);
								writerSpike.WriteLine("      <string name=\"dmg-type\">pierce</string>");
								writerSpike.WriteLine("    </dict>");
								writerSpike.WriteLine("  </behavior>");
								writerSpike.WriteLine("  <scenes>");
								writerSpike.WriteLine("    <scene>");
								writerSpike.WriteLine("    </scene>");
								writerSpike.WriteLine("  </scenes>");
								writerSpike.WriteLine("</unit>");
							}

							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">TowerFlowerSkill</string>");
							writer.WriteLine("        <string name=\"anim\">attack</string>");
							writer.WriteLine("        <int name=\"castpoint\">50</int>");
							writer.WriteLine("        <int name=\"radius\">{0}</int>", aggroRange);
							writer.WriteLine("        <int name=\"interval\">{0}</int>", attackDelay);
							writer.WriteLine("        <int name=\"interval-random\">0</int>");
							writer.WriteLine("        <string name=\"spike\">{1}actors/spikes/{0}_spike.unit</string>", unitName, Settings.OutputPrefix);
							writer.WriteLine("        <string name=\"spike-effect\">attack-effect</string>");
							writer.WriteLine("        <string name=\"cast-snd\">{0}</string>", Settings.OutputPrefix + attackSound);
							writer.WriteLine("      </dict>");

							isActorResolve = false;

						} else if (behavior == "tower-nova") {
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">NovaSkill</string>");
							writer.WriteLine("        <string name=\"anim\">attack</string>");
							writer.WriteLine("        <int name=\"cooldown\">{0}</int>", frequency);
							writer.WriteLine("        <int name=\"min-range\">0</int>");
							writer.WriteLine("        <int name=\"range\">{0}</int>", aggroRange);
							writer.WriteLine("        <string name=\"projectile\">{0}</string>", Settings.OutputPrefix + Path.ChangeExtension(projectile, "unit"));
							writer.WriteLine("        <string name=\"fire-snd\">{0}</string>", Settings.OutputPrefix + attackSound);
							writer.WriteLine("        <int name=\"proj-count\">{0}</int>", novaParts);
							writer.WriteLine("      </dict>");
						}

						if (corpse != "") {
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">CompositeActorTriggeredSkill</string>");
							writer.WriteLine("        <string name=\"trigger\">OnDeath</string>");
							writer.WriteLine("        <array name=\"actions\">");
							writer.WriteLine("          <dict>");
							writer.WriteLine("            <string name=\"class\">SpawnUnit</string>");
							writer.WriteLine("            <string name=\"unit\">{0}</string>", Settings.OutputPrefix + Path.ChangeExtension(corpse, "unit"));
							writer.WriteLine("          </dict>");
							writer.WriteLine("        </array>");
							writer.WriteLine("      </dict>");
						}

						foreach (var skill in compositeSkills) {
							skill.Write(writer);
						}

						writer.WriteLine("    </array>"); // name=skills

						writer.WriteLine("  </behavior>");
						writer.WriteLine();
						break;

					case "breakable":
						writer.WriteLine("  <behavior class=\"Breakable\">");

						var tagDestroySnd = dict["entry[name=destroy-snd]"];
						if (tagDestroySnd != null) {
							string fnm = Program.ChangeExtension(tagDestroySnd.Children[0].Value, "sbnk");
							writer.WriteLine("    <string name=\"break-sound\">{0}</string>", Settings.OutputPrefix + fnm);
						}

						var tagDestroyParticle = dict["entry[name=destroy-particle]"];
						if (tagDestroyParticle != null) {
							var goreName = GoreConverter.ConvertParticle(tagDestroyParticle.Children[0].Value);
							if (!string.IsNullOrEmpty(goreName))
								writer.WriteLine("    <string name=\"gore\">{0}</string>", Settings.OutputPrefix + goreName);
						}


						var tagLoot = dict.FindTagByAttribute("name", "loot");
						if (tagLoot != null) {
							CreateLoot(slot, fullUnitName, tagLoot);
							writer.WriteLine("    <string name=\"loot\">{0}</string>", Settings.OutputPrefix + "loot/" + slot + ".sval:" + fullUnitName);
						}

						if (tagLoot != null) {
							//TODO: Loot on breakables
							/*
							for (int i = 0; i < tagLoot.Children.Count;) {
								if (tagLoot.Children[i].FindTagByName("int") != null) {
									break;
								}
								int chance = int.Parse(tagLoot.Children[i++].Value);
								string unit = Program.ChangeExtension(Program.m_conversionPrefix + tagLoot.Children[i++].Value, "unit");
								LootConverter.Add(chance, unit);
							}
							LootConverter.End(slot, unitName);

							string fnm = "loot/" + slot + ".sval:" + unitName;
							writer.WriteLine("    <string name=\"loot\">{0}</string>", Settings.OutputPrefix + fnm);
							*/
						}

						writer.WriteLine("  </behavior>");
						writer.WriteLine();
						break;

					case "food":
					case "money":
					case "key":
					case "collectable":
					case "mana":
					case "life":
					case "potion":
					case "present": // uhh
					case "upgrade":
						sensorColliders = true;

						writer.WriteLine("  <behavior class=\"Pickup\">");

						if (behavior != "key") {
							writer.WriteLine("    <bool name=\"bounce\">false</bool>");
						}

						var tagSound = dict["entry[name=pickup-snd]"]?.Children[0];
						if (tagSound != null) {
							writer.WriteLine("    <string name=\"sound\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(tagSound.Value, "sbnk"));
						}


						writer.WriteLine("    <bool name=\"global\">true</bool>");
						writer.WriteLine("    <array name=\"effects\">");

						var tagHp = dict["entry[name=hp]"]?.Children[0];
						if (behavior == "food" && tagHp != null) {
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">Heal</string>");
							writer.WriteLine("        <int name=\"heal\">{0}</int>", tagHp.Value);
							writer.WriteLine("        <bool name=\"pickup\">true</bool>");
							writer.WriteLine("      </dict>");
						}

						var tagKeyType = dict["entry[name=type]"]?.Children[0];
						if (behavior == "key" && tagKeyType != null) {
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">GiveKey</string>");
							writer.WriteLine("        <int name=\"type\">{0}</int>", tagKeyType.Value);
							writer.WriteLine("      </dict>");
						}

						var tagAmount = dict["entry[name=amount]"]?.Children[0];
						if (behavior == "money") {
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">GiveGold</string>");
							writer.WriteLine("        <int name=\"amount\">{0}</int>", int.Parse(tagAmount.Value));
							writer.WriteLine("      </dict>");
						}

						var tagPickupText = dict["entry[name=pickup-text]"]?.Children[0];
						if (tagPickupText != null) {
							writer.WriteLine("      <dict>");
							writer.WriteLine("        <string name=\"class\">ShowFloatingText</string>");
							writer.WriteLine("        <string name=\"text\">.{0}</string>", Settings.StringsKeyPrefix + tagPickupText.Value);
							writer.WriteLine("        <vec2 name=\"offset\">0 -18</vec2>");
							writer.WriteLine("      </dict>");
						}

						writer.WriteLine("    </array>"); // name=effects

						writer.WriteLine("  </behavior>");
						writer.WriteLine();
						break;

					case "door":
						var tagType = dict["entry[name=type]"]?.Children[0];
						if (tagType != null) {
							writer.WriteLine("  <behavior class=\"Door\">");
							writer.WriteLine("    <int name=\"type\">{0}</int>", tagType.Value);

							var tagOpenSound = dict["entry[name=open-snd]"]?.Children[0];
							if (tagOpenSound != null) {
								writer.WriteLine("    <string name=\"open-snd\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(tagOpenSound.Value, "sbnk"));
							}

							var tagNoKeySound = dict["entry[name=no-key-snd]"]?.Children[0];
							if (tagNoKeySound != null) {
								writer.WriteLine("    <string name=\"no-key-snd\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(tagNoKeySound.Value, "sbnk"));
							}

							writer.WriteLine("  </behavior>");
							writer.WriteLine();
						}
						break;
				}
			} else if (isProjectile) {
				// Projectiles don't have any defined behavior except in their root xml attributes
				float speed = float.Parse(root.Attributes["speed"]) * Settings.SpeedScale;
				float damage = float.Parse(root.Attributes["damage"]);
				staticColl = false;
				collRadius = float.Parse(root.Attributes["collision"]);

				if (!root.Attributes.ContainsKey("behavior") || root.Attributes["behavior"] != "spray") {
					speed *= SPEED_PROJECTILE_MULT;
				}

				// ..except sometimes, they do have a defined behavior:
				string ttl = null;
				string buff = null;
				float range = 0.0f;
				XmlTag tagBehaviorDict = root["behavior"]?["dictionary"];
				if (tagBehaviorDict != null) {
					buff = tagBehaviorDict["string[name=buff]"]?.Value;
					ttl = tagBehaviorDict["int[name=ttl]"]?.Value;

					string strRange = tagBehaviorDict["float[name=range]"]?.Value;
					if (strRange != null && strRange != "") {
						range = float.Parse(strRange) * 16.0f * Settings.RangeScale;
					}
				}

				writer.WriteLine("  <behavior class=\"{0}\">", spriteMostPixels <= 64 ? "RayProjectile" : "Projectile");
				writer.WriteLine("    <string name=\"anim\">idle {0}</string>", spriteCount);
				writer.WriteLine("    <float name=\"speed\">{0}</float>", speed);
				if (ttl != null && ttl != "") {
					writer.WriteLine("    <int name=\"ttl\">{0}</int>", ttl);
				} else if (range > 0) {
					writer.WriteLine("    <int name=\"range\">{0}</int>", range);
				}
				writer.WriteLine("    <array name=\"effects\">");
				writer.WriteLine("      <dict>");
				writer.WriteLine("        <string name=\"class\">Damage</string>");
				writer.WriteLine("        <int name=\"dmg\">{0}</int>", (int)damage * Settings.DamageScale);
				writer.WriteLine("        <string name=\"dmg-type\">pierce</string>");
				writer.WriteLine("      </dict>");
				if (buff != null && buff != "") {
					writer.WriteLine("      <dict>");
					writer.WriteLine("        <string name=\"class\">ApplyBuff</string>");
					//TODO: BuffConverter
					writer.WriteLine("        <string name=\"buff\">{0}:{1}</string>", Settings.OutputPrefix + Program.ChangeExtension(buff, "sval"), unitName);
					writer.WriteLine("      </dict>");
				}
				writer.WriteLine("    </array>");
				writer.WriteLine("  </behavior>");
				writer.WriteLine();

			} else if (tagStates != null) {
				writer.WriteLine("  <behavior class=\"StateAnimations\">");
				if (tagStates.Attributes.ContainsKey("default")) {
					writer.WriteLine("    <string name=\"default\">{0}</string>", tagStates.Attributes["default"]);
				}
				writer.WriteLine("    <array name=\"transitions\">");

				foreach (var transition in tagStates.Children) {
					if (transition.Name != "transition") {
						continue;
					}
					writer.WriteLine("      <dict>");
					writer.WriteLine("        <string name=\"from\">{0}</string>", transition.Attributes["from"]);
					writer.WriteLine("        <string name=\"to\">{0}</string>", transition.Attributes["to"]);
					writer.WriteLine("      </dict>");
				}
				writer.WriteLine("    </array>");
				writer.WriteLine("  </behavior>");
			}

			// Find the scene states
			string startScene = null;
			var transitions = new List<Tuple<string, string>>();

			if (tagStates != null) {
				// Take the default scene from the attribute (starting scene)
				if (tagStates.Attributes.ContainsKey("default")) {
					startScene = tagStates.Attributes["default"];
				}

				// The children of the states tag contain "transitions", which is not an SSBD feature.
				// However, we can emulate this to *some* extend by (ab)using the "loopback" feature on sprites.
				foreach (var transition in tagStates.Children) {
					if (transition.Name != "transition") {
						continue;
					}
					var from = transition.Attributes["from"];
					var to = transition.Attributes["to"];
					transitions.Add(new Tuple<string, string>(from, to));
				}
			} else {
				// We can look up any random-start attributes here
				var randomStartSprites = root.FindTagsByAttribute("random-start", "true");
				foreach (var sprite in randomStartSprites) {
					string name = "";
					if (sprite.Attributes.ContainsKey("name")) {
						name = sprite.Attributes["name"];
					} else {
						name = "hwport_def";
					}

					// If this is an actor, resolve the converted sprite name first
					if (isActor) {
						name = ResolveActorName(name);
					}

					// Add it to the random start array
					startScene += name + " ";
				}

				// If there were no random-start elements, we gotta improvise
				if (randomStartSprites.Length == 0) {
					var defaultSprite = root.FindTagByNameAndAttribute("sprite", "name", "default");
					if (defaultSprite != null) {
						// Maybe there is a sprite named "default", if so, we use that
						startScene = "default";
					} else if (isActor) {
						// Actors often have idle-0 through idle-7, so that works nicely
						for (int i = 0; i < 8; i++) {
							startScene += "idle-" + i + " ";
						}
					} else {
						//TODO: Something?
						//Console.WriteLine("WARNING: No starting scene found for " + unitName);
					}
				}
			}

			// Get starting scene
			if (startScene == null) {
				if (isProjectile) {
					// Not the best solution I suppose, but it works
					startScene = ResolveProjectileName("0", spriteCount);
				} else {
					startScene = "hwport_def";
				}
			}

			{ // Unit scenes
				writer.WriteLine("  <scenes start=\"{0}\">", startScene.TrimEnd());

				// Start a shared scene for collisions and shadows
				writer.WriteLine("    <scene name=\"hwport_shared\">");

				{ // Collisions in shared scene
					bool startedCollisions = false;
					var collision = root.FindTagByName("collision");

					// Circles
					if (collision != null) {
						bool shootThrough = false;
						if (collision.Attributes.ContainsKey("shoot-through")) {
							shootThrough = bool.Parse(collision.Attributes["shoot-through"]);
						}

						bool staticCollTmp = true;
						if (collision.Attributes.ContainsKey("static")) {
							staticCollTmp = bool.Parse(collision.Attributes["static"]);
						}

						var circles = collision.FindTagsByName("circle");
						foreach (var circle in circles) {
							if (!startedCollisions) {
								writer.WriteLine("      <collision static=\"{0}\">", staticCollTmp ? "true" : "false");
								startedCollisions = true;
							}

							writer.WriteLine("        <circle offset=\"{0}\" radius=\"{1}\"{2}{3} />",
								circle.Attributes["offset"],
								circle.Attributes["radius"],
								sensorColliders ? " sensor=\"true\"" : "",
								shootThrough ? " shoot-through=\"true\"" : ""
							);
						}
					}

					{ // Polygons (can be outside of <collision> tags, because of backwards compatibility)
						var polys = root.FindTagsByName("polygon");
						foreach (var coll in polys) {
							bool shootThrough = false;

							if (!coll.Attributes.ContainsKey("collision")) {
								if (coll.Parent.Name == "collision") {
									if (coll.Parent.Attributes.ContainsKey("shoot-through") && bool.Parse(coll.Parent.Attributes["shoot-through"])) {
										shootThrough = true;
									}
								} else {
									continue;
								}
							} else {
								if (!bool.Parse(coll.Attributes["collision"])) {
									continue;
								}
							}

							// Checkpoints are polygons without a sensor type, so we have to force that here
							if (behavior == "checkpoint") {
								sensorColliders = true;
							}

							if (!startedCollisions) {
								writer.WriteLine("      <collision static=\"{0}\">", staticColl ? "true" : "false");
								startedCollisions = true;
							}

							writer.WriteLine("        <polygon{0}{1}>",
								sensorColliders ? " sensor=\"true\"" : "",
								shootThrough ? " shoot-through=\"true\"" : "");

							var points = coll.FindTagsByName("point");
							foreach (var point in points) {
								writer.WriteLine("          <point>{0}</point>", ConvertCollisionPoint(unitName, point.Value));
							}
							writer.WriteLine("        </polygon>");
						}
					}

					// We have to add our own collision because it's not defined for some types
					if (isActor && !startedCollisions) {
						writer.WriteLine("      <collision static=\"{0}\">", staticColl ? "true" : "false");
						writer.WriteLine("        <circle offset=\"0 0\" aim-through=\"true\" radius=\"{0}\" />", collRadius);
						writer.WriteLine("        <circle offset=\"0 {1}\" sensor=\"true\" shoot-through=\"false\" aim-through=\"true\" radius=\"{0}\" />", collRadius, -collRadius * 1.5f);
						startedCollisions = true;
					} else if (isProjectile && !startedCollisions) {
						writer.WriteLine("      <collision static=\"{0}\">", staticColl ? "true" : "false");
						writer.WriteLine("        <circle offset=\"0 0\" radius=\"{0}\" projectile=\"true\" />", collRadius);
						startedCollisions = true;
					}

					if (startedCollisions) {
						writer.WriteLine("      </collision>");
					}
				}

				{ // Shadows in shared scene
					var shadows = root.FindTagsByName("polygon");
					foreach (var shadow in shadows) {
						if (!shadow.Attributes.ContainsKey("shadow") || !bool.Parse(shadow.Attributes["shadow"])) {
							continue;
						}
						writer.WriteLine("      <shadow darkness=\"1.0\">");
						writer.WriteLine("        <polygon>");
						var points = shadow.FindTagsByName("point");



						int yOffset = SpriteConverter.UnitYOffset(unitName);
						if (yOffset == 0) {
							foreach (var point in points)
								writer.WriteLine("          <point>{0}</point>", point.Value);
						} else {
							foreach (var point in points) {
								var pts = point.Value.Split(' ');
								writer.WriteLine("          <point>{0} {1}</point>", pts[0], (int.Parse(pts[1]) - yOffset));
							}
						}

						writer.WriteLine("        </polygon>");
						writer.WriteLine("      </shadow>");
					}
				}

				{ // Lights in shared scene
					var lights = root.FindTagsByName("light");
					foreach (var light in lights) {
						string origin = "0 0";
						var tagOrigin = light.FindTagByName("origin");
						if (tagOrigin != null) {
							origin = tagOrigin.Value;
						}

						string texture = "system/light_L.png";
						string frame = "0 0 128 128";

						var tagTexture = light.FindTagByName("texture");
						if (tagTexture != null) {
							texture = tagTexture.Value;
							Program.CopyAsset(texture);

							using (Image img = Image.FromFile(Settings.OutputPath + texture)) {
								frame = "0 0 " + img.Width + " " + img.Height;
							}

							texture = Settings.OutputPrefix + texture;
						}

						// We add 2 lights - color1 from mul, and color1 from add
						var rangeTags = light.FindTagsByName("range");
						var colorTags = light.FindTagsByName("color1");

						for (int i = 0; i < 2; i++) {
							var color = colorTags[i].Value.Split(' ');

							int lr, lg, lb;
							int.TryParse(color[0], out lr);
							int.TryParse(color[1], out lg);
							int.TryParse(color[2], out lb);

							lr = Math.Min(255, (int)(Light.srgb_to_linear(lr / 255.0f * 1.45f) * 255.0f));
							lg = Math.Min(255, (int)(Light.srgb_to_linear(lg / 255.0f * 1.45f) * 255.0f));
							lb = Math.Min(255, (int)(Light.srgb_to_linear(lb / 255.0f * 1.45f) * 255.0f));

							writer.WriteLine("      <light pos=\"{0}\">", origin);
							writer.WriteLine("        <sprite texture=\"{0}\">", texture);
							writer.WriteLine("          <frame>{0}</frame>", frame);
							writer.WriteLine("        </sprite>");
							writer.WriteLine("        <length value=\"50\" />");
							writer.WriteLine("        <looping value=\"true\" />");
							writer.WriteLine("        <cast-shadows value=\"false\" />");
							writer.WriteLine("        <shadow-cast-pos value=\"0 0\" />");
							writer.WriteLine("        <shadow-cast-pos-jitter value=\"0 0 0 0\" />");
							writer.WriteLine("        <sizes>");
							writer.WriteLine("          <size value=\"{0}\" />", rangeTags[i].Value);
							writer.WriteLine("        </sizes>");
							writer.WriteLine("        <colors>");
							writer.WriteLine("          <color value=\"{0} {1} {2} {3} {1}\" />", lr, lg, lb, i == 0 ? 0 : 25);
							writer.WriteLine("        </colors>");
							writer.WriteLine("      </light>");
						}
					}
				}

				// End the shared scene
				writer.WriteLine("    </scene>");
				writer.WriteLine();

				// Start making scenes for each sprite
				int pulseCount = 0;
				foreach (var sprite in sprites) {
					bool shouldHaveShared = true;
					bool looping = true;

					if (sprite.Attributes.ContainsKey("name")) {
						string name = sprite.Attributes["name"];

						if (name == "attack-effect")
							shouldHaveShared = false;

						if (isActor && name.StartsWith("attack"))
							looping = false;

						// If this is an actor, we need to put some effort into changing the sprite names
						if (isActor && isActorResolve) {
							name = ResolveActorName(name);
						}

						if (isProjectile) {
							// Projectile hit effect (TODO: use this somehow?)
							if (name.StartsWith("d")) {
								continue;
							}
							name = ResolveProjectileName(name, spriteCount);
						}
						// If this is a BombActorBehavior, we want pulse names to be incremental (this could be done nicer, but this is fine for now)
						if (name == "pulse" && root.Name == "actor" && behavior == "bomb") {
							name = "pulse-" + pulseCount;
							pulseCount++;
						}
						writer.WriteLine("    <scene name=\"{0}\">", name);
					} else {
						writer.WriteLine("    <scene name=\"hwport_def\">");
					}

					if (shouldHaveShared)
						writer.WriteLine("	  <scene src=\"hwport_shared\" />");

					if (isEditor) {
						writer.WriteLine("%if EDITOR");
					}

					SpriteConverter.Convert(sprite, writer, SpriteConverter.GetMaterial(unitName, behavior, slot), looping, unitName);

					if (isEditor) {
						writer.WriteLine("%endif");
					}

					writer.WriteLine("    </scene>");
					writer.WriteLine();
				}

				writer.WriteLine("  </scenes>");
			}

			writer.WriteLine();
			writer.WriteLine("</unit>");
		}

		private static void CreateLoot(string slot, string fullUnitName, XmlTag entry)
		{
			var spread = 0f;
			var origin = new int[2];

			if (entry.Name == "dictionary") {
				// Some units have loot defined as a dictionary with some offset information

				var originArr = entry["string[name=origin]"].Value.Split(' ');
				int.TryParse(originArr[0], out origin[0]);
				int.TryParse(originArr[1], out origin[1]);

				float.TryParse(entry["float[name=spread]"].Value, out spread);

				var useArray = entry["array[name=loot]"];
				for (int i = 0; i < useArray.Children.Count; i++) {
					for (int j = 0; j < useArray.Children[i].Children.Count; j += 2) {
						int chance = int.Parse(useArray.Children[i].Children[j].Value);
						string unit = Program.ChangeExtension(useArray.Children[i].Children[j + 1].Value, "unit");
						LootConverter.Add(chance, unit);
					}

					LootConverter.EndSet(slot, fullUnitName);
				}
			} else {
				// Loot is defined as arrays in HW, but needs to be in a file in SSBD
				for (int i = 0; i < entry.Children.Count; i += 2) {
					int chance = int.Parse(entry.Children[i].Value);
					string unit = Program.ChangeExtension(entry.Children[i + 1].Value, "unit");
					LootConverter.Add(chance, unit);
				}

				LootConverter.EndSet(slot, fullUnitName);
			}
			LootConverter.End(slot, fullUnitName, spread, origin);
		}

		static int FindAttackLength(XmlTag unit)
		{
			var sprite = unit.FindTagByAttribute("name", "east-attack");
			int time = 0;

			foreach (var frame in sprite.Children) {
				if (frame.Name != "frame") {
					continue;
				}

				if (frame.Attributes.ContainsKey("time")) {
					time += int.Parse(frame.Attributes["time"]);
				} else {
					time += 100;
				}
			}

			return time;
		}

		static string ResolveActorName(string name)
		{
			string realName = "idle";
			if (name.Contains('-')) {
				var parse = name.Split(new[] { '-' }, 2);
				name = parse[0];
				realName = parse[1];
			}
			switch (name) {
				case "east": return realName + "-0";
				case "northeast": return realName + "-7";
				case "north": return realName + "-6";
				case "northwest": return realName + "-5";
				case "west": return realName + "-4";
				case "southwest": return realName + "-3";
				case "south": return realName + "-2";
				case "southeast": return realName + "-1";
			}
			return name;
		}

		static string ResolveProjectileName(string name, int spriteCount)
		{
			if (spriteCount == 16) {
				switch (name) {
					case "0": return "idle-8";
					case "1": return "idle-9";
					case "2": return "idle-10";
					case "3": return "idle-11";
					case "4": return "idle-12";
					case "5": return "idle-13";
					case "6": return "idle-14";
					case "7": return "idle-15";
					case "8": return "idle-0";
					case "9": return "idle-1";
					case "10": return "idle-2";
					case "11": return "idle-3";
					case "12": return "idle-4";
					case "13": return "idle-5";
					case "14": return "idle-6";
					case "15": return "idle-7";
				}
			} else if (spriteCount == 8) {
				switch (name) {
					case "0": return "idle-4";
					case "1": return "idle-5";
					case "2": return "idle-6";
					case "3": return "idle-7";
					case "4": return "idle-0";
					case "5": return "idle-1";
					case "6": return "idle-2";
					case "7": return "idle-3";
				}
			} else if (spriteCount == 4) {
				switch (name) {
					case "0": return "idle-2";
					case "1": return "idle-3";
					case "2": return "idle-0";
					case "3": return "idle-1";
				}
			} else if (spriteCount == 1) {
				return "idle-0";
			} else {
				Console.WriteLine("WARNING: Unknown how to handle {0} sprite count", spriteCount);
			}
			return name;
		}

		public static string ConvertCollisionPoint(string unitName, string point)
		{
			if (!Settings.ModifyWallCollision) {
				return point;
			}

			if (unitName.Contains("_h_") || unitName.Contains("_v_") || unitName.Contains("_crn_") || unitName.Contains("_x_") ||
					unitName.Contains("_special_pillar") || unitName.Contains("_special_deteriorate")) {
				var pts = point.Split(' ');
				if ((pts[1])[0] == '-')
					return pts[0] + " 0";

				return point;
			}

			return point;
		}
	}
}
