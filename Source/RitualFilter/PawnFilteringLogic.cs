using System;
using System.Globalization;
using System.Linq;
using RimWorld;
using Verse;

namespace RitualFilter
{
    public static class PawnFilteringLogic
    {
        public static bool PawnIsValid(Pawn pawn)
        {
            var filter = RitualFilterModStatic.CurrentFilter.ToLower().Split(' ');

            foreach (var filterItem in filter)
            {
                if (filterItem.StartsWith("passion:"))
                {
                    var skillName = filterItem.Split(':')[1];
                    var skill = FindSkill(pawn, skillName);

                    if (skill != null && skill.passion == Passion.None)
                        return false;
                } else if (filterItem.StartsWith("!passion:"))
                {
                    var skillName = filterItem.Split(':')[1];
                    var skill = FindSkill(pawn, skillName);

                    if (skill != null && skill.passion != Passion.None)
                        return false;
                } else if (filterItem.StartsWith("trait:"))
                {
                    var traitName = filterItem.Split(':')[1];
                    var trait = FindTrait(pawn, traitName);

                    if (trait == null)
                        return false;
                } else if (filterItem.StartsWith("!trait:"))
                {
                    var traitName = filterItem.Split(':')[1];
                    var trait = FindTrait(pawn, traitName);

                    if (trait != null)
                        return false;
                } else if (filterItem.StartsWith("age>"))
                {
                    try
                    {
                        var age = int.Parse(filterItem.Split('>')[1]);
                        if (pawn.ageTracker.AgeBiologicalYears <= age)
                        {
                            return false;
                        }
                    }
                    catch (FormatException)
                    {
                        //Forgive formatting sins
                    }
                } else if (filterItem.StartsWith("age<"))
                {
                    try
                    {
                        var age = int.Parse(filterItem.Split('<')[1]);
                        if (pawn.ageTracker.AgeBiologicalYears >= age)
                        {
                            return false;
                        }
                    }
                    catch (FormatException)
                    {
                        //Forgive formatting sins
                    }
                } else if (filterItem.StartsWith("hediff:"))
                {
                    var hediffName = filterItem.Split(':')[1];

                    if (!Enumerable.Any(pawn.health.hediffSet.hediffs, hediff => hediff.Label.ToLower().Replace(' ', '_').Contains(hediffName.ToLower())))
                        return false;

                } else if (filterItem.StartsWith("!hediff:"))
                {
                    var hediffName = filterItem.Split(':')[1];

                    if (Enumerable.Any(pawn.health.hediffSet.hediffs, hediff => hediff.Label.ToLower().Replace(' ', '_').Contains(hediffName.ToLower())))
                        return false;

                } else if (filterItem.Contains(">"))
                {
                    try
                    {
                        var parts = filterItem.Split('>');
                        var value = int.Parse(parts[1]);
                        var skill = FindSkill(pawn, parts[0]);

                        if (skill != null && skill.GetLevel() <= value)
                            return false;
                    }
                    catch (FormatException)
                    {
                        //Forgive formatting sins
                    }
                } else if (filterItem.Contains("<"))
                {
                    try
                    {
                        var parts = filterItem.Split('<');
                        var value = int.Parse(parts[1]);
                        var skill = FindSkill(pawn, parts[0]);

                        if (skill != null && skill.GetLevel() >= value)
                            return false;
                    }
                    catch (FormatException)
                    {
                        //Forgive formatting sins
                    }                    
                }
                else
                {
                    if (!ConstainsIgnoreDiacritics(pawn.Name.ToStringFull.ToLower(), filterItem.Replace('_', ' ')))
                        return false;
                }
            }

            return true;
        }
        
        private static bool ConstainsIgnoreDiacritics(string text, string contained)
        {
            var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace;
            return -1 != CultureInfo.InvariantCulture.CompareInfo.IndexOf(text, contained, options);
        }
        
        private static SkillRecord FindSkill(Pawn pawn, string skillName)
        {
            return Enumerable.FirstOrDefault(pawn.skills.skills, skill => skill.def.skillLabel == skillName);
        }
        
        private static Trait FindTrait(Pawn pawn, string traitName)
        {
            return Enumerable.FirstOrDefault(pawn.story.traits.allTraits, trait => trait.Label.Replace(' ', '_').ToLower() == traitName);
        }

    }
}