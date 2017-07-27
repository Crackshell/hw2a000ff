using hw2a000ff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2unit
{
	public static class LootConverter
	{
		class LootDefPart : List<List<Tuple<int, string>>>
		{
			public float spread;
			public int[] origin;
		}

		class LootDef : Dictionary<string, LootDefPart>
		{
		}

		private static Dictionary<string, LootDef> m_definitions = new Dictionary<string, LootDef>();
		private static List<Tuple<int, string>> m_currList = new List<Tuple<int, string>>();

		public static void EndSet(string slot, string name)
		{
			if (!m_definitions.ContainsKey(slot)) {
				m_definitions.Add(slot, new LootDef());
			}

			var dic = m_definitions[slot];
			if (!dic.ContainsKey(name)) {
				dic.Add(name, new LootDefPart());
			}

			dic[name].Add(m_currList);
			m_currList = new List<Tuple<int, string>>();
		}

		public static void Add(int chance, string unit)
		{
			m_currList.Add(new Tuple<int, string>(chance, unit));
		}

		public static void End(string slot, string name, float spread, int[] origin)
		{
			if (!m_definitions.ContainsKey(slot)) {
				m_definitions.Add(slot, new LootDef());
			}

			var dic = m_definitions[slot];
			if (!dic.ContainsKey(name)) {
				dic.Add(name, new LootDefPart());
			}

			dic[name].spread = spread;
			dic[name].origin = origin;
		}

		public static string[] GetSlots()
		{
			var ret = new List<string>();
			foreach (var slot in m_definitions) {
				ret.Add(slot.Key);
			}
			return ret.ToArray();
		}

		public static void Convert(string slot, StreamWriter writer)
		{
			writer.WriteLine("<dict>");
			var dic = m_definitions[slot];
			foreach (var def in dic) {
				writer.WriteLine("  <array name=\"{0}\">", Settings.OutputPrefix + def.Key);
				writer.WriteLine("    <vec2>{0} {1}</vec2> <!-- Offset -->", (int)(def.Value.origin[0] * 16.0f), (int)(-def.Value.origin[1] * 16.0f));
				writer.WriteLine("    <vec2>{0} {0}</vec2> <!-- Spread -->", (int)(def.Value.spread * 16.0f));
				foreach (var arr in def.Value) {
					writer.WriteLine("    <array>");
					foreach (var entry in arr) {
						writer.WriteLine("      <int>{0}</int><string>{1}</string>", entry.Item1, Settings.OutputPrefix + entry.Item2);
					}
					writer.WriteLine("    </array>");
				}
				writer.WriteLine("  </array>");
			}
			writer.WriteLine("</dict>");
		}
	}
}
