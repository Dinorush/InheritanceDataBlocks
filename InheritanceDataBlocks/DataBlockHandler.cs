using GameData;
using Il2CppJsonNet.Linq;
using InjectLib.JsonNETInjection.Handler;
using InjectLib.JsonNETInjection;
using Il2CppSystem.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;
using InheritanceDataBlocks.API;

namespace InheritanceDataBlocks
{
    internal sealed class DataBlockHandler<T> : Il2CppJsonReferenceTypeHandler<T> where T : GameDataBlockBase<T>
    {
        public override void OnRead(in Il2CppSystem.Object result, in JToken jToken)
        {
            JObject jObject = jToken.TryCast<JObject>()!;
            if (jObject.TryGetValue("parentID", out JToken idToken))
            {
                // Parse out fields. We will acquire values from the datablock itself.
                Type type = typeof(T);
                List<PropertyInfo> properties = new(jObject.Count - 1);
                JProperty[] jProperties = jObject.Properties().ToArray();
                foreach (JProperty jProperty in jProperties)
                {
                    if (jProperty.Name == "parentID")
                        continue;

                    PropertyInfo? info = InheritanceAPI<T>.CacheProperty(type, jProperty.Name);
                    if (info != null)
                        properties.Add(info);
                }

                InheritanceAPI<T>.AddDataBlock(result.TryCast<T>()!, properties, (uint) idToken);
            }
        }
    }

    internal static class DataBlockHandlerManager
    {
        internal static void AddHandler<T>() where T : GameDataBlockBase<T>
        {
            JsonInjector.AddHandler(new DataBlockHandler<T>());
        }

        // Reflection defeated me
        internal static void Init()
        {
            AddHandler<ArchetypeDataBlock>();
            AddHandler<ArtifactDataBlock>();
            AddHandler<ArtifactDistributionDataBlock>();
            AddHandler<ArtifactTagDataBlock>();
            AddHandler<AtmosphereDataBlock>();
            AddHandler<BigPickupDistributionDataBlock>();
            AddHandler<BoosterImplantConditionDataBlock>();
            AddHandler<BoosterImplantEffectDataBlock>();
            AddHandler<BoosterImplantTemplateDataBlock>();
            AddHandler<ChainedPuzzleDataBlock>();
            AddHandler<ChainedPuzzleTypeDataBlock>();
            AddHandler<CloudsDataBlock>();
            AddHandler<CommodityDataBlock>();
            AddHandler<ComplexResourceSetDataBlock>();
            AddHandler<ConsumableDistributionDataBlock>();
            AddHandler<CustomAssetShardDataBlock>();
            AddHandler<DimensionDataBlock>();
            AddHandler<EnemyBalancingDataBlock>();
            AddHandler<EnemyBehaviorDataBlock>();
            AddHandler<EnemyDataBlock>();
            AddHandler<EnemyDetectionDataBlock>();
            AddHandler<EnemyGroupDataBlock>();
            AddHandler<EnemyMovementDataBlock>();
            AddHandler<EnemyPopulationDataBlock>();
            AddHandler<EnemySFXDataBlock>();
            AddHandler<EnvironmentFeedbackDataBlock>();
            AddHandler<EventSequenceActionDataBlock>();
            AddHandler<EventSequenceDataBlock>();
            AddHandler<ExpeditionBalanceDataBlock>();
            AddHandler<ExtractionEventDataBlock>();
            AddHandler<FeedbackDataBlock>();
            AddHandler<FlashlightSettingsDataBlock>();
            AddHandler<FogScenarioDataBlock>();
            AddHandler<FogSettingsDataBlock>();
            AddHandler<GameSetupDataBlock>();
            AddHandler<GearCategoryDataBlock>();
            AddHandler<GearCategoryFilterDataBlock>();
            AddHandler<GearDataBlock>();
            AddHandler<GearDecalDataBlock>();
            AddHandler<GearFlashlightPartDataBlock>();
            AddHandler<GearFrontPartDataBlock>();
            AddHandler<GearMagPartDataBlock>();
            AddHandler<GearMeleeHandlePartDataBlock>();
            AddHandler<GearMeleeHeadPartDataBlock>();
            AddHandler<GearMeleeNeckPartDataBlock>();
            AddHandler<GearMeleePommelPartDataBlock>();
            AddHandler<GearPaletteDataBlock>();
            AddHandler<GearPartAttachmentDataBlock>();
            AddHandler<GearPatternDataBlock>();
            AddHandler<GearPerkDataBlock>();
            AddHandler<GearReceiverPartDataBlock>();
            AddHandler<GearSightPartDataBlock>();
            AddHandler<GearStockPartDataBlock>();
            AddHandler<GearToolDeliveryPartDataBlock>();
            AddHandler<GearToolGripPartDataBlock>();
            AddHandler<GearToolMainPartDataBlock>();
            AddHandler<GearToolPayloadPartDataBlock>();
            AddHandler<GearToolScreenPartDataBlock>();
            AddHandler<GearToolTargetingPartDataBlock>();
            AddHandler<ItemDataBlock>();
            AddHandler<ItemFPSSettingsDataBlock>();
            AddHandler<ItemMovementAnimationDataBlock>();
            AddHandler<ItemPartDataBlock>();
            AddHandler<LevelGenSettingsDataBlock>();
            AddHandler<LevelLayoutDataBlock>();
            AddHandler<LightSettingsDataBlock>();
            AddHandler<LootDataBlock>();
            AddHandler<MarkerGroupDataBlock>();
            AddHandler<MeleeAnimationSetDataBlock>();
            AddHandler<MeleeArchetypeDataBlock>();
            AddHandler<MeleeSFXDataBlock>();
            AddHandler<MiningMarkerDataBlock>();
            AddHandler<MLSArrayDescriptorReferenceDataBlock>();
            AddHandler<MusicStateDataBlock>();
            AddHandler<PlayerDataBlock>();
            AddHandler<PlayerDialogDataBlock>();
            AddHandler<PlayerOfflineGearDataBlock>();
            AddHandler<RecoilDataBlock>();
            AddHandler<RundownDataBlock>();
            AddHandler<ServiceMarkerDataBlock>();
            AddHandler<StaticSpawnDataBlock>();
            AddHandler<SurvivalWavePopulationDataBlock>();
            AddHandler<SurvivalWaveSettingsDataBlock>();
            AddHandler<TechMarkerDataBlock>();
            AddHandler<TextCharacterMetaDataBlock>();
            AddHandler<TextDataBlock>();
            AddHandler<VanityItemsGroupDataBlock>();
            AddHandler<VanityItemsLayerDropsDataBlock>();
            AddHandler<VanityItemsTemplateDataBlock>();
            AddHandler<WardenObjectiveDataBlock>();
            AddHandler<WeaponAudioDataBlock>();
            AddHandler<WeaponDataBlock>();
            AddHandler<WeaponMuzzleFlashDataBlock>();
            AddHandler<WeaponShellCasingDataBlock>();

        }

        /* I really did try. But the reflected AddHandler function couldn't accept my subclass of the base handler, so I give up.
        internal static void Init()
        {
            var blockTypes = typeof(GameDataBlockBase<>).Assembly.GetTypes()
                .Where(x => !x.IsAbstract && !x.IsGenericType && x.BaseType?.IsGenericType == true)
                .Where(x => x.BaseType?.GetGenericTypeDefinition() == typeof(GameDataBlockBase<>));
            MethodInfo[] methods = typeof(JsonInjector).GetMethods();

            // For some reason there's a difference between the reflected and compiled type. Reflected one has a blank full name.
            // Didn't investigate too deeply into differences, but GUID works.
            MethodInfo? addHandler = methods
                .FirstOrDefault(method => 
                    method.GetParameters().Length > 0
                 && method.GetParameters()[0].ParameterType.GUID == typeof(Il2CppJsonReferenceTypeHandler<>).GUID
                 );

            if (addHandler == null) return;

            foreach (var blockType in blockTypes)
            {
                MethodInfo addHandlerGeneric = addHandler.MakeGenericMethod(blockType);
                var handlerType = typeof(DataBlockHandler<>).MakeGenericType(blockType);
                object? handler = Activator.CreateInstance(handlerType);
                if (handler == null)
                {
                    IDBLogger.Error("Unable to create handler for" + blockType.Name);
                    continue;
                }
                addHandlerGeneric.Invoke(null, new object[] { handler });
            }
        }*/
    }
}
