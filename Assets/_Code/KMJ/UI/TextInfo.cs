using System;
using UnityEngine;

namespace Code.UI
{
    [CreateAssetMenu(fileName = "TextInfo", menuName = "TextInfo", order = 0)]
    public class TextInfo : ScriptableObject
    {
        public string textName;
        [ColorUsage(true, true)] public Color textColor;

        public int nameHash;
        public float fontSize;

        private void OnValidate()
        {
            nameHash = Animator.StringToHash(textName);
        }
    }
}