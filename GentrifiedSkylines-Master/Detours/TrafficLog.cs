using ColossalFramework;
using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    public static class TrafficLog
    {
        private static BuildingTrafficLog[] masterLog;//--------------------------------------------Master log of Building Traffic Log
        private static float[,] grid;//-------------------------------------------------------------Float[x,z] geographcally virtualizing index values
        public static ushort gridLength = (ushort)(byte.MaxValue + 1);//----------------------------Length of each side of the grid for convenience
        public static ushort logLength = (ushort)(byte.MaxValue + 1);//---------------------------------Lenght of each buildingLog for convenience
        private static float[,] gridLast;//---------------------------------------------------------Stores last good grid
        private static float[,] gridLong = new float[byte.MaxValue + 1, byte.MaxValue + 1];//-------Float[x,z] of values tracked over several iterations
        private static ushort[] buildingIDs;//------------------------------------------------------Array joining arbitrary masterLog indeces to real building instance IDs
        private static ushort buildingIndex = 0;//--------------------------------------------------Tracking variable indicating the next index primed for writing
        private static uint gridCount = 0;//--------------------------------------------------------Tracks number of iterations for gridLong
        private static uint gridArchive = 0;//----------------------------------------------------- Tracks number of iterations without reset
        private static bool activated = false;//----------------------------------------------------Indicates that masterLog and buildingIndex have been substantiated
        private static bool used = false;//---------------------------------------------------------Indicates that the masterLog should have at least one value to avoid NullReferences
        private static uint maxLength = (ushort.MaxValue + 1);//------------------------------------Maximum length of arrays for convenience
        private static byte errorCounter = 0;//-----------------------------------------------------Counts buildingID exceedance errors to prevent overreporting through ChirpPanel
        private static byte errorMax = 16;//--------------------------------------------------------Distance between buildingID exceedence error reports
        private static bool errorReported = false;//------------------------------------------------Tracks if a buildingID exceedance error has been reported recently
        
        public static void Activate()                                   //Substantiates attributes
        {
            masterLog = new BuildingTrafficLog[maxLength];
            buildingIndex = 0;
            buildingIDs = new ushort[maxLength];
            activated = true;
        }

        private static void buildingExceedance()                        //TODO: Write Method : Mitigates buildingID exceedance
        {
        }

        public static void AddBuildingRef(ushort tempID)                //Adds a buildingID to the reference array for tracking through TrafficLog
        {
            if ((uint)(buildingIndex + 1) <= maxLength)                 //Check that the new buildingIndex will not exceed the size of the array
            {
                buildingIDs[buildingIndex] = tempID;
                buildingIndex++;
            }
            else
            {
                if (!errorReported)
                {
                    buildingExceedance();
                    Singleton<ChirpPanel>.instance.AddMessage(new ChirpMessage("Accessibility Log", "Building Reference Log"));
                    errorCounter = 0;
                    errorReported = true;
                }
                else
                {
                    if (errorCounter < errorMax)
                        errorCounter += 1;
                    else
                    {
                        errorReported = false;
                    }
                }
            }
        }

        public static void CitizenStepBroadcastedSource(uint id)        //Informs the source's BuildingLog's trip associated with the citizenID to retrive data
        {
            if (!activated)                                                                                     //Check to make sure all is activated
                Activate();
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

        public static void CitizenStepBroadcastedTarget(uint id)        //Informs the target's BuildingLog's trip associated with the citizenID to retrive data
        {
            if (!activated)                                                                                             //Check to make sure all is activated
                Activate();
            ushort num1 = Singleton<CitizenManager>.instance.m_instances.m_buffer[id].m_targetBuilding;                 //Assign Building ID to a temporary variable
            if (IDToRef(num1) != 0)                                                                                     //If the BuildingLog for this building exists
                masterLog[num1].targetLog[masterLog[num1].LookupTripCitizenJoinTarget(id)].CitizenStepBroadcasted();
            else                                                                                                        //If the BuildingLog for this building does not exist
            {
                TryAddBuildingLog(num1);                                                                                    //Call Try AddBuildingLog for the ID
                CitizenStepBroadcastedTarget(id);                                                                           //Recall this method
            }
        }

        public static float[,] CollectRatings()                         //Iteratively scans BuildingLogs for their ratings, updating the grid
        {
            if (activated)
            {
                grid = new float[byte.MaxValue + 1, byte.MaxValue + 1];                                                                 //Create grid[,]
                if (used == true)                                                                                                       //if this has been used
                {
                    for (ushort i = 0; i <= buildingIndex; i++)                                                                         //Loop for building index (<= operator is used because index=1 -> array[0]
                    {
                        ushort localID = buildingIDs[i];                                                                                    //Assign localID to real current buildingID
                        float x = Singleton<BuildingManager>.instance.m_buildings.m_buffer[localID].m_position.x;                           //Get building X
                        float z = Singleton<BuildingManager>.instance.m_buildings.m_buffer[localID].m_position.z;                           //Get building Z
                        byte x2 = Convert.ToByte(Mathf.Clamp(((x + (128f * 38.4f)) / 38.4f), 0, byte.MaxValue));                            //Transform x coordinates
                        byte z2 = Convert.ToByte(Mathf.Clamp(((z + (128f * 38.4f)) / 38.4f), 0, byte.MaxValue));                            //Transform z coordinates
                        grid[x2, z2] += masterLog[i].GetRating(Tracker.GetSquared());                                                       //Get ratings for criteria defined in tracker
                        gridCount += 1;
                    }
                }
                if (Tracker.accessibilityLinger)
                {
                    if (gridCount != 0)
                    {
                        for (int i = 0; i <= byte.MaxValue; i++)
                        {
                            for (int j = 0; j <= byte.MaxValue; j++)
                            {
                                gridLong[i, j] = (gridLong[i, j] * (1 - Tracker.accessibilityLingerFactor)) + (grid[i, j] * Tracker.accessibilityLingerFactor);
                                gridLast = gridLong;
                                return gridLong;
                            }
                        }
                    }
                    else
                        gridLong = grid;
                    gridLast = grid;
                    return grid;
                }
                else
                {
                    gridLong = grid;
                    gridLast = grid;
                    return grid;
                }
            }
            else
            {
                Activate();                   //Activate
                return CollectRatings();      //Try again
            }
        }

        public static float[,] GetAggergateGrid()                       //Returns the value grid
        {
            return gridLong;
        }

        public static float[,] GetLastGrid()                            //Returns the value grid
        {
            return gridLast;
        }

        public static ushort IDToRef(ushort tempID)                     //Takes real buildingID and returns the reference index position
        {
            if (activated)
            {
                for (ushort i = 0; i < buildingIndex; i++)
                {
                    if (buildingIDs[i] == tempID)
                    {
                        return i;
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

        public static void Reset(bool grids, bool logs, bool master)    //Reset tracking
        {
            if (master)                                                                 //If: Master is true
            {
                Activate();                                                                 //Reset BuildingID, buildingIndex and MasterLog
                grids = true;                                                               //Grids is true
            }
            if (logs & used & !master)                                              //If logs true, masterLog is filled and master is false
            {
                for (int i = 0; i < buildingIndex; i++)
                {
                    masterLog[i].ResetMyTrips();
                }
            }
            if (grids)
            {
                gridArchive += gridCount;       //Add count to archive
                gridCount = 0;                  //Resets counter and effectively resets gridLong
                gridLong = new float[byte.MaxValue + 1, byte.MaxValue + 1];
            }
        }

        public static void TryAddBuildingLog(ushort tempID)             //Flag as used and add a new buildingLog if a log for that building isn't yet recorded
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
                masterLog[buildingIndex].AssignBuildingID(tempID);      //Calls secondary constructor in the BuidingTrafficLog and assigns its buildingID
            }
        }
    }
}