//using System.Runtime.CompilerServices;
using ColossalFramework;
using GentrifiedSkylines.Redirection;
using System;
using UnityEngine;

//using ColossalFramework.IO;
//using System.Threading;
//using UnityEngine;

namespace GentrifiedSkylines.Detours

{
    [TargetType(typeof(ImmaterialResourceManager))]
    public class ImmaterialResourceManagerDetour : ImmaterialResourceManager
    {
        public static string getSubstring(string s)
        {
            double d;
            try
            {
                d = Convert.ToDouble(s);
                return s;
            }
            catch (FormatException)
            {
                return getSubstring(s.Substring(1, s.Length - 1));
            }
        }

        [RedirectMethod]
        private static bool CalculateLocalResources(int x, int z, ushort[] buffer, int[] global, ushort[] target, int index)
        {
            //int num10 = ((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate1, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate3, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate2, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate4, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate5, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate6, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate7, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate8, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate12, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate10, 100, 500, 100, 200) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate17, 60, 100, 0, 50) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate16, 60, 100, 0, 50) - ImmaterialResourceManager.CalculateResourceEffect(100 - resourceRate17, 60, 100, 0, 50) - ImmaterialResourceManager.CalculateResourceEffect(100 - resourceRate16, 60, 100, 0, 50) - ImmaterialResourceManager.CalculateResourceEffect(num9, 50, (int)byte.MaxValue, 50, 100) - ImmaterialResourceManager.CalculateResourceEffect(resourceRate9, 10, 100, 0, 100) - ImmaterialResourceManager.CalculateResourceEffect(resourceRate15, 10, 100, 0, 100) - ImmaterialResourceManager.CalculateResourceEffect(resourceRate14, 50, 100, 10, 50) - ImmaterialResourceManager.CalculateResourceEffect(resourceRate11, 15, 50, 100, 200) + (ImmaterialResourceManager.CalculateResourceEffect(resourceRate13, 33, 67, 300, 0) * Mathf.Max(0, 32 - num9) >> 5)) / 10 : 0;

            int resourceRate1 = (int)buffer[index] + global[0];
            int resourceRate2 = (int)buffer[index + 1] + global[1];
            int resourceRate3 = (int)buffer[index + 2] + global[2];
            int resourceRate4 = (int)buffer[index + 3] + global[3];
            int resourceRate5 = (int)buffer[index + 4] + global[4];
            int resourceRate6 = (int)buffer[index + 5] + global[5];
            int resourceRate7 = (int)buffer[index + 6] + global[6];
            int resourceRate8 = (int)buffer[index + 7] + global[7];
            int resourceRate9 = (int)buffer[index + 8] + global[8];
            int num1 = (int)buffer[index + 9] + global[9];
            int num2 = (int)buffer[index + 10] + global[10];
            int num3 = (int)buffer[index + 11] + global[11];
            int num4 = (int)buffer[index + 12] + global[12];
            int resourceRate10 = (int)buffer[index + 13] + global[13];
            int num5 = (int)buffer[index + 14] + global[14];
            int num6 = (int)buffer[index + 15] + global[15];
            int num7 = (int)buffer[index + 16] + global[16];
            int num8 = (int)buffer[index + 17] + global[17];
            int resourceRate11 = (int)buffer[index + 18] + global[18];
            int resourceRate12 = (int)buffer[index + 19] + global[19];
            Rect area = new Rect((float)(((double)x - 128.0 - 1.5) * 38.4000015258789), (float)(((double)z - 128.0 - 1.5) * 38.4000015258789), 153.6f, 153.6f);
            float groundPollution;
            float waterProximity;
            Singleton<NaturalResourceManager>.instance.AveragePollutionAndWater(area, out groundPollution, out waterProximity);
            int num9 = (int)((double)groundPollution * 100.0);
            int resourceRate13 = (int)((double)waterProximity * 100.0);
            if (resourceRate13 > 33 && resourceRate13 < 99)
            {
                area = new Rect((float)(((double)x - 128.0 + 0.25) * 38.4000015258789), (float)(((double)z - 128.0 + 0.25) * 38.4000015258789), 19.2f, 19.2f);
                Singleton<NaturalResourceManager>.instance.AveragePollutionAndWater(area, out groundPollution, out waterProximity);
                resourceRate13 = Mathf.Max(Mathf.Min(resourceRate13, (int)((double)waterProximity * 100.0)), 33);
            }
            int resourceRate14 = num8 * 2 / (resourceRate2 + 50);
            int resourceRate15;
            int resourceRate16;
            int resourceRate17;
            if (num4 == 0)
            {
                resourceRate15 = 0;
                resourceRate16 = 50;
                resourceRate17 = 50;
            }
            else
            {
                resourceRate15 = num1 / num4;
                resourceRate16 = num2 / num4;
                resourceRate17 = num3 / num4;
                num7 += Mathf.Min(num4, 10) * 10;
            }

            //Accessibility Grid Test Chamber
            float[,] grid3;
            //Now it calls every 256^2 steps
            //However, there are unexplained errors, and the ratings are never properly reached
            int num10 = 0;
            try
            {
                if (Tracker.CheckAccessibilityUpdate())
                {
                    grid3 = TrafficLog.CollectRatings();
                }
                else
                {
                    grid3 = TrafficLog.CollectOldRatings();
                }
                float accessValue;
                try
                {
                    //Debug.Log("Attemping Grid Query. X: " + x + " . Z: " + z + ". + Value: " + grid3[x, z] + ".");
                    accessValue = grid3[x, z];
                }
                catch (IndexOutOfRangeException e)
                {
                    accessValue = 0;
                }
                catch (NullReferenceException e)
                {
                    accessValue = 0;
                }
                //Debug.Log("X: " + x + " Z: " + z + "AccessValue: " + accessValue);
                num10 = (int)accessValue;
            }
            catch (NullReferenceException e)
            {
                num10 = 0;
            }

            //num10 Notes
            /*
            HealthCare = 0,                 * Yes
            FireDepartment = 1,             * Yes
            PoliceDepartment = 2,           * Yes
            EducationElementary = 3,        * Yes
            EducationHighSchool = 4,        * Yes
            EducationUniversity = 5,        * Yes
            DeathCare = 6,                  * Yes
            PublicTransport = 7,            * Yes / Imperfect
            NoisePollution = 8,             * Yes
            CrimeRate = 9,                  * Yes
            Health = 10,                    * Yes
            Wellbeing = 11,                 * Ambiguous
            Density = 12,                   * Ambiguous
            Entertainment = 13,             * Yes
            LandValue = 14,                 * Ambiguous
            Attractiveness = 15,            * Global
            Coverage = 16,                  * Ambiguous
            FireHazard = 17,                * Useless
            Abandonment = 18,               * X
            CargoTransport = 19,            * Yes

            Ground Pollution                * Yes
            Water Proximity                 * Yes
            */

            ////////////////////num10 Test Chamber///////////////

            //Water Proximity - Needs scaling
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(Math.Sqrt(waterProximity))*20) : 0);

            //Ground Pollution - Needs scaling
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(groundPollution) * 10) : 0);

            //Wellbeing - Needs scaling
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(num3) * 2) : 0);

            //Density
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(num4) * 10) : 0);

            //Land Value???
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(num5) * 1) : 0);

            //Attractiveness - Either broken or a totally global variable that provides city-wide bonuses?
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(buffer[index + 15]) * 1) : 0);

            //Coverage - Unclear but it seems to matter when building become inhabited
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(num7) * 1) : 0);

            //Fire Hazard - Even water towers set this off.
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(num8) * 1) : 0);

            //Abandonment
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (Convert.ToInt32(resourceRate11) * 1) : 0);

            //////////////////////////Num10/////////////////////////////
            //Original

            //int num10 = ((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate1, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate3, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate2, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate4, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate5, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate6, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate7, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate8, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate12, 100, 500, 50, 100) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate10, 100, 500, 100, 200) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate17, 60, 100, 0, 50) + ImmaterialResourceManager.CalculateResourceEffect(resourceRate16, 60, 100, 0, 50) - ImmaterialResourceManager.CalculateResourceEffect(100 - resourceRate17, 60, 100, 0, 50) - ImmaterialResourceManager.CalculateResourceEffect(100 - resourceRate16, 60, 100, 0, 50) - ImmaterialResourceManager.CalculateResourceEffect(num9, 50, (int)byte.MaxValue, 50, 100) - ImmaterialResourceManager.CalculateResourceEffect(resourceRate9, 10, 100, 0, 100) - ImmaterialResourceManager.CalculateResourceEffect(resourceRate15, 10, 100, 0, 100) - ImmaterialResourceManager.CalculateResourceEffect(resourceRate14, 50, 100, 10, 50) - ImmaterialResourceManager.CalculateResourceEffect(resourceRate11, 15, 50, 100, 200) + (ImmaterialResourceManager.CalculateResourceEffect(resourceRate13, 33, 67, 300, 0) * Mathf.Max(0, 32 - num9) >> 5)) / 10 : 0;

            //Add arbirary value

            if (((x % 2 == 1) & (z % 2 == 1)))
            {
                Vector3 m_vector3 = new Vector3((Convert.ToInt32(x * 38.4000015258789)) - 4915, 0, (Convert.ToInt32(z * 38.4000015258789) - 4915));
                DistrictManager DM = Singleton<DistrictManager>.instance;
                byte m_byte = DM.GetDistrict(m_vector3);
                string m_name = DM.GetDistrictName(m_byte);         //Full Name

                //Solve Single Operator Crash
                if ((m_name.Length == 1) & ((m_name.EndsWith("+") | m_name.EndsWith("-")) | (m_name.EndsWith("*") | m_name.EndsWith("/"))))
                {
                    ChirpPanel chirper = Singleton<ChirpPanel>.instance;
                    chirper.AddMessage(new ChirpMessage("System", "Invalid Syntax: Cannot Pass Single Operator Token"));
                    DM.StartCoroutine(DM.SetDistrictName(m_byte, null));
                }
                else
                {
                    string m_lastvalidname;
                    m_lastvalidname = m_name;
                    string m_realname = m_name;                         //Real Name
                    string m_operator = Tracker.GetOperator(m_byte);    //Operator
                    string m_deltaS = "";                               //Value String
                    double m_deltaD = 0;                                //Value Number
                    string m_currentOperator = Tracker.GetOperator(m_byte);
                    double m_CurrentValue = Convert.ToDouble(Tracker.GetLandValue(m_byte)); //Set current operator
                    num10 = Convert.ToInt32(m_CurrentValue);
                    if ((m_name.EndsWith("+") | m_name.EndsWith("-")) | (m_name.EndsWith("*") | m_name.EndsWith("/")))
                    {
                        num10 = 0;
                        Tracker.SetOperator(m_byte, m_name.Substring(m_name.Length - 1));       //Set new operator
                        m_operator = Tracker.GetOperator(m_byte);                    // Get Operator
                        try
                        {
                            m_deltaS = getSubstring(m_name.Substring(0, m_name.Length - 1));    //Get the Value
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            m_deltaS = "0";
                        }
                        try
                        {
                            m_deltaD = Convert.ToDouble(m_deltaS);
                        }
                        catch (FormatException e2)
                        {
                            m_deltaD = 0;
                        }

                        if (!((m_name.Length - (m_deltaS.Length + 1)) < 0))
                        {
                            m_realname = m_name.Substring(0, (m_name.Length - (m_deltaS.Length + 1)));  //Get name without value and operator
                        }
                        while (m_realname.EndsWith(" "))
                        {
                            m_realname = m_realname.Substring(0, m_realname.Length - 1);   //Remove extra spaces from end
                        }
                        while (m_realname.StartsWith(" "))
                        {
                            m_realname = m_realname.Substring(1, m_realname.Length);    //Remove extra spaces from start
                        }
                        while (((m_realname.EndsWith("+") | m_realname.EndsWith("-")) | (m_realname.EndsWith("*") | m_realname.EndsWith("/"))))
                        {
                            if (m_realname.Length > 1)
                            {
                                m_realname = m_realname.Substring(0, m_realname.Length - 1);   //Remove extra operators
                            }
                        }
                        if (!((m_realname.EndsWith("+") | m_realname.EndsWith("-")) | (m_realname.EndsWith("*") | m_realname.EndsWith("/"))))
                        {
                            DM.StartCoroutine(DM.SetDistrictName(m_byte, m_realname));      // Rename District
                        }
                        else
                        {
                            if (m_realname.IsNullOrWhiteSpace())
                            {
                                DM.StartCoroutine(DM.SetDistrictName(m_byte, m_lastvalidname));
                            }
                            else //Not null but ends in an operator
                            {
                                DM.StartCoroutine(DM.SetDistrictName(m_byte, m_lastvalidname));
                            }
                        }
                        Tracker.SetLandValue(m_byte, m_deltaS);                         //Assign number to Array
                    }
                    else
                    {
                        m_operator = " ";
                    }
                    m_currentOperator = Tracker.GetOperator(m_byte);        //Reassign operator

                    string landModifier = Tracker.GetLandValue(m_byte);

                    if (m_operator.EndsWith("+"))
                    {
                        num10 = Mathf.Clamp(Convert.ToInt32(num10 + Convert.ToDouble(landModifier)), 0, ushort.MaxValue);       //  + current or new value to num10
                    }
                    else if (m_operator.EndsWith("-"))
                    {
                        num10 = Mathf.Clamp(Convert.ToInt32(num10 - Convert.ToDouble(landModifier)), 0, ushort.MaxValue);       //  - current or new value to num10
                    }
                    else if (m_operator.EndsWith("*"))
                    {
                        num10 = Mathf.Clamp(Convert.ToInt32(num10 * Convert.ToDouble(landModifier)), 0, ushort.MaxValue);       //  * current or new value to num10
                    }
                    else if (m_operator.EndsWith("/"))
                    {
                        try
                        {
                            num10 = Mathf.Clamp(Convert.ToInt32(num10 / Convert.ToDouble(landModifier)), 0, ushort.MaxValue);   //  / current or new value to num10
                        }
                        catch (DivideByZeroException e4) { }
                    }
                    Tracker.SetLandValue(m_byte, Convert.ToString(num10));
                }
            }

            //ResourceRate1 = Medicine
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate1, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate2 = Fire Protection
            //num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate2, 1, 1000, 1, 3000) >> 5) / 1 : 0);

            //ResourceRate3 = Police Protection
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate3, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate4 = Elementary
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate4, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate5 = High School
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate5, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate6 = College
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate6, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate7 = Deathcare
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate7, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate8 = Public Transport
            //num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate8, 1, 1000, 1, 3000) >> 5) / 1 : 0);

            //ResourceRate9 = NoisePollition
            //num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate9, 1, 2, 1, 1000) >> 5) / 1 : 0);

            //ResourceRate10 = Entertainment
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate10, 1, 500, 1, 3000) >> 5) / 10 : 0);

            //ResourceRate11 = -? (Abandonment (R))
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate11, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate12 = Cargo Transport
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate12, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate13 = Proximity to Water and Pollution
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate13, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate14 = FireHazard over Fire Protection? (R) - Water and shoreline confirmed add value.
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate14, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate15 = If Density=0 then 0, otherwise its Crime Rate divided by density? (R)
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate15, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate16 = Starts at a positive value, reduced by public buildings. If Density = 0 then 50, otherwise its health divided by density? (R)
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate16, 1, 2, 1, 1000) >> 5) / 10 : 0);

            //ResourceRate17 = If Density= 0 then 50, otherwise Wellbeing deivided by density? (R)
            //int num10 = (((Singleton<GameAreaManager>.instance.PointOutOfArea(VectorUtils.X_Y(area.center)) ? 1 : 0) | (x <= 1 || x >= 254 || z <= 1 ? 1 : (z >= 254 ? 1 : 0))) == 0 ? (num5 + ImmaterialResourceManager.CalculateResourceEffect(resourceRate17, 1, 2, 1, 1000) >> 5) / 10 : 0);
            /* 1. Yes
             * 2. Yes
             * 3. Yes
             * 4. Yes
             * 5. Yes
             * 6. Yes
             * 7. Yes
             * 8. Yes but imperfect
             * 9. Yes
             * 10. Yes
             * 11. Unproven
             * 12. Yes
             * 13. Yes
             * 14. Inconsistent
             * 15. Yes but uncertain
             * 16. Yes but weird
             * 17. Various buildings have effect, weird
             */
            //End

            int num11 = Mathf.Clamp(resourceRate1, 0, (int)ushort.MaxValue);
            int num12 = Mathf.Clamp(resourceRate2, 0, (int)ushort.MaxValue);
            int num13 = Mathf.Clamp(resourceRate3, 0, (int)ushort.MaxValue);
            int num14 = Mathf.Clamp(resourceRate4, 0, (int)ushort.MaxValue);
            int num15 = Mathf.Clamp(resourceRate5, 0, (int)ushort.MaxValue);
            int num16 = Mathf.Clamp(resourceRate6, 0, (int)ushort.MaxValue);
            int num17 = Mathf.Clamp(resourceRate7, 0, (int)ushort.MaxValue);
            int num18 = Mathf.Clamp(resourceRate8, 0, (int)ushort.MaxValue);
            int num19 = Mathf.Clamp(resourceRate9, 0, (int)ushort.MaxValue);
            int num20 = Mathf.Clamp(resourceRate15, 0, (int)ushort.MaxValue);
            int num21 = Mathf.Clamp(resourceRate16, 0, (int)ushort.MaxValue);
            int num22 = Mathf.Clamp(resourceRate17, 0, (int)ushort.MaxValue);
            int num23 = Mathf.Clamp(num4, 0, (int)ushort.MaxValue);
            int num24 = Mathf.Clamp(resourceRate10, 0, (int)ushort.MaxValue);

            /////////////////////////////////Clamp Land Value//////////////////////////////
            //Landvalue comes from num10
            //Original
            //int landvalue = Mathf.Clamp(num10, 0, (int)ushort.MaxValue);
            //Test
            int landvalue = Mathf.Clamp(num10, 0, (int)ushort.MaxValue);
            int num25 = Mathf.Clamp(num6, 0, (int)ushort.MaxValue);
            int coverage = Mathf.Clamp(num7, 0, (int)ushort.MaxValue);
            int num26 = Mathf.Clamp(resourceRate14, 0, (int)ushort.MaxValue);
            int num27 = Mathf.Clamp(resourceRate11, 0, (int)ushort.MaxValue);
            int num28 = Mathf.Clamp(resourceRate12, 0, (int)ushort.MaxValue);
            DistrictManager instance = Singleton<DistrictManager>.instance;
            byte district = instance.GetDistrict(x * 2, z * 2);

            /////////////////////////////////Apply to District//////////////////////////////
            //Districts have landvalue, pollution and coverage. num9 refers to ground pollution, coverage
            //has something to do with num7. The method assigns templandvalue, temppollution and tempcoverage.
            //The first two are the inputs * coverage, tempcoverage is simply coverage.
            instance.m_districts.m_buffer[(int)district].AddGroundData(landvalue, num9, coverage);

            bool flag = false;
            if (num11 != (int)target[index])
            {
                target[index] = (ushort)num11;
                flag = true;
            }
            if (num12 != (int)target[index + 1])
            {
                target[index + 1] = (ushort)num12;
                flag = true;
            }
            if (num13 != (int)target[index + 2])
            {
                target[index + 2] = (ushort)num13;
                flag = true;
            }
            if (num14 != (int)target[index + 3])
            {
                target[index + 3] = (ushort)num14;
                flag = true;
            }
            if (num15 != (int)target[index + 4])
            {
                target[index + 4] = (ushort)num15;
                flag = true;
            }
            if (num16 != (int)target[index + 5])
            {
                target[index + 5] = (ushort)num16;
                flag = true;
            }
            if (num17 != (int)target[index + 6])
            {
                target[index + 6] = (ushort)num17;
                flag = true;
            }
            if (num18 != (int)target[index + 7])
            {
                target[index + 7] = (ushort)num18;
                flag = true;
            }
            if (num19 != (int)target[index + 8])
            {
                target[index + 8] = (ushort)num19;
                flag = true;
            }
            if (num20 != (int)target[index + 9])
            {
                target[index + 9] = (ushort)num20;
                flag = true;
            }
            if (num21 != (int)target[index + 10])
            {
                target[index + 10] = (ushort)num21;
                flag = true;
            }
            if (num22 != (int)target[index + 11])
            {
                target[index + 11] = (ushort)num22;
                flag = true;
            }
            if (num23 != (int)target[index + 12])
            {
                target[index + 12] = (ushort)num23;
                flag = true;
            }
            if (num24 != (int)target[index + 13])
            {
                target[index + 13] = (ushort)num24;
                flag = true;
            }

            ///////////////////////////////Flag///////////////////////
            //What this means is unceertain, flag is a bool returned. If a condition is met
            //then it is true, otherwise false. This probably does not matter.
            if (landvalue != (int)target[index + 14])
            {
                target[index + 14] = (ushort)landvalue;
                flag = true;
            }
            if (num25 != (int)target[index + 15])
            {
                target[index + 15] = (ushort)num25;
                flag = true;
            }
            if (coverage != (int)target[index + 16])
            {
                target[index + 16] = (ushort)coverage;
                flag = true;
            }
            if (num26 != (int)target[index + 17])
            {
                target[index + 17] = (ushort)num26;
                flag = true;
            }
            if (num27 != (int)target[index + 18])
            {
                target[index + 18] = (ushort)num27;
                flag = true;
            }
            if (num28 != (int)target[index + 19])
            {
                target[index + 19] = (ushort)num28;
                flag = true;
            }
            return flag;
        }
    }
}