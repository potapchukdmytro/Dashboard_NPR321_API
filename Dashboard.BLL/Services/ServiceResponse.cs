﻿using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Dashboard.BLL.Services
{
    public class ServiceResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object? Payload { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public static ServiceResponse GetResponse(string message, bool success, object? payload, HttpStatusCode statusCode)
        {
            return new ServiceResponse
            {
                Message = message,
                Success = success,
                Payload = payload,
                StatusCode = statusCode
            };
        }

        public static ServiceResponse OkResponse(string message, object? payload = null)
        {
            return GetResponse(message, true, payload, HttpStatusCode.OK);
        }

        public static ServiceResponse BadRequestResponse(string message, object? payload = null)
        {
            return GetResponse(message, false, payload, HttpStatusCode.BadRequest);
        }

        public static ServiceResponse InternalServerErrorResponse(string message, object? payload = null)
        {
            return GetResponse(message, false, payload, HttpStatusCode.InternalServerError);
        }

        public static ServiceResponse ByIdentityResult(IdentityResult result, string successMessage)
        {
            if (result.Succeeded)
            {
                return ServiceResponse.OkResponse(successMessage);
            }
            else
            {
                return ServiceResponse.BadRequestResponse(result.Errors.First().Description);
            }
        }
    }
}
