﻿using LSS.BE.Core.Common.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LSS.BE.Core.Common.Exceptions
{
    public class ValidationError : AuthenticationError
    {
        public ValidationError()
        {
            IsRequestSuccess = false;
            ErrorMessage = string.Empty;
            ErrorDetails = new ErrorDetails();
        }

        [JsonProperty("is_request_success")]
        public bool IsRequestSuccess { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("error_details")]
        public ErrorDetails ErrorDetails { get; set; }
    }
}
