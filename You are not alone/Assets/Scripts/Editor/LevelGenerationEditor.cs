using UnityEngine;
using UnityEditor;


namespace Editor
{
    [CustomEditor(typeof(LevelGenerationScript))]
    public class LevelGenerationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var levelGenerationScript = (LevelGenerationScript) target;
            
            //Button to generate the level
            if (GUILayout.Button("Generate Level"))
                levelGenerationScript.GenerateLevel();
            
            //Button to clear the level
            if (GUILayout.Button("Clear Level"))
                levelGenerationScript.ClearLevel();
        }
    }
}