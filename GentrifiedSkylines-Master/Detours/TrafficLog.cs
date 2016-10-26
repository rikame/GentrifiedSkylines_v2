using ColossalFramework;
using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    public static class TrafficLog
    {
        private static bool activated = false;
        private static ushort[] buildingIDs;
        private static ushort buildingIndex;
        private static float[,] grid2;
        private static BuildingTrafficLog[] masterLog;
        private static bool used = false;

        public static void Activate()
        {
            activated = true;
            buildingIndex = 0;
            masterLog = new BuildingTrafficLog[ushort.MaxValue];
            buildingIDs = new ushort[ushort.MaxValue];
        }
        /*        public static void Activate(ushort tempID)
                {
                    Activate();
                    TryAddBuildingLog(tempID);
                }*/
        /*        public static void Activate(uint id)
                {
                    Activate();
                    CitizenStepBroadcastedTarget(id);
                }*/
        public static void AddBuildingRef(ushort tempID)
        {
            buildingIDs[buildingIndex] = tempID;
            buildingIndex++;
        }
        public static void CitizenStepBroadcastedSource(uint id)
        {
            if (!activated)                                                                                     //Check to make sure all is activated
            {
                Activate();
            }
            ushort num1 = Singleton<CitizenManager>.instance.m_instances.m_buffer[id].m_sourceBuilding;         //Assign Building ID to a temporary variable
            if (IDToRef(num1) != 0)                                                                             //If the BuildingLog for this building exists
            {
                masterLog[num1].sourceLog[masterLog[num1].LookupTripCitizenJoinSource(id)].CitizenStepBroadcasted();
            }
            else                                                                                                //If the BuildingLog for this building does not exist
            {
                TryAddBuildingLog(num1);                                                                            //Call Try AddBuildingLog for the ID
                CitizenStepBroadcastedSource(id);                                                                         //Recall this method
            }
        }

        public static void CitizenStepBroadcastedTarget(uint id)
        {
            if (!activated)                                                                                     //Check to make sure all is activated
            {
                Activate();
            }
            ushort num1 = Singleton<CitizenManager>.instance.m_instances.m_buffer[id].m_targetBuilding;         //Assign Building ID to a temporary variable
            if (IDToRef(num1) != 0)                                                                             //If the BuildingLog for this building exists
            {
                masterLog[num1].targetLog[masterLog[num1].LookupTripCitizenJoinTarget(id)].CitizenStepBroadcasted();
            }
            else                                                                                                //If the BuildingLog for this building does not exist
            {
                TryAddBuildingLog(num1);                                                                            //Call Try AddBuildingLog for the ID
                CitizenStepBroadcastedTarget(id);                                                                         //Recall this method
            }
        }
        public static float[,] CollectOldRatings()
        {
            return grid2;
        }
        public static float[,] CollectRatings()
        {
            if (activated)
            {
                grid2 = new float[byte.MaxValue + 1, byte.MaxValue + 1];                                                                 //Create grid[,]
                for (int i = 0; i <= byte.MaxValue; i++)                                                                                //Cycle through grid[,] and zero values
                {
                    for (int j = 0; j <= byte.MaxValue; j++)
                    {
                        grid2[i, j] = 0;
                    }
                }
                if (used == true)                                                                                                       //if this has been used
                {
                    for (ushort i = 0; i <= buildingIndex; i++)                                                                         //Loop for building index (<= operator is used because index=1 -> array[0]
                    {
                        ushort localID = buildingIDs[i];                                                                                    //Assign localID to real current buildingID
                        float x = Singleton<BuildingManager>.instance.m_buildings.m_buffer[localID].m_position.x;                           //Get building X
                        float z = Singleton<BuildingManager>.instance.m_buildings.m_buffer[localID].m_position.z;                           //Get building Z
                        byte x2 = Convert.ToByte(Mathf.Clamp(((x + (128f * 38.4f)) / 38.4f), 0, byte.MaxValue));                            //Transform x coordinates
                        byte z2 = Convert.ToByte(Mathf.Clamp(((z + (128f * 38.4f)) / 38.4f), 0, byte.MaxValue));                            //Transform z coordinates
                        grid2[x2, z2] += masterLog[i].GetRating(Tracker.AccessibilitySourceActive(), Tracker.AccessibilityTargetActive(), Tracker.GetSquared());  //Get ratings for criteria defined in tracker
                    }
                }
                return grid2;
            }
            else
            {
                Activate();                                 //Activate
                return CollectRatings();      //Try again
            }
        }
        public static ushort IDToRef(ushort tempID)     //Takes real ID and finds index position
        {
            if (activated)
            {
                for (int i = 0; i <= buildingIndex; i++)
                {
                    if (buildingIDs[i] == tempID)
                    {
                        return Convert.ToUInt16(i);
                    }
                }
                return 0;
            }
            else
            {
                Activate();
                return IDToRef(tempID);
            }
        }
        public static void Reset()
        {
            //Reset tracking
        }
        public static void TryAddBuildingLog(ushort tempID)
        {
            used = true;
            if (!activated)
            {
                Activate();       //Try again
            }
            if (IDToRef(tempID) == 0)   //Queries if the building has been added already.
            {
                AddBuildingRef(tempID);                                 //Add tempID for tracking array progress
                masterLog[buildingIndex] = new BuildingTrafficLog();    //Add new BuildingTrafficLog to the master log
                masterLog[buildingIndex].NewBuildingLog(tempID);        //Calls secondary constructor in the BuidingTrafficLog and assigns its buildingID
            }
        }
    }
}