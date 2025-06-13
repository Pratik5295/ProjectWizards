using UnityEditor;
using UnityEngine;

namespace Team.Tool
{

    public class LevelCreatorWindow : EditorWindow
    {
        private GameObject tilePrefab = null;
        [MenuItem("Tools/Level Creator")]
        public static void ShowWindow()
        {
            GetWindow<LevelCreatorWindow>("Level Creator").Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Build your level");

            //Tile/Grid Creator
            GUILayout.Label("Tile Creator Section");

            tilePrefab = (GameObject)EditorGUILayout.ObjectField(tilePrefab, typeof(GameObject),false);

            if(GUILayout.Button("Create Level"))
            {
                Debug.Log($"Check if the level prefab is created: {tilePrefab.name}");
            }
        }
    }
}
