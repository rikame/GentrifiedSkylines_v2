// Decompiled with JetBrains decompiler
// Type: BuildingAI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19C073A7-376D-4654-856C-851D76451F95
// Assembly location: D:\SteamLibrary\steamapps\common\Cities_Skylines\Cities_Data\Managed\Assembly-CSharp.dll
using ColossalFramework;
using ColossalFramework.Math;
using GentrifiedSkylines.Redirection;
using UnityEngine;
namespace GentrifiedSkylines.Detours
{
    [TargetType(typeof(BuildingAI))]
    public class BuildingAIDetour : BuildingAI
    {
        [RedirectMethod]
        public override void CalculateUnspawnPosition(ushort buildingID, ref Building data, ref Randomizer randomizer, CitizenInfo info, ushort ignoreInstance, out Vector3 position, out Vector3 target, out Vector2 direction, out CitizenInstance.Flags specialFlags)
        {
            float localMeshOffset = Building.CalculateLocalMeshOffset(this.m_info, data.Length);
            bool flag1 = this.m_info.m_specialPlaces != null && this.m_info.m_specialPlaces.Length != 0;
            bool flag2 = this.m_info.m_enterDoors != null && this.m_info.m_enterDoors.Length != 0;
            if (flag1 && (!flag2 || randomizer.Int32(4U) == 0))
            {
                BuildingInfo.Prop prop = this.m_info.m_specialPlaces[randomizer.Int32((uint)this.m_info.m_specialPlaces.Length)];
                if (new Randomizer((int)buildingID << 6 | prop.m_index).Int32(100U) < prop.m_probability && data.Length >= prop.m_requiredLength)
                {
                    Vector3 offset = prop.m_position;
                    offset.z = localMeshOffset - offset.z;
                    position = data.CalculatePosition(offset);
                    int index = randomizer.Int32((uint)prop.m_finalProp.m_specialPlaces.Length);
                    PropInfo.SpecialPlace specialPlace = prop.m_finalProp.m_specialPlaces[index];
                    float f = data.m_angle + prop.m_radAngle;
                    float num1 = Mathf.Cos(f);
                    float num2 = Mathf.Sin(f);
                    if (!prop.m_fixedHeight)
                        position.y = Singleton<TerrainManager>.instance.SampleDetailHeight(position);
                    else if (this.m_info.m_requireHeightMap)
                        position.y = Singleton<TerrainManager>.instance.SampleDetailHeight(position) + offset.y;
                    position.x += (float)((double)specialPlace.m_position.x * (double)num1 + (double)specialPlace.m_position.z * (double)num2);
                    position.y += specialPlace.m_position.y;
                    position.z += (float)((double)specialPlace.m_position.x * (double)num2 - (double)specialPlace.m_position.z * (double)num1);
                    if (!this.IsSomeBodyThere(position, ignoreInstance))
                    {
                        direction.x = (float)((double)specialPlace.m_direction.x * (double)num1 + (double)specialPlace.m_direction.z * (double)num2);
                        direction.y = (float)((double)specialPlace.m_direction.x * (double)num2 - (double)specialPlace.m_direction.z * (double)num1);
                        //specialFlags = hangaround _ specialPlace.specialFlags
                        specialFlags = CitizenInstance.Flags.HangAround | specialPlace.m_specialFlags;
                        Debug.Log("SF: " + specialFlags.ToString().PadRight(25, ' ') + "; SP: " + specialPlace.m_specialFlags.ToString().PadRight(25, ' ') + "; Layer: " + data.Info.m_class.m_layer.ToString().PadRight(15, ' ') + "; Level: " + data.Info.m_class.m_level.ToString().PadRight(5, ' ') + "; Service: " + data.Info.m_class.m_service.ToString().PadRight(20, ' ') + "; Subservice: " + data.Info.m_class.m_subService.ToString().PadRight(25, ' ') + ";");
                        if (this.m_info.m_hasPedestrianPaths)
                        {
                            target = position;
                            return;
                        }
                        target = data.CalculateSidewalkPosition(offset.x, 0.0f);
                        return;
                    }
                }
            }
            if (flag2)
            {
                BuildingInfo.Prop prop = this.m_info.m_enterDoors[randomizer.Int32((uint)this.m_info.m_enterDoors.Length)];
                if (new Randomizer((int)buildingID << 6 | prop.m_index).Int32(100U) < prop.m_probability && data.Length >= prop.m_requiredLength)
                {
                    Vector3 offset = prop.m_position;
                    offset.z = localMeshOffset - offset.z;
                    float num1 = prop.m_finalProp.m_generatedInfo.m_size.x * 0.5f - info.m_radius;
                    if ((double)num1 >= 0.100000001490116)
                    {
                        float num2 = num1 * Mathf.Sqrt((float)randomizer.Int32(1000U) * (1f / 1000f));
                        float f = (float)randomizer.Int32(1000U) * ((float)System.Math.PI / 500f);
                        offset.x += Mathf.Cos(f) * num2;
                        offset.z += Mathf.Sin(f) * num2;
                    }
                    position = data.CalculatePosition(offset);
                    if (!prop.m_fixedHeight)
                        position.y = Singleton<TerrainManager>.instance.SampleDetailHeight(position);
                    else if (this.m_info.m_requireHeightMap)
                        position.y = Singleton<TerrainManager>.instance.SampleDetailHeight(position) + offset.y;
                    direction = Vector2.zero;
                    specialFlags = (prop.m_finalProp.m_doorType & PropInfo.DoorType.HangAround) != PropInfo.DoorType.HangAround ? CitizenInstance.Flags.None : CitizenInstance.Flags.HangAround;
                    if (this.m_info.m_hasPedestrianPaths)
                    {
                        target = position;
                        return;
                    }
                    target = data.CalculateSidewalkPosition(offset.x, 0.0f);
                    return;
                }
            }
            Vector3 offset1 = new Vector3(0.0f, 0.0f, localMeshOffset);
            position = data.CalculatePosition(offset1);
            position.y = Singleton<TerrainManager>.instance.SampleDetailHeight(position);
            direction = Vector2.zero;
            specialFlags = CitizenInstance.Flags.None;
            if (this.m_info.m_hasPedestrianPaths)
                target = position;
            else
                target = data.CalculateSidewalkPosition(offset1.x, 0.0f);
        }
        [RedirectMethod]
        private bool IsSomeBodyThere(Vector3 position, ushort ignoreInstance)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            int num1 = Mathf.Max((int)(((double)position.x - 0.5) / 8.0 + 1080.0), 0);
            int num2 = Mathf.Max((int)(((double)position.z - 0.5) / 8.0 + 1080.0), 0);
            int num3 = Mathf.Min((int)(((double)position.x + 0.5) / 8.0 + 1080.0), 2159);
            int num4 = Mathf.Min((int)(((double)position.z + 0.5) / 8.0 + 1080.0), 2159);
            for (int index1 = num2; index1 <= num4; ++index1)
            {
                for (int index2 = num1; index2 <= num3; ++index2)
                {
                    ushort num5 = instance.m_citizenGrid[index1 * 2160 + index2];
                    int num6 = 0;
                    while ((int)num5 != 0)
                    {
                        if ((int)num5 != (int)ignoreInstance && (double)Vector3.SqrMagnitude((Vector3)instance.m_instances.m_buffer[(int)num5].m_targetPos - position) < 0.00999999977648258)
                            return true;
                        num5 = instance.m_instances.m_buffer[(int)num5].m_nextGridInstance;
                        if (++num6 > 65536)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            return false;
        }
    }
}