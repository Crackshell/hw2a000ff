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
	public static class StringConverter
	{
		public static void Convert(XmlFile xml, StreamWriter writer)
		{
			var root = xml.Root.Children[0];

			writer.WriteLine("<lang>");

			foreach (var child in root.Children) {
				if (child.Name != "string") {
					continue;
				}
				writer.WriteLine("  <string name=\"{0}\">{1}</string>", Settings.StringsKeyPrefix + child.Attributes["name"], child.Value);
			}

			writer.WriteLine("</lang>");
		}
	}
}
