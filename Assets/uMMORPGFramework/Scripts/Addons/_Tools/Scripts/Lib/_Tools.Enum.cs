namespace uMMORPG
{
    [System.Flags]
    public enum EntityType
    {
        Player = 1 << 0,
        Npc = 1 << 1,
        Monster = 1 << 2, 
        Pet = 1 << 3, 
        Mount = 1 << 4
    }
    
    public enum AlignmentState
    {
        LAWFUL_GOOD,
        NEUTRAL_GOOD,
        CHAOTIC_GOOD,
        LAWFUL_NEUTRAL,
        TRUE_NEUTRAL,
        CHAOTIC_NEUTRAL,
        LAWFUL_EVIL,
        NEUTRAL_EVIL,
        CHAOTIC_EVIL
    }
    
    public enum TeleportationType { onScene, offScene }
    
    public enum BuffType { Both, Buff, Nerf, None }
    
    public enum ThresholdType { None, Below, Above }
    
    public enum GroupType { None, Party, Guild, Realm }
    
}
