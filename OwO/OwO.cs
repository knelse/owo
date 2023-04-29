using ModGenesia;
using RogueGenesia.Localization;
using UnityEngine;
using UnityEngine.Localization;
using static System.Text.RegularExpressions.Regex;
using Random = System.Random;

namespace OwO
{
    public class OwOMod : RogueGenesiaMod
    {
        private static readonly Random _rng = new ();
        private static readonly string[] faces = { "(・`ω´・)", ";;w;;", "owo", "UwU", ">w<", "^w^" };
        private static string RndFace() => faces[_rng.Next(0, faces.Length - 1)];
        
        public override void OnModLoaded(ModData modData)
        {
            Debug.Log("OwO loaded");
        }

        public override void OnAllContentLoaded()
        {
            var localizationSO = ScriptableObject.CreateInstance<LocalizationTablesSO>();

            localizationSO.UI = CreateOwofiedStringTable(LocalizationHandler.GetUI);
            localizationSO.SoulCardName = CreateOwofiedStringTable(LocalizationHandler.GetSoulCardName);
            localizationSO.SoulCardDescription = CreateOwofiedStringTable(LocalizationHandler.GetSoulCardDescription);
            localizationSO.ArtifactsName = CreateOwofiedStringTable(LocalizationHandler.GetArtifactsName);
            localizationSO.ArtifactsDescription = CreateOwofiedStringTable(LocalizationHandler.GetArtifactsDescription);
            localizationSO.AchievementName = CreateOwofiedStringTable(LocalizationHandler.GetAchievementName);
            localizationSO.AchievementDescription = CreateOwofiedStringTable(LocalizationHandler.GetAchievementDescription);
            localizationSO.ToolTip = CreateOwofiedStringTable(LocalizationHandler.GetToolTip);
            localizationSO.Talent = CreateOwofiedStringTable(LocalizationHandler.GetTalent);
            localizationSO.Event = CreateOwofiedStringTable(LocalizationHandler.GetEvent);
            
            LocalizationHandler.Init(localizationSO);
        }

        private static LocalizedStringTable CreateOwofiedStringTable(LocalizedStringTable origin)
        {
            var table = origin.GetTable();
            var owofiedTable = new LocalizedStringTable
            {
                TableReference = origin.TableReference
            };
            var owoUiTable = owofiedTable.GetTable();
            foreach (var kvp in table)
            {
                var key = kvp.Key;
                var value = kvp.Value.Value;
                owoUiTable.AddEntry(key, value.Contains("<") ? OwofyWithTags(value) : OwofyWithBracket(value));
            }

            return owofiedTable;
        }
        
        private static string OwofyWithBracket(string input)
        {
            input = Replace(input, "([lr]+(?![^{]*\\}))", "w");
            input = Replace(input, "([LR]+(?![^{]*\\}))", "W");
            input = Replace(input, "(n([aeiou])+(?![^{]*\\}))", "ny$2");
            input = Replace(input, "(N([aeiou])+(?![^{]*\\}))", "Ny$2");
            input = Replace(input, "(N([AEIOU])+(?![^{]*\\}))", "NY$2");
            input = Replace(input, "(ove+(?![^{]*\\}))", "uv");
            input = Replace(input, "[!|?]+", " " + RndFace() + " ");

            return input;
        }

        private static string OwofyWithTags(string input)
        {
            input = Replace(input, "([lr]+(?![^<]*\\>))", "w");
            input = Replace(input, "([LR]+(?![^<]*\\>))", "W");
            input = Replace(input, "(n([aeiou])+(?![^<]*\\>))", "ny$2");
            input = Replace(input, "(N([aeiou])+(?![^<]*\\>))", "Ny$2");
            input = Replace(input, "(N([AEIOU])+(?![^<]*\\>))", "NY$2");
            input = Replace(input, "(ove+(?![^<]*\\>))", "uv");
            input = Replace(input, "[!|?]+", " " + RndFace() + " ");

            return input;
        }
    }
}