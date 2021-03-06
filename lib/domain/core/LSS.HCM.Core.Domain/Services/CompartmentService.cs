﻿using LSS.HCM.Core.Common.Enums;
using LSS.HCM.Core.DataObjects.Settings;
using System;
using System.Collections.Generic;
using CompartmentConfiguration = LSS.HCM.Core.DataObjects.Settings.Compartment;
using Compartment = LSS.HCM.Core.Entities.Locker.Compartment;
using LSS.HCM.Core.Domain.Helpers;
using System.Linq;
using Serilog;
using System.Text.Json;

namespace LSS.HCM.Core.Domain.Services
{
    /// <summary>
    ///   Represents locker compartment service for open comparment and it's status.
    ///</summary>
    public class CompartmentService
    {
        /// <summary>
        /// Prepare open compartment object by communicating command to communication port service. 
        /// </summary>
        /// <returns>
        ///  Open compartment actual result with status. 
        /// </returns>
        public static Compartment CompartmentOpen(string compartmentId, AppSettings lockerConfiguration)
        {
            // Find the target compartment
            var target_compartment = lockerConfiguration.Locker.Compartments.Find(compartment => compartment.CompartmentId.Contains(compartmentId));

            // Commanding
            var DoorOpenResult = CommunicationPortControlService.SendCommand(CommandType.DoorOpen, CompartmentHelper.MapModuleId(target_compartment), lockerConfiguration);
            
            Compartment compartmentResult = new Compartment(lockerConfiguration.Locker.LockerId,
                                                             target_compartment.CompartmentId,
                                                             target_compartment.CompartmentSize,
                                                             Convert.ToBoolean(DoorOpenResult["DoorOpen"]),
                                                             !Convert.ToBoolean(DoorOpenResult["DoorOpen"]),
                                                             false,
                                                             Convert.ToBoolean(DoorOpenResult["DoorOpen"]) ? "ON" : "OFF");
            return compartmentResult;
        }

        /// <summary>
        /// Prepare compartment status object by communicating command to communication port service. 
        /// </summary>
        /// <returns>
        ///  Compartment status actual result including real hardware status. 
        /// </returns>
        public static List<Compartment> CompartmentStatus(DataObjects.Models.Compartment model, AppSettings lockerConfiguration)
        {
            // Find locker controller board module id and object detection module id list
            var lcbModuleList = new List<string> { };
            var odbModuleList = new List<string> { };
            List<CompartmentConfiguration> compartments = lockerConfiguration.Locker.Compartments;
           
            if (model.CompartmentIds.Any(CompartmentId => CompartmentId == "All")) model.CompartmentIds = lockerConfiguration.Locker.Compartments.Select(compartment => compartment.CompartmentId).ToArray();
           
            foreach (var compartmentId in model.CompartmentIds)
            {
                var compartment = compartments.Find(c => c.CompartmentId == compartmentId);
                if (!odbModuleList.Contains(compartment.CompartmentCode.Odbmod)) // Get object detection module id
                {
                    odbModuleList.Add(compartment.CompartmentCode.Odbmod);
                }

                // Get locker controller module id
                if (!lcbModuleList.Contains(compartment.CompartmentCode.Lcbmod))
                {
                    lcbModuleList.Add(compartment.CompartmentCode.Lcbmod);
                }
            }

            // Get all module update
            List<Compartment> compartmentList = new List<Compartment>();
            var opendoorStatusAry = new Dictionary<string, Dictionary<string, byte>> { };
            foreach (string moduleNo in lcbModuleList)
            {
                opendoorStatusAry[moduleNo] = CompartmentHelper.GetStatusByModuleId(CommandType.DoorStatus, moduleNo, lockerConfiguration);
            }
            var objectdetectStatusAry = new Dictionary<string, Dictionary<string, byte>> { };
            foreach (string moduleNo in odbModuleList)
            {
                objectdetectStatusAry[moduleNo] = CompartmentHelper.GetStatusByModuleId(CommandType.ItemDetection ,moduleNo, lockerConfiguration);
            }

            foreach (var compatmentId in model.CompartmentIds)
            {
                var compartment = compartments.Find(c => c.CompartmentId == compatmentId);
                
                Dictionary<string, byte> opendoorStatus = opendoorStatusAry[compartment.CompartmentCode.Lcbmod]; //[compatment.CompartmentCode.Lcbid];
                Dictionary<string, byte> objectdetectStatus = objectdetectStatusAry[compartment.CompartmentCode.Odbmod]; //[compatment.CompartmentCode.Odbid];
                Compartment compartmentResult = new Compartment(lockerConfiguration.Locker.LockerId,
                                                                 compartment.CompartmentId,
                                                                 compartment.CompartmentSize,
                                                                 opendoorStatus[compartment.CompartmentCode.Lcbid] == 0 ? true : false,
                                                                 opendoorStatus[compartment.CompartmentCode.Lcbid] == 0 ? false : true,
                                                                 objectdetectStatus[compartment.CompartmentCode.Odbid] == 1 ? true : false,
                                                                 opendoorStatus[compartment.CompartmentCode.Lcbid] == 0 ? "ON" : "OFF");

                Log.Debug("[HCM][Compartment Service][Compartment Status]" + "[Compartment ID : " + compartmentResult.CompartmentId.ToString() + "]"
                                                                           + "[Open Door status: " + compartmentResult.CompartmentDoorOpen.ToString() + "]"
                                                                           + "[Door Available status: " + compartmentResult.CompartmentDoorAvailable.ToString() + "]"
                                                                           + "[Object Detection status: " + compartmentResult.ObjectDetected.ToString() + "]"
                                                                           + "[LED status: " + compartmentResult.StatusLed.ToString() + "]"
                                                                           );
                compartmentList.Add(compartmentResult);
            }

            return compartmentList;
        }
        
    }
}
