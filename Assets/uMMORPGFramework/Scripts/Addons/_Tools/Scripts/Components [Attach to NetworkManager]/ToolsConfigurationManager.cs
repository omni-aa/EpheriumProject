using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    public partial class ToolsConfigurationManager : MonoBehaviour
    {
        [Header("Configuration")]
        public TemplateConfiguration configTemplate;
    
        //[Header("Defines")]
        //public TemplateDefines addonTemplate;
    
        [Header("Game Rules")]
        public TemplateGameRules rulesTemplate;
    }
    
}
#endif
