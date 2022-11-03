using Enums;
using Generation;
using Generation.VazGriz_Generation_Scripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BlockGenData))]
    public class BlockGenEditor : UnityEditor.Editor
    {
        private BlockGenData data;
        private CellState replaceState = CellState.None;
        private CellState newState = CellState.Any;
        
        private void OnEnable()
        {
            data = (BlockGenData) target;
        }
     
        public override void OnInspectorGUI()
        {
            data.Probability = EditorGUILayout.Slider("Spawn Probability", data.Probability, 0, 1);
            EditorGUILayout.LabelField("");
            
            EditorGUILayout.LabelField("Set the allowed neighbors for this block to be generated (Room and hallway are the same thing)");
            GUILayout.BeginHorizontal();
            GUILayout.Label("(-1, -1)");
            data.Neighbours[0,0] = (CellState) EditorGUILayout.EnumPopup(data.Neighbours[0,0]);
            GUILayout.Label("(0, -1)");
            data.Neighbours[1,0] = (CellState) EditorGUILayout.EnumPopup(data.Neighbours[1,0]);
            GUILayout.Label("(1, -1)");
            data.Neighbours[2,0] = (CellState) EditorGUILayout.EnumPopup(data.Neighbours[2,0]);
            GUILayout.EndHorizontal();
         
            GUILayout.BeginHorizontal();
            GUILayout.Label("(-1, 0)");
            data.Neighbours[0,1] = (CellState) EditorGUILayout.EnumPopup(data.Neighbours[0,1]);
            GUILayout.Label("(0, 0)");
            EditorGUILayout.ObjectField(data.gameObject, typeof(GameObject), true);
            GUILayout.Label("(1, 0)");
            data.Neighbours[2,1] = (CellState) EditorGUILayout.EnumPopup(data.Neighbours[2,1]);
            GUILayout.EndHorizontal();
         
            GUILayout.BeginHorizontal();
            GUILayout.Label("(-1, 1)");
            data.Neighbours[0,2] = (CellState) EditorGUILayout.EnumPopup(data.Neighbours[0,2]);
            GUILayout.Label("(0, 1)");
            data.Neighbours[1,2] = (CellState) EditorGUILayout.EnumPopup(data.Neighbours[1,2]);
            GUILayout.Label("(1, 1)");
            data.Neighbours[2,2] = (CellState) EditorGUILayout.EnumPopup(data.Neighbours[2,2]);
            GUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField("");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Replace"))
            {
                for (var x = 0; x < data.Neighbours.Size.x; x++)
                {
                    for (var y = 0; y < data.Neighbours.Size.y; y++)
                    {
                        if (data.Neighbours[x, y] == replaceState)
                            data.Neighbours[x, y] = newState;
                    }
                }
            }
            replaceState = (CellState) EditorGUILayout.EnumPopup(replaceState);
            GUILayout.Label(" with ");
            newState = (CellState) EditorGUILayout.EnumPopup(newState);
            if (GUILayout.Button("Swap"))
                (replaceState, newState) = (newState, replaceState);
            GUILayout.EndHorizontal();
        }
    }
}