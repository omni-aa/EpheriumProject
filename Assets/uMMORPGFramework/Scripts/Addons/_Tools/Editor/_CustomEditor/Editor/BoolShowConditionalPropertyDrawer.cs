using UnityEditor;

#if UNITY_EDITOR

namespace uMMORPG
{
    
    [CustomPropertyDrawer(typeof(BoolShowConditionalAttribute))]
    public class BoolShowConditionalPropertyDrawer : BaseShowConditionalPropertyDrawer<BoolShowConditionalAttribute>
    {
    }
}
#endif
