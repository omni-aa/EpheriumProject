#if UNITY_EDITOR
namespace uMMORPG
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(StringShowConditionalAttribute))]
    public class StringShowConditionalPropertyDrawer : BaseShowConditionalPropertyDrawer<StringShowConditionalAttribute>{}
}
#endif
