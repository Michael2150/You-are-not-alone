using Generation;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LevelBuilderScript))]
    public class LevelBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var levelBuilder = (LevelBuilderScript) target;

            if (GUILayout.Button("Build Level"))
                levelBuilder.BuildLevel();

            if (GUILayout.Button("Clear Level"))
                levelBuilder.DestroyLevel();
        }
    }
}