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
	public static class SpriteConverter
	{
		public static string GetMaterial(string unitName, string behavior, string slot)
		{
			if (behavior == "money")
				return "item-money";
			if (behavior == "breakable")
				return "proj-prop";

			if (unitName.Contains("round_crn"))
				return slot;
			if (unitName.Contains("pillar_off_up"))
				return slot;
			if (unitName.Contains("stone_up"))
				return slot;

			if (unitName.Contains("_overhang"))
				return "wall";

			if (unitName.Contains("_h_"))
				return "proj-wall";

			if (unitName.Contains("_v_")) {
				if (unitName.Contains("_v_cap_dn"))
					return "proj-wall";

				return "wall";
			}

			if (unitName.Contains("_crn_l_dn"))
				return "proj-wall";
			if (unitName.Contains("_crn_r_dn"))
				return "proj-wall";

			if (unitName.Contains("_x_t_")) {
				if (unitName.Contains("_x_t_dn"))
					return "proj-wall";

				return "wall";
			}

			if (unitName.Contains("_exit_h_"))
				return "proj-wall";

			if (unitName.Contains("_special_")) {
				if (unitName.Contains("_special_pillar"))
					return "proj-wall";
				if (unitName.Contains("_special_eyes"))
					return "proj-wall";
				if (unitName.Contains("_special_deteriorate"))
					return "proj-wall";
			}

			if (unitName.Contains("_floorsign_"))
				return "proj-prop";
			if (unitName.Contains("_ivy_"))
				return "proj-prop";
			if (unitName.Contains("_chains_"))
				return "proj-prop";
			if (unitName.Contains("trap_shooter_"))
				return "proj-prop";
			if (unitName.Contains("trap_spikes"))
				return "floor";
			if (unitName.Contains("trap_turret"))
				return "floor";
			if (unitName.Contains("maggot_slime"))
				return "floor";

			if (unitName.Contains("deco_"))
				return "proj-prop";
			if (unitName.Contains("marker"))
				return "floor";
			if (unitName.Contains("vendor_speech"))
				return slot;
			if (unitName.Contains("vendor_"))
				return "proj-prop";
			if (unitName.Contains("floor"))
				return "floor";

			return slot;
		}

		public static int UnitYOffset(string unitName)
		{
			if (!Settings.ModifyWallCollision || string.IsNullOrEmpty(unitName)) {
				return 0;
			}

			if ((unitName.Contains("_crn_") && unitName.Contains("_up")) ||
				(unitName.Contains("_x_") && !unitName.Contains("_x_t_dn")) ||
				(unitName.Contains("_v_") && !unitName.Contains("_v_cap_dn"))) {
				return 16;
			}

			return 0;
		}

		public static string GetOrigin(string unitName, XmlTag tag)
		{
			if (tag == null)
				return "0 0";

			int offset = UnitYOffset(unitName);
			if (offset != 0) {
				var pts = tag.Value.Split(' ');
				return pts[0] + ' ' + (int.Parse(pts[1]) + offset);
			}

			return tag.Value;
		}

		public static void Convert(XmlTag sprite, StreamWriter writer, string material, bool looping = true, string unitName = "")
		{
			string texture = sprite.FindTagByName("texture").Value;
			Program.CopyAsset(texture);

			var tagOrigin = sprite.FindTagByName("origin");

			writer.WriteLine("      <sprite origin=\"{0}\" looping=\"{3}\" texture=\"{1}\" material=\"{4}system/hammerwatch.mats:{2}\">",
								GetOrigin(unitName, tagOrigin),
				Settings.OutputPrefix + texture,
				material,
				looping ? "true" : "false",
				Settings.OutputPrefix
			);
			foreach (var frame in sprite.Children) {
				if (frame.Name != "frame") {
					continue;
				}
				if (frame.Attributes.ContainsKey("time")) {
					writer.WriteLine("        <frame time=\"{0}\">{1}</frame>", frame.Attributes["time"], frame.Value);
				} else {
					writer.WriteLine("        <frame>{0}</frame>", frame.Value);
				}
			}
			writer.WriteLine("      </sprite>");
			var glow = sprite.FindTagByName("glow");
			if (glow != null) {
				writer.WriteLine("      <sprite origin=\"{0}\" looping=\"{2}\" texture=\"{1}\" material=\"{3}system/hammerwatch.mats:glow\">",
					sprite.FindTagByName("origin").Value,
					Settings.OutputPrefix + texture,
					looping ? "true" : "false",
					Settings.OutputPrefix
				);

				foreach (var frame in glow.Children) {
					if (frame.Name != "frame") {
						continue;
					}
					if (frame.Attributes.ContainsKey("time")) {
						writer.WriteLine("        <frame time=\"{0}\">{1}</frame>", frame.Attributes["time"], frame.Value);
					} else {
						writer.WriteLine("        <frame>{0}</frame>", frame.Value);
					}
				}
				writer.WriteLine("      </sprite>");
			}
		}
		/*
		public static void ConvertToShadow(XmlTag sprite, StreamWriter writer, bool looping = true, string unitName = "")
		{
			string texture = sprite.FindTagByName("texture").Value;
			Program.CopyAsset(texture);

			var tagOrigin = sprite.FindTagByName("origin");

			writer.WriteLine("      <shadow ignore-at-no-height=\"true\">");
			writer.WriteLine("        <sprite origin=\"{0}\" looping=\"{2}\" texture=\"{1}\" >",
				GetOrigin(unitName, tagOrigin),
				Settings.OutputPrefix + texture,
				looping ? "true" : "false"
			);

			foreach (var frame in sprite.Children) {
				if (frame.Name != "frame")
					continue;

				if (frame.Attributes.ContainsKey("time"))
					writer.WriteLine("        <frame time=\"{0}\">{1}</frame>", frame.Attributes["time"], frame.Value);
				else
					writer.WriteLine("        <frame>{0}</frame>", frame.Value);
			}

			writer.WriteLine("        </sprite>");
			writer.WriteLine("      </shadow>");
		}
		*/
	}
}
