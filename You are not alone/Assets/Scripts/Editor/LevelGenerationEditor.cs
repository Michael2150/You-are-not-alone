using Generation;
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
            {
                //Time how long it takes to generate the level
                var startTime = Time.realtimeSinceStartup;
                Debug.Log("GeneratingLevel");
                levelGenerationScript.GenerateLevel();
                Debug.Log("Level Generated in " + (Time.realtimeSinceStartup - startTime) + " seconds");
            }

            //Button to clear the level
            if (GUILayout.Button("Clear Level"))
            {
                var startTime = Time.realtimeSinceStartup;
                Debug.Log("Clearing Level");
                levelGenerationScript.ClearLevel();
                Debug.Log("Level Cleared in " + (Time.realtimeSinceStartup - startTime) + " seconds");
            }
        }
    }
}