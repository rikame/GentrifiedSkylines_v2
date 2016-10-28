using ColossalFramework;
using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    public class BuildingTrafficLog
    {
        public Trip[] sourceLog;//------------------------------------------------------Trip log of sources for the respective building
        public Trip[] targetLog;//------------------------------------------------------Trip log of targets for the respective building
        public uint[] TripCitizenJoinsSource = new uint[byte.MaxValue + 1];//-----------Array for sources linking logs to citizens to write to the correct trips
        public uint[] TripCitizenJoinsTarget = new uint[byte.MaxValue + 1];//-----------Array for targets linking logs to citizens to write to the correct trips
        private bool activatedSource = false;//-----------------------------------------Gains value when the source log gets written-to to avoid invalid calls
        private bool activatedTarget = false;//-----------------------------------------Gains value when the target log gets written-to to avoid invalid calls
        private bool filledSource = false;//--------------------------------------------Becomes true when a source is added to avoid invalid calls
        private bool filledTarget = false;//--------------------------------------------Becomes true when a target is added to avoid invalid calls
        private byte indexSource;//-----------------------------------------------------Index tracking the current source array position primed to be written
        private byte indexTarget;//-----------------------------------------------------Index tracking the current target array position primed to be written
        private ushort countSource = 0;//-----------------------------------------------Counts source indeces until maxing out at maximum length
        private ushort countTarget = 0;//-----------------------------------------------Counts target indeces until maxing out at maximum length
        private ushort m_building = 0;//------------------------------------------------BuildingID associated with this building log

        public void addSourceTrip(Trip t)                                               //Adds a source trip and activates the source array if needed. Counts and increments indeces
        {
            if (!activatedSource)
            {
                ActivateSource(t);
                filledSource = true;
            }
            sourceLog[indexSource] = t;
            if (++countSource >= TrafficLog.logLength)
                countSource = TrafficLog.logLength;
            if (!(indexSource++ < TrafficLog.logLength-1))
                indexSource = 0;                         
        }

        public void addTargetTrip(Trip t)                                               //Adds a target trip and activates the source array if needed. Counts and increments indeces
        {
            if (!activatedTarget)
            {
                ActivateTarget(t);
                filledTarget = true;
            }
            targetLog[indexTarget] = t;
            if (++countTarget >= TrafficLog.logLength)
                countTarget = TrafficLog.logLength;
            if (!(indexTarget++ < TrafficLog.logLength - 1))
                indexTarget = 0;
        }

        public Trip[] GetAllTrips()                                                     //Returns array combining source and target trips
        {
            Trip[] temp = new Trip[countSource + countTarget];
            int i;
            for (i = 0; i < countSource; i++)
                temp[i] = sourceLog[i];
            for (i = countSource; i < (countSource + countTarget); i++)
                temp[i] = targetLog[i];
            return temp;
        }

        public Building GetBuilding()                                                   //Returns the building instance associated with this log
        {
            return Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_building];
        }

        public ushort GetBuildingID()                                                   //Returns the buildingID associated with this log
        {
            return m_building;
        }

        public float GetRating(bool squared)                                            //Scan trips, calculate and return rating
        {
            if (m_building != 0)
            {
                //Count ; Cost ; Distance ; Distance Average ; LegCount
                float accumulated = 0;
                float accumulatedSource = 0;
                byte count = 0;
                Trip[] temp = GetAllTrips();
                for (int i = 0; i < temp.Length; i++)
                {
                    accumulated += targetLog[Convert.ToByte(i)].GetContribution();
                    accumulatedSource = accumulated;
                    accumulatedSource *= accumulatedSource;
                    count++;
                }
                if (accumulated > 0)
                {
                    if (squared)
                        return (float)Math.Sqrt((double)accumulatedSource);
                    return accumulated;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public Trip GetSourceTrip(byte b)                                               //Queries and returns the source trip at specified index
        {
            return sourceLog[Mathf.Clamp(b, 0, countSource - 1)];
        }

        public Trip[] GetSourceTrips()                                                  //Returns array of all source trips
        {
            Trip[] temp = new Trip[countSource];
            for (int i = 0; i < countSource; i++)
                temp[i] = sourceLog[i];
            return temp;
        }

        public Trip GetTargetTrip(byte b)                                               //Queries and returns the target trip at specified index
        {
            return targetLog[Mathf.Clamp(b, 0, countTarget- 1)];
        }

        public Trip[] GetTargetTrips()                                                  //Returns array of all target trips
        {
            Trip[] temp = new Trip[countTarget];
            for (int i = 0; i < countTarget; i++)
                temp[i] = targetLog[i];
            return temp;
        }

        public byte LookupTripCitizenJoinSource(uint i)                                 //Queries and returns the index of the source trip associated with the provided CitizenID
        {
            int i2;
            i2 = Array.IndexOf(TripCitizenJoinsSource, i);
            if (i2 == -1)                                       //If citizenID not found in array
            {
                byte b = indexSource;                               //Record index to be written to
                addSourceTrip(CreateTrip(i));                       //Add citizenID and index
                return b;                                           //Return index written to
            }
            else
                return (byte)i2;                                //Otherwise, return the citizenID's index
        }

        public byte LookupTripCitizenJoinTarget(uint i)                                 //Queries and returns the index of the target trip associated with the provided CitizenID
        {
            int i2;
            i2 = Array.IndexOf(TripCitizenJoinsSource, i);      
            if (i2 == -1)                                       //If citizenZID not found in array
            {
                byte b = indexTarget;                               //Record index to be written to
                addTargetTrip(CreateTrip(i));                       //Add citizenID and index
                return b;                                           //Return index written to
            }
            else
                return (byte)i2;                                //Otherwise, return the citizenID's index
        }

        public void AssignBuildingID(ushort ID)                                         //Assign a buildingID to this log
        {
            m_building = ID;
        }

        public float SumTripRatings(bool source, bool target)                           //Generates a byte[,] grid by caling on finalized trips to retrieve values
        {
            float tripSum = 0;
            float tripCount = 0;
            if (source)
            {
                for (int i = 0; i < countSource; i++)                               //Set both grids to 0 throughout
                {
                    if (sourceLog[i].IsValid())
                    {
                        tripSum += sourceLog[i].GetContribution();
                        tripCount++;
                    }
                }
            }
            if (target)
            {
                for (int i = 0; i < countTarget; i++)                               //Set both grids to 0 throughout
                {
                    if (targetLog[i].IsValid())
                    {
                        tripSum += targetLog[i].GetContribution();
                        tripCount++;
                    }
                }
            }
            if (tripCount == 0)
                return 0;
            return ((float)(tripSum / tripCount));
        }

        private Trip CreateTrip(uint citizenID)                                         //Returns a new Trip instance
        {
            return (new Trip(citizenID, Singleton<CitizenManager>.instance.m_instances.m_buffer[citizenID].m_sourceBuilding, Singleton<CitizenManager>.instance.m_instances.m_buffer[citizenID].m_sourceBuilding));
        }

        private void ActivateSource(Trip t)                                             //Instantiates the attributes necessary to track source trips
        {
            sourceLog = NewTripArray();
            activatedSource = true;
            indexSource = 0;
            addSourceTrip(t);
        }

        private Trip[] NewTripArray()                                                   //Creates a new array of trips at the proper length
        {
            return new Trip[TrafficLog.logLength];
        }

        private void ActivateTarget(Trip t)                                             //Instantiates the attributes necessary to track target trips
        {
            targetLog = NewTripArray();
            activatedTarget = true;
            indexTarget = 0;
            addTargetTrip(t);
        }

        public void ResetMyTrips()                                                      //Completely resets trip arrays
        {
            if (Tracker.AccessibilitySourceActive())
                sourceLog = new Trip[TrafficLog.logLength];
            if (Tracker.AccessibilityTargetActive())
                targetLog = new Trip[TrafficLog.logLength];
        }
    }
}