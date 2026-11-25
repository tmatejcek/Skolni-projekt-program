using System;

public static class Input
{
    public static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (input == null)
            {
                throw new EndOfStreamException("Vstup byl ukončen.");
            }

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            Console.WriteLine("Vstup nesmí být prázdný.");
        }
    }

    public static int ReadInteger(string prompt, int minimum, int maximum)
    {
        if (minimum > maximum)
        {
            throw new ArgumentException("Minimum nesmí být větší než maximum.");
        }

        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (input == null)
            {
                throw new EndOfStreamException("Vstup byl ukončen.");
            }

            int number;
            if (int.TryParse(input, out number) && number >= minimum && number <= maximum)
            {
                return number;
            }

            Console.WriteLine("Zadejte celé číslo v rozsahu " + minimum + " až " + maximum + ".");
        }
    }

    public static int ReadMenuChoice(string prompt, int minimumChoice, int maximumChoice)
    {
        return ReadInteger(prompt, minimumChoice, maximumChoice);
    }

    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (input == null)
            {
                throw new EndOfStreamException("Vstup byl ukončen.");
            }

            input = input.Trim().ToLower();

            if (input == "a" || input == "ano" || input == "y" || input == "yes")
            {
                return true;
            }

            if (input == "n" || input == "ne" || input == "no")
            {
                return false;
            }

            Console.WriteLine("Odpovězte ano nebo ne.");
        }
    }
}
