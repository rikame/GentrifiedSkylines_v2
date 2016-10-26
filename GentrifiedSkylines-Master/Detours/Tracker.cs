using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    public static class Tracker
    {
        public static Int64[] DistrictsArray;
        public static bool? flag;
        public static bool? flagLandValue;
        public static String[] LandValue;
        public static String[] Operator;
        private static bool accessibility_active = true;
        private static bool accessibility_source = false;
        private static bool accessibility_target = true;
        private static ushort accessibilityGridTracker = 0;
        private static bool squared = false;
        static Tracker()
        {
            Activate();
            serialize(new Int64[255]);
            serializeLandValue(new String[255]);
        }
        public static bool AccessibilitySourceActive()
        {
            return accessibility_source;
        }
        public static bool AccessibilityTargetActive()
        {
            if (accessibility_target | !accessibility_source)
                return true;
            return false;
        }
        public static void Activate()
        {
            if (!flag.HasValue)
            {
                flag = true;
                serialize(new Int64[255]);
                serializeLandValue(new String[255]);
                serializeOperator(new String[255]);
            }
        }
        public static bool CheckAccessibilityUpdate()
        {
            if (accessibilityGridTracker == 0) //Three-step check assures that the TrafficLog return non-null value
            {
                accessibilityGridTracker++;
                return true;
            }
            else if (accessibilityGridTracker == 1)
            {
                accessibilityGridTracker = 0;
                return false;
            }
            else
            {
                accessibilityGridTracker++;
                return false;
            }
        }
        public static bool GetAccessibilityActive()
        {
            return accessibility_active;
        }
        public static Int64 GetDistricts(byte district)
        {
            district = Convert.ToByte(Mathf.Clamp(district, 0, 255));
            if (flag.HasValue)
            {
                return DistrictsArray[district];
            }
            else
            {
                Activate();
                return GetDistricts(district);
            }
        }
        public static string GetLandValue(byte district)
        {
            district = Convert.ToByte(Mathf.Clamp(district, 0, 255));
            if (flag.HasValue)
            {
                return LandValue[district];
            }
            else
            {
                Activate();
                return GetLandValue(district);
            }
        }
        public static string GetOperator(byte district)
        {
            district = Convert.ToByte(Mathf.Clamp(district, 0, 255));
            if (flag.HasValue)
            {
                return Operator[district];
            }
            else
            {
                Activate();
                return GetOperator(district);
            }
        }
        public static bool GetSquared()
        {
            return squared;
        }
        public static void LoadDistricts(byte l, Int64 v)
        {
            if (flag.HasValue)
            {
                l = Convert.ToByte(Mathf.Clamp(l, 0, 255));
                DistrictsArray[l] += v;
            }
            else
            {
                Activate();
                LoadDistricts(l, v);
            }
        }
        public static void serialize(Int64[] t)
        {
            for (byte i = 0; (i < 255); i++)
            {
                t[i] = 0;
                DistrictsArray = t;
            }
        }
        public static void serializeLandValue(String[] s)
        {
            for (byte i = 0; (i < 255); i++)
            {
                s[i] = "0";
                LandValue = s;
            }
        }
        public static void serializeOperator(String[] s)
        {
            for (byte i = 0; (i < 255); i++)
            {
                s[i] = "0";
                Operator = s;
            }
        }
        public static void SetDistricts(byte l, Int64 v)
        {
            if (flag.HasValue)
            {
                l = Convert.ToByte(Mathf.Clamp(l, 0, 255));
                DistrictsArray[l] = v;
            }
            else
            {
                Activate();
                SetDistricts(l, v);
            }
        }
        public static void SetLandValue(byte l, string str)
        {
            if (flag.HasValue)
            {
                l = Convert.ToByte(Mathf.Clamp(l, 0, 255));
                try
                {
                    double temp = Convert.ToDouble(str);
                    LandValue[l] = str;
                }
                catch (System.FormatException e)
                {
                    SetLandValue(l, str.Substring(1, str.Length - 1));
                }
                catch (IndexOutOfRangeException e2)
                {
                    LandValue[l] = "0";
                }
            }
            else
            {
                Activate();
                SetLandValue(l, str);
            }
        }
        public static void SetOperator(byte l, string str)
        {
            if (flag.HasValue)
            {
                l = Convert.ToByte(Mathf.Clamp(l, 0, 255));
                try
                {
                    Operator[l] = str;
                }
                catch (System.FormatException e)
                {
                    SetOperator(l, str.Substring(1, str.Length - 1));
                }
                catch (IndexOutOfRangeException e2)
                {
                    Operator[l] = "X";
                }
            }
            else
            {
                Activate();
                SetOperator(l, str);
            }
        }
    }
}