using ColossalFramework;
using System;
using UnityEngine;

namespace GentrifiedSkylines.Detours
{
    public class Trip
    {
        //-----------------------------------------------------------------//
        private bool activated;
        private Leg.Flags currentVehicleType;
        private uint m_citizen;
        private byte m_current;
        private CitizenInstance.Flags m_flags; //TODO: Determine significance of remaining flags (WaitingTaxi and Transition)
        private byte m_legCount;
        private Leg[] m_legs;
        private ushort m_source;
        private ushort m_target;
        private Leg.Flags previousVehicleType = Leg.Flags.None;
        private DateTime ticksPrevious;
        private DateTime ticksCurrent = Singleton<SimulationManager>.instance.FrameToTime(Singleton<SimulationManager>.instance.m_currentFrameIndex);
        private bool valid = true;
        //-----------------------------------------------------------------//

        public Trip(uint citizen, ushort source, ushort target)                                 //Default Constructor
        {
            m_legCount = 0;
            activated = false;
            m_legs = new Leg[0];
            m_source = source;
            m_target = target;
            m_citizen = citizen;
            //m_flags = Singleton<CitizenManager>.instance.m_instances.m_buffer[m_citizen].m_flags;
        }
        public void CitizenStepBroadcasted()                                                    //Effectively called in each HumanAI's SimulationStep() methods
        {
            ticksPrevious = ticksCurrent;
            ticksCurrent = Singleton<SimulationManager>.instance.FrameToTime(Singleton<SimulationManager>.instance.m_currentFrameIndex);
            TryAddLeg();    //VERIFY: Test if activation is needed or already handled sufficiently
        }
        public void FinalizeLeg(Vector3 vec)                                                    //Directs this trip's current leg to finalize
        {
            m_legs[m_current].finalize(vec);
        }
        public uint GetCitizen()                                                                //Return this trip's CitizenInstance
        {
            return m_citizen;
        }
        public float GetContribution()                                                          //Calculate and return the overall rating contribution of this trip
        {
            return 7;       //TODO: Cycle through legs and determining individual value, aggregate value, hangup penalties and so-on
        }
        public byte GetLegCount()                                                               //Get the number of legs in this trip
        {
            return m_legCount;
        }
        public Leg[] GetLegs()                                                                  //Retrive this trip's array of legs
        {
            if (activated)
            {
                return m_legs;
            }
            else
            {
                return new Leg[0];
            }
        }
        public ushort GetSource()                                                               //Return this trip's arbitrary source ID
        {
            return m_source;
        }
        public Building GetSourceBuilding()                                                     //Return this trip's source building instance
        {
            return Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_source];
        }
        public Vector3 GetSourcePosition()                                                      //Return this trip's source Vector3
        {
            return GetSourceBuilding().m_position;
        }
        public ushort GetTarget()                                                               //Return this trip's arbitrary target ID
        {
            return m_target;
        }
        public Building GetTargetBuilding()                                                     //Return this trip's target building instance
        {
            return Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_target];
        }
        public Vector3 GetTargetPosition()                                                      //Return this trip's target Vector3
        {
            return GetSourceBuilding().m_position;
        }
        public bool IsValid()                                                                   //Returns as bool the validity of this trip
        {
            return valid;
        }
        public void ResetTrip()                                                                 //Reset a trip (Mistakenly written, likely will never be used)
        {
            activated = false;
            currentVehicleType = Leg.Flags.None;
            m_citizen = 0;
            m_current = 0;
            m_flags = CitizenInstance.Flags.None;
            m_legCount = 0;
            m_legs = new Leg[0];
            m_source = 0;
            m_target = 0;
            previousVehicleType = Leg.Flags.None;
            valid = false;
        }
        public void SetSource(ushort s)                                                         //Set the arbitrary ID of this trip's source
        {
            m_source = s;
        }
        public void SetTarget(ushort t)                                                         //Set the arbitrary ID of this trip's target
        {
            m_target = t;
        }
        public void TryAddLeg()                                                                 //Gets new CitizenInstance flags for analysis: [new , update, ignore, close, invalidate]
        {
            CitizenInstance.Flags t_flags = Singleton<CitizenManager>.instance.m_instances.m_buffer[m_citizen].m_flags;
            if (CheckMoving())                                      //If the CI is moving
            {
                if (CheckNewVehicle())                                  //If the CI has a new vehicle
                {
                    FinalizeLeg(GetMyPosition());
                    AddLeg();                                               //[New]
                    previousVehicleType = currentVehicleType;               //Update previousVehicleType
                }
                CheckFlags(t_flags);                                    //Check CI flags
                int num = (int)t_flags;                                 //Cast CI flags to int
                MediateMode(t_flags);                                   //[Update]
            }
            UpdateMyFlags(t_flags);                                 //Update m_flags for next cycle
        }
        private void AddLeg()                                                                   //Attempt to add a Leg to this trip
        {
            //Activate if not yet activated
            if (!activated)
            {
                activated = true;
            }
            //Cycle Leg array to a new array of size n+1
            byte oldLength = Convert.ToByte(Mathf.Clamp(m_legs.Length, 0, byte.MaxValue - 1));
            byte newLength = Convert.ToByte(oldLength + 1);
            Leg[] temp = new Leg[oldLength + 1];
            for (byte i = 1; i <= oldLength; i++)
            {
                temp[i - 1] = m_legs[i - 1];
            }

            temp[newLength] = new Leg(currentVehicleType, GetMyPosition());      //currentVehicleType is set before the call to this method in TryAddLeg() when CheckVehicleType() is called
            m_legs = temp;
            m_legCount += 1;
            //Set current, uses this order to avoid null/range exceptions
            m_current = (byte)(m_legCount - 1);
        }
        private void CheckFlags(CitizenInstance.Flags t_flags)                                  //Checks changes to CitizenInstance flags
        {
            if (m_flags != t_flags)                                                                                             //If Flags have been updated
            {                                                                                                                   //then evaluate if a new trip must be added
                if ((m_flags & CitizenInstance.Flags.Underground) != (t_flags & CitizenInstance.Flags.Underground))
                {
                    if ((m_flags & CitizenInstance.Flags.Underground) != CitizenInstance.Flags.None)
                    //Start Underground
                    {
                    }
                    else
                    //Cease Underground
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.BorrowCar) != (t_flags & CitizenInstance.Flags.BorrowCar))
                {
                    if ((m_flags & CitizenInstance.Flags.BorrowCar) != CitizenInstance.Flags.None)
                    //Start BorrowCar
                    {
                    }
                    else
                    //Cease BrorrowCar
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.EnteringVehicle) != (t_flags & CitizenInstance.Flags.EnteringVehicle))
                {
                    if ((m_flags & CitizenInstance.Flags.EnteringVehicle) != CitizenInstance.Flags.None)
                    //Start EnteringVehicle
                    {
                    }
                    else
                    //Cease EnteringVehicle
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.RidingBicycle) != (t_flags & CitizenInstance.Flags.RidingBicycle))
                {
                    if ((m_flags & CitizenInstance.Flags.RidingBicycle) != CitizenInstance.Flags.None)
                    //Start RidingBicycle
                    {
                    }
                    else
                    //Cease RidingBicycle
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.OnBikeLane) != (t_flags & CitizenInstance.Flags.OnBikeLane))
                {
                    if ((m_flags & CitizenInstance.Flags.OnBikeLane) != CitizenInstance.Flags.None)
                    //Start OnBikeLane
                    {
                    }
                    else
                    //Cease OnBikeLane
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.OnPath) != (t_flags & CitizenInstance.Flags.OnPath))
                {
                    if ((m_flags & CitizenInstance.Flags.OnPath) != CitizenInstance.Flags.None)
                    //Start OnPath
                    {
                    }
                    else
                    //End OnPath
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.SittingDown) != (t_flags & CitizenInstance.Flags.SittingDown))
                {
                    if ((m_flags & CitizenInstance.Flags.SittingDown) != CitizenInstance.Flags.SittingDown)
                    //Start SittingDown
                    {
                    }
                    else
                    //Cease SittingDown
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.TryingSpawnVehicle) != (t_flags & CitizenInstance.Flags.TryingSpawnVehicle))
                {
                }
                if ((m_flags & CitizenInstance.Flags.WaitingPath) != (t_flags & CitizenInstance.Flags.WaitingPath))
                {
                }
                if ((m_flags & CitizenInstance.Flags.WaitingTransport) != (t_flags & CitizenInstance.Flags.WaitingTransport))
                {
                }
                if ((m_flags & CitizenInstance.Flags.WaitingTaxi) != (t_flags & CitizenInstance.Flags.WaitingTaxi))
                {
                    if ((m_flags & CitizenInstance.Flags.WaitingTaxi) != CitizenInstance.Flags.None)
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.BoredOfWaiting) != (t_flags & CitizenInstance.Flags.BoredOfWaiting))
                {
                    if ((m_flags & CitizenInstance.Flags.BoredOfWaiting) != CitizenInstance.Flags.None)
                    //Start BoredOfWaiting
                    {
                    }
                    else
                    //Cease BoredOfWaiting
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.Panicking) != (t_flags & CitizenInstance.Flags.Panicking))
                {
                }
                if ((m_flags & CitizenInstance.Flags.AtTarget) != (t_flags & CitizenInstance.Flags.AtTarget))
                {
                    if ((m_flags & CitizenInstance.Flags.AtTarget) != CitizenInstance.Flags.None)
                    //Start AtTarget
                    {
                    }
                    else
                    //Cease AtTarget
                    {
                    }
                }
                if ((m_flags & CitizenInstance.Flags.InsideBuilding) != (t_flags & CitizenInstance.Flags.InsideBuilding))
                {
                }
                if ((m_flags & CitizenInstance.Flags.Deleted) != (t_flags & CitizenInstance.Flags.Deleted))
                {
                }
                if ((m_flags & CitizenInstance.Flags.CannotUseTransport) != (t_flags & CitizenInstance.Flags.CannotUseTransport))
                {
                }
                if ((m_flags & CitizenInstance.Flags.CannotUseTaxi) != (t_flags & CitizenInstance.Flags.CannotUseTaxi))
                {
                }
                /*
                * ----------------------------------------------------------------------------------------------------------------------------------------------------------
                * UNUSED:                           TargetFlags ; RequireSlowStart ; Transition ; Created ; CustomName ; CustomColor ; HangAround ;
                * ----------------------------------------------------------------------------------------------------------------------------------------------------------
                */
            }
        }
        private void CheckHangups()                                                             //Verifies that the leg was created correctly and either closes or invalidates the leg or the trip
        {
            //Unsure                                                                            //TODO: Write the CheckHangups() method.
        }
        private bool CheckMoving()                                                              //Queries and returns bool of if the citizen is moving
        {
            if (GetMyCitizen().CurrentLocation == Citizen.Location.Moving)
                return true;
            return false;
        }
        private bool CheckNewVehicle()                                                          //Queries and returns bool of if the citizen  has a new vehicle
        {
            bool found = false;

            if (GetValidVehicle())
            {
                VehicleInfo.VehicleType t_mode = GetVehicleType(); //Needs updating
                if (t_mode == VehicleInfo.VehicleType.Bicycle)
                {
                    currentVehicleType = Leg.Flags.Bicycle;
                    found = true;
                }
                if (t_mode == VehicleInfo.VehicleType.Car)
                {
                    currentVehicleType = Leg.Flags.Car;
                    found = true;
                }
                if (t_mode == VehicleInfo.VehicleType.Metro)
                {
                    currentVehicleType = Leg.Flags.Metro;
                    found = true;
                }
                if (t_mode == VehicleInfo.VehicleType.Plane)
                {
                    currentVehicleType = Leg.Flags.Plane;
                    found = true;
                }
                if (t_mode == VehicleInfo.VehicleType.Ship)
                {
                    currentVehicleType = Leg.Flags.Ship;
                    found = true;
                }
                if (t_mode == VehicleInfo.VehicleType.Train)
                {
                    currentVehicleType = Leg.Flags.Train;
                    found = true;
                }
                if (t_mode == VehicleInfo.VehicleType.Tram)
                {
                    currentVehicleType = Leg.Flags.Tram;
                    found = true;
                }
                ItemClass.SubService t_subservice = GetMyVehicle().Info.m_class.m_subService;
                if (t_subservice == ItemClass.SubService.PublicTransportBus)
                {
                    currentVehicleType = Leg.Flags.Bus;
                    found = true;
                }
                if (t_subservice == ItemClass.SubService.PublicTransportTaxi)
                {
                    currentVehicleType = Leg.Flags.Taxi;
                    found = true;
                }
                ItemClass.Service t_service = GetMyVehicle().Info.m_class.m_service;
                if (t_service != ItemClass.Service.None)
                {
                    this.Invalidate();                          //Trip is taking place in a special case and is excluded from consideration with regard to accessibility
                }
                if (!found)                                     //Citizen is moving but without any detectable vehicle. Walking is assumed
                    currentVehicleType = Leg.Flags.Walk;
                if (currentVehicleType != previousVehicleType)  //Checks temporary flag 'found' indicating a new vehicle and returns as bool
                    return true;
                return false;
            }
            currentVehicleType = Leg.Flags.Walk;                //Repeated code block. If citizen is moving with an invalid vehicle then walking is assumed
            if (currentVehicleType != previousVehicleType)
                return true;
            return false;
        }
        private Vector3 GetCitizenPosition()                                                    //Returns citizen position as a Vector3
        {
            return Singleton<CitizenManager>.instance.m_instances.m_buffer[m_citizen].GetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex).m_position;   //VERIFY: Make sure that this Singleton<SimulationManager> isn't crashing the application
        }
        private Vector3 GetCitizenVelocity()                                                    //Returns citizenVelocity as a Vector3
        {
            return Singleton<CitizenManager>.instance.m_instances.m_buffer[m_citizen].GetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex).m_velocity;   //VERIFY: Make sure that this Singleton<SimulationManager> isn't crashing the application
        }
        private Citizen GetMyCitizen()                                                          //Returns this trip's Citizen
        {
            return Singleton<CitizenManager>.instance.m_citizens.m_buffer[m_citizen];
        }
        private Vector3 GetMyPosition()                                                         //Returns this trip's citizen's position or its vehicle position, if applicable
        {
            if (GetValidVehicle())
                return GetVehiclePosition();
            return GetCitizenPosition();
        }
        private Vector3 GetMyVelocity()                                                         //Returns this trip's citizen's velocity or its vehicle velocity, if applicable
        {
            if (GetValidVehicle())
                return GetVehicleVelocity();
            return GetCitizenVelocity();
        }
        private Vehicle GetMyVehicle()                                                          //Returns this trip's citizen's vehicle
        {
            return Singleton<VehicleManager>.instance.m_vehicles.m_buffer[Singleton<CitizenManager>.instance.m_citizens.m_buffer[m_citizen].m_vehicle];
        }
        private bool GetValidVehicle()                                                          //Queries bool for if this trip's citizen has a valid vehicle
        {
            if (Singleton<CitizenManager>.instance.m_citizens.m_buffer[m_citizen].m_vehicle == 0)
                return false;
            return true;
        }
        private Vector3 GetVehiclePosition()                                                    //Returns this citizen's vehicle position as Vector 3, used as publically accessible GetMyPosition()
        {
            return GetMyVehicle().GetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex).m_position;           //VERIFY: Make sure that this Singleton<SimulationManager> isn't crashing the application
        }
        private float GetVehicleTravelDistance()                                                //Returns this citizen's vehicle's travel distance when
        {
            return GetMyVehicle().GetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex).m_travelDistance;     //VERIFY: Make sure that this Singleton<SimulationManager> isn't crashing the application
        }
        private VehicleInfo.VehicleType GetVehicleType()                                        //Returns this citizen's vehicle type, returns none if vehicle is not valid
        {
            if (GetValidVehicle())
                return GetMyVehicle().Info.m_vehicleType;
            return VehicleInfo.VehicleType.None;
        }
        private Vector3 GetVehicleVelocity()                                                    //Returns this citizen's vehicle velocity as a Vector3
        {
            if (GetValidVehicle())
                return GetMyVehicle().GetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex).m_velocity;       //VERIFY: Make sure that this Singleton<SimulationManager> isn't crashing the application
            return Vector3.zero;
        }
        private void Invalidate()                                                               //Calls this trip to be marked as invalid
        {
            valid = false;
        }
        private bool MediateMode(CitizenInstance.Flags t_flags)                                 //Called if a new leg is not established to update leg information for later analysis by checking
        {
            if (activated && m_legs[m_current].getMode() != Leg.Flags.None)     //Check if current leg has been initialized
            {
                long tickInterval = ticksCurrent.Ticks - ticksPrevious.Ticks;
                double sx = GetMyVelocity().x;
                double sz = GetMyVelocity().z;
                double sv = Math.Sqrt((sx*sx) + (sz*sz));
                m_legs[m_current].addSpeed((float)sv);
                switch (m_legs[m_current].getMode())                            //TODO: Write MediateMode() cases to record data
                {                                                               //TODO: Determine what needs to be done for each Leg.Flags case in MediateMode()
                    case Leg.Flags.Bicycle:
                        if ((m_flags & CitizenInstance.Flags.OnBikeLane) != CitizenInstance.Flags.None)
                        {
                            m_legs[m_current].addTimeOnBikeLane(tickInterval);
                        }
                        break;

                    case Leg.Flags.Bus:
                        break;

                    case Leg.Flags.Car:
                        break;

                    case Leg.Flags.Metro:
                        break;

                    case Leg.Flags.Plane:
                        break;

                    case Leg.Flags.Ship:
                        break;

                    case Leg.Flags.Taxi:
                        break;

                    case Leg.Flags.Train:
                        break;

                    case Leg.Flags.Tram:
                        break;

                    case Leg.Flags.Walk:
                        break;
                }
                return true;
            }
            return false;
        }
        private void UpdateMyFlags(CitizenInstance.Flags t_flags)                               //Update flags upon simulation step
        {
            m_flags = t_flags;
        }
        public float GetDistanceTraveled()
        {
            float f = 0;
            for (int i = 0; i < m_legCount; i++)
            {
                f += m_legs[i].GetDistanceTraveled(GetMyPosition());
            }
            return f;
        }
    }
}