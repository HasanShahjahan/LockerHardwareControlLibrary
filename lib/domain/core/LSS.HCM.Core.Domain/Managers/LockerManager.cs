﻿using LSS.HCM.Core.Common.Enums;
using LSS.HCM.Core.Common.Exceptions;
using LSS.HCM.Core.DataObjects.Dtos;
using LSS.HCM.Core.DataObjects.Mappers;
using LSS.HCM.Core.DataObjects.Settings;
using LSS.HCM.Core.Validator;
using System;
using System.IO;
using System.Text.Json;
using Compartment = LSS.HCM.Core.DataObjects.Models.Compartment;

namespace LSS.HCM.Core.Domain.Managers
{

    /// <summary>
    ///   Represents locker as a sequence of compartment open and it's status.
    ///</summary>
    public sealed class LockerManager
    {
        /// <summary>
        ///   Set Initialization value for locker management.
        ///</summary>
        private readonly AppSettings lockerConfiguration;

        /// <summary>
        ///   Initialization information for locker configuration including Microcontroller board, Serial port and Communication port.
        ///</summary>
        public LockerManager(string configurationFilePath)
        {
            var content = File.ReadAllText(configurationFilePath);
            lockerConfiguration = JsonSerializer.Deserialize<AppSettings>(content);
        }

        /// <summary>
        /// Gets the open compartment parameters with Json web token credentials and MongoDB database credentials.
        /// Json web token credentials contains enable or disable, if enable then need to provide jwt secret and token.
        /// </summary>
        /// <returns>
        ///  List of compartment open status with object detection, LED status by requested compartment id's.
        /// </returns>
        public LockerDto OpenCompartment(Compartment model)
        {
            var lockerDto = new LockerDto();

            try {
                var (statusCode, errorResult) = LockerManagementValidator.PayloadValidator(lockerConfiguration, model.JwtCredentials.IsEnabled, model.JwtCredentials.Secret, model.JwtCredentials.Token, PayloadTypes.OpenCompartment, model.LockerId, model.TransactionId, model.CompartmentIds, null);
                if (statusCode != StatusCode.Status200OK) return OpenCompartmentMapper.ToError(new LockerDto { StatusCode = statusCode, Error = errorResult });
                var result = CompartmentManager.CompartmentOpen(model, lockerConfiguration);
                lockerDto = OpenCompartmentMapper.ToObject(result);
            }
            catch (Exception ex)
            {
                return OpenCompartmentMapper.ToError(new LockerDto { StatusCode = StatusCode.Status500InternalServerError, Error = new Common.Exceptions.ApplicationException(ApplicationErrorCodes.InternalServerError, ex.Message) });
            }

            return lockerDto;
        }

        /// <summary>
        /// Gets the compartment status with Json web token credentials and MongoDB database credentials.
        /// Json web token credentials contains enable or disable, if enable then need to provide jwt secret and token.
        /// </summary>
        /// <returns>
        ///  List of compartment status with object detection and LED status based requested compartment id's.
        /// </returns>
        public CompartmentStatusDto CompartmentStatus(Compartment model)
        {
            var compartmentStatusDto = new CompartmentStatusDto();
            try 
            {
                var (statusCode, errorResult) = LockerManagementValidator.PayloadValidator(lockerConfiguration, model.JwtCredentials.IsEnabled, model.JwtCredentials.Secret, model.JwtCredentials.Token, PayloadTypes.CompartmentStatus, model.LockerId, model.TransactionId, model.CompartmentIds, null);
                if (statusCode != StatusCode.Status200OK) return CompartmentStatusMapper.ToError(new CompartmentStatusDto { StatusCode = statusCode, Error = errorResult });
                var result = CompartmentManager.CompartmentStatus(model, lockerConfiguration);
                compartmentStatusDto = CompartmentStatusMapper.ToObject(result);
            }
            catch (Exception ex)
            {
                return CompartmentStatusMapper.ToError(new CompartmentStatusDto { StatusCode = StatusCode.Status500InternalServerError, Error = new Common.Exceptions.ApplicationException(ApplicationErrorCodes.InternalServerError, ex.Message) });
            }
            
            return compartmentStatusDto;
        }
    }
}
