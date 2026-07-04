namespace CampusERP.Shared.Utilities;

public static class PasswordGenerator
{
    private const string Uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";

    private const string Lowercase = "abcdefghijkmnopqrstuvwxyz";

    private const string Numbers = "23456789";

    private const string Special = "@#$%&*!";

    public static string Generate(int length = 10)
    {
        var random = new Random();

        var passwordChars = new List<char>
        {
            Uppercase[random.Next(Uppercase.Length)],
            Lowercase[random.Next(Lowercase.Length)],
            Numbers[random.Next(Numbers.Length)],
            Special[random.Next(Special.Length)]
        };

        var allChars = Uppercase + Lowercase + Numbers + Special;

        while (passwordChars.Count < length)
        {
            passwordChars.Add(allChars[random.Next(allChars.Length)]);
        }

        return new string(passwordChars.OrderBy(_ => random.Next()).ToArray());
    }
}