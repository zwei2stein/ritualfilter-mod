using Verse;

namespace RitualFilter
{
    [StaticConstructorOnStartup]
    public static class RitualFilterModStatic
    {

        public static string CurrentFilter = "";
        
        static RitualFilterModStatic()
        {
            //Log.Message("[RitualFilter] loading!");
            
            //Run harmony patches
            var harmony = new HarmonyLib.Harmony("RitualFilter");
            harmony.PatchAll();

            Log.Message("[RitualFilter] loaded!");
        }
    }
}