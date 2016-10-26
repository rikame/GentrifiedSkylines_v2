using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    public class Leg
    {
        private float deltaX = 0;
        private float deltaZ = 0;

        //CitizenInstance.Flags
        private bool finalized = false;

        private Vector3 finalVector;
        private Vector3 initialVector;
        private Leg.Flags my_mode;
        private int pause = 0;
        private float speed = 0;
        private float speedCount = 0;
        private float speedSum = 0;
        private int time = 0;
        private int timeBikeLane = 0;
        private float x1 = 0;
        private float x2 = 0;
        private float z1 = 0;
        private float z2 = 0;
        public Leg(Leg.Flags mode, Vector3 init)
        {
            my_mode = mode;
            initialVector = init;
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
        public void addPause(int t)
        {
            if (!finalized)
                pause = Mathf.Clamp(pause + t, 0, int.MaxValue);
        }
        //Speed of Citizen
        public void addSpeed(float s)
        {
            if (!finalized)
            {
                speedSum = Mathf.Max(speedSum + s, float.MaxValue);
                speedCount += 1;
            }
        }
        public void addTime(int t)
        {
            if (!finalized)
                time = Mathf.Clamp(time + t, 0, int.MaxValue);
        }
        public void addTimeOnBikeLane(int t)
        {
            if (!finalized)
                timeBikeLane = Mathf.Clamp(timeBikeLane, 0, int.MaxValue);
        }
        public void addVehicalSpeed(float s)
        {
            if (!finalized)
            {
                speedSum = Mathf.Max(speedSum + s, float.MaxValue);
                speedCount += 1;
            }
        }
        public void finalize(Vector3 final)
        {
            finalVector = final;
            finalized = true;
        }
        public float getBikeLaneRatio()
        {
            if (time != 0)
                return (timeBikeLane / time);
            return 0;
        }
        public float? getDeltaX()
        {
            if (finalized)
                return finalVector.x - initialVector.x;
            else
                return null;
        }
        public float? getDeltaZ()
        {
            if (finalized)
                return finalVector.z - initialVector.z;
            else
                return null;
        }
        public Leg.Flags getMode()
        {
            return my_mode;
        }
        public float getPause()
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
        public float getTime()
        {
            return time;
        }
        public int getTimeOnBikeLane()
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
    }
}