namespace Vtodo.Infrastructure.Interfaces.Services
{
    internal interface IClientInfoService
    {
        string Ip { get; set; }
        string DeviceInfo { get; set; }
    }
}