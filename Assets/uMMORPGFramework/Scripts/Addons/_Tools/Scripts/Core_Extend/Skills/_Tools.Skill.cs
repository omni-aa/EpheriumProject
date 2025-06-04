namespace uMMORPG
{
    public partial struct Skill
    {
    
    #if _iMMOSTAMINA
        public int staminaCosts => data.staminaCosts.Get(level);
    #endif
    
        // wont show up in the skill window for learning/upgrade - only works with skill window addon
        public bool unlearnable { get { return data.unlearnable; } }
    
        // is considered to be a negative status effect and can be removed by certain skills
        public bool disadvantageous { get { return data.disadvantageous; } }
    
    }
    
}
