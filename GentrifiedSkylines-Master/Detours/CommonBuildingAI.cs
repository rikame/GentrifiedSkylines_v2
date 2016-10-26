//using System.Runtime.CompilerServices;
using ColossalFramework;
using GentrifiedSkylines.Redirection;
using UnityEngine;

//using ColossalFramework.IO;
//using System.Threading;
//using UnityEngine;

namespace GentrifiedSkylines.Detours

{
    [TargetType(typeof(CommonBuildingAI))]
    //Not sure about second field
    public class CommonBuildingAIDetour : CommonBuildingAI
    {
        private bool m_logged = false;
        [RedirectMethod]
        //Static or Override?

        ///////////////////////////////////////////////////////////

        public override void SimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            if (Tracker.GetAccessibilityActive() && !m_logged)                                                  //Simply check to see if a log is needed, usage is activated in AI classes
            {
                TrafficLog.TryAddBuildingLog(buildingID);
                m_logged = true;
            }
            //base.SimulationStep(buildingID, ref buildingData, ref frameData);

            DistrictManager instanceG = Singleton<DistrictManager>.instance;
            //DistrictManagerDetour instanceG = Singleton<DistrictManagerDetour>.instance;

            //Create a new policy or modify an existing policy
            DistrictPolicies.Policies policyG = DistrictPolicies.Policies.Recycling;
            //tra tra = new tra();
            //Tracker.Activate();
            //tra.serialize(new byte[255]);
            if (instanceG.IsDistrictPolicySet(policyG, instanceG.GetDistrict(buildingData.m_position)))
            {
                //This functiion does implement a district effect. However, Abandonment as a resource
                //does not induce abandonment itself.
                //Singleton<ImmaterialResourceManager>.instance.AddResource(ImmaterialResourceManager.Resource.NoisePollution, 1000000, buildingData.m_position, 400);
                //base.EmptyBuilding(buildingID, ref buildingData);
                //base.ManualDeactivation(buildingID, ref buildingData);
                Vector3 m_vector3 = buildingData.m_position;

                //string m_districtname = instanceG.GetDistrictName(instanceG.GetDistrict(m_vector3), true);
                string m_districtname = instanceG.GetDistrictName(instanceG.GetDistrict(m_vector3));

                byte m_districtbyte = instanceG.GetDistrict(m_vector3);
                Debug.Log("CommonBuildingAIDetour: " + m_districtname + " X: " + m_vector3.x + " Y: " + m_vector3.y + " Z: " + m_vector3.z);

                //This function adds gentrification by district. The concept works but
                //it is implemented in the wrong place and by an arbitrary cause
                //that requires a temporary slowdown via RNG to avoid overburdening
                //the chirper ChirperPanel instance.

                //TRANSFER LATER, GENTRIFICATION TRACKING

                /*
                System.Random rng = new System.Random();
                int r = rng.Next(0, 10);
                if (r==5)
                {
                    Tracker.Load(m_districtbyte, 1);
                    //Tracker.Load(m_districtbyte, 1);
                    int m_gentrifiedint = Convert.ToInt32(Tracker.Get(m_districtbyte));
                    string m_gentrifiedstring = Convert.ToString(m_gentrifiedint);
                    ChirpPanel chirper = Singleton<ChirpPanel>.instance;
                    if ((m_gentrifiedint % 100) == 0)
                    {
                        //chirper.AddMessage(new ChirpMessage(m_districtname + " Gentrification Status: ", m_gentrifiedstring + " households have been gentrified"));
                    }
                    //Debug.Log(m_districtname + " Gentrification Status: "+ m_gentrifiedstring + " (" + Convert.ToString(m_districtbyte) + ") " + "households have been gentrified");
                }
                */
            }

            if ((buildingData.m_flags & Building.Flags.Abandoned) != Building.Flags.None)
            {
                GuideController guideController = Singleton<GuideManager>.instance.m_properties;
                if (guideController != null)
                {
                    Singleton<BuildingManager>.instance.m_buildingAbandoned1.Activate(guideController.m_buildingAbandoned1, buildingID);
                    Singleton<BuildingManager>.instance.m_buildingAbandoned2.Activate(guideController.m_buildingAbandoned2, buildingID);
                }
                if ((int)buildingData.m_majorProblemTimer < (int)byte.MaxValue)
                    ++buildingData.m_majorProblemTimer;
                float radius = (float)(buildingData.Width + buildingData.Length) * 2.5f;
                Singleton<ImmaterialResourceManager>.instance.AddResource(ImmaterialResourceManager.Resource.Abandonment, 10, buildingData.m_position, radius);
            }
            else if ((buildingData.m_flags & Building.Flags.BurnedDown) != Building.Flags.None)
            {
                GuideController guideController = Singleton<GuideManager>.instance.m_properties;
                if (guideController != null)
                    Singleton<BuildingManager>.instance.m_buildingBurned.Activate(guideController.m_buildingBurned, buildingID);
                float radius = (float)(buildingData.Width + buildingData.Length) * 2.5f;
                Singleton<ImmaterialResourceManager>.instance.AddResource(ImmaterialResourceManager.Resource.Abandonment, 10, buildingData.m_position, radius);
            }
            else if ((buildingData.m_flags & Building.Flags.Completed) == Building.Flags.None)
            {
                bool flag = (buildingData.m_flags & Building.Flags.Upgrading) != Building.Flags.None;
                int constructionTime = this.GetConstructionTime();
                frameData.m_constructState = constructionTime != 0 ? (byte)Mathf.Min((int)byte.MaxValue, (int)frameData.m_constructState + 1088 / constructionTime) : byte.MaxValue;
                if ((int)frameData.m_constructState == (int)byte.MaxValue)
                {
                    this.BuildingCompleted(buildingID, ref buildingData);
                    if (Singleton<GuideManager>.instance.m_properties != null)
                        Singleton<BuildingManager>.instance.m_buildingLevelUp.Deactivate(buildingID, true);
                }
                else if (flag)
                {
                    GuideController guideController = Singleton<GuideManager>.instance.m_properties;
                    if (guideController != null)
                        Singleton<BuildingManager>.instance.m_buildingLevelUp.Activate(guideController.m_buildingLevelUp, buildingID);
                }
                if (!flag)
                    return;
                this.SimulationStepActive(buildingID, ref buildingData, ref frameData);
            }
            else
                this.SimulationStepActive(buildingID, ref buildingData, ref frameData);
        }

        ///////////////////////////////////////////////////////////
        //Redirect Method?
    }
}