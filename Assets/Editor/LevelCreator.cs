using UnityEditor;
using UnityEngine;

namespace Team.Tool
{

    public class LevelCreator : EditorWindow
    {
        [MenuItem("Tools/Level Creator")]
        public static void ShowWindow()
        {
            GetWindow<LevelCreator>("Level Creator").Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Build your level");

            if(GUILayout.Button("Create Label"))
            {
                Debug.Log("Cehck if the level prefab is created");
            }
        }
    }
}
