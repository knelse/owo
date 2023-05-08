using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ModGenesia;
using RogueGenesia.Data;
using RogueGenesia.GameManager;
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
        private const string _modOptionPrefix = "Owo_";
        private static readonly Dictionary<string, double> persistentGameDataStats = new();

        private static float OwofyUI => GetOwoOptionValue(nameof(OwoModOptions.Owo_UI));
        private static float OwofySoulCardName => GetOwoOptionValue(nameof(OwoModOptions.Owo_SoulCardName));
        private static float OwofySoulCardDescription => GetOwoOptionValue(nameof(OwoModOptions.Owo_SoulCardDescription));
        private static float OwofyArtifactName => GetOwoOptionValue(nameof(OwoModOptions.Owo_ArtifactName));
        private static float OwofyArtifactDescription => GetOwoOptionValue(nameof(OwoModOptions.Owo_ArtifactDescription));
        private static float OwofyAchievementName => GetOwoOptionValue(nameof(OwoModOptions.Owo_AchievementName));
        private static float OwofyAchievementDescription => GetOwoOptionValue(nameof(OwoModOptions.Owo_AchievementDescription));
        private static float OwofyTooltip => GetOwoOptionValue(nameof(OwoModOptions.Owo_Tooltip));
        private static float OwofyTalent => GetOwoOptionValue(nameof(OwoModOptions.Owo_Talent));
        private static float OwofyEvent => GetOwoOptionValue(nameof(OwoModOptions.Owo_Event));
        private static float StepRog => GetOwoOptionValue(nameof(EvilOwoModOptions.Owo_StepRog));

        private static readonly Dictionary<OwoModOptions, string> LocaleFriendlyOptionNames = new ()
        {
            [OwoModOptions.Owo_UI] = "Owofy UI",
            [OwoModOptions.Owo_SoulCardName] = "Owofy Soul Card Name",
            [OwoModOptions.Owo_SoulCardDescription] = "Owofy Soul Card Description",
            [OwoModOptions.Owo_ArtifactName] = "Owofy Artifact Name",
            [OwoModOptions.Owo_ArtifactDescription] = "Owofy Artifact Description",
            [OwoModOptions.Owo_AchievementName] = "Owofy Achievement Name",
            [OwoModOptions.Owo_AchievementDescription] = "Owofy Achievement Description",
            [OwoModOptions.Owo_Tooltip] = "Owofy Tooltip",
            [OwoModOptions.Owo_Talent] = "Owofy Talent",
            [OwoModOptions.Owo_Event] = "Owofy Event",
        };

        private static readonly Dictionary<EvilOwoModOptions, string> LocaleFriendlyEvilOptionNames = new ()
        {
            [EvilOwoModOptions.Owo_StepRog] = "Family is important",
        };

        public override void OnModLoaded(ModData modData)
        {
            Debug.Log("OwO loaded");
        }

        public override void OnAllContentLoaded()
        {
            // 1 iteration over stats vs 1 per mod option
            foreach (var stat in GameData.PersistantGameData.StatsList)
            {
                persistentGameDataStats.Add(stat.Name, stat.Value);
            }

            foreach (var localeOption in LocaleFriendlyOptionNames)
            {
                var key = localeOption.Key.ToString();
                if (!persistentGameDataStats.ContainsKey(key))
                {
                    persistentGameDataStats[key] = 1;
                    GameData.PersistantGameData.SetStat(key, 1);
                }

                var modOption = ModOption.MakeToggleOption(key,
                    CreateEnglishLocalization(localeOption.Value), true, CreateEnglishLocalization(localeOption.Value));
                ModOption.AddModOption(modOption, "Settings UwU", "OwO");
            }
            
            foreach (var localeOption in LocaleFriendlyEvilOptionNames)
            {
                var key = localeOption.Key.ToString();
                if (!persistentGameDataStats.ContainsKey(key))
                {
                    persistentGameDataStats[key] = 1;
                    GameData.PersistantGameData.SetStat(key, 1);
                }
                var modOption = ModOption.MakeToggleOption(key,
                    CreateEnglishLocalization(localeOption.Value), true, CreateEnglishLocalization(localeOption.Value));
                ModOption.AddModOption(modOption, "Evil Settings oNo", "OwO");
            }
            
            ApplyOwoSettings();
            
            GameEventManager.OnOptionConfirmed.AddListener(Owofy_OnOptionConfirmed);
        }

        private void Owofy_OnOptionConfirmed(string name, float value)
        {
            if (!name.StartsWith(_modOptionPrefix))
            {
                return;
            }

            if (!Enum.TryParse(name, true, out OwoModOptions _) &&
                !Enum.TryParse(name, true, out EvilOwoModOptions _))
            {
                return;
            }
            
            SetOwoOptionValue(name, value);
            ApplyOwoSettings();
        }

        private void ApplyOwoSettings()
        {
            var localizationSO = ScriptableObject.CreateInstance<LocalizationTablesSO>();
            var stepRog = StepRog;

            localizationSO.UI = CreateStringTable(LocalizationHandler.GetUI, OwofyUI, stepRog);
            localizationSO.SoulCardName = CreateStringTable(LocalizationHandler.GetSoulCardName, OwofySoulCardName, stepRog);
            localizationSO.SoulCardDescription = CreateStringTable(LocalizationHandler.GetSoulCardDescription, OwofySoulCardDescription, stepRog);
            localizationSO.ArtifactsName = CreateStringTable(LocalizationHandler.GetArtifactsName, OwofyArtifactName, stepRog);
            localizationSO.ArtifactsDescription = CreateStringTable(LocalizationHandler.GetArtifactsDescription, OwofyArtifactDescription, stepRog);
            localizationSO.AchievementName = CreateStringTable(LocalizationHandler.GetAchievementName, OwofyAchievementName, stepRog);
            localizationSO.AchievementDescription = CreateStringTable(LocalizationHandler.GetAchievementDescription, OwofyAchievementDescription, stepRog);
            localizationSO.ToolTip = CreateStringTable(LocalizationHandler.GetToolTip, OwofyTooltip, stepRog);
            localizationSO.Talent = CreateStringTable(LocalizationHandler.GetTalent, OwofyTalent, stepRog);
            localizationSO.Event = CreateStringTable(LocalizationHandler.GetEvent, OwofyEvent, stepRog);
            
            LocalizationHandler.Init(localizationSO);
        }

        private static float GetOwoOptionValue(string optionName) =>
            (float)GameData.PersistantGameData.GetStatValue(optionName);

        private static void SetOwoOptionValue(string optionName, float value) =>
            GameData.PersistantGameData.SetStat(optionName, value);

        private LocalizedStringTable CreateStringTable(LocalizedStringTable origin, float owofied, float stepRog)
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
                if (stepRog > 0 && !value.Contains("Step-"))
                {
                    value = value.Replace("Rog", "Step-Rog");
                    value = value.Replace("Corrupted Avatar", "Corrupted Step-Rog");
                }

                owoUiTable.AddEntry(key, owofied > 0 ? Owofy(value) : value);
            }

            return owofiedTable;
        }
        
        private static string Owofy(string input)
        {
            input = Replace(input, "([lr](?![^<{]*[>}]))", "w", RegexOptions.Compiled);
            input = Replace(input, "([LR](?![^<{]*[>}]))", "W", RegexOptions.Compiled);
            input = Replace(input, "(n([aeiou])(?![^<{]*[>}]))", "ny$2", RegexOptions.Compiled);
            input = Replace(input, "(N([aeiou])(?![^<{]*[>}]))", "Ny$2", RegexOptions.Compiled);
            input = Replace(input, "(N([AEIOU])(?![^<{]*[>}]))", "NY$2", RegexOptions.Compiled);
            input = Replace(input, "(ove(?![^<{]*[>}]))", "uv", RegexOptions.Compiled);
            input = Replace(input, "[!|?]", " " + RndFace() + " ");

            return input;
        }

        private static LocalizationDataList CreateEnglishLocalization(string defaultValue)
        {
            return new LocalizationDataList(defaultValue)
            {
                localization = new List<LocalizationData>
                {
                    new()
                    {
                        Key = "en",
                        Value = defaultValue
                    }
                }
            };
        }
    }
}