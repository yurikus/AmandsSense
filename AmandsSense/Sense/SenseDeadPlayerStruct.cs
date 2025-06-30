using EFT;

namespace AmandsSense.Sense;

public struct SenseDeadPlayerStruct(Player Victim, Player Aggressor)
{
    public Player victim = Victim;
    public Player aggressor = Aggressor;
}
