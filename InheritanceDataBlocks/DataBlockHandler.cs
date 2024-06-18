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
                for (int i = 0; i < jObject.Count - 1; i++)
                {
                    if (jProperties[i].Name == "parentID")
                        continue;

                    PropertyInfo? info = InheritanceAPI<T>.CacheProperty(type, jProperties[i].Name);
                    if (info != null)
                        properties.Add(info);
                }

                InheritanceAPI<T>.AddDataBlock(result.TryCast<T>()!, properties, (uint)idToken);
            }
        }
    }

    internal static class DataBlockHandlerSetup
    {
        // Reflection defeated me
        internal static void Init()
        {
            JsonInjector.AddHandler(new DataBlockHandler<ArchetypeDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ArtifactDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ArtifactDistributionDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ArtifactTagDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<AtmosphereDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<BigPickupDistributionDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<BoosterImplantConditionDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<BoosterImplantEffectDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<BoosterImplantTemplateDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ChainedPuzzleDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ChainedPuzzleTypeDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<CloudsDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<CommodityDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ComplexResourceSetDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ConsumableDistributionDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<CustomAssetShardDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<DimensionDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnemyBalancingDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnemyBehaviorDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnemyDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnemyDetectionDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnemyGroupDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnemyMovementDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnemyPopulationDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnemySFXDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EnvironmentFeedbackDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EventSequenceActionDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<EventSequenceDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ExpeditionBalanceDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ExtractionEventDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<FeedbackDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<FlashlightSettingsDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<FogScenarioDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<FogSettingsDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GameSetupDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearCategoryDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearCategoryFilterDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearDecalDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearFlashlightPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearFrontPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearMagPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearMeleeHandlePartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearMeleeHeadPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearMeleeNeckPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearMeleePommelPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearPaletteDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearPartAttachmentDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearPatternDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearPerkDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearReceiverPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearSightPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearStockPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearToolDeliveryPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearToolGripPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearToolMainPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearToolPayloadPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearToolScreenPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<GearToolTargetingPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ItemDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ItemFPSSettingsDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ItemMovementAnimationDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ItemPartDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<LevelGenSettingsDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<LevelLayoutDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<LightSettingsDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<LootDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<MarkerGroupDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<MeleeAnimationSetDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<MeleeArchetypeDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<MeleeSFXDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<MiningMarkerDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<MLSArrayDescriptorReferenceDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<MusicStateDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<PlayerDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<PlayerDialogDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<PlayerOfflineGearDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<RecoilDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<RundownDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<ServiceMarkerDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<StaticSpawnDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<SurvivalWavePopulationDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<SurvivalWaveSettingsDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<TechMarkerDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<TextCharacterMetaDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<TextDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<VanityItemsGroupDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<VanityItemsLayerDropsDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<VanityItemsTemplateDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<WardenObjectiveDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<WeaponAudioDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<WeaponDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<WeaponMuzzleFlashDataBlock>());
            JsonInjector.AddHandler(new DataBlockHandler<WeaponShellCasingDataBlock>());

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
