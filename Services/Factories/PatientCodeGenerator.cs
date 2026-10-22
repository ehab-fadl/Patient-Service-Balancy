using System.Security.Cryptography;

namespace PatientService.Services.Factories;

public class PatientCodeGenerator : IPatientCodeGenerator
{
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public string Generate()
    {
        var firstLetter = Letters[RandomNumberGenerator.GetInt32(Letters.Length)];
        var secondLetter = Letters[RandomNumberGenerator.GetInt32(Letters.Length)];
        var digits = RandomNumberGenerator.GetInt32(100000, 1000000);

        return $"{firstLetter}{secondLetter}{digits}";
    }
}