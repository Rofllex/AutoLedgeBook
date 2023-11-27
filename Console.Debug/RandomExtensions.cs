using System;


namespace AutoLedgeBook.ConsoleDebug;

public static class RandomExtensions
{
    public static string RandomString(this Random rand, int length, char[] alphabet)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));
        string randString = string.Empty;
        for (var i = 0; i < length; i++)
            randString += alphabet[rand.Next(0, alphabet.Length)];
        return randString;
    }
}