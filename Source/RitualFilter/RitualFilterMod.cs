using UnityEngine;
using Verse;

namespace RitualFilter
{
    public class RitualFilterMod : Mod
    {
        public RitualFilterModSettings settings;

        public RitualFilterMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<RitualFilterModSettings>();
        }

        public override string SettingsCategory()
        {
            return "RitualFilterModName".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            
            //This is bad, evil solution, but best I have:
            if (Find.WindowStack != null)
            {
                foreach (Window window in Find.WindowStack.Windows)
                {
                    window.closeOnAccept = false;
                }
            }

            var listingStandard = new Listing_Standard();

            var gapWidth = 12f;

            listingStandard.Begin(inRect);

            listingStandard.Label("RitualFilterModName_Presets_Title".Translate());

            listingStandard.Indent(gapWidth);
            listingStandard.ColumnWidth -= gapWidth;

            var rect = listingStandard.GetRect(Text.LineHeight * 10); // 10 text lines
            RitualFilterModSettings.filterPresets = GUI.TextArea(
                rect,
                RitualFilterModSettings.filterPresets
            );

            listingStandard.GapLine();

            listingStandard.Label("RitualFilterModName_Presets_HowTo".Translate());
            
            listingStandard.GapLine();

            listingStandard.Label("RitualFilter_Help_Tooltip".Translate());
                
            listingStandard.Outdent(gapWidth);
            listingStandard.ColumnWidth += gapWidth;

            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }
    }
}