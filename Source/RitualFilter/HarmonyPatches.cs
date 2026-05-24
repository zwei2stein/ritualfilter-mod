using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RitualFilter
{

    [HarmonyPatch(typeof(Dialog_BeginLordJob), nameof(Dialog_BeginLordJob.DoLeftColumn))]
    public class Dialog_BeginLordJobPatch
    {
        static void Prefix(ref RectDivider layout)
        {
            var row = layout.NewRow(Text.LineHeight);
            var col1 = row.NewCol(layout.Rect.width - (18f + 4f), marginOverride: new float?(4.0f));
            var col2 = row.NewCol(18f, marginOverride: new float?(0.0f));
            
            RitualFilterModStatic.CurrentFilter = Widgets.TextEntryLabeled(col1, "RitualFilterModName_Field_Filter_Name".Translate(), RitualFilterModStatic.CurrentFilter);
            
            if (Widgets.ButtonImage(col2, TexButton.CloseXSmall, tooltip:"RitualFilterModName_Tooltip_Clear".Translate()))
            {
                RitualFilterModStatic.CurrentFilter = "";
            }

        }
    }
    
    [HarmonyPatch(typeof(RitualRoleAssignments), nameof(RitualRoleAssignments.CanParticipate))]
    public class ShouldGrayOutPatch1
    {
        static bool Prefix(Pawn pawn, out TaggedString reason, ref bool __result)
        {
            if (string.IsNullOrEmpty(RitualFilterModStatic.CurrentFilter))
                return true;
            
            if (!PawnFilteringLogic.PawnIsValid(pawn))
            {
                __result = false;
                reason = new TaggedString("RitualFilterModName_Message_FilteredOut".Translate());
                return false;
            }
            // otherwise, do the normal logic
            return true;
        }
    }

    [HarmonyPatch(typeof(PsychicRitualRoleAssignments), nameof(PsychicRitualRoleAssignments.CanParticipate))]
    public class ShouldGrayOutPatch2
    {
        static bool Prefix(Pawn pawn, out TaggedString reason, ref bool __result)
        {
            if (string.IsNullOrEmpty(RitualFilterModStatic.CurrentFilter))
                return true;
            
            if (!PawnFilteringLogic.PawnIsValid(pawn))
            {
                __result = false;
                reason = new TaggedString("RitualFilterModName_Message_FilteredOut".Translate());
                return false;
            }
            // otherwise, do the normal logic
            return true;
        }
    }
    
}