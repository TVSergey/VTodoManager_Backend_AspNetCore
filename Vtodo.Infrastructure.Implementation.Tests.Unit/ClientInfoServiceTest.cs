using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Moq;
using Vtodo.Infrastructure.Implementation.Services;
using Xunit;

namespace Vtodo.Infrastructure.Implementation.Tests.Unit
{
    public class ClientInfoServiceTest
    {
        [Fact]
        public void Ip_GetIp_ReturnsString()
        {
            var expectedIp = IPAddress.Parse("127.0.0.1");
            
            var clientInfoService = new ClientInfoService(SetupMockHttpContextAccessor(expectedIp).Object);
            
            Assert.Equal(expectedIp.ToString(), clientInfoService.Ip);
        }

        [Fact]
        public void DeviceInfo_GetUnknownDeviceOsName_ReturnsString()
        {
            var clientInfoService = new ClientInfoService(SetupMockHttpContextAccessor(IPAddress.Parse("127.0.0.1"), "TestUnknownDeviceOsName").Object);
            
            Assert.Equal("Unknown device", clientInfoService.DeviceInfo);
        }
        
        [Theory]
        [MemberData(nameof(GetMockHttpContextAccessorsForTestDeviceOsNames))]
        public void DeviceInfo_GetDeviceOsName_ReturnsString(Mock<IHttpContextAccessor> mockHttpContextAccessor, string exceptedOsName)
        {
            var clientInfoService = new ClientInfoService(mockHttpContextAccessor.Object);
            
            Assert.Equal(exceptedOsName, clientInfoService.DeviceInfo);
        }
        
        private static Mock<IHttpContextAccessor> SetupMockHttpContextAccessor(IPAddress ip, string userAgentString = "")
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var defaultContext = new DefaultHttpContext
            {
                Connection =
                {
                    RemoteIpAddress = ip
                }
            };

            defaultContext.Request.Headers["User-Agent"] = userAgentString;
            
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(defaultContext);
            
            return mockHttpContextAccessor;
        }
        
        public static IEnumerable<object[]> GetMockHttpContextAccessorsForTestDeviceOsNames()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            //Linux
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Ubuntu Chromium/37.0.2062.94 Chrome/37.0.2062.94 Safari/537.36"), "Linux" };
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (X11; Linux x86_64; rv:31.0) Gecko/20100101 Firefox/31.0"), "Linux" };
            
            //Mac OS
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.85 Safari/537.36"), "Mac OS" };
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.9; rv:37.0) Gecko/20100101 Firefox/37.0"), "Mac OS" };
            
            //Windows 10+
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.157 Safari/537.36"), "Windows 10+" };
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; MAARJS; rv:11.0) like Gecko"), "Windows 10+" };
            
            //Windows 8.1
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.94 Safari/537.36"), "Windows 8.1" };
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Windows NT 6.3; Win64; x64; Trident/7.0; MAARJS; rv:11.0) like Gecko"), "Windows 8.1" };
           
            //Windows 8
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.85 Safari/537.36"), "Windows 8" };
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.85 Safari/537.36"), "Windows 8" };
            
            //Windows 7
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:33.0) Gecko/20100101 Firefox/33.0"), "Windows 7" };
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Windows NT 6.1; rv:37.0) Gecko/20100101 Firefox/37.0"), "Windows 7" };

            //Android
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Linux; U; Android 4.0.3; en-us; KFTT Build/IML74K) AppleWebKit/537.36 (KHTML, like Gecko) Silk/3.68 like Chrome/39.0.2171.93 Safari/537.36"), "Android" };
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (Linux; Android 5.0.2; SM-T800 Build/LRX22G) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.133 Safari/537.36"), "Android" };
            
            //iOS
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (iPad; CPU OS 8_3 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Version/8.0 Mobile/12F69 Safari/600.1.4"), "iOS" };
            yield return new object[] { SetupMockHttpContextAccessor(ip, "Mozilla/5.0 (iPhone; CPU iPhone OS 8_4 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Version/8.0 Mobile/12H143 Safari/600.1.4"), "iOS" };

        }
    }
}