using Harmony;
using Klei;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TUNING;

namespace OliveOil
{
    class ElementLoaderPatch
    {
        [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
        class ElementLoader_CollectElementsFromYAML
        {
            static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                Strings.Add($"STRINGS.ELEMENTS.{OliveOilElement.OliveOilName.ToUpper()}.NAME", STRINGS.UI.FormatAsLink(OliveOilElement.OliveOilDescr, OliveOilElement.OliveOilName.ToUpper()));
                Strings.Add($"STRINGS.ELEMENTS.{OliveOilElement.OliveOilName.ToUpper()}.DESC", "Olive oil!");
                Strings.Add($"STRINGS.ELEMENTS.{OliveOilElement.FrozenOliveOilName.ToUpper()}.NAME", STRINGS.UI.FormatAsLink(OliveOilElement.FrozenOliveOilDescr, OliveOilElement.FrozenOliveOilName.ToUpper()));
                Strings.Add($"STRINGS.ELEMENTS.{OliveOilElement.FrozenOliveOilName.ToUpper()}.DESC", "Simply frozen olive oil!");

                var elementCollection = YamlIO.Parse<ElementLoader.ElementEntryCollection>(OliveOilElement.CONFIG, new FileHandle());
                __result.AddRange(elementCollection.elements);
            }
        }

        [HarmonyPatch(typeof(ElementLoader), "Load")]
        class ElementLoader_Load
        {
            static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                var water = substanceTable.GetSubstance(SimHashes.Water);
                var ice = substanceTable.GetSubstance(SimHashes.Ice);
                substanceList[OliveOilElement.OliveOilSimHash] = OliveOilElement.CreateOliveOilSubstance(water);
                substanceList[OliveOilElement.FrozenOliveOilSimHash] = OliveOilElement.CreateFrozenOliveOilSubstance(ice.material, ice.anim);
            }
        }
    }

    //[HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
    //class SimHashes_ToString
    //{
    //    static bool Prefix(ref Enum __instance, ref string __result)
    //    {
    //        if (!(__instance is SimHashes)) return true;
    //        return !OliveOilElement.SimHashNameLookup.TryGetValue((SimHashes)__instance, out __result);
    //    }
    //}

    [HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
    class SimHashes_Parse
    {
        static bool Prefix(Type enumType, string value, ref object __result)
        {
            if (!enumType.Equals(typeof(SimHashes))) return true;
            return !OliveOilElement.ReverseSimHashNameLookup.TryGetValue(value, out __result);
        }
    }

    [HarmonyPatch(typeof(EntityConfigManager))]
    [HarmonyPatch(nameof(EntityConfigManager.LoadGeneratedEntities))]
    public class EntityConfigManager_LoadGeneratedEntities_Patch
    {
        public static void Prefix()
        {
            Strings.Add($"STRINGS.CREATURES.SPECIES.{OliveTreeConfig.Name.ToUpperInvariant()}.NAME", UI.FormatAsLink(OliveTreeConfig.Name, OliveTreeConfig.Id));
            Strings.Add($"STRINGS.CREATURES.SPECIES.{OliveTreeConfig.Description.ToUpperInvariant()}.DESC", OliveTreeConfig.Description);
            Strings.Add($"STRINGS.CREATURES.SPECIES.{OliveTreeConfig.DomesticatedDescription.ToUpperInvariant()}.DOMESTICATEDDESC", OliveTreeConfig.DomesticatedDescription);

            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{OliveTreeConfig.SeedName.ToUpperInvariant()}.NAME", UI.FormatAsLink(OliveTreeConfig.SeedName, OliveTreeConfig.Id));
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{OliveTreeConfig.SeedName.ToUpperInvariant()}.DESC", OliveTreeConfig.SeedDescription);

            Strings.Add($"STRINGS.ITEMS.FOOD.{OliveOilRecipe.RecipeName.ToUpperInvariant()}.NAME", UI.FormatAsLink(OliveOilRecipe.RecipeName, OliveOilElement.OliveOilId));
            Strings.Add($"STRINGS.ITEMS.FOOD.{OliveOilRecipe.RecipeName.ToUpperInvariant()}.DESC", OliveOilRecipe.RecipeDescr);
            Strings.Add($"STRINGS.ITEMS.FOOD.{OliveOilRecipe.RecipeName.ToUpperInvariant()}.RECIPEDESC", OliveOilRecipe.RecipeDescr);

            Strings.Add($"STRINGS.ITEMS.FOOD.{OlivesConfig.Id.ToUpperInvariant()}.NAME", UI.FormatAsLink(OlivesConfig.Name, OlivesConfig.Id));
            Strings.Add($"STRINGS.ITEMS.FOOD.{OlivesConfig.Id.ToUpperInvariant()}.DESC", OlivesConfig.Description);


            var domesticatedGrowthTimeInCycles = 1 * 600;
            var producedPerHarvest = 250;
            CROPS.CROP_TYPES.Add(new Crop.CropVal(OlivesConfig.Id, domesticatedGrowthTimeInCycles, producedPerHarvest));
        }
    }

    [HarmonyPatch(typeof(Immigration))]
    [HarmonyPatch("ConfigureCarePackages")]
    public static class Immigration_ConfigureCarePackages_Patch
    {
        public static void Postfix(ref Immigration __instance)
        {
            var field = Traverse.Create(__instance).Field("carePackages");
            var list = field.GetValue<CarePackageInfo[]>().ToList();

            list.Add(new CarePackageInfo(OliveTreeConfig.SeedId, 1f, () => { return GameClock.Instance.GetCycle() >= 1; }));

            field.SetValue(list.ToArray());
        }
    }

    [HarmonyPatch(typeof(MicrobeMusherConfig))]
    [HarmonyPatch("ConfigureBuildingTemplate")]
    public class MicrobeMusherConfig_ConfigureBuildingTemplate_Patch
    {
        public static void Postfix()
        {
            BasicRecipe.AddComplexRecipe
            (
                input: OliveOilRecipe.ingredients,
                output: OliveOilRecipe.results,
                fabricatorId: MicrobeMusherConfig.ID,
                productionTime: 60f,
                recipeDescription: "Olive Oil for dummies!",
                nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                sortOrder: 1000
            );
        }
    }

    [HarmonyPatch(typeof(CookingStationConfig))]
    [HarmonyPatch("ConfigureBuildingTemplate")]
    public class CookingStationConfig_ConfigureBuildingTemplate_Patch
    {
        public static void Postfix()
        {
            BasicRecipe.AddComplexRecipe
            (
                input: DeepFriedMushBarRecipe.ingredients,
                output: DeepFriedMushBarRecipe.results,
                fabricatorId: CookingStationConfig.ID,
                productionTime: 30f,
                recipeDescription: DeepFriedMushBarRecipe.RecipeDescr,
                nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                sortOrder: 1001
            );

            //BasicRecipe.AddComplexRecipe
            //(
            //    input: DeepFriedLiceloafRecipe.ingredients,
            //    output: DeepFriedLiceloafRecipe.results,
            //    fabricatorId: CookingStationConfig.ID,
            //    productionTime: 30f,
            //    recipeDescription: DeepFriedLiceloafRecipe.RecipeDescr,
            //    nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
            //    sortOrder: 1002
            //);

            //BasicRecipe.AddComplexRecipe
            //(
            //    input: FriedLiceloafRecipe.ingredients,
            //    output: FriedLiceloafRecipe.results,
            //    fabricatorId: CookingStationConfig.ID,
            //    productionTime: 20f,
            //    recipeDescription: FriedLiceloafRecipe.RecipeDescr,
            //    nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
            //    sortOrder: 1003
            //);

            BasicRecipe.AddComplexRecipe
            (
                input: FriedMeatRecipe.ingredients,
                output: FriedMeatRecipe.results,
                fabricatorId: CookingStationConfig.ID,
                productionTime: 25f,
                recipeDescription: FriedMeatRecipe.RecipeDescr,
                nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                sortOrder: 1004
            );

            BasicRecipe.AddComplexRecipe
            (
                input: FriedEggRecipe.ingredients,
                output: FriedEggRecipe.results,
                fabricatorId: CookingStationConfig.ID,
                productionTime: 25f,
                recipeDescription: FriedEggRecipe.RecipeDescr,
                nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                sortOrder: 1005
            );

            BasicRecipe.AddComplexRecipe
            (
                input: FriedBristleRecipe.ingredients,
                output: FriedBristleRecipe.results,
                fabricatorId: CookingStationConfig.ID,
                productionTime: 20f,
                recipeDescription: FriedBristleRecipe.RecipeDescr,
                nameDisplayType: ComplexRecipe.RecipeNameDisplay.Result,
                sortOrder: 1006
            );
        }
    }
}
