using System;
using System.Collections.Generic;

public class Player
{
    private static readonly List<string> playerList = [];

    public Player(string nickname, string name)
    {
        Nickname = nickname;
        Name = name;
    }

    public Player(string name)
    {
        Nickname = name;
        Name = name;
    }

    public string Nickname { get; set; }
    public string Name { get; set; }

    public static void AddPlayer()
    {
        string name = Input.ReadText("Jméno hráče: ");
        AddPlayer(name);
    }

    public static void AddPlayer(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Jméno hráče nemůže být prázdné.");
            }
            else if (name.Contains("|"))
            {
                throw new ArgumentException("Jméno hráče nesmí obsahovat znak '|'.");
            }
            else if (!name.All(character => char.IsLetter(character) || char.IsWhiteSpace(character)))
            {
                throw new ArgumentException("Jméno hráče může obsahovat pouze písmena a mezery.");
            }
            else if (playerList.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine("Hráč " + name + " již je v turnaji.");
            }
            else
            {
                playerList.Add(name);
                Console.WriteLine("Hráč " + name + " byl přidán do turnaje.");
            }
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine("Nepodařilo se přidat hráče: " + exception.Message);
        }
    }

    public static void RemovePlayer()
    {
        string name = Input.ReadText("Jméno hráče: ");
        if (!playerList.Contains(name, StringComparer.OrdinalIgnoreCase))
        {
            RemovePlayer(name);
            return;
        }

        if (Input.ReadYesNo("Opravdu chcete odebrat hráče " + name + "? (a/n): "))
        {
            RemovePlayer(name);
        }
        else
        {
            Console.WriteLine("Odebrání hráče bylo zrušeno.");
        }
    }

    public static void RemovePlayer(string name)
    {
        string? existingName = playerList.FirstOrDefault(
            player => string.Equals(player, name, StringComparison.OrdinalIgnoreCase));

        if (existingName == null)
        {
            Console.WriteLine("Hráč " + name + " není v turnaji.");
        }
        else
        {
            playerList.Remove(existingName);
            Console.WriteLine("Hráč " + existingName + " byl odstraněn z turnaje.");
        }
    }
    
    public static void ListPlayers()
    {
        if (playerList.Count == 0)
        {
            Console.WriteLine("Žádní hráči nejsou v turnaji.");
            return;
        }

        Console.WriteLine("Seznam hráčů v turnaji:");
        foreach (var player in playerList)
        {
            Console.WriteLine(player);
        }
    }
}   