using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BatchRenameTool : EditorWindow
{
    [MenuItem("Aron's Tools/Batch Rename Tool 批量重命名工具")]
    public static void ShowWindow()
    {
        BatchRenameTool toolWindow = (BatchRenameTool)GetWindow(typeof(BatchRenameTool), false, "Batch Rename Tool", true);
        toolWindow.Show();
        
    }

    static string baseName = "NewName";
    private string prefix = "";
    private string suffix = "";
    //private int startIndex = 0;
    //private bool useIndex = true;
    //private bool isPrefabSkipped = true;
    //private bool isAnimSkipped = true;

    private Dictionary<string, string> suffixByType = new Dictionary<string, string>()
    {
        {".prefab", "_Pfb"},
        {".mat","_Mat" },
        { ".fbx","_Fbx"},
        {".png","_Tex" },
        {".anim","_Anim" }
    };

    private Dictionary<string, int> startIndexByType = new Dictionary<string, int>()
    {
        {".prefab", 0},
        {".mat",0 },
        { ".fbx",0},
        {".png",0 },
        {".anim",0 }
    };

    private Dictionary<string, bool> skipByType = new Dictionary<string, bool>()
    {
        {".prefab", true},
        {".anim", true }
    };


    private void OnGUI()
    {
        GUILayout.Label("Batch Rename Settings", EditorStyles.boldLabel);

        baseName = EditorGUILayout.TextField("Base Name", baseName);
        prefix = EditorGUILayout.TextField("Prefix",prefix);
        //suffix = EditorGUILayout.TextField("Suffix", suffix);
        //useIndex = EditorGUILayout.Toggle("Add Index", useIndex);
        //startIndex = EditorGUILayout.IntField("Start Index", startIndex);

        GUILayout.Space(10);

        GUILayout.Label("Customise Suffix By Types", EditorStyles.boldLabel);

        List<string> keys = new List<string>(suffixByType.Keys);
        foreach (var ext in keys)
        {
            GUILayout.BeginHorizontal();
            //GUILayout.Label(ext, GUILayout.Width(50));
            suffixByType[ext] = EditorGUILayout.TextField(ext, suffixByType[ext]);//, GUILayout.Width(100));
            startIndexByType[ext] = EditorGUILayout.IntField(startIndexByType[ext], GUILayout.Width(80));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        List<string> skipKeys = new List<string>(skipByType.Keys);
        foreach (var ext in skipKeys)
        {
            GUILayout.BeginHorizontal();
            //GUILayout.Label(ext, GUILayout.Width(50));
            skipByType[ext] = EditorGUILayout.Toggle($"Skip {ext}", skipByType[ext]);
            GUILayout.EndHorizontal();
        }


        GUILayout.Space(10);

        if (GUILayout.Button("Rename Selected Assets"))
        {
            RenameSelectedAssets();
        }

        var selectObjects = Selection.gameObjects;

        string tempPrefix = prefix.Length == 0 ? prefix : prefix + "_";

        /*
        string tempPrefix = "";
        if(prefix != null)
        {
            tempPrefix = prefix + "_";
        }
        string indexPart = (startIndex).ToString("D3");//useIndex ? (startIndex).ToString("D3") : "";
        if (selectObjects.Length != 0)
        {*/
        if (selectObjects.Length != 0)
        {
            foreach (var ext in keys)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Example: Selected: {selectObjects[0].name} --> Changed Name: {tempPrefix}{baseName}{"_" + startIndexByType[ext].ToString("D3")}{suffixByType[ext]}");
                EditorGUILayout.EndHorizontal();
            }
        }
        
        /*}*/



    }

    private void RenameSelectedAssets()
    {
        var selectedGUIDs = Selection.assetGUIDs;
        int failCount = 0;

        if(selectedGUIDs.Length == 0)
        {
            Debug.LogWarning("No assets selected!");
            return;
        }

        //Counters per file type
        Dictionary<string, int> currentIndexByType = new(startIndexByType);

        AssetDatabase.StartAssetEditing();

        try
        {
            for(int i = 0; i< selectedGUIDs.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(selectedGUIDs[i]);
                string ext = System.IO.Path.GetExtension(assetPath);

                if (!suffixByType.ContainsKey(ext))
                {
                    Debug.Log($"Skipping unsupported type: {assetPath}");
                    continue;
                }

                if (skipByType.ContainsKey(ext) && skipByType[ext])
                {
                    continue;
                }


                string folder = System.IO.Path.GetDirectoryName(assetPath);

                string tempPrefix = prefix.Length == 0? prefix : prefix + "_";
                

                suffix = suffixByType[ext];
                string indexPart = (currentIndexByType[ext]++).ToString("D3");//useIndex ? (startIndex + i).ToString("D3") : "";
                
                string newFileName = $"{tempPrefix}{baseName}{"_"+indexPart}{suffix}{ext}";

                string result = AssetDatabase.RenameAsset(assetPath, System.IO.Path.GetFileNameWithoutExtension(newFileName));

                if (!string.IsNullOrEmpty(result))
                {
                    Debug.LogError($"Failed to rename: {assetPath} -> {newFileName}, Reason: {result}");
                    failCount++;
                }
            }

            
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        Debug.Log($"Batch renaming complete. Renamed {selectedGUIDs.Length - failCount} assets");
    }
}
