using ColossalFramework;
using ColossalFramework.Math;
using GentrifiedSkylines.Redirection;
using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    [TargetType(typeof(HumanAI))]
    internal class HumanAIDetour : HumanAI
    {
        /*public override void SimulationStep(ushort instanceID, ref CitizenInstance data, Vector3 physicsLodRefPos)
        {
            if ((data.m_flags & CitizenInstance.Flags.WaitingPath) != CitizenInstance.Flags.None)                               //If citizen is waiting path
            {
                byte num = Singleton<PathManager>.instance.m_pathUnits.m_buffer[(long)data.m_path].m_pathFindFlags;                 //num = path flags
                if (((int)num & 4) != 0)                                                                                            //if flag ready is true
                {
                    this.Spawn(instanceID, ref data);                                                                                   //spawn this
                    data.m_pathPositionIndex = byte.MaxValue;                                                                           //index = byte.maxvalue
                    data.m_flags &= ~CitizenInstance.Flags.WaitingPath;                                                                 //set flag WaitingPath to false
                    data.m_flags &= ~CitizenInstance.Flags.TargetFlags;                                                                 //set flag TargetFlags to false
                    this.PathfindSuccess(instanceID, ref data);                                                                         //PathFind Succcess
                }
                else if (((int)num & 8) != 0)                                                                                       //else (if flag ready is false andflag Flag_Failed is true)
                {
                    data.m_flags &= ~CitizenInstance.Flags.WaitingPath;                                                                 //set flag WaitingPath to true
                    data.m_flags &= ~CitizenInstance.Flags.TargetFlags;                                                                 //set flag TargetFlags to true
                    Singleton<PathManager>.instance.ReleasePath(data.m_path);
                    data.m_path = 0U;                                                                                                   //empty m_path
                    this.PathfindFailure(instanceID, ref data);                                                                         //PathFind Failure
                    return;                                                                                                             //END SIMULATION STEP
                }
            }
            base.SimulationStep(instanceID, ref data, physicsLodRefPos);                                                            //Run simulationstep of CitizenAI *******
            CitizenManager instance1 = Singleton<CitizenManager>.instance;                                                          //instance1 = Citizen Manager
            VehicleManager instance2 = Singleton<VehicleManager>.instance;                                                          //instance2 = Vehicle Manager
            ushort num1 = 0;                                                                                                        //num1 = 0
            if ((int)data.m_citizen != 0)                                                                                           //if this citizen's ID is not 0
                num1 = instance1.m_citizens.m_buffer[(long)data.m_citizen].m_vehicle;                                                   //num1 = this citizen's vehicle
            if ((int)num1 != 0)                                                                                                     //if num1 is not 0 (is now valid)
            {
                VehicleInfo info = instance2.m_vehicles.m_buffer[(int)num1].Info;                                                       //info is the vehicle manager's buffer of num1 (car ID)'s info
                if (info.m_vehicleType == VehicleInfo.VehicleType.Bicycle)                                                              //if info's vehicle type is Bicycle
                {
                    info.m_vehicleAI.SimulationStep(num1, ref instance2.m_vehicles.m_buffer[(int)num1], num1, ref instance2.m_vehicles.m_buffer[(int)num1], 0);
                    //Run the VehicleAI's simulation step (car ID, its buffer (a vehicle object), carID (Leader), Vehicle Object (leader), 0 (physics)  .
                    num1 = (ushort)0;                                                                                                       //num1 (reset vehicleID)
                }
            }
            if ((int)num1 != 0 || (data.m_flags & (CitizenInstance.Flags.Character | CitizenInstance.Flags.WaitingPath)) != CitizenInstance.Flags.None)
                //if num1 (vehicleID) is empty or if either the Character flag or the WaitingPath flag are true
                return;                                                                                                                 //END SIMULATION STEP
            data.m_flags &= ~CitizenInstance.Flags.TargetFlags;                                                                     //TargetFlags = false
            this.ArriveAtDestination(instanceID, ref data, false);                                                                  //ArriveAtDestination(failed)
            instance1.ReleaseCitizenInstance(instanceID);                                                                           //Release this citizen
        }*/

        [RedirectMethod]
        public override void SimulationStep(ushort instanceID, ref CitizenInstance citizenData, ref CitizenInstance.Frame frameData, bool lodPhysics)
        {
            //Debug.Log(CitizenManager.instance.GetDefaultCitizenName(citizenData.m_citizen) + ", leaving from " + BuildingManager.instance.GetBuildingName(citizenData.m_sourceBuilding, new InstanceID { Building = citizenData.m_sourceBuilding }) + ", is headed to target " + BuildingManager.instance.GetBuildingName(citizenData.m_targetBuilding, new InstanceID { Building = citizenData.m_targetBuilding }) + ".");
            //Debug.Log(CitizenManager.instance.GetDefaultCitizenName(citizenData.m_citizen) + ", leaving from " + BuildingManager.instance.m_buildings.m_buffer[(int)citizenData.m_sourceBuilding].m_position.x + "/" + BuildingManager.instance.m_buildings.m_buffer[(int)citizenData.m_sourceBuilding].m_position.z + " is traveling to " + BuildingManager.instance.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_position.x + "/" + BuildingManager.instance.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_position.z + ".");
            //Debug.Log(CitizenManager.instance.GetDefaultCitizenName(citizenData.m_citizen) + ", is traveling to " + citizenData.m_targetPos.x + "/" + citizenData.m_targetPos.z + ". and is at " + frameData.m_position.x + "/" + frameData.m_position.z + ".");

            /*
            citizenData.m_path
            citizenData.m_targetDir
            citizenData.m_targetPos
            PathManager.instance.m_properties
            frameData.m_insideBuilding
            frameData.m_position
            frameData.m_velocity
            */

            uint num1 = Singleton<SimulationManager>.instance.m_currentFrameIndex;                                                                                                      //num1 = current frame
            Vector3 vector3_1 = (Vector3)citizenData.m_targetPos - frameData.m_position;                                                                                                //vector3_1 = targetpos - citizen's frame location
            float f = lodPhysics || (double)citizenData.m_targetPos.w <= 1.0 / 1000.0 ? vector3_1.sqrMagnitude : VectorUtils.LengthSqrXZ(vector3_1);                                    //set physics
            float sqrMagnitude1 = frameData.m_velocity.sqrMagnitude;                                                                                                                    //sqrMagnitude1 = square lendth of the frame vector
            float minSqrDistance = Mathf.Max(sqrMagnitude1 * 3f, 3f);                                                                                                                   //minSqrDistance = max (sqrMagnitude1 * 3 , 3)
            if (lodPhysics && (long)(num1 >> 4 & 3U) == (long)((int)instanceID & 3))                                                                                                    //if physics and if ???
                minSqrDistance *= 4f;                                                                                                                                                       //multiply minSqrDistance by 4
            bool flag1 = false;                                                                                                                                                         //flag1 = false
            if ((citizenData.m_flags & CitizenInstance.Flags.TryingSpawnVehicle) != CitizenInstance.Flags.None)                                                                         //if the citizen is trying to spawn a vehicle
            {
                bool flag2 = true;                                                                                                                                                          //flag2 is true
                if ((int)++citizenData.m_waitCounter == (int)byte.MaxValue || (int)citizenData.m_path == 0)                                                                                 //if the citizen has no path or if their wait counter is approaching max
                    flag2 = false;                                                                                                                                                              //flag2 is false
                if (flag2)                                                                                                                                                                  //if flag is true
                {
                    PathUnit.Position position;
                    flag2 = Singleton<PathManager>.instance.m_pathUnits.m_buffer[(long)citizenData.m_path].GetPosition((int)citizenData.m_pathPositionIndex >> 1, out position);                //flag2 (bool) set to validity of citizen's location
                    if (flag2)                                                                                                                                                                  //if flag2 is still true
                        flag2 = this.SpawnVehicle(instanceID, ref citizenData, position);                                                                                                           //flag2 is set to the success of spawning a vehicle
                }
                if (!flag2)                                                                                                                                                                 //if flag2 is false
                {
                    citizenData.m_flags &= ~CitizenInstance.Flags.TryingSpawnVehicle;                                                                                                           //set flag TryingSpawnVehicle is false
                    citizenData.m_flags &= ~CitizenInstance.Flags.BoredOfWaiting;                                                                                                               //set flag Bored of Waiting to false
                    citizenData.m_waitCounter = (byte)0;                                                                                                                                        //reset Wait counter
                    this.InvalidPath(instanceID, ref citizenData);                                                                                                                              //report an invalid path
                }
            }
            else if ((citizenData.m_flags & CitizenInstance.Flags.WaitingTransport) != CitizenInstance.Flags.None)                                                                      //else if flag WaitingTransport is true
            {
                bool flag2 = true;                                                                                                                                                          //flag2 is true
                if ((int)citizenData.m_waitCounter < (int)byte.MaxValue)                                                                                                                    //if wait counter is less than 255
                {
                    if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2U) == 0)                                                                                                      //have a 33%-50% chance of in
                        ++citizenData.m_waitCounter;
                }
                else if ((citizenData.m_flags & CitizenInstance.Flags.BoredOfWaiting) == CitizenInstance.Flags.None)                                                                        //else if flag BoredofWaiting is false
                {
                    citizenData.m_flags |= CitizenInstance.Flags.BoredOfWaiting;                                                                                                                //set flag BoredOfWaiting to true
                    citizenData.m_waitCounter = (byte)0;                                                                                                                                        //reset wait counter
                }                                                                                                                                                                           //else (wait counter exceeded AND bored of waiting is true)
                else
                {
                    citizenData.m_flags &= ~CitizenInstance.Flags.WaitingTransport;                                                                                                             //flag WaitingTransport is false
                    citizenData.m_flags &= ~CitizenInstance.Flags.BoredOfWaiting;                                                                                                               //flag BoredOfWaiting is false
                    citizenData.m_flags |= CitizenInstance.Flags.CannotUseTransport;                                                                                                            //flag CannotUseTransport is true
                    citizenData.m_waitCounter = (byte)0;                                                                                                                                        //reset wait counter
                    flag2 = false;                                                                                                                                                              //flag2 is false
                    this.InvalidPath(instanceID, ref citizenData);                                                                                                                              //report invalid path
                }
                if (flag2 && (double)f < (double)minSqrDistance)                                                                                                                            //if flag2 is true and f (physics) < minSqrDistance
                {
                    if ((long)(num1 >> 4 & 7U) == (long)((int)instanceID & 7))                                                                                                                  //if num1 (currentframeindex) is equal to the instanceID ???
                        citizenData.m_targetPos = this.GetTransportWaitPosition(instanceID, ref citizenData, ref frameData, minSqrDistance);                                                        //set the target position to GetTransportWaitPosition **
                    vector3_1 = (Vector3)citizenData.m_targetPos - frameData.m_position;                                                                                                        //vector3_1 = target position - frame position (current position?)
                    f = lodPhysics || (double)citizenData.m_targetPos.w <= 1.0 / 1000.0 ? vector3_1.sqrMagnitude : VectorUtils.LengthSqrXZ(vector3_1);                                          //something with physics
                }
            }
            else if ((citizenData.m_flags & CitizenInstance.Flags.WaitingTaxi) != CitizenInstance.Flags.None)                                                                           //else if flag WaitingTaxi is true
            {
                bool flag2 = false;                                                                                                                                                         //flag2 is false
                if ((int)citizenData.m_citizen != 0)                                                                                                                                        //if citizen is valid
                {
                    flag2 = (int)Singleton<CitizenManager>.instance.m_citizens.m_buffer[(long)citizenData.m_citizen].m_vehicle != 0;                                                            //flag2 is the validity of the citizen's vehicle
                    if (!flag2 && (long)(num1 >> 4 & 15U) == (long)((int)instanceID & 15))                                                                                                       //if vehicle is valid and if frame instance is equal to the citizen instance
                        Singleton<TransferManager>.instance.AddIncomingOffer(TransferManager.TransferReason.Taxi, new TransferManager.TransferOffer()                                               //AddIncomingOFfer(reason: taxi)
                        {
                            Priority = 7,                                                                                                                                                               //Sets attributes of transfer offer
                            Citizen = citizenData.m_citizen,
                            Position = frameData.m_position,
                            Amount = 1,
                            Active = false
                        });
                }
                if ((int)citizenData.m_waitCounter < (int)byte.MaxValue)                                                                                                                    //if wait counter isn't exceeded
                {
                    if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2U) == 0)                                                                                                      //33%-50% chance of incrementing counter
                        ++citizenData.m_waitCounter;
                }
                else if ((citizenData.m_flags & CitizenInstance.Flags.BoredOfWaiting) == CitizenInstance.Flags.None)                                                                        //else if flag BoredOfWaiting is false
                {
                    citizenData.m_flags |= CitizenInstance.Flags.BoredOfWaiting;                                                                                                                //flag BoredOfWaiting is true
                    citizenData.m_waitCounter = (byte)0;                                                                                                                                        //reset wait counter
                }
                else if (!flag2)                                                                                                                                                            //else if flag2 is false
                {
                    citizenData.m_flags &= ~CitizenInstance.Flags.WaitingTaxi;                                                                                                                  //flag WaitingTaxi is false
                    citizenData.m_flags &= ~CitizenInstance.Flags.BoredOfWaiting;                                                                                                               //flag BoredOfWaiting is false
                    citizenData.m_flags |= CitizenInstance.Flags.CannotUseTaxi;                                                                                                                 //flag CannotUseTaxi is true
                    citizenData.m_waitCounter = (byte)0;                                                                                                                                        //reset wait counter
                    this.InvalidPath(instanceID, ref citizenData);                                                                                                                              //report invalid path
                }
            }
            else if ((citizenData.m_flags & CitizenInstance.Flags.EnteringVehicle) != CitizenInstance.Flags.None)                                                                       //else if flag EnteringVehicle is true
            {
                if ((double)f < (double)minSqrDistance)                                                                                                                                     //if f (physics) is less than minSqrDistance
                {
                    citizenData.m_targetPos = this.GetVehicleEnterPosition(instanceID, ref citizenData, minSqrDistance);                                                                        //set target position to GetVechileEnterPosition (citizen instance, minSqrDistance
                    vector3_1 = (Vector3)citizenData.m_targetPos - frameData.m_position;                                                                                                        //vector3_1 is target position - frame (instance?) position
                    f = lodPhysics || (double)citizenData.m_targetPos.w <= 1.0 / 1000.0 ? vector3_1.sqrMagnitude : VectorUtils.LengthSqrXZ(vector3_1);                                          //something with physics
                }
            }
            else if ((double)f < (double)minSqrDistance)                                                                                                                                //else if f (physics) is less than minSqrDistance
            {
                if ((int)citizenData.m_path != 0)                                                                                                                                           //if citizen has a path
                {
                    if ((citizenData.m_flags & CitizenInstance.Flags.WaitingPath) == CitizenInstance.Flags.None)                                                                                //if flag WaitingPathis false
                    {
                        citizenData.m_targetPos = this.GetPathTargetPosition(instanceID, ref citizenData, ref frameData, minSqrDistance);                                                           //target position = GetPathTargetPosition (ID, citizen instance, minSqrDistance
                        if ((citizenData.m_flags & CitizenInstance.Flags.OnPath) == CitizenInstance.Flags.None)                                                                                     //if flag OnPath is false
                            citizenData.m_targetPos.w = 1f;                                                                                                                                             //target position's w = 1
                    }
                }
                else
                {                                                                                                                                                                           //else (if citizen lacks a path)
                    if ((citizenData.m_flags & CitizenInstance.Flags.RidingBicycle) != CitizenInstance.Flags.None)                                                                              //if flag RidingBicycle is true
                    {
                        if ((int)citizenData.m_citizen != 0)                                                                                                                                        //if citizen is valid
                            Singleton<CitizenManager>.instance.m_citizens.m_buffer[(long)citizenData.m_citizen].SetVehicle(citizenData.m_citizen, (ushort)0, 0U);                                       //set this citizen's vehicle to 0
                        citizenData.m_flags &= ~CitizenInstance.Flags.RidingBicycle;                                                                                                                //flag RidingBicycle is false
                    }
                    citizenData.m_flags &= ~(CitizenInstance.Flags.OnPath | CitizenInstance.Flags.OnBikeLane);                                                                                  //??? probably means that both flags OnPath and OnBikeLane are set to false?
                    if ((int)citizenData.m_targetBuilding != 0 && ((citizenData.m_flags & CitizenInstance.Flags.AtTarget) == CitizenInstance.Flags.None || (long)(num1 >> 4 & 15U) == (long)((int)instanceID & 15)))
                        //if target building is valid and if (flag AtTarget is false or if some condition comparing frame instance and citizen ID is met)
                        this.GetBuildingTargetPosition(instanceID, ref citizenData, minSqrDistance);                                                                                                //GetBuildingTargetPosition (iD, citizen instanec and minSqrDistance)
                    if ((citizenData.m_flags & CitizenInstance.Flags.Panicking) == CitizenInstance.Flags.None)                                                                                  //if flag Panicking is false
                        flag1 = true;                                                                                                                                                               //flag1 is true
                }
                vector3_1 = (Vector3)citizenData.m_targetPos - frameData.m_position;                                                                                                            //Vector3_1 = tgarget position - frame (instance?) position
                f = lodPhysics || (double)citizenData.m_targetPos.w <= 1.0 / 1000.0 ? vector3_1.sqrMagnitude : VectorUtils.LengthSqrXZ(vector3_1);                                              //physics command
            }
            float num2 = this.m_info.m_walkSpeed;                                                                                                       //num2 = walk speed
            float b = 2f;                                                                                                                                                                       //b = 2
            if ((citizenData.m_flags & CitizenInstance.Flags.HangAround) != CitizenInstance.Flags.None)                                                                                         //if flag HangAround is true
                num2 = Mathf.Max(num2 * 0.5f, 1f);                                                                                                                                                  //num2 = max(num2 / 2 , 1)
            else if ((citizenData.m_flags & CitizenInstance.Flags.RidingBicycle) != CitizenInstance.Flags.None)                                                                                 //else if flag RidingBicycle is true (meaning that flag HangAround is false)
            {
                if ((citizenData.m_flags & CitizenInstance.Flags.OnBikeLane) != CitizenInstance.Flags.None)                                                                                         //if flag OnBikeLane is true
                    num2 *= 2f;                                                                                                                                                                         //num2 *=
                else
                    num2 *= 1.5f;
                //else
                //num2 *= 1.5
            }
            if ((double)sqrMagnitude1 > 0.00999999977648258)                                                                                                                                        //if sqrMagnitude1 > 0.999999 (could mean >= 1 or a loss of precision (>1)
                frameData.m_position += frameData.m_velocity * 0.5f;                                                                                                                                    //position += velocity / 2
            Vector3 vector3_2;
            if ((double)f < 1.0)                                                                                                                                                                    //
            {
                vector3_2 = Vector3.zero;
                if ((citizenData.m_flags & CitizenInstance.Flags.EnteringVehicle) != CitizenInstance.Flags.None)
                {
                    if (this.EnterVehicle(instanceID, ref citizenData))
                        return;
                }
                else if (flag1)
                {
                    if (this.ArriveAtTarget(instanceID, ref citizenData))
                        return;
                    citizenData.m_flags |= CitizenInstance.Flags.AtTarget;
                    if (Singleton<SimulationManager>.instance.m_randomizer.Int32(256U) == 0)
                        citizenData.m_targetSeed = (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(256U);
                }
                else
                    citizenData.m_flags &= ~CitizenInstance.Flags.AtTarget;
            }
            else
            {
                float num3 = Mathf.Sqrt(f);
                float num4 = Mathf.Sqrt(sqrMagnitude1);
                float num5 = Mathf.Max(0.0f, Vector3.Dot(vector3_1, frameData.m_velocity) / Mathf.Max(1f, num4 * num3));
                float num6 = Mathf.Max(0.5f, num2 * num5 * num5 * num5);
                vector3_2 = vector3_1 * Mathf.Min(0.577f, num6 / num3);
                citizenData.m_flags &= ~CitizenInstance.Flags.AtTarget;
                if ((citizenData.m_flags & CitizenInstance.Flags.RequireSlowStart) != CitizenInstance.Flags.None && (int)citizenData.m_waitCounter < 8)
                {
                    ++citizenData.m_waitCounter;
                    frameData.m_velocity = Vector3.zero;
                    return;
                }
            }
            frameData.m_underground = (citizenData.m_flags & CitizenInstance.Flags.Underground) != CitizenInstance.Flags.None;
            frameData.m_insideBuilding = (citizenData.m_flags & CitizenInstance.Flags.InsideBuilding) != CitizenInstance.Flags.None;
            frameData.m_transition = (citizenData.m_flags & CitizenInstance.Flags.Transition) != CitizenInstance.Flags.None;
            if ((double)f < 1.0 && flag1 && (citizenData.m_flags & CitizenInstance.Flags.SittingDown) != CitizenInstance.Flags.None)
            {
                citizenData.m_flags |= CitizenInstance.Flags.RequireSlowStart;
                citizenData.m_waitCounter = (byte)0;
                frameData.m_velocity = ((Vector3)citizenData.m_targetPos - frameData.m_position) * 0.5f;
                frameData.m_position += frameData.m_velocity * 0.5f;
                if ((double)citizenData.m_targetDir.sqrMagnitude <= 0.00999999977648258)
                    return;
                frameData.m_rotation = Quaternion.LookRotation(VectorUtils.X_Y(citizenData.m_targetDir));
            }
            else
            {
                citizenData.m_flags &= ~CitizenInstance.Flags.RequireSlowStart;
                Vector3 vector3_3 = vector3_2 - frameData.m_velocity;
                float magnitude = vector3_3.magnitude;
                vector3_3 *= b / Mathf.Max(magnitude, b);
                frameData.m_velocity += vector3_3;
                frameData.m_velocity -= Mathf.Max(0.0f, Vector3.Dot(frameData.m_position + frameData.m_velocity - (Vector3)citizenData.m_targetPos, frameData.m_velocity)) / Mathf.Max(0.01f, frameData.m_velocity.sqrMagnitude) * frameData.m_velocity;
                float sqrMagnitude2 = frameData.m_velocity.sqrMagnitude;
                bool flag2 = !lodPhysics && (double)citizenData.m_targetPos.w > 1.0 / 1000.0 && ((double)sqrMagnitude2 > 0.00999999977648258 || (double)sqrMagnitude1 > 0.00999999977648258);
                ushort buildingID = !flag2 ? (ushort)0 : Singleton<BuildingManager>.instance.GetWalkingBuilding(frameData.m_position + frameData.m_velocity * 0.5f);
                if ((double)sqrMagnitude2 > 0.00999999977648258)
                {
                    if (!lodPhysics)
                    {
                        Vector3 zero = Vector3.zero;
                        float pushDivider = 0.0f;
                        this.CheckCollisions(instanceID, ref citizenData, frameData.m_position, frameData.m_position + frameData.m_velocity, buildingID, ref zero, ref pushDivider);
                        if ((double)pushDivider > 0.00999999977648258)
                        {
                            Vector3 vector3_4 = Vector3.ClampMagnitude(zero * (1f / pushDivider), Mathf.Sqrt(sqrMagnitude2) * 0.9f);
                            frameData.m_velocity += vector3_4;
                        }
                    }
                    frameData.m_position += frameData.m_velocity * 0.5f;
                    Vector3 forward = frameData.m_velocity;
                    if ((citizenData.m_flags & CitizenInstance.Flags.RidingBicycle) == CitizenInstance.Flags.None)
                        forward.y = 0.0f;
                    if ((double)forward.sqrMagnitude > 0.00999999977648258)
                        frameData.m_rotation = Quaternion.LookRotation(forward);
                }
                if (!flag2)
                    return;
                Vector3 worldPos = frameData.m_position;
                float terrainHeight = Singleton<TerrainManager>.instance.SampleDetailHeight(worldPos);
                if ((int)buildingID != 0)
                {
                    float num3 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)buildingID].SampleWalkingHeight(worldPos, terrainHeight);
                    worldPos.y = worldPos.y + (num3 - worldPos.y) * Mathf.Min(1f, citizenData.m_targetPos.w * 4f);
                    frameData.m_position.y = worldPos.y;
                }
                else
                {
                    if ((double)Mathf.Abs(terrainHeight - worldPos.y) >= 2.0)
                        return;
                    worldPos.y = worldPos.y + (terrainHeight - worldPos.y) * Mathf.Min(1f, citizenData.m_targetPos.w * 4f);
                    frameData.m_position.y = worldPos.y;
                }
            }
            if ((int)citizenData.m_citizen != 0)
            {
                if (Tracker.AccessibilitySourceActive())
                {
                    TrafficLog.CitizenStepBroadcastedSource(citizenData.m_citizen);
                }
                if (Tracker.AccessibilityTargetActive())
                {
                    TrafficLog.CitizenStepBroadcastedTarget(citizenData.m_citizen);
                }
            }
        }

        [RedirectMethod]
        private Vector4 GetTransportWaitPosition(ushort instanceID, ref CitizenInstance citizenData, ref CitizenInstance.Frame frameData, float minSqrDistance)
        {
            PathManager instance1 = Singleton<PathManager>.instance;
            NetManager instance2 = Singleton<NetManager>.instance;
            PathUnit.Position position1;
            if (!instance1.m_pathUnits.m_buffer[citizenData.m_path].GetPosition((int)citizenData.m_pathPositionIndex >> 1, out position1))
            {
                this.InvalidPath(instanceID, ref citizenData);
                return citizenData.m_targetPos;
            }
            ushort num1 = instance2.m_segments.m_buffer[(int)position1.m_segment].m_startNode;
            if ((citizenData.m_flags & CitizenInstance.Flags.BoredOfWaiting) != CitizenInstance.Flags.None)
                instance2.m_nodes.m_buffer[(int)num1].m_maxWaitTime = byte.MaxValue;
            else if ((int)citizenData.m_waitCounter > (int)instance2.m_nodes.m_buffer[(int)num1].m_maxWaitTime)
                instance2.m_nodes.m_buffer[(int)num1].m_maxWaitTime = citizenData.m_waitCounter;
            uint laneID = instance2.m_nodes.m_buffer[(int)num1].m_lane;
            if ((int)laneID == 0)
                return citizenData.m_targetPos;
            uint num2 = (uint)instance2.m_lanes.m_buffer[laneID].m_segment;
            NetInfo.Lane laneInfo;
            if (!instance2.m_segments.m_buffer[num2].GetClosestLane(laneID, NetInfo.LaneType.Pedestrian, VehicleInfo.VehicleType.None, out laneID, out laneInfo))
                return citizenData.m_targetPos;
            ushort num3 = instance2.m_segments.m_buffer[num2].m_startNode;
            ushort num4 = instance2.m_segments.m_buffer[num2].m_endNode;
            if (((instance2.m_nodes.m_buffer[(int)num3].m_flags | instance2.m_nodes.m_buffer[(int)num4].m_flags) & NetNode.Flags.Disabled) != NetNode.Flags.None)
                citizenData.m_waitCounter = byte.MaxValue;
            Randomizer randomizer = new Randomizer((uint)instanceID | laneID << 16);
            float num5 = instance2.m_nodes.m_buffer[(int)num1].Info.m_netAI.MaxTransportWaitDistance();
            int num6 = (int)instance2.m_nodes.m_buffer[(int)num1].m_laneOffset << 8;
            int @int = Mathf.RoundToInt(num5 * 65280f / Mathf.Max(1f, instance2.m_lanes.m_buffer[laneID].m_length));
            int min = Mathf.Clamp(num6 - @int, 0, 65280);
            int max = Mathf.Clamp(num6 + @int, 0, 65280);
            int num7 = randomizer.Int32(min, max);
            Vector3 position2;
            Vector3 direction;
            instance2.m_lanes.m_buffer[laneID].CalculatePositionAndDirection((float)num7 * 1.531863E-05f, out position2, out direction);
            float num8 = (float)((double)Mathf.Max(0.0f, laneInfo.m_width - 1f) * (double)randomizer.Int32(-500, 500) * (1.0 / 1000.0));
            position2 += Vector3.Cross(Vector3.up, direction).normalized * num8;
            return new Vector4(position2.x, position2.y, position2.z, 0.0f);
        }

        /*protected virtual void ArriveAtDestination(ushort instanceID, ref CitizenInstance citizenData, bool success)
        {
            //Debug.Log("Citizen " + instanceID + " arrived at their destination.");
            uint citizenID = citizenData.m_citizen;
            if ((int)citizenID != 0)
            {
                CitizenManager instance1 = Singleton<CitizenManager>.instance;
                instance1.m_citizens.m_buffer[(long)citizenID].SetVehicle(citizenID, (ushort)0, 0U);
                if (success)
                    instance1.m_citizens.m_buffer[(long)citizenID].SetLocationByBuilding(citizenID, citizenData.m_targetBuilding);
                if ((int)citizenData.m_targetBuilding != 0 && instance1.m_citizens.m_buffer[(long)citizenID].CurrentLocation == Citizen.Location.Visit)
                {
                    BuildingManager instance2 = Singleton<BuildingManager>.instance;
                    BuildingInfo info = instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].Info;
                    int amountDelta = -100;
                    info.m_buildingAI.ModifyMaterialBuffer(citizenData.m_targetBuilding, ref instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding], TransferManager.TransferReason.Shopping, ref amountDelta);
                    ushort eventID = instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_eventIndex;
                    if ((int)eventID != 0)
                    {
                        EventManager instance3 = Singleton<EventManager>.instance;
                        instance3.m_events.m_buffer[(int)eventID].Info.m_eventAI.VisitorEnter(eventID, ref instance3.m_events.m_buffer[(int)eventID], citizenID);
                    }
                }
            }
            if ((citizenData.m_flags & CitizenInstance.Flags.HangAround) != CitizenInstance.Flags.None && success)
                return;
            this.SetSource(instanceID, ref citizenData, (ushort)0);
            this.SetTarget(instanceID, ref citizenData, (ushort)0);
            citizenData.Unspawn(instanceID);
        }*/

        /*protected virtual bool ArriveAtTarget(ushort instanceID, ref CitizenInstance citizenData)
        {
            //Debug.Log("Citizen " + instanceID + " arrived at their target.");
            if ((citizenData.m_flags & CitizenInstance.Flags.HangAround) != CitizenInstance.Flags.None)
            {
                uint num = citizenData.m_citizen;
                if ((int)num != 0)
                {
                    CitizenManager instance = Singleton<CitizenManager>.instance;
                    if (instance.m_citizens.m_buffer[(long)num].CurrentLocation == Citizen.Location.Moving)
                        this.ArriveAtDestination(instanceID, ref citizenData, true);
                    if ((int)instance.m_citizens.m_buffer[(long)num].GetBuildingByLocation() == (int)citizenData.m_targetBuilding)
                        return false;
                }
                citizenData.m_flags &= ~CitizenInstance.Flags.TargetFlags;
                citizenData.Unspawn(instanceID);
            }
            else
                this.ArriveAtDestination(instanceID, ref citizenData, true);
            return true;
        }*/

        /*protected virtual bool EnterVehicle(ushort instanceID, ref CitizenInstance citizenData)
        {
            //Debug.Log(instanceID.ToString() + " entered a vehicle.");
            citizenData.m_flags &= ~CitizenInstance.Flags.EnteringVehicle;
            citizenData.Unspawn(instanceID);
            uint num = citizenData.m_citizen;
            if ((int)num != 0)
            {
                VehicleManager instance = Singleton<VehicleManager>.instance;
                ushort vehicleID = Singleton<CitizenManager>.instance.m_citizens.m_buffer[(long)num].m_vehicle;
                if ((int)vehicleID != 0)
                    vehicleID = instance.m_vehicles.m_buffer[(int)vehicleID].GetFirstVehicle(vehicleID);
                if ((int)vehicleID != 0)
                {
                    VehicleInfo info = instance.m_vehicles.m_buffer[(int)vehicleID].Info;
                    int ticketPrice = info.m_vehicleAI.GetTicketPrice(vehicleID, ref instance.m_vehicles.m_buffer[(int)vehicleID]);
                    if (ticketPrice != 0)
                        Singleton<EconomyManager>.instance.AddResource(EconomyManager.Resource.PublicIncome, ticketPrice, info.m_class);
                }
            }
            return false;
        }*/

        /*protected virtual void GetBuildingTargetPosition(ushort instanceID, ref CitizenInstance citizenData, float minSqrDistance)
        {
            //Debug.Log("Citizen " + instanceID + " is headed toward a building that is " + minSqrDistance + " units away.");
            if ((int)citizenData.m_targetBuilding != 0)
            {
                BuildingManager instance = Singleton<BuildingManager>.instance;
                if ((int)instance.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_fireIntensity != 0)
                {
                    citizenData.m_flags |= CitizenInstance.Flags.Panicking;
                    citizenData.m_targetDir = Vector2.zero;
                }
                else
                {
                    BuildingInfo info = instance.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].Info;
                    Randomizer randomizer = new Randomizer((int)instanceID << 8 | (int)citizenData.m_targetSeed);
                    Vector3 position;
                    Vector3 target;
                    Vector2 direction;
                    CitizenInstance.Flags specialFlags;
                    info.m_buildingAI.CalculateUnspawnPosition(citizenData.m_targetBuilding, ref instance.m_buildings.m_buffer[(int)citizenData.m_targetBuilding], ref randomizer, this.m_info, instanceID, out position, out target, out direction, out specialFlags);
                    citizenData.m_flags = citizenData.m_flags & ~CitizenInstance.Flags.TargetFlags | specialFlags;
                    citizenData.m_targetPos = new Vector4(position.x, position.y, position.z, 1f);
                    citizenData.m_targetDir = direction;
                }
            }
            else
            {
                citizenData.m_flags &= ~CitizenInstance.Flags.TargetFlags;
                citizenData.m_targetDir = Vector2.zero;
            }
        }*/

        /*protected new TransferManager.TransferReason GetLeavingReason(uint citizenID, ref Citizen data)
        {
            switch (data.WealthLevel)
            {
                case Citizen.Wealth.Low:
                    return TransferManager.TransferReason.LeaveCity0;

                case Citizen.Wealth.Medium:
                    return TransferManager.TransferReason.LeaveCity1;

                case Citizen.Wealth.High:
                    return TransferManager.TransferReason.LeaveCity2;

                default:
                    return TransferManager.TransferReason.LeaveCity0;
            }
        }*/

        /*
        public const byte FLAG_CREATED = 1;
        public const byte FLAG_IS_HEAVY = 16;
        public const byte FLAG_IGNORE_BLOCKED = 32;
        public const byte FLAG_STABLE_PATH = 64;
        public const byte FLAG_RANDOM_PARKING = 128;
        public const byte FLAG_QUEUED = 1;
        public const byte FLAG_CALCULATING = 2;
        public const byte FLAG_READY = 4;
        public const byte FLAG_FAILED = 8;
        */

        /*protected virtual Vector4 GetVehicleEnterPosition(ushort instanceID, ref CitizenInstance citizenData, float minSqrDistance)
        {
            //Debug.Log("Citizen " + instanceID + " entered a vehicle with a minimum square distance of " + minSqrDistance + ".");
            CitizenManager instance1 = Singleton<CitizenManager>.instance;
            VehicleManager instance2 = Singleton<VehicleManager>.instance;
            uint num1 = citizenData.m_citizen;
            if ((int)num1 != 0)
            {
                ushort num2 = instance1.m_citizens.m_buffer[(long)num1].m_vehicle;
                if ((int)num2 != 0)
                {
                    Vector4 vector4 = (Vector4)instance2.m_vehicles.m_buffer[(int)num2].GetClosestDoorPosition((Vector3)citizenData.m_targetPos, VehicleInfo.DoorType.Enter);
                    vector4.w = citizenData.m_targetPos.w;
                    return vector4;
                }
            }
            return citizenData.m_targetPos;
        }*/

        /* CitizenInstance Flags
        None = 0,
        Created = 1,
        Deleted = 2,
        Underground = 4,
        CustomName = 8,
        Character = 16,
        BorrowCar = 32,
        HangAround = 64,
        InsideBuilding = 128,
        WaitingPath = 256,
        WaitingTransport = 512,
        TryingSpawnVehicle = 1024,
        EnteringVehicle = 2048,
        BoredOfWaiting = 4096,
        CannotUseTransport = 8192,
        Panicking = 16384,
        OnPath = 32768,
        SittingDown = 65536,
        AtTarget = 131072,
        RequireSlowStart = 262144,
        Transition = 524288,
        RidingBicycle = 1048576,
        OnBikeLane = 2097152,
        WaitingTaxi = 4194304,
        CannotUseTaxi = 8388608,
        CustomColor = 16777216,
        TargetFlags = SittingDown | Panicking | HangAround,
        All = -1,
        */
    }
}