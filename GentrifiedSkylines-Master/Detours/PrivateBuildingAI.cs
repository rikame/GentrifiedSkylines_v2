// Decompiled with JetBrains decompiler
// Type: PrivateBuildingAI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19C073A7-376D-4654-856C-851D76451F95
// Assembly location: D:\SteamLibrary\steamapps\common\Cities_Skylines\Cities_Data\Managed\Assembly-CSharp.dll

using ColossalFramework;
using ColossalFramework.Math;
using GentrifiedSkylines.Redirection;
using UnityEngine;

namespace GentrifiedSkylines.Detours

{
    [TargetType(typeof(PrivateBuildingAI))]
    public class PrivateBuildingAIDetour : PrivateBuildingAI
    {
        public override void SimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            base.SimulationStep(buildingID, ref buildingData, ref frameData);
            if (Singleton<SimulationManager>.instance.m_randomizer.Int32(10U) == 0)
            {
                DistrictManager instance = Singleton<DistrictManager>.instance;
                byte district = instance.GetDistrict(buildingData.m_position);
                ushort num = instance.m_districts.m_buffer[(int)district].m_Style;
                if ((int)num > 0 && (int)num - 1 < instance.m_Styles.Length)
                {
                    DistrictStyle districtStyle = instance.m_Styles[(int)num - 1];
                    if (districtStyle != null && (UnityEngine.Object)this.m_info.m_class != (UnityEngine.Object)null && (districtStyle.AffectsService(this.m_info.GetService(), this.m_info.GetSubService(), this.m_info.m_class.m_level) && !districtStyle.Contains(this.m_info)) && (int)Singleton<ZoneManager>.instance.m_lastBuildIndex == (int)Singleton<SimulationManager>.instance.m_currentBuildIndex)
                    {
                        buildingData.m_flags |= Building.Flags.Demolishing;
                        ++Singleton<SimulationManager>.instance.m_currentBuildIndex;
                    }
                }
            }
            if ((buildingData.m_flags & Building.Flags.ZonesUpdated) != Building.Flags.None)
            {
                SimulationManager instance = Singleton<SimulationManager>.instance;
                if ((int)buildingData.m_fireIntensity != 0 || instance.m_randomizer.Int32(10U) != 0 || (int)Singleton<ZoneManager>.instance.m_lastBuildIndex != (int)instance.m_currentBuildIndex)
                    return;
                buildingData.m_flags &= ~Building.Flags.ZonesUpdated;
                if (buildingData.CheckZoning(this.m_info.m_class.GetZone(), this.m_info.m_class.GetSecondaryZone()))
                    return;
                buildingData.m_flags |= Building.Flags.Demolishing;
                PrivateBuildingAIDetour.CheckNearbyBuildingZones(buildingData.m_position);
                ++instance.m_currentBuildIndex;
            }
            else
            {
                if ((buildingData.m_flags & (Building.Flags.Abandoned | Building.Flags.Downgrading)) == Building.Flags.None || (int)buildingData.m_majorProblemTimer != (int)byte.MaxValue && (buildingData.m_flags & Building.Flags.Abandoned) != Building.Flags.None)
                    return;
                SimulationManager instance1 = Singleton<SimulationManager>.instance;
                ZoneManager instance2 = Singleton<ZoneManager>.instance;
                int num1;
                switch (this.m_info.m_class.m_service)
                {
                    case ItemClass.Service.Residential:
                        num1 = instance2.m_actualResidentialDemand;
                        break;

                    case ItemClass.Service.Commercial:
                        num1 = instance2.m_actualCommercialDemand;
                        break;

                    case ItemClass.Service.Industrial:
                        num1 = instance2.m_actualWorkplaceDemand;
                        break;

                    case ItemClass.Service.Office:
                        num1 = instance2.m_actualWorkplaceDemand;
                        break;

                    default:
                        num1 = 0;
                        break;
                }
                if (instance1.m_randomizer.Int32(100U) >= num1 || (int)instance2.m_lastBuildIndex != (int)instance1.m_currentBuildIndex || (double)Singleton<TerrainManager>.instance.WaterLevel(VectorUtils.XZ(buildingData.m_position)) > (double)buildingData.m_position.y)
                    return;
                ItemClass.SubService subService = this.m_info.m_class.m_subService;
                ItemClass.Level level = ItemClass.Level.Level1;
                int width = buildingData.Width;
                int num2 = buildingData.Length;
                if (this.m_info.m_class.m_service == ItemClass.Service.Industrial)
                    ZoneBlock.GetIndustryType(buildingData.m_position, out subService, out level);
                else if (this.m_info.m_class.m_service == ItemClass.Service.Commercial)
                    ZoneBlock.GetCommercialType(buildingData.m_position, this.m_info.m_class.GetZone(), width, num2, out subService, out level);
                DistrictManager instance3 = Singleton<DistrictManager>.instance;
                byte district = instance3.GetDistrict(buildingData.m_position);
                ushort num3 = instance3.m_districts.m_buffer[(int)district].m_Style;
                BuildingInfo randomBuildingInfo = Singleton<BuildingManager>.instance.GetRandomBuildingInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, subService, level, width, num2, this.m_info.m_zoningMode, (int)num3);
                if (randomBuildingInfo == null)
                    return;
                buildingData.m_flags |= Building.Flags.Demolishing;
                float num4 = buildingData.m_angle + 1.570796f;
                float num5;
                if (this.m_info.m_zoningMode == BuildingInfo.ZoningMode.CornerLeft && randomBuildingInfo.m_zoningMode == BuildingInfo.ZoningMode.CornerRight)
                {
                    num5 = num4 - 1.570796f;
                    num2 = width;
                }
                else if (this.m_info.m_zoningMode == BuildingInfo.ZoningMode.CornerRight && randomBuildingInfo.m_zoningMode == BuildingInfo.ZoningMode.CornerLeft)
                {
                    num5 = num4 + 1.570796f;
                    num2 = width;
                }
                ushort building;
                if (Singleton<BuildingManager>.instance.CreateBuilding(out building, ref Singleton<SimulationManager>.instance.m_randomizer, randomBuildingInfo, buildingData.m_position, buildingData.m_angle, num2, Singleton<SimulationManager>.instance.m_currentBuildIndex))
                {
                    ++Singleton<SimulationManager>.instance.m_currentBuildIndex;
                    switch (this.m_info.m_class.m_service)
                    {
                        case ItemClass.Service.Residential:
                            instance2.m_actualResidentialDemand = Mathf.Max(0, instance2.m_actualResidentialDemand - 5);
                            break;

                        case ItemClass.Service.Commercial:
                            instance2.m_actualCommercialDemand = Mathf.Max(0, instance2.m_actualCommercialDemand - 5);
                            break;

                        case ItemClass.Service.Industrial:
                            instance2.m_actualWorkplaceDemand = Mathf.Max(0, instance2.m_actualWorkplaceDemand - 5);
                            break;

                        case ItemClass.Service.Office:
                            instance2.m_actualWorkplaceDemand = Mathf.Max(0, instance2.m_actualWorkplaceDemand - 5);
                            break;
                    }
                }
                ++instance1.m_currentBuildIndex;
            }
        }

        protected override void SimulationStepActive(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            base.SimulationStepActive(buildingID, ref buildingData, ref frameData);
            if ((buildingData.m_problems & Notification.Problem.MajorProblem) != Notification.Problem.None)
            {
                if ((int)buildingData.m_fireIntensity != 0)
                    return;
                buildingData.m_majorProblemTimer = (byte)Mathf.Min((int)byte.MaxValue, (int)buildingData.m_majorProblemTimer + 1);
                if ((int)buildingData.m_majorProblemTimer < 64 || Singleton<BuildingManager>.instance.m_abandonmentDisabled)
                    return;
                buildingData.m_majorProblemTimer = (byte)192;
                buildingData.m_flags &= ~Building.Flags.Active;
                buildingData.m_flags |= Building.Flags.Abandoned;
                buildingData.m_problems = Notification.Problem.FatalProblem | buildingData.m_problems & ~Notification.Problem.MajorProblem;
                this.RemovePeople(buildingID, ref buildingData);
                this.BuildingDeactivated(buildingID, ref buildingData);
                Singleton<BuildingManager>.instance.UpdateBuildingRenderer(buildingID, true);
            }
            else
                buildingData.m_majorProblemTimer = (byte)0;
        }
        protected void StartUpgrading(ushort buildingID, ref Building buildingData)
        {
            buildingData.m_frame0.m_constructState = (byte)0;
            buildingData.m_frame1.m_constructState = (byte)0;
            buildingData.m_frame2.m_constructState = (byte)0;
            buildingData.m_frame3.m_constructState = (byte)0;
            Building.Flags flags = (buildingData.m_flags | Building.Flags.Upgrading) & ~Building.Flags.Completed & ~Building.Flags.LevelUpEducation & ~Building.Flags.LevelUpLandValue;
            buildingData.m_flags = flags;
            BuildingManager instance1 = Singleton<BuildingManager>.instance;
            instance1.UpdateBuildingRenderer(buildingID, true);
            EffectInfo effect = instance1.m_properties.m_levelupEffect;
            if (effect != null)
            {
                InstanceID instance2 = new InstanceID();
                instance2.Building = buildingID;
                Vector3 meshPosition;
                Quaternion meshRotation;
                buildingData.CalculateMeshPosition(out meshPosition, out meshRotation);
                EffectInfo.SpawnArea spawnArea = new EffectInfo.SpawnArea(Matrix4x4.TRS(meshPosition, meshRotation, Vector3.one), this.m_info.m_lodMeshData);
                Singleton<EffectManager>.instance.DispatchEffect(effect, instance2, spawnArea, Vector3.zero, 0.0f, 1f, instance1.m_audioGroup);
            }
            Vector3 position = buildingData.m_position;
            position.y += this.m_info.m_size.y;
            Singleton<NotificationManager>.instance.AddEvent(NotificationEvent.Type.LevelUp, position, 1f);
            ++Singleton<SimulationManager>.instance.m_currentBuildIndex;
        }
        private static void CheckNearbyBuildingZones(Vector3 position)
        {
            int num1 = Mathf.Max((int)(((double)position.x - 35.0) / 64.0 + 135.0), 0);
            int num2 = Mathf.Max((int)(((double)position.z - 35.0) / 64.0 + 135.0), 0);
            int num3 = Mathf.Min((int)(((double)position.x + 35.0) / 64.0 + 135.0), 269);
            int num4 = Mathf.Min((int)(((double)position.z + 35.0) / 64.0 + 135.0), 269);
            Array16<Building> array16 = Singleton<BuildingManager>.instance.m_buildings;
            ushort[] numArray = Singleton<BuildingManager>.instance.m_buildingGrid;
            for (int index1 = num2; index1 <= num4; ++index1)
            {
                for (int index2 = num1; index2 <= num3; ++index2)
                {
                    ushort building = numArray[index1 * 270 + index2];
                    int num5 = 0;
                    while ((int)building != 0)
                    {
                        ushort num6 = array16.m_buffer[(int)building].m_nextGridBuilding;
                        if ((array16.m_buffer[(int)building].m_flags & (Building.Flags.Created | Building.Flags.Deleted | Building.Flags.Demolishing)) == Building.Flags.Created)
                        {
                            BuildingInfo info = array16.m_buffer[(int)building].Info;
                            if (info != null && info.m_placementStyle == ItemClass.Placement.Automatic)
                            {
                                ItemClass.Zone zone = info.m_class.GetZone();
                                ItemClass.Zone secondaryZone = info.m_class.GetSecondaryZone();
                                if (zone != ItemClass.Zone.None && (array16.m_buffer[(int)building].m_flags & Building.Flags.ZonesUpdated) != Building.Flags.None && (double)VectorUtils.LengthSqrXZ(array16.m_buffer[(int)building].m_position - position) <= 1225.0)
                                {
                                    array16.m_buffer[(int)building].m_flags &= ~Building.Flags.ZonesUpdated;
                                    if (!array16.m_buffer[(int)building].CheckZoning(zone, secondaryZone))
                                        Singleton<BuildingManager>.instance.ReleaseBuilding(building);
                                }
                            }
                        }
                        building = num6;
                        if (++num5 >= 49152)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
        }
    }
}