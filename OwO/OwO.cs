using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ModGenesia;
using OwO.Pets;
using OwO.Talents;
using RogueGenesia.Data;
using RogueGenesia.GameManager;
using RogueGenesia.Localization;
using UnityEngine;
using UnityEngine.Localization;
using static System.Text.RegularExpressions.Regex;
using static OwO.OwoModOptions;
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
        private static string modFolder = "";

        private static float OwofyUI => GetOwoOptionValue(nameof(Owo_UI));
        private static float OwofySoulCardName => GetOwoOptionValue(nameof(Owo_SoulCardName));
        private static float OwofySoulCardDescription => GetOwoOptionValue(nameof(Owo_SoulCardDescription));
        private static float OwofyArtifactName => GetOwoOptionValue(nameof(Owo_ArtifactName));
        private static float OwofyArtifactDescription => GetOwoOptionValue(nameof(Owo_ArtifactDescription));
        private static float OwofyAchievementName => GetOwoOptionValue(nameof(Owo_AchievementName));
        private static float OwofyAchievementDescription => GetOwoOptionValue(nameof(Owo_AchievementDescription));
        private static float OwofyTooltip => GetOwoOptionValue(nameof(Owo_Tooltip));
        private static float OwofyTalent => GetOwoOptionValue(nameof(Owo_Talent));
        private static float OwofyEvent => GetOwoOptionValue(nameof(Owo_Event));
        private static float OwofyBestiary => GetOwoOptionValue(nameof(Owo_Bestiary));
        private static float InsultFreeRog => GetOwoOptionValue(nameof(Owo_InsultFreeRog));
        private static float StepRog => GetOwoOptionValue(nameof(EvilOwoModOptions.Owo_StepRog));
        private static float PetRog => GetOwoOptionValue(nameof(EvilOwoModOptions.Owo_PetRog));
        private static float Talents => GetOwoOptionValue(nameof(EvilOwoModOptions.Owo_Talents));

        private static readonly Dictionary<OwoModOptions, string> LocaleFriendlyOptionNames = new ()
        {
            [Owo_UI] = "Owofy UI",
            [Owo_SoulCardName] = "Owofy Soul Card Name",
            [Owo_SoulCardDescription] = "Owofy Soul Card Description",
            [Owo_ArtifactName] = "Owofy Artifact Name",
            [Owo_ArtifactDescription] = "Owofy Artifact Description",
            [Owo_AchievementName] = "Owofy Achievement Name",
            [Owo_AchievementDescription] = "Owofy Achievement Description",
            [Owo_Tooltip] = "Owofy Tooltip",
            [Owo_Talent] = "Owofy Talent",
            [Owo_Event] = "Owofy Event",
            [Owo_Bestiary] = "Owofy Bestiary",
            [Owo_InsultFreeRog] = "Use different replacement for Rog (turns out, old was an insult)",
        };

        private static readonly Dictionary<EvilOwoModOptions, string> LocaleFriendlyEvilOptionNames = new ()
        {
            [EvilOwoModOptions.Owo_StepRog] = "Family is important",
            [EvilOwoModOptions.Owo_PetRog] = "Heavy petting zoo",
            [EvilOwoModOptions.Owo_Talents] = "I am a person of many talents",
        };

        public override void OnModLoaded(ModData modData)
        {
            Debug.Log("OwO loaded");
            modFolder = modData.ModDirectory.FullName;
        }

        public override void OnRegisterModdedContent()
        {
            AddOwoPets();
            AddOwoTalents();
        }

        public override void OnAllContentLoaded()
        {
            // 1 iteration over stats vs 1 per mod option
            foreach (var stat in GameData.PersistantGameData.StatsList)
            {
                // will crash on reloads otherwise
                if (!persistentGameDataStats.ContainsKey(stat.Name))
                {
                    persistentGameDataStats.Add(stat.Name, stat.Value);
                }
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

        private void AddOwoTalents()
        {
            var shouldAdd = Talents > 0;
            if (!shouldAdd)
            {
                return;
            }
            
            var talentsPath = Path.Combine(modFolder, "Talents");
            var genderRevealStoneTexturePath = Path.Combine(talentsPath, "gender_reveal_stone.png");
            if (!File.Exists(genderRevealStoneTexturePath))
            {
                Debug.Log("Owo: no talent textures found in mod folder");
                return;
            }
            var nameLocaleList = CreateEnglishLocalization (Owofy(ProcessAvatars("<b><color='magenta'>Gender</color><color='#00EAF7'> Reveal</color> Stone</b>"))).localization;
            var descriptionLocaleList = CreateEnglishLocalization(Owofy(ProcessAvatars("<color='red'>The gale spread</color> <color='orange'>the fire, and</color><color='yellow'> the Gods " +
                "of</color><color='green'> the wild were</color><color='lightblue'> not pleased. 'It's</color>" +
                "<color='blue'> a Rog!' the</color><color='purple'> people shouted.</color>\n" +
                "\n" +
                "In a more evolved society it would have selected a random avatar on run start, " +
                "but we can't have nice things." +
                "\n\n" +
                "<color='#fcd1d1'><i>And Plexus said it doesn't fit the lore...</i></color>"))).localization;

            var genderReavealStone = ContentAPI.AddTalent("GenderRevealStone", typeof(GenderRevealStone).GetConstructor(Array.Empty<Type>()),
                nameLocaleList, descriptionLocaleList, ModGenesia.ModGenesia.LoadSprite(genderRevealStoneTexturePath));
            genderReavealStone.LevelData = new TalentLevelData[100];

            for (var i = 0; i < 100; i++)
            {
                genderReavealStone.LevelData[i] = new TalentLevelData
                {
                    RequiredMastery = (float)(50 * Math.Sqrt(i) * Math.PI * Math.E * i * i)
                };
            }
        }

        private void AddOwoPets()
        {
            var shouldAdd = PetRog > 0;
            if (!shouldAdd)
            {
                return;
            }
            var nudeRoguePath = Path.Combine(modFolder, "Pets", "NudeRogue");
            var idleTexturePath = Path.Combine(nudeRoguePath, "rogue_idle_nude.png");
            var runTexturePath = Path.Combine(nudeRoguePath, "rogue_run_nude.png");
            var iconPath = Path.Combine(nudeRoguePath, "rogue_icon_nude.png");
            if (!File.Exists(idleTexturePath) || !File.Exists(runTexturePath) || !File.Exists(iconPath))
            {
                Debug.Log("Owo: no pet textures found in mod folder");
                return;
            }

            var animations = new PetAnimations
            {
                Icon = ModGenesia.ModGenesia.LoadSprite(iconPath),
                IdleAnimation = new PixelAnimationData
                {
                    Frames = new Vector2Int(8, 1), 
                    Texture = ModGenesia.ModGenesia.LoadPNGTexture(idleTexturePath)
                },
                RunAnimation = new PixelAnimationData
                {
                    Frames = new Vector2Int(8, 1), 
                    Texture = ModGenesia.ModGenesia.LoadPNGTexture(runTexturePath)
                }
            };
            
            // is not added if already exists, i.e. on mod reload
            var pet = PetAPI.AddCustomPet("NudeRog", typeof(NudeRogPet), animations, ERequiredPetDLC.Dog);
            pet.Unlocked = true;
            pet.PetScale = new Vector2(2, 2);
        }

        private void ApplyOwoSettings()
        {
            var localizationSO = ScriptableObject.CreateInstance<LocalizationTablesSO>();

            localizationSO.UI = CreateStringTable(LocalizationHandler.GetUI, OwofyUI);
            localizationSO.SoulCardName = CreateStringTable(LocalizationHandler.GetSoulCardName, OwofySoulCardName);
            localizationSO.SoulCardDescription = CreateStringTable(LocalizationHandler.GetSoulCardDescription, OwofySoulCardDescription);
            localizationSO.ArtifactsName = CreateStringTable(LocalizationHandler.GetArtifactsName, OwofyArtifactName);
            localizationSO.ArtifactsDescription = CreateStringTable(LocalizationHandler.GetArtifactsDescription, OwofyArtifactDescription);
            localizationSO.AchievementName = CreateStringTable(LocalizationHandler.GetAchievementName, OwofyAchievementName);
            localizationSO.AchievementDescription = CreateStringTable(LocalizationHandler.GetAchievementDescription, OwofyAchievementDescription);
            localizationSO.ToolTip = CreateStringTable(LocalizationHandler.GetToolTip, OwofyTooltip);
            localizationSO.Talent = CreateStringTable(LocalizationHandler.GetTalent, OwofyTalent);
            localizationSO.Event = CreateStringTable(LocalizationHandler.GetEvent, OwofyEvent);
            localizationSO.Bestiary = CreateStringTable(LocalizationHandler.GetBestiary, OwofyBestiary);
            
            LocalizationHandler.Init(localizationSO);
        }

        private static float GetOwoOptionValue(string optionName) =>
            (float)GameData.PersistantGameData.GetStatValue(optionName);

        private static void SetOwoOptionValue(string optionName, float value) =>
            GameData.PersistantGameData.SetStat(optionName, value);

        private LocalizedStringTable CreateStringTable(LocalizedStringTable origin, float owofied)
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
                var value = ProcessAvatars(kvp.Value.Value);

                owoUiTable.AddEntry(key, owofied > 0 ? Owofy(value) : value);
            }

            return owofiedTable;
        }

        public static string ProcessAvatars(string value)
        {
            var rogName = InsultFreeRog > 0 ? "Pog" : "Rog";
            if (value.StartsWith("\"Psychological warfare is not considered a warcrime"))
            {
                // dog rog description
                value = rogName + "'s inner fursona. Once hidden and suppressed, now a shining beacon of things to come, " +
                        "harbinger of the new progressive world, symbol of humanity's endless quest for another fetish!\n\n" +
                        "And who would've guessed -- it typically ends up being even more depraved than the previous.\n\n" + value;
            }
            value = value.Replace("Corrupted Avatar", "Corrupted " + rogName);
            value = value.Replace("Knight Rog", "Knight " + rogName);
            value = value.Replace("Rogue", rogName + "ue");
            
            if (StepRog > 0 && !value.Contains("Step-"))
            {
                value = value.Replace("Dog Rog", "Dog, Step-Dog");
                value = value.Replace("Rog", "Step-" + rogName);
                value = value.Replace("Corrupted Avatar", "Corrupted Step-" + rogName);
            }

            return value;
        }
        
        public static string Owofy(string input)
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