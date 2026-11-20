public class Player
{
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

    public string Nickname { get; }
    public string Name { get; }
}
