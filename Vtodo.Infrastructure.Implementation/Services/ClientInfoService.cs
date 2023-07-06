using System;
using Microsoft.AspNetCore.Http;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.Infrastructure.Implementation.Services
{
    internal class ClientInfoService : IClientInfoService
    {
        public ClientInfoService(IHttpContextAccessor httpContextAccessor)
        {
            Ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            string deviceInfoResult = "Unknown device";
            string deviceInfoFromRequest = deviceInfoResult;
            
            if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue("User-Agent", out var device))
            {
                deviceInfoFromRequest = device.ToString();
            }
            
            if (deviceInfoFromRequest.IndexOf("Linux", StringComparison.Ordinal) > 0)
            {
                deviceInfoResult = "Linux";
            }
            else if (deviceInfoFromRequest.IndexOf("Mac OS", StringComparison.Ordinal) > 0)
            {
                deviceInfoResult = "Mac OS";
            }
            else if (deviceInfoFromRequest.IndexOf("Windows NT 10.0", StringComparison.Ordinal) > 0)
            {
                deviceInfoResult = "Windows 10+";
            }
            else if (deviceInfoFromRequest.IndexOf("Windows NT 6.3", StringComparison.Ordinal) > 0)
            {
                deviceInfoResult = "Windows 8.1";
            }
            else if (deviceInfoFromRequest.IndexOf("Windows NT 6.2", StringComparison.Ordinal) > 0)
            {
                deviceInfoResult = "Windows 8";
            }
            else if (deviceInfoFromRequest.IndexOf("Windows NT 6.1", StringComparison.Ordinal) > 0)
            {
                deviceInfoResult = "Windows 7";
            }
            
            if (deviceInfoFromRequest.IndexOf("Android", StringComparison.Ordinal) > 0)
            {
                deviceInfoResult = "Android";
            }
            
            if (deviceInfoFromRequest.IndexOf("like Mac OS", StringComparison.Ordinal) > 0)
            {
                deviceInfoResult = "iOS";
            }
            DeviceInfo = deviceInfoResult;
        }
        
        public string Ip { get; set; }
        public string DeviceInfo { get; set; }
    }
}