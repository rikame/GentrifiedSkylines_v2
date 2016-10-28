using System;
using ColossalFramework;
using GentrifiedSkylines.Redirection;

namespace GentrifiedSkylines.Detours
{
    class GentrificationManager
    {
        public void GentrifiedImplementation(uint citizen, GentrificationManager.Outcome outcome)
        {
            if (outcome == GentrificationManager.Outcome.LeaveCity)
            {
                Singleton<CitizenManager>.instance.ReleaseCitizen(citizen);
            }
            if (outcome == GentrificationManager.Outcome.Evicted)
            {
                //
            }
        }

        public void TryMoveFamily(uint citizenID)
        {
            uint temp = citizenID;
            int familySize = 0;
            CitizenUnit data = Singleton<CitizenManager>.instance.m_units.m_buffer[citizenID];
            Citizen cit = Singleton<CitizenManager>.instance.m_citizens.m_buffer[citizenID];
            if ((int)data.m_citizen4 != 0 && !Singleton<CitizenManager>.instance.m_citizens.m_buffer[data.m_citizen4].Dead)
            {
                ++familySize;
                citizenID = data.m_citizen4;
            }
            if ((int)data.m_citizen3 != 0 && !Singleton<CitizenManager>.instance.m_citizens.m_buffer[data.m_citizen3].Dead)
            {
                ++familySize;
                citizenID = data.m_citizen3;
            }
            if ((int)data.m_citizen2 != 0 && !Singleton<CitizenManager>.instance.m_citizens.m_buffer[data.m_citizen2].Dead)
            {
                ++familySize;
                citizenID = data.m_citizen2;
            }
            if ((int)data.m_citizen1 != 0 && !Singleton<CitizenManager>.instance.m_citizens.m_buffer[data.m_citizen1].Dead)
            {
                ++familySize;
                citizenID = data.m_citizen1;
            }
            if ((int)data.m_citizen0 != 0 && !Singleton<CitizenManager>.instance.m_citizens.m_buffer[data.m_citizen0].Dead)
            {
                ++familySize;
                citizenID = data.m_citizen0;
            }
            if ((int)citizenID == 0)
                return;
            ResidentAI ai = cit.GetCitizenInfo(citizenID).GetAI() as ResidentAI;
            citizenID = temp;   //TODO: Unsure if citizenID should be kept or changed below
            ai.TryMoveFamily(citizenID, ref cit, familySize);
        }

        /*private void MoveFamily(uint homeID, ref CitizenUnit data, ushort targetBuilding)
        {
            BuildingManager instance1 = Singleton<BuildingManager>.instance;
            CitizenManager instance2 = Singleton<CitizenManager>.instance;
            uint unitID = 0;
            if ((int)targetBuilding != 0)
                unitID = instance1.m_buildings.m_buffer[(int)targetBuilding].GetEmptyCitizenUnit(CitizenUnit.Flags.Home);
            for (int index = 0; index < 5; ++index)
            {
                uint citizen = data.GetCitizen(index);
                if ((int)citizen != 0 && !instance2.m_citizens.m_buffer[(IntPtr)citizen].Dead)
                {
                    instance2.m_citizens.m_buffer[(IntPtr)citizen].SetHome(citizen, (ushort)0, unitID);
                    if ((int)instance2.m_citizens.m_buffer[(IntPtr)citizen].m_homeBuilding == 0)
                        instance2.ReleaseCitizen(citizen);
                }
            }
        }*/

        /*private void UpdateHome(uint citizenID, ref Citizen data)
        {
            if ((int)data.m_homeBuilding != 0 || (data.m_flags & Citizen.Flags.DummyTraffic) != Citizen.Flags.None)
                return;
            TransferManager.TransferOffer offer = new TransferManager.TransferOffer();
            offer.Priority = 7;
            offer.Citizen = citizenID;
            offer.Amount = 1;
            offer.Active = true;
            if ((int)data.m_workBuilding != 0)
            {
                BuildingManager instance = Singleton<BuildingManager>.instance;
                offer.Position = instance.m_buildings.m_buffer[(int)data.m_workBuilding].m_position;
            }
            else
            {
                offer.PositionX = Singleton<SimulationManager>.instance.m_randomizer.Int32(256U);
                offer.PositionZ = Singleton<SimulationManager>.instance.m_randomizer.Int32(256U);
            }
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
        }*/
        public enum Outcome
        {
            None = 0,
            LeaveCity = 1,
            Evicted = 2,
            Moving = 4
        }
    }
}
