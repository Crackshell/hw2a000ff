using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw2a000ff
{
	public static class Settings
	{
		public static string SourcePath;
		public static string SourceFallbackPath;
		public static string LevelsPath;
		public static string OutputPath;
		public static string OutputPrefix;

		public static bool ConvertActors;
		public static bool ConvertProjectiles;
		public static bool ConvertDoodads;
		public static bool ConvertTilesets;
		public static bool ConvertItems;
		public static bool ConvertStrings;
		public static bool ConvertSpeechStyles;
		public static bool ConvertFonts;
		public static bool ConvertLoot;
		public static bool ConvertLevels;
		public static bool ConvertSounds;

		public static float HealthScale;
		public static float RangeScale;
		public static float DamageScale;
		public static float SpeedScale;

		public static bool ModifyWallCollision;

		public static string StringsKeyPrefix;
	}
}
