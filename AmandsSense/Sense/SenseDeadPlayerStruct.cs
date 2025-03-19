using EFT;

namespace AmandsSense.Sense;

public struct SenseDeadPlayerStruct
{
    public Player victim;
    public Player aggressor;

    public SenseDeadPlayerStruct(Player Victim, Player Aggressor)
    {
        victim = Victim;
        aggressor = Aggressor;
    }
}
