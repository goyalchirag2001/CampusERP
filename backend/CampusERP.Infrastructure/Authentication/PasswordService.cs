using CampusERP.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusERP.Infrastructure.Authentication;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new object(), password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(new object(),passwordHash,password);

        return result != PasswordVerificationResult.Failed;
    }

    public void ValidatePasswordPolicy(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required.");
        }

        if (password.Length < 8 || password.Length > 16)
        {
            errors.Add("Password must be between 8 and 16 characters.");
        }

        if (!password.Any(char.IsUpper))
        {
            errors.Add("At least one uppercase letter is required.");
        }

        if (!password.Any(char.IsLower))
        {
            errors.Add("At least one lowercase letter is required.");
        }

        if (!password.Any(char.IsDigit))
        {
            errors.Add("At least one number is required.");
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            errors.Add("At least one special character is required.");
        }

        if (errors.Any())
        {
            throw new Exception(string.Join(Environment.NewLine, errors));
        }
    }
}