using System;
using UnityEngine;
using ColossalFramework;

namespace GentrifiedSkylines.Detours
{
    public class Leg
    {
        private Vector3 initialVector;                                      //Vector3 of initial position
        private Vector3 finalVector;                                        //Vector3 of final position
        private Leg.Flags my_mode;                                          //Attribute of mode type
        private bool finalized = false;                                     //Indicates if the leg has been finalized
        private long duration = 0;                                               //Counter of time elapsed
        private long pause = 0;                                              //Counter of pause time
        private float speedSum = 0;                                         //Sum of speed recordings
        private float speedCount = 0;                                       //Counter of speed recordings
        private long timeBikeLane = 0;                                       //Counter of time on bike lane
        private DateTime m_startTime;
        private DateTime m_endTime;

        public Leg(Leg.Flags mode, Vector3 init)                            //Default Constructor
        {
            m_startTime = Singleton<SimulationManager>.instance.FrameToTime(Singleton<SimulationManager>.instance.m_currentFrameIndex);
            my_mode = mode;
            initialVector = init;
        }
        public long getStartingTicks()
        {
            return m_startTime.Ticks;
        }
        public void addPause(long t)                                         //Add a 
        {
            if (!finalized)
                pause += t;
        }
        public void addSpeed(float s)
        {
            if (!finalized)
            {
                speedSum = Mathf.Clamp(speedSum + s, 0, float.MaxValue);
                speedCount++;
            }
        }
        public void addTimeOnBikeLane(long t)
        {
            if (!finalized)
                timeBikeLane += t;
        }
        public float GetDistanceTraveled(Vector3 vec3)
        {
            double sx;
            double sz;
            if (finalized)
            {
                sx = finalVector.x - initialVector.x;
                sz = finalVector.z - initialVector.z;
            }
            else
            {
                sx = vec3.x - initialVector.x;
                sz = vec3.z - initialVector.z;
            }
            double sv = Math.Sqrt((sx * sx) + (sz * sz));
            return (float)sv;

        }
        public void finalize(Vector3 final)
        {
            finalVector = final;
            finalized = true;
            duration = Singleton<SimulationManager>.instance.FrameToTime(Singleton<SimulationManager>.instance.m_currentFrameIndex).Ticks - m_startTime.Ticks;
        }
        public float getBikeLaneRatio()
        {
            long t = getTime();
            if (t != 0)
                return (timeBikeLane / t);
            return 0;
        }
        public Leg.Flags getMode()
        {
            return my_mode;
        }
        public long getPause()
        {
            return pause;
        }
        public float getRawCost()
        {
            if (!finalized)
                return 0;
            else
            {
                switch (getMode())
                {
                    case Leg.Flags.None:
                        return 0;

                    case Leg.Flags.Bicycle:
                        return 1;

                    case Leg.Flags.Bus:
                        return 1;

                    case Leg.Flags.Car:
                        return 1;

                    case Leg.Flags.Metro:
                        return 1;

                    case Leg.Flags.Plane:
                        return 1;

                    case Leg.Flags.Ship:
                        return 1;

                    case Leg.Flags.Taxi:
                        return 1;

                    case Leg.Flags.Train:
                        return 1;

                    case Leg.Flags.Tram:
                        return 1;

                    case Leg.Flags.Walk:
                        return 1;
                }
                return 0;
            }
        }
        public float getSpeed()
        {
            if (speedCount > 0)
                return speedSum / speedCount;
            return 0;
        }
        public long getTime()
        {
            if (finalized)
                return duration;
            return (Singleton<SimulationManager>.instance.FrameToTime(Singleton<SimulationManager>.instance.m_currentFrameIndex)).Ticks - m_startTime.Ticks;
        }
        public long getTimeOnBikeLane()
        {
            return timeBikeLane;
        }
        public bool setMode(Leg.Flags f)
        {
            if ((f != Leg.Flags.None) & (my_mode == Leg.Flags.None))
            {
                my_mode &= f;
                return true;
            }
            return false;
        }
        [System.Flags]
        public enum Flags
        {
            None = 0,
            Car = 1,
            Metro = 2,
            Train = 4,
            Ship = 8,
            Plane = 16,
            Bicycle = 32,
            Tram = 64,
            Walk = 128,
            Bus = 256,
            Taxi = 512,
            Bonus = 1024,
            All = 2047
        }
    }
}