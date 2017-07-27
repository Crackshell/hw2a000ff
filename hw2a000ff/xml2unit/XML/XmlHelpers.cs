using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;

using Nimble.Extensions;

namespace Nimble.XML
{
	internal class XmlParseAttributesResult
	{
		public Dictionary<string, string> attributes = new Dictionary<string, string>();
		public bool bOpenTag = false;
	}

	internal static class XmlHelpers
	{
		internal static XmlParseAttributesResult ParseAttributes(StreamReader fs)
		{
			var ret = new XmlParseAttributesResult();

			var strKey = new StringBuilder();
			var strValue = new StringBuilder();
			bool bReadingKey = true;
			bool bReadingString = false;

			while (!fs.EndOfStream) {
				// "version="1.0" encoding="UTF-8"?>"
				char c = fs.ReadChar();

				if (bReadingKey) {
					// "<test a b c/>"
					// "<test a="b"/>"
					// "<test a=b/>"
					// "<test a ="b"/>"
					if (c == '=') {
						bReadingKey = false;
						if (fs.PeekChar() == '"') {
							bReadingString = true;
							fs.ReadChar();
						}
					} else if (c == ' ') {
						if (strKey.Length > 0) {
							if (fs.PeekChar() == '=') {
								bReadingKey = false;
								fs.ReadChar();
								if (fs.PeekChar() == '"') {
									bReadingString = true;
									fs.ReadChar();
								}
							} else {
								// add empty attribute
								ret.attributes.Add(strKey.ToString(), "");
								strKey.Clear();
							}
						}
					} else if (c == '/' || c == '?') {
						Debug.Assert(fs.Expect('>'));
						ret.bOpenTag = false;
						break;
					} else if (c == '>') {
						ret.bOpenTag = true;
						break;
					} else {
						if (c != '\r' && c != '\n' && c != '\t') {
							strKey.Append(c);
						}
					}
				} else {
					bool bAddAttrib = false;

					if (c == '"' || (c == ' ' && !bReadingString) || (c == '/' && !bReadingString)) {
						bAddAttrib = true;
					} else {
						strValue.Append(c);
					}

					if (bAddAttrib) {
						bReadingKey = true;
						bReadingString = false;

						// add attribute with value
						ret.attributes.Add(strKey.ToString(), WebUtility.HtmlDecode(strValue.ToString()));

						strKey.Clear();
						strValue.Clear();
					}
				}
			}

			return ret;
		}
	}
}
