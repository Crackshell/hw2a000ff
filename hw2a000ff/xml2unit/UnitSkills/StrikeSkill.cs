using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hw2a000ff;

namespace xml2unit.UnitSkills
{
	public class StrikeSkill : UnitSkill
	{
		public string m_anim = "";
		public int m_cooldown = 0;
		public float m_range = 0.0f;
		public int m_damage = 0;

		public override void Write(StreamWriter writer)
		{
			writer.WriteLine("      <dict>");
			writer.WriteLine("        <string name=\"class\">EnemyMeleeStrike</string>");
			writer.WriteLine();
			writer.WriteLine("        <string name=\"anim\">{0} 8</string>", m_anim);
			if (m_doSnd != "") {
				writer.WriteLine("        <string name=\"snd\">{0}</string>", Settings.OutputPrefix + m_doSnd);
			}
			writer.WriteLine();
			writer.WriteLine("        <int name=\"cooldown\">{0}</int>", m_cooldown);
			writer.WriteLine("        <int name=\"range\">{0}</int>", m_range);
			writer.WriteLine("        <dict name=\"effect\">");
			writer.WriteLine("          <string name=\"class\">Damage</string>");
			writer.WriteLine("          <int name=\"dmg\">{0}</int>", m_damage * Settings.DamageScale);
			writer.WriteLine("          <string name=\"dmg-type\">pierce</string>");
			writer.WriteLine("        </dict>");
			writer.WriteLine("      </dict>");
		}
	}
}
