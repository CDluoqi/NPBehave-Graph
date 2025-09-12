using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPBehave
{
    public class NPBehaveTreeAsset : ScriptableObject
    {
        [SerializeField]
        private string _code = "";
        public string Code => _code;
     
        public static NPBehaveTreeAsset Create(string code)
        {
            var asset = CreateInstance<NPBehaveTreeAsset>();
            asset.Initialize(code);
            return asset;
        }

        private void Initialize(string text)
        {
            Debug.Log(text);
            _code = text;
        }
    }
}
