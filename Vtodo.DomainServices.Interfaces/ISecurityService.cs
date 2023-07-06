namespace Vtodo.DomainServices.Interfaces
{
    internal interface ISecurityService
    {
        string HashPassword(string password, int keySize, int iteration, out byte[] salt);
        bool VerifyPassword(string password, string hashedPassword, int keySize, int iterations, byte[]salt);
        byte[] GenerateSalt(int keySize);
    }
}