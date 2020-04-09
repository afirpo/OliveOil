using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using CREATURES = STRINGS.CREATURES;

namespace OliveOil
{
    public class OliveTreeConfig : IEntityConfig
    {
        public const string Id = "OliveTreePlant";
        public const string SeedId = "OliveTreeSeed";

        public static string Name = "Olive Tree";
        public static string Description = $"A {UI.FormatAsLink("Plant", "PLANTS")} that can be grown in farm buildings. It produces {UI.FormatAsLink(OlivesConfig.Description, OlivesConfig.Name)} that in turn can be used to refine {UI.FormatAsLink(OliveOilElement.OliveOilName, OliveOilElement.OliveOilDescr)}.";
        public static string DomesticatedDescription = $"A {UI.FormatAsLink("Plant", "PLANTS")} that grows {UI.FormatAsLink(OlivesConfig.Description, OlivesConfig.Name)}.";

        public static string SeedName = "Olive Tree Seed";
        public static string SeedDescription = $"The {UI.FormatAsLink("Seed", "PLANTS")} of a {UI.FormatAsLink(Name, Id)}.";

        public GameObject CreatePrefab()
        {
            var placedEntity = EntityTemplates.CreatePlacedEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                anim: Assets.GetAnim("tree_kanim"),
                //initialAnim: "idle_loop",
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 2,
                decor: DECOR.BONUS.TIER1,
                defaultTemperature: 298.15f);

            EntityTemplates.ExtendEntityToBasicPlant(
                template: placedEntity,
                temperature_lethal_low: 258.15f,
                temperature_warning_low: 288.15f,
                temperature_warning_high: 313.15f,
                temperature_lethal_high: 448.15f,
                safe_elements: (SimHashes[])null,
                crop_id: OlivesConfig.Id,
                max_age: 2400);

            EntityTemplates.ExtendPlantToIrrigated(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
               {
                  new PlantElementAbsorber.ConsumeInfo()
                  {
                    tag = GameTags.Water,
                    massConsumptionRate = 0.1166667f
                  }
               });

            EntityTemplates.ExtendPlantToFertilizable(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
            {
                  new PlantElementAbsorber.ConsumeInfo()
                  {
                    tag = GameTags.Dirt,
                    massConsumptionRate = 0.01666667f
                  }
            });

            placedEntity.AddOrGet<OliveTree>();

            var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
                plant: placedEntity,
                id: SeedId,
                name: UI.FormatAsLink(SeedName, Id),
                desc: SeedDescription,
                //productionType: SeedProducer.ProductionType.Harvest,
                productionType: SeedProducer.ProductionType.Hidden,
                anim: Assets.GetAnim("seed_tree_kanim"),
                //numberOfSeeds: 0,
                numberOfSeeds: 1,
                additionalTags: new List<Tag> { GameTags.CropSeed },
                sortOrder: 7,
                domesticatedDescription: DomesticatedDescription,
                width: 0.3f,
                height: 0.3f);

            EntityTemplates.CreateAndRegisterPreviewForPlant(
                seed: seed,
                id: "OliveTree_preview",
                anim: Assets.GetAnim("tree_kanim"),
                initialAnim: "place",
                width: 3,
                height: 3);

            SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER1);

            return placedEntity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }

    public class OlivesConfig : IEntityConfig
    {
        public static string Id = "Olives";
        public static string Name = Id;
        public static string Description = "Olives directly from the trees!";

        public GameObject CreatePrefab()
        {
            var entity = EntityTemplates.CreateLooseEntity(
                id: Id,
                name: UI.FormatAsLink(Name, Id),
                desc: Description,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("seed_bristlebriar_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.8f,
                height: 0.4f,
                isPickupable: true);

            var foodInfo = new EdiblesManager.FoodInfo(
                id: Id,
                caloriesPerUnit: 350f,
                quality: TUNING.FOOD.FOOD_QUALITY_AWFUL,
                preserveTemperatue: 255.15f,
                rotTemperature: 277.15f,
                spoilTime: TUNING.FOOD.SPOIL_TIME.SLOW,
                can_rot: true);

            var foodEntity = EntityTemplates.ExtendEntityToFood(entity, foodInfo);

            return foodEntity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }

    public class OliveTree : StateMachineComponent<OliveTree.StatesInstance>
    {
        [MyCmpReq]
        private Crop crop;

        [MyCmpReq]
        private WiltCondition wiltCondition;

        [MyCmpReq]
        private Growing growing;

        [MyCmpReq]
        private ReceptacleMonitor rm;

        [MyCmpReq]
        private Harvestable harvestable;

        protected override void OnSpawn()
        {
            base.OnSpawn();

            smi.Get<KBatchedAnimController>().randomiseLoopedOffset = true;

            smi.StartSM();
        }

        protected void DestroySelf(object callbackParam)
        {
            CreatureHelpers.DeselectCreature(gameObject);
            Util.KDestroyGameObject(gameObject);
        }

        public Notification CreateDeathNotification()
        {
            return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, HashedString.Invalid, (notificationList, data) =>
                     CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + gameObject.GetProperName());
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, OliveTree, object>.GameInstance
        {
            public StatesInstance(OliveTree master) : base(master) { }

            public bool IsOld() => master.growing.PercentOldAge() > 0.5;
        }

        public class AnimSet
        {
            public const string grow = nameof(grow);
            public const string grow_pst = nameof(grow_pst);
            public const string idle_full = nameof(idle_full);
            public const string wilt_pre = nameof(wilt_pre);
            public const string wilt = nameof(wilt);
            public const string wilt_pst = nameof(wilt_pst);
            public const string harvest = nameof(harvest);
        }

        public class States : GameStateMachine<States, StatesInstance, OliveTree>
        {
            public AliveStates Alive;
            public State Dead;

            public override void InitializeStates(out BaseState defaultState)
            {
                serializable = true;
                defaultState = Alive;

                var dead = CREATURES.STATUSITEMS.DEAD.NAME;
                var tooltip = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
                var main = Db.Get().StatusItemCategories.Main;

                Dead
                    .ToggleStatusItem(dead, tooltip, string.Empty, StatusItem.IconType.Info, 0, false, OverlayModes.None.ID, 0, category: main)
                    .Enter(smi =>
                    {
                        if (smi.master.rm.Replanted && !smi.master.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
                            smi.master.gameObject.AddOrGet<Notifier>().Add(smi.master.CreateDeathNotification());

                        GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront).SetActive(true);
                        if (smi.master.harvestable != null && smi.master.harvestable.CanBeHarvested && GameScheduler.Instance != null)
                            GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, smi.master.crop.SpawnFruit);

                        smi.master.Trigger((int)GameHashes.Died);
                        smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
                        Destroy(smi.master.GetComponent<KBatchedAnimController>());
                        smi.Schedule(0.5f, smi.master.DestroySelf);
                    });

                Alive
                    .InitializeStates(masterTarget, Dead)
                    .DefaultState(Alive.Idle)
                    .ToggleComponent<Growing>();

                Alive.Idle
                    .EventTransition(GameHashes.Wilt, Alive.WiltingPre, smi => smi.master.wiltCondition.IsWilting())
                    .EventTransition(GameHashes.Grow, Alive.PreFruiting, smi => smi.master.growing.ReachedNextHarvest())
                    .PlayAnim(AnimSet.grow, KAnim.PlayMode.Once);
                    //.PlayAnim(AnimSet.grow, KAnim.PlayMode.Loop);

                Alive.PreFruiting
                    .PlayAnim("grow", KAnim.PlayMode.Once)
                    .EventTransition(GameHashes.AnimQueueComplete, Alive.Fruiting);

                Alive.FruitingLost
                    .Enter(smi => smi.master.harvestable.SetCanBeHarvested(false))
                    .GoTo(Alive.Idle);

                Alive.WiltingPre
                    .QueueAnim(AnimSet.wilt_pre)
                    .OnAnimQueueComplete(Alive.Wilting);

                Alive.Wilting
                    .PlayAnim(AnimSet.wilt, KAnim.PlayMode.Loop)
                    .EventTransition(GameHashes.WiltRecover, Alive.WiltingPst, smi => !smi.master.wiltCondition.IsWilting())
                    .EventTransition(GameHashes.Harvest, Alive.Harvest);

                Alive.WiltingPst
                    .PlayAnim(AnimSet.wilt, KAnim.PlayMode.Once)
                    .OnAnimQueueComplete(Alive.Idle);

                Alive.Fruiting
                    .DefaultState(Alive.Fruiting.FruitingIdle)
                    .EventTransition(GameHashes.Wilt, Alive.WiltingPre)
                    .EventTransition(GameHashes.Harvest, Alive.Harvest)
                    .EventTransition(GameHashes.Grow, Alive.FruitingLost, smi => !smi.master.growing.ReachedNextHarvest());

                Alive.Fruiting.FruitingIdle
                    .PlayAnim(AnimSet.idle_full, KAnim.PlayMode.Loop)
                    .Enter(smi => smi.master.harvestable.SetCanBeHarvested(true))
                    .Update("fruiting_idle", (smi, dt) =>
                    {
                        if (!smi.IsOld())
                            return;
                        smi.GoTo(Alive.Fruiting.FruitingOld);
                    }, UpdateRate.SIM_4000ms);

                Alive.Fruiting.FruitingOld
                    .PlayAnim(AnimSet.wilt, KAnim.PlayMode.Loop)
                    .Enter(smi => smi.master.harvestable.SetCanBeHarvested(true))
                    .Update("fruiting_old", (smi, dt) =>
                    {
                        if (smi.IsOld())
                            return;
                        smi.GoTo(Alive.Fruiting.FruitingIdle);
                    }, UpdateRate.SIM_4000ms);

                Alive.Harvest
                    .PlayAnim(AnimSet.harvest, KAnim.PlayMode.Once)
                    .Enter(smi =>
                    {
                        if (GameScheduler.Instance == null || smi.master == null) return;

                        GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, smi.master.crop.SpawnFruit);
                        smi.master.harvestable.SetCanBeHarvested(false);
                    })
                    .OnAnimQueueComplete(Alive.Idle);
            }

            public class AliveStates : PlantAliveSubState
            {
                public State Idle;
                public State PreFruiting;
                public State FruitingLost;
                public State WiltingPre;
                public State Wilting;
                public State WiltingPst;
                public State Harvest;
                public FruitingState Fruiting;
            }

            public class FruitingState : State
            {
                public State FruitingIdle;
                public State FruitingOld;
            }
        }
    }
}
