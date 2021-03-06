﻿using LSS.BE.Core.DataObjects.Dtos;
using LSS.BE.Core.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSS.BE.Core.DataObjects.Mappers
{
    public static class BookingStatusMapper
    {
        public static BookingStatusDto ToObject(this BookingStatusResponse model)
        {
            return new BookingStatusDto()
            {
                IsRequestSuccess = model.IsRequestSuccess,
                BookingId = model.BookingId,
                Status = model.BookingStatus,
                AuthenticationError = new BaseDtos.AuthenticatedErrorDto(model.Success, model.Status, model.Message),
                ValidationError = new BaseDtos.ValidationErrorDto(model.ErrorMessage, model.ErrorDetails.LockerStationId, model.ErrorDetails.LspId, model.ErrorDetails.BookingId)
            };
        }
    }
}
