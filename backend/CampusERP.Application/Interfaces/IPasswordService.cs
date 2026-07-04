namespace CampusERP.Application.Interfaces;

public interface IPasswordService
{
    string HashPassword(string password);

    bool VerifyPassword(string password,string passwordHash);

    void ValidatePasswordPolicy(string password);
}