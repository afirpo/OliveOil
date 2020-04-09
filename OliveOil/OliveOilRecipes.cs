using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OliveOil
{
    public abstract class BasicRecipe
    {
        public static ComplexRecipe AddComplexRecipe(ComplexRecipe.RecipeElement[] input, ComplexRecipe.RecipeElement[] output,
            string fabricatorId, float productionTime, string recipeDescription, ComplexRecipe.RecipeNameDisplay nameDisplayType, int sortOrder, string requiredTech = null)
        {
            var recipeId = ComplexRecipeManager.MakeRecipeID(fabricatorId, input, output);

            return new ComplexRecipe(recipeId, input, output)
            {
                time = productionTime,
                description = recipeDescription,
                nameDisplay = nameDisplayType,
                fabricators = new List<Tag> { fabricatorId },
                sortOrder = sortOrder,
                requiredTech = requiredTech
            };
        }
    }

    public class OliveOilRecipe : BasicRecipe
    {
        public static readonly string RecipeName = OliveOilElement.OliveOilDescr;
        public static readonly string RecipeDescr = RecipeName;

        public static readonly ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[]
        {
          new ComplexRecipe.RecipeElement(OlivesConfig.Name.ToTag(), 1000f)
        };

        public static readonly ComplexRecipe.RecipeElement[] results = new ComplexRecipe.RecipeElement[]
        {
            new ComplexRecipe.RecipeElement(OliveOilElement.OliveOilName.ToTag(), 500f)
        };
    }

    public class DeepFriedMushBarRecipe : BasicRecipe
    {
        public static readonly string RecipeName = "DeepFriedMushBarRecipe";
        public static readonly string RecipeDescr = "A deep fried Mush Bar, of course!";

        public static readonly ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[]
        {
          new ComplexRecipe.RecipeElement(MushBarConfig.ID.ToTag(), 1f),
          new ComplexRecipe.RecipeElement(OliveOilElement.OliveOilName.ToTag(), 500f),
        };

        public static readonly ComplexRecipe.RecipeElement[] results = FriedMushBarConfig.recipe.results;
    }

    public class FriedLiceloafRecipe : BasicRecipe
    {
        public static readonly string RecipeName = "FriedLiceloafRecipe";
        public static readonly string RecipeDescr = "A fried mess baked by three meal lices.";

        public static readonly ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[]
        {
          new ComplexRecipe.RecipeElement(BasicPlantFoodConfig.ID.ToTag(), 3f),
          new ComplexRecipe.RecipeElement(OliveOilElement.OliveOilName.ToTag(), 250f),
        };
        
        public static readonly ComplexRecipe.RecipeElement[] results = BasicPlantBarConfig.recipe.results;
    }

    public class FriedBristleRecipe : BasicRecipe
    {
        public static readonly string RecipeName = "FriedBristleRecipe";
        public static readonly string RecipeDescr = "A nicely fried bristle blossom triplet.";

        public static readonly ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[]
        {
          new ComplexRecipe.RecipeElement(RawEggConfig.ID.ToTag(), 3f),
          new ComplexRecipe.RecipeElement(OliveOilElement.OliveOilName.ToTag(), 250f),
        };

        public static readonly ComplexRecipe.RecipeElement[] results = GrilledPrickleFruitConfig.recipe.results;
    }

    public class DeepFriedLiceloafRecipe : BasicRecipe
    {
        public static readonly string RecipeName = "DeepFriedLiceloafRecipe";
        public static readonly string RecipeDescr = "A deep fried meal lice, of course!";

        public static readonly ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[]
        {
          new ComplexRecipe.RecipeElement(BasicPlantBarConfig.ID.ToTag(), 1f),
          new ComplexRecipe.RecipeElement(OliveOilElement.OliveOilName.ToTag(), 500f),
        };

        public static readonly ComplexRecipe.RecipeElement[] results = FruitCakeConfig.recipe.results;
    }

    public class FriedMeatRecipe : BasicRecipe
    {
        public static readonly string RecipeName = "FriedMeatRecipe";
        public static readonly string RecipeDescr = "A chunk of meat carefully fried.";

        public static readonly ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[]
        {
          new ComplexRecipe.RecipeElement(MeatConfig.ID.ToTag(), 1f),
          new ComplexRecipe.RecipeElement(OliveOilElement.OliveOilName.ToTag(), 150f),
        };

        public static readonly ComplexRecipe.RecipeElement[] results = CookedMeatConfig.recipe.results;
    }

    public class FriedEggRecipe : BasicRecipe
    {
        public static readonly string RecipeName = "FriedEggRecipe";
        public static readonly string RecipeDescr = "A nicely fried couple of eggs";

        public static readonly ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[]
        {
          new ComplexRecipe.RecipeElement(RawEggConfig.ID.ToTag(), 2f),
          new ComplexRecipe.RecipeElement(OliveOilElement.OliveOilName.ToTag(), 300f),
        };

        public static readonly ComplexRecipe.RecipeElement[] results = CookedEggConfig.recipe.results;
    }
}
