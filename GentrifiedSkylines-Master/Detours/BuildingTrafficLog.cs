using ColossalFramework;
using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    public class BuildingTrafficLog
    {
        public Trip[] sourceLog = new Trip[byte.MaxValue + 1];                      //Trip log of sources for the respective building
        public Trip[] targetLog = new Trip[byte.MaxValue + 1];                      //Trip log of targets for the respective building
        public uint[] TripCitizenJoinsSource = new uint[byte.MaxValue + 1];         //Array for sources linking logs to citizens to write to the correct trips
        public uint[] TripCitizenJoinsTarget = new uint[byte.MaxValue + 1];         //Array for targets linking logs to citizens to write to the correct trips
        private bool? activatedS;                                                   //Gains value when the source log gets written-to to avoid invalid calls
        private bool? activatedT;                                                   //Gains value when the target log gets written-to to avoid invalid calls
        private bool filledS = false;                                               //Becomes true when a source is added to avoid invalid calls
        private bool filledT = false;                                               //Becomes true when a target is added to avoid invalid calls
        private byte indexSource;                                                   //Index tracking the current source array position primed to be written
        private byte indexTarget;                                                   //Index tracking the current target array position primed to be written
        private ushort m_building;                                                  //BuildingID associated with this building log
        private bool used = false;                                                  //Becomes true on creation to indicate retrievable values

        public void addSourceTrip(Trip t)
        {
            filledS = true;
            if (!activatedS.HasValue)
            {
                SelfActivateS(t);
            }
            else
            {
                sourceLog[indexSource] = t;
                if (indexSource <= byte.MaxValue)
                {
                    indexSource += 1;
                }
                else
                {
                    indexSource = 0;
                }
            }
        }
        public void addTargetTrip(Trip t)
        {
            filledT = true;
            if (!activatedT.HasValue)
            {
                SelfActivateT(t);
            }
            else
            {
                targetLog[indexSource] = t;
                if (indexTarget <= byte.MaxValue)
                {
                    indexTarget += 1;
                }
                else
                {
                    indexTarget = 0;
                }
            }
        }
        public Trip[] GetAllTrips()
        {
            Trip[] temp = new Trip[byte.MaxValue * 2 + 2];
            int i;
            for (i = 0; i <= byte.MaxValue; i++)
            {
                temp[i] = sourceLog[i];
            }
            for (i++; i <= (byte.MaxValue * 2) + 1; i++)
            {
                temp[i] = targetLog[i];
            }
            return temp;
        }
        public Building GetBuilding()
        {
            return Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_building];
        }
        public ushort GetBuildingID()
        {
            return m_building;
        }
        public float GetRating(bool source, bool target, bool squared)
        {
            if (used == true)
            {
                /*
                int count = 0;
                int cost = 0;
                int distance = 0;
                int distanceAverage = 0;
                int legs = 0;
                */
                float accumulated = 0;
                float accumulated2 = 0;
                byte count = 0;
                if (source & filledS)
                {
                    for (int i = 0; i <= indexSource; i++)
                    {
                        accumulated += sourceLog[Convert.ToByte(i)].GetContribution();
                        accumulated2 = accumulated;
                        accumulated2 *= accumulated2;
                        count++;
                    }
                }
                if (target & filledT)
                {
                    for (int i = 0; i <= indexTarget; i++)
                    {
                        accumulated += targetLog[Convert.ToByte(i)].GetContribution();
                        accumulated2 = accumulated;
                        accumulated2 *= accumulated2;
                        count++;
                    }
                }
                if (accumulated > 0)
                {
                    if (squared)
                        return (float)Math.Sqrt((double)accumulated2);
                    return accumulated;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public Trip GetSourceTrip(byte b)
        {
            return sourceLog[Mathf.Clamp(b, 0, 255)];
        }
        public Trip[] GetSourceTrips()
        {
            Trip[] temp = new Trip[byte.MaxValue + 1];
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                temp[i] = sourceLog[i];
            }
            return temp;
        }
        public Trip GetTargetTrip(byte b)
        {
            return targetLog[Mathf.Clamp(b, 0, 255)];
        }
        public Trip[] GetTargetTrips()
        {
            Trip[] temp = new Trip[byte.MaxValue + 1];
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                temp[i] = targetLog[i];
            }
            return temp;
        }
        public byte LookupTripCitizenJoinSource(uint i)
        {
            int i2;
            i2 = Array.IndexOf(TripCitizenJoinsSource, i);
            if (i2 == -1)
            {
                byte b = indexSource;
                addSourceTrip(CreateTrip(i));
                return b;
            }
            else
            {
                return (byte)i2;
            }
        }
        public byte LookupTripCitizenJoinTarget(uint i)
        {
            int i2;
            i2 = Array.IndexOf(TripCitizenJoinsSource, i);
            if (i2 == -1)
            {
                byte b = indexTarget;
                addTargetTrip(CreateTrip(i));
                return b;
            }
            else
            {
                return (byte)i2;
            }
        }
        public void NewBuildingLog(ushort ID)
        {
            m_building = ID;
            used = true;
        }
        public float SumTripRatings(bool source, bool target)   //Generates a byte[,] grid by caling on finalized trips to retrieve values
        {
            float tripSum = 0;
            float tripCount = 0;
            if (source)
            {
                for (int i = 0; i < indexSource; i++)                               //Set both grids to 0 throughout
                {
                    tripSum += sourceLog[i].GetContribution();
                    tripCount += 1;
                }
            }
            if (target)
            {
                for (int i = 0; i < indexTarget; i++)                               //Set both grids to 0 throughout
                {
                    tripSum += targetLog[i].GetContribution();
                    tripCount += 1;
                }
            }
            if (tripCount == 0)
                return 0;
            return ((float)(tripSum / tripCount));
        }
        private Trip CreateTrip(uint citizenID)
        {
            return (new Trip(citizenID, Singleton<CitizenManager>.instance.m_instances.m_buffer[citizenID].m_sourceBuilding, Singleton<CitizenManager>.instance.m_instances.m_buffer[citizenID].m_sourceBuilding));
        }
        private void SelfActivateS(Trip t)
        {
            activatedS = true;
            indexSource = 0;
            addSourceTrip(t);
        }

        private void SelfActivateT(Trip t)
        {
            activatedT = true;
            indexTarget = 0;
            addTargetTrip(t);
        }
    }
}