﻿using LSS.HCM.Core.Common.Enums;
using LSS.HCM.Core.DataObjects.Settings;
using LSS.HCM.Core.Domain.Core.InputOutpuPorts;
using System.Collections.Generic;

namespace LSS.HCM.Core.Domain.Services
{
    public sealed class CommunicationPortControlService
    {
        public static Dictionary<string, string> SendCommand(string commandName, List<byte> commandData, AppSettings lockerConfiguration)
        {
            List<byte> commandString = BoardInitializationService.GenerateCommand(commandName, commandData, lockerConfiguration);
            SerialPortControlService controlModule;
            List<byte> responseData;
            Dictionary<string, string> result = null;
            if (commandName == CommandType.DoorOpen)
            {
                controlModule = new SerialPortControlService(new SerialPortResource(lockerConfiguration.Microcontroller.LockControl.Port, lockerConfiguration.Microcontroller.LockControl.Baudrate, lockerConfiguration.Microcontroller.LockControl.DataBits, 500, 500));
                responseData = controlModule.Write(commandString, lockerConfiguration.Microcontroller.Commands.OpenDoor.ResLen);
                result = BoardInitializationService.ExecuteCommand(CommandType.DoorOpen, responseData);
            }
            else if (commandName == CommandType.DoorStatus)
            {
                controlModule = new SerialPortControlService(new SerialPortResource(lockerConfiguration.Microcontroller.LockControl.Port, lockerConfiguration.Microcontroller.LockControl.Baudrate, lockerConfiguration.Microcontroller.LockControl.DataBits, 500, 500));
                responseData = controlModule.Write(commandString, lockerConfiguration.Microcontroller.Commands.DoorStatus.ResLen);
                result = BoardInitializationService.ExecuteCommand(CommandType.DoorStatus, responseData);
            }
            else if (commandName == CommandType.ItemDetection)
            {
                controlModule = new SerialPortControlService(new SerialPortResource(lockerConfiguration.Microcontroller.ObjectDetection.Port, lockerConfiguration.Microcontroller.ObjectDetection.Baudrate, lockerConfiguration.Microcontroller.ObjectDetection.DataBits, 500, 500));
                responseData = controlModule.Write(commandString, lockerConfiguration.Microcontroller.Commands.DetectItem.ResLen);
                result = BoardInitializationService.ExecuteCommand(CommandType.ItemDetection, responseData);
            }
            Dictionary<string, string> commandResult = result;
            commandResult.Add("Command", commandName);
            return commandResult;
        }
    }
}
