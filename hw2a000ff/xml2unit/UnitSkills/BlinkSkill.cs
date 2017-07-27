using hw2a000ff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2unit.UnitSkills
{
	public class BlinkSkill : UnitSkill
	{
		public int m_cooldown;
		public float m_range;
		public float m_distance;
		public string m_sound;
		public string m_fx;

		public override void Write(StreamWriter writer)
		{
			writer.WriteLine("      <dict>");
			writer.WriteLine("        <string name=\"class\">BlinkSkill</string>");
			writer.WriteLine("        <int name=\"cooldown\">{0}</int>", m_cooldown);
			writer.WriteLine("        <int name=\"range\">{0}</int>", (int)m_range);
			writer.WriteLine("        <float name=\"distance\">{0}</float>", m_distance);
			if (m_sound != null && m_sound != "") {
				writer.WriteLine("        <string name=\"snd\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(m_sound, "sbnk"));
			}
			if (m_fx != null && m_fx != "") {
				writer.WriteLine("        <string name=\"fx-producer\">{0}</string>", Settings.OutputPrefix + ParticleConverter.Convert(m_fx));
				writer.WriteLine("        <string name=\"fx-name\">{0}</string>", m_fx.Split(':').Last());
			}
			writer.WriteLine("      </dict>");
		}
	}
}
