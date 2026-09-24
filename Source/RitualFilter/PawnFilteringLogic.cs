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
                if (filterItem.StartsWith("passion:") || filterItem.StartsWith("p:"))
                {
                    var skillName = filterItem.Split(new[] { ':' }, 2)[1];
                    var skill = FindSkill(pawn, skillName);

                    if (skill != null && skill.passion == Passion.None)
                        return false;
                } else if (filterItem.StartsWith("!passion:") || filterItem.StartsWith("!p:") || filterItem.StartsWith("-passion:") || filterItem.StartsWith("-p:"))
                {
                    var skillName = filterItem.Split(new[] { ':' }, 2)[1];
                    var skill = FindSkill(pawn, skillName);

                    if (skill != null && skill.passion != Passion.None)
                        return false;
                } else if (filterItem.StartsWith("trait:") || filterItem.StartsWith("t:"))
                {
                    var traitName = filterItem.Split(new[] { ':' }, 2)[1];
                    var trait = FindTrait(pawn, traitName);

                    if (trait == null)
                        return false;
                } else if (filterItem.StartsWith("!trait:") || filterItem.StartsWith("!t:") || filterItem.StartsWith("-trait:") || filterItem.StartsWith("-t:"))
                {
                    var traitName = filterItem.Split(new[] { ':' }, 2)[1];
                    var trait = FindTrait(pawn, traitName);

                    if (trait != null)
                        return false;
                } else if (filterItem.StartsWith("age>") || filterItem.StartsWith("a>"))
                {
                    try
                    {
                        var age = int.Parse(filterItem.Split('>')[1]);
                        if (pawn.ageTracker.AgeBiologicalYears <= age)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        //Forgive formatting sins and crash attempts
                    }
                } else if (filterItem.StartsWith("age<") || filterItem.StartsWith("a<"))
                {
                    try
                    {
                        var age = int.Parse(filterItem.Split('<')[1]);
                        if (pawn.ageTracker.AgeBiologicalYears >= age)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        //Forgive formatting sins and crash attempts
                    }
                } else if (filterItem.StartsWith("hediff:") || filterItem.StartsWith("h:"))
                {
                    var hediffName = filterItem.Split(new[] { ':' }, 2)[1];

                    if (FindHediff(pawn, hediffName) == null)
                        return false;

                } else if (filterItem.StartsWith("!hediff:") || filterItem.StartsWith("!h:") || filterItem.StartsWith("-hediff:") || filterItem.StartsWith("-h:"))
                {
                    var hediffName = filterItem.Split(new[] { ':' }, 2)[1];

                    if (FindHediff(pawn, hediffName) != null)
                        return false;
                    
                } else if (filterItem.StartsWith("ability:") || filterItem.StartsWith("ab:"))
                {
                    var abilityName = filterItem.Split(new[] { ':' }, 2)[1];
                    
                    if (FindAbility(pawn, abilityName) == null)
                        return false;
                    
                } else if (filterItem.StartsWith("!ability:") || filterItem.StartsWith("!ab:") || filterItem.StartsWith("-ability:") || filterItem.StartsWith("-ab:"))
                {
                    var abilityName = filterItem.Split(new[] { ':' }, 2)[1];
                    
                    if (FindAbility(pawn, abilityName) != null)
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
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        //Forgive formatting sins and crash attempts
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
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        //Forgive formatting sins and crash attempts
                    }                 
                }
                else
                {
                    if (!ContainsIgnoreDiacritics(pawn.Name.ToStringFull.ToLower(), filterItem.Replace('_', ' ')))
                        return false;
                }
            }

            return true;
        }

        private static bool ContainsIgnoreDiacritics(string text, string contained)
        {
            var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace;
            return -1 != CultureInfo.InvariantCulture.CompareInfo.IndexOf(text, contained, options);
        }
        
        private static SkillRecord FindSkill(Pawn pawn, string skillName)
        {
            if (pawn?.skills?.skills == null)
                return null;
            
            return Enumerable.FirstOrDefault(pawn.skills.skills, skill => string.Equals(skill.def.skillLabel, skillName, StringComparison.OrdinalIgnoreCase));
        }
        
        private static Trait FindTrait(Pawn pawn, string traitName)
        {
            if (pawn?.story?.traits?.allTraits == null)
                return null;
            
            return Enumerable.FirstOrDefault(pawn.story.traits.allTraits, trait => string.Equals(trait.Label.Replace(' ', '_'), traitName, StringComparison.OrdinalIgnoreCase));
        }
        
        private static Hediff FindHediff(Pawn pawn, string hediffName)
        {
            if (pawn?.health?.hediffSet?.hediffs == null)
                return null;
            
            return Enumerable.FirstOrDefault(pawn.health.hediffSet.hediffs,
                hediff => ContainsIgnoreDiacritics(hediff.Label.ToLower().Replace(' ', '_'), hediffName.ToLower()));
        }

        private static Ability FindAbility(Pawn pawn, string abilityName)
        {
            if  (pawn?.abilities == null)
                return null;
            
            return Enumerable.FirstOrDefault(pawn.abilities.AllAbilitiesForReading,
                ability => ContainsIgnoreDiacritics(ability.def.label.ToLower().Replace(' ', '_'),
                    abilityName.ToLower()));
        }

    }
}