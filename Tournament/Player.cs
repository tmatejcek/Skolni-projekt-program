using System;
using System.Collections.Generic;

public class Player
{
    private static readonly List<string> playerList = [];

    public string Name { get; set; }

    public static void AddPlayer(string name)
    {
        try
        {
            if (playerList.Contains(name))
            {
                Console.WriteLine("Hráč " + name + " již je v turnaji.");
                return;
            }

            playerList.Add(name);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Nepodařilo se přidat hráče: " + exception.Message);
        }
    }

    public static void RemovePlayer(string name)
    {
        if (!playerList.Remove(name))
        {
            Console.WriteLine("Hráč " + name + " není v turnaji.");
        }
        else
        {
            Console.WriteLine("Hráč " + name + " byl odstraněn z turnaje.");
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