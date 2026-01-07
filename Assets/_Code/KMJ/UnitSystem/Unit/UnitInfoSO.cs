using UnityEngine;

namespace Code.UnitSystem
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Unit/UnitInfo", order = 0)]
    public class UnitInfoSO : ScriptableObject
    {
        public string UnitName;

        public GameObject UnitPrefab;
    }
}