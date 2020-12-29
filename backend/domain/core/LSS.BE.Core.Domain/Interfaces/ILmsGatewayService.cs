﻿using LSS.BE.Core.DataObjects.Dtos;
using LSS.BE.Core.Entities.Models;
using LSS.HCM.Core.DataObjects.Dtos;
using LSS.HCM.Core.DataObjects.Models;
using Newtonsoft.Json.Linq;

namespace LSS.BE.Core.Domain.Interfaces
{
    /// <summary>
    ///   Represents gateway service as a sequence of use cases.
    ///</summary>
    public interface ILmsGatewayService
    {
        /// <summary>
        /// Sets the Lsp verification member by providing locker station id, key and pin.
        /// </summary>
        /// <returns>
        ///  Gets the Lsp Id, Lsp user Id, reference code and expiration date with sucess result. 
        /// </returns>
        JObject LspVerification(LspUserAccess model);

        /// <summary>
        /// Sets the verify otp member by providing locker station id, code, ref code, lsp id and phone number.
        /// </summary>
        /// <returns>
        ///  Gets the sucess result with bool type true. 
        /// </returns>
        JObject VerifyOtp(VerifyOtp model);

        /// <summary>
        /// Sets the send otp member by providing locker station id, phone number, lsp id, booking id.
        /// </summary>
        /// <returns>
        ///  Gets the success bool flag and ref code.
        /// </returns>
        JObject SendOtp(SendOtp model);

        /// <summary>
        /// Sets the locker station details by locker station id.
        /// </summary>
        /// <returns>
        ///  Gets the success bool flag, hardware details, opening hour details and languages details.
        /// </returns>
        JObject LockerStationDetails(string lockerStationId);

        /// <summary>
        /// Sets the find booking details by locker station id, tracking number, lsp id.
        /// </summary>
        /// <returns>
        ///  Gets the success bool flag, hardware door number, locker preview.
        /// </returns>
        JObject FindBooking(string trackingNumber, string lockerStationId, string lspId);

        /// <summary>
        /// Sets the assign similar size locker by locker station id, booking id and reason.
        /// </summary>
        /// <returns>
        ///  Gets the success bool flag, hardware door number, assigned locker and locker preview.
        /// </returns>
        JObject AssignSimilarSizeLocker(AssignSimilarSizeLocker model);

        /// <summary>
        /// Sets the get available size by locker station id.
        /// </summary>
        /// <returns>
        ///  Gets the success bool flag and available sizes with categories.
        /// </returns>
        JObject GetAvailableSizes(string lockerStationId);

        /// <summary>
        /// Sets the change locker size by locker station id, booking id and size.
        /// </summary>
        /// <returns>
        ///  Gets the success bool flag, booking id, assigned lockers, hardware door number and locker preview.
        /// </returns>
        JObject ChangeLockerSize(ChangeLockerSize model);

        /// <summary>
        /// Sets the update booking status by locker station id, booking id, status, mobile number and reason.
        /// </summary>
        /// <returns>
        ///  Gets the success bool flag, booking id and status.
        /// </returns>
        JObject UpdateBookingStatus(BookingStatus model);

        /// <summary>
        /// Sets the consumer pin verification by locker station id, booking id, status, mobile number and reason.
        /// </summary>
        /// <returns>
        ///  Gets the success bool flag, booking id and status.
        /// </returns>
        JObject GetBookingByConsumerPin(ConsumerPin model);

        /// <summary>
        /// Sets the change locker status by locker station id, hwapi id and status.
        /// </summary>
        JObject ChangeSingleLockerStatus(ChangeLockerStatus model);

        /// <summary>
        /// Sets the retrieve locker belongs to courier by locker station id, lsp id, tracking number and status.
        /// </summary>
        JObject RetrieveLockersBelongsToCourier(string lockerStationId, string lspId, string trackingNumber, string status);

        /// <summary>
        /// Gets the open compartment parameters with Json web token credentials and MongoDB database credentials.
        /// Json web token credentials contains enable or disable, if enable then need to provide jwt secret and token.
        /// </summary>
        /// <returns>
        ///  List of compartment open status with object detection, LED status by requested compartment id's.
        /// </returns>
        JObject OpenCompartment(Compartment model);

        /// <summary>
        /// Gets the compartment status with Json web token credentials.
        /// Json web token credentials contains enable or disable, if enable then need to provide jwt secret and token.
        /// </summary>
        /// <returns>
        ///  List of compartment status with object detection and LED status based requested compartment id's.
        /// </returns>
        JObject CompartmentStatus(Compartment model);

        /// <summary>
        /// Gets the capture image parameters with Json web token credentials.
        /// Json web token credentials contains enable or disable, if enable then need to provide jwt secret and token.
        /// </summary>
        /// <returns>
        ///  Byte array of image with transaction Id and image extension.
        /// </returns>
        JObject CaptureImage(Capture model);
    }
}
