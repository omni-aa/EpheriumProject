using System;
using UnityEngine;

namespace uMMORPG
{
    
    [Serializable]
    public partial class AddOn
    {
        [HideInInspector] public string name;
        [HideInInspector] public string define;
        [ReadOnly] public string basis;
        [ReadOnly] public string author;
        [ReadOnly] public string version;
        [ReadOnly] public string dependencies;
        [ReadOnly][TextArea(1, 30)] public string comments;
        public bool active;
    
        public void Copy(AddOn addon)
        {
            name = addon.name;
            define = addon.define;
            basis = addon.basis;
            author = addon.author;
            version = addon.version;
            dependencies = addon.dependencies;
            comments = addon.comments;
            active = addon.active;
        }
    
    }
    
}
