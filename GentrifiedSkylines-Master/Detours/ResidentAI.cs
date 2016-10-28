using ColossalFramework;
using GentrifiedSkylines.Redirection;
using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    [TargetType(typeof(ResidentAI))]
    class ResidentAIDetour : ResidentAI
    {
        [RedirectMethod]
        public void TryMoveFamily(uint citizenID, ref Citizen data, int familySize, int priority1, int priority7)
        {
            if (data.Dead || (int)data.m_homeBuilding == 0)
                return;
            TransferManager.TransferOffer offer = new TransferManager.TransferOffer();
            offer.Priority = Singleton<SimulationManager>.instance.m_randomizer.Int32(priority1, priority7);
            offer.Citizen = citizenID;
            offer.Position = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)data.m_homeBuilding].m_position;
            offer.Amount = 1;
            offer.Active = true;
            if (familySize == 1)
            {
                if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2U) == 0)
                {
                    switch (data.EducationLevel)
                    {
                        case Citizen.Education.Uneducated:
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Single0, offer);
                            break;
                        case Citizen.Education.OneSchool:
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Single1, offer);
                            break;
                        case Citizen.Education.TwoSchools:
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Single2, offer);
                            break;
                        case Citizen.Education.ThreeSchools:
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Single3, offer);
                            break;
                    }
                }
                else
                {
                    switch (data.EducationLevel)
                    {
                        case Citizen.Education.Uneducated:
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Single0B, offer);
                            break;
                        case Citizen.Education.OneSchool:
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Single1B, offer);
                            break;
                        case Citizen.Education.TwoSchools:
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Single2B, offer);
                            break;
                        case Citizen.Education.ThreeSchools:
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Single3B, offer);
                            break;
                    }
                }
            }
            else
            {
                switch (data.EducationLevel)
                {
                    case Citizen.Education.Uneducated:
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Family0, offer);
                        break;
                    case Citizen.Education.OneSchool:
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Family1, offer);
                        break;
                    case Citizen.Education.TwoSchools:
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Family2, offer);
                        break;
                    case Citizen.Education.ThreeSchools:
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Family3, offer);
                        break;
                }
            }
        }
    }
}