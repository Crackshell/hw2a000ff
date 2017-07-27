using hw2a000ff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2unit.UnitSkills
{
	public class SpewSkill : UnitSkill
	{
		public string m_anim = "";
		public float m_range = 0.0f;
		public int m_duration = 0;
		public int m_cooldown = 0;
		public string m_projectile = "";
		public float m_spread = 0.0f;
		public int m_rate = 0;

		public override void Write(StreamWriter writer)
		{
			writer.WriteLine("      <dict>");
			writer.WriteLine("        <string name=\"class\">SpewSkill</string>");
			writer.WriteLine("        <string name=\"anim\">{0} 8</string>", m_anim);
			writer.WriteLine("        <int name=\"range\">{0}</int>", (int)m_range);
			writer.WriteLine("        <int name=\"cooldown\">{0}</int>", m_cooldown);
			writer.WriteLine("        <int name=\"duration\">{0}</int>", m_duration);
			writer.WriteLine("        <string name=\"projectile\">{0}</string>", Settings.OutputPrefix + Program.ChangeExtension(m_projectile, "unit"));
			writer.WriteLine("        <float name=\"spread\">{0}</float>", m_spread);
			writer.WriteLine("        <int name=\"rate\">{0}</int>", m_range);
			writer.WriteLine("      </dict>");
		}
	}
}
