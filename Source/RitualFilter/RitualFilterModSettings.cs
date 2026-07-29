using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RitualFilter
{
    public class RitualFilterModSettings : ModSettings
    {
        public static string filterPresets = "";

        public static List<string> FilterPresetsList()
        {
            return new List<string>(filterPresets.Split('\n').Where(x => !string.IsNullOrEmpty(x)));
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<string>(ref filterPresets, "filterPresets", "");

            base.ExposeData();
        }
    }
}