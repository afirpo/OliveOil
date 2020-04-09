using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OliveOil
{
    class OliveOilElement
    {
        public static readonly string OliveOilName = "OliveOil"
            , FrozenOliveOilName = "FrozenOliveOil";

        public static readonly string OliveOilDescr = "Olive Oil"
            , FrozenOliveOilDescr = "Frozen Olive Oil";

        public static readonly string OliveOilId = OliveOilName;
        
        public static readonly Color32 OLIVEOIL_GREEN = new Color32(128, 128, 0, 255);

        public static readonly SimHashes OliveOilSimHash = (SimHashes)Hash.SDBMLower(OliveOilName);

        public static readonly SimHashes FrozenOliveOilSimHash = (SimHashes)Hash.SDBMLower(FrozenOliveOilName);

        public static readonly Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>
        {
          { OliveOilSimHash, OliveOilName },
          { FrozenOliveOilSimHash, FrozenOliveOilName }
        };

        public static readonly Dictionary<string, object> ReverseSimHashNameLookup = SimHashNameLookup.ToDictionary(x => x.Value, x => x.Key as object);

        public const string CONFIG = @"
---
elements:
    -   elementId: OliveOil
        maxMass: 1000
        liquidCompression: 1.01
        speed: 50
        minHorizontalFlow: 0.1
        minVerticalFlow: 0.1
        specificHeatCapacity: 1.97
        thermalConductivity: 0.17
        solidSurfaceAreaMultiplier: 1
        liquidSurfaceAreaMultiplier: 25
        gasSurfaceAreaMultiplier: 1
        lowTemp: 267.15
        highTemp: 973.15
        lowTempTransitionTarget: FrozenOliveOil
        # highTempTransitionTarget: Steam
        defaultTemperature: 293.15
        defaultMass: 1000
        molarMass: 282.46
        toxicity: 0
        lightAbsorptionFactor: 0.4
        tags:
        - AnyWater
        isDisabled: false
        state: Liquid
        localizationID: STRINGS.ELEMENTS.OLIVEOIL.NAME

    -   elementId: FrozenOliveOil
        specificHeatCapacity: 0.96
        thermalConductivity: 0.71
        solidSurfaceAreaMultiplier: 1
        liquidSurfaceAreaMultiplier: 1
        gasSurfaceAreaMultiplier: 1
        strength: 1
        highTemp: 267.15
        highTempTransitionTarget: OliveOil
        defaultTemperature: 232.15
        defaultMass: 1000
        maxMass: 1100
        # hardnessTier: 3
        hardness: 25
        molarMass: 282.46
        lightAbsorptionFactor: 0.6
        materialCategory: Liquifiable
        tags:
        - IceOre
        - BuildableAny
        buildMenuSort: 5
        isDisabled: false
        state: Solid
        localizationID: STRINGS.ELEMENTS.FROZENOLIVEOIL.NAME
";

        public static Substance CreateOliveOilSubstance(Substance source)
        {
            return ModUtil.CreateSubstance(
              name: OliveOilName,
              state: Element.State.Liquid,
              kanim: source.anim,
              material: source.material,
              colour: OLIVEOIL_GREEN,
              ui_colour: OLIVEOIL_GREEN,
              conduit_colour: OLIVEOIL_GREEN
            );
        }

        static Texture2D TintTextureOliveOilGreen(Texture sourceTexture, string name)
        {
            Texture2D newTexture = TextureUtil.DuplicateTexture(sourceTexture as Texture2D);
            var pixels = newTexture.GetPixels32();
            for (int i = 0; i < pixels.Length; ++i)
            {
                var gray = ((Color)pixels[i]).grayscale * 1.5f;
                pixels[i] = (Color)OLIVEOIL_GREEN * gray;
            }
            newTexture.SetPixels32(pixels);
            newTexture.Apply();
            newTexture.name = name;
            return newTexture;
        }

        static Material CreateFrozenOliveOilMaterial(Material source)
        {
            var frozenOliveOilMaterial = new Material(source);

            Texture2D newTexture = TintTextureOliveOilGreen(frozenOliveOilMaterial.mainTexture, FrozenOliveOilName.ToLower());

            frozenOliveOilMaterial.mainTexture = newTexture;
            frozenOliveOilMaterial.name = "mat" + FrozenOliveOilName;

            return frozenOliveOilMaterial;
        }

        public static Substance CreateFrozenOliveOilSubstance(Material sourceMaterial, KAnimFile sourceAnim)
        {
            return ModUtil.CreateSubstance(
              name: FrozenOliveOilName,
              state: Element.State.Solid,
              kanim: sourceAnim,
              material: CreateFrozenOliveOilMaterial(sourceMaterial),
              colour: OLIVEOIL_GREEN,
              ui_colour: OLIVEOIL_GREEN,
              conduit_colour: OLIVEOIL_GREEN
            );
        }
    }
}
