using UnityEditor;

#if UNITY_EDITOR

namespace uMMORPG
{
    [CustomPropertyDrawer(typeof(IntShowConditionalAttribute))]
    public class IntShowConditionalPropertyDrawer : BaseShowConditionalPropertyDrawer<IntShowConditionalAttribute>
    {
    }
}
#endif
