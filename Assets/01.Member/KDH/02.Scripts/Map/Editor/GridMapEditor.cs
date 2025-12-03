namespace Code.Map.Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GridMap))]
    public class GridMapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        
            GridMap gridMap = (GridMap)target;
        
            EditorGUILayout. Space();
        
            if (GUILayout. Button("Generate Map"))
            {
                gridMap.GenerateMap();
                EditorUtility.SetDirty(gridMap);
            }
        }
    }
#endif
}