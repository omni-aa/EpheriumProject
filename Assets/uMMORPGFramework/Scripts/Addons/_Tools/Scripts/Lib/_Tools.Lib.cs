#if UNITY_EDITOR && _iMMOTOOLS
namespace uMMORPG
{
    public partial class DefinesManager
    {
        public static void Constructor_Tools()
        {
            AddOn addon = new()
            {
                name = "Tools",
                basis = "uMMORPG3d Remastered",
                define = "_iMMOTOOLS",
                author = "Trugord",
                version = "",
                dependencies = "none",
                comments = "none",
                active = true
            };
            addons.Add(addon);
            DefineSymbols.AddScriptingDefine(addon.define); // mandatory
#if !_MYSQL && !_SQLITE
            DefineSymbols.AddScriptingDefine("_SQLITE"); // mandatory
#endif
        }
    }
    
}
#endif
