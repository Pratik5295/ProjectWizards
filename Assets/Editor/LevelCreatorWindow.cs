using System.Collections.Generic;
using Team.Data;
using Team.Gameplay.GameLevelSystem;
using Team.Gameplay.GridSystem;
using Team.Gameplay.ObjectiveSystem;
using UnityEditor;
using UnityEngine;

namespace Team.Tool
{

    public class LevelCreatorWindow : EditorWindow
    {
        private GameObject root;
        private GameLevel GameLevel => root.GetComponent<GameLevel>();


        private LevelTileCreator tileCreator;

        private string levelId;


        private GameObject tilePrefab = null;
        private Vector2 tileSize = Vector2.one; // Default to (1,1)

        //Characters
        private List<CharacterData> characterList = new List<CharacterData>();

        //Objectives
        private List<GameObjectiveData> objectiveList = new List<GameObjectiveData>();
        

        private Vector2 scrollPos;
        private Vector2 objectiveScrollPos;


        [MenuItem("Tools/Level Creator")]
        public static void ShowWindow()
        {
            GetWindow<LevelCreatorWindow>("Level Creator").Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Build your level", EditorStyles.boldLabel);

            GUILayout.Label("Enter level Identifier");
            levelId = EditorGUILayout.TextField(levelId);

            GUILayout.Space(5);

            //Tile/Grid Creator
            GUILayout.Label("Tile Creator Section", EditorStyles.boldLabel);

            GUILayout.Label("Set Default Tile Prefab");
            tilePrefab = (GameObject)EditorGUILayout.ObjectField(tilePrefab, typeof(GameObject),false);

            GUILayout.Label("Enter tile size");
            tileSize = EditorGUILayout.Vector2Field("Tile Size", tileSize);


            if (GUILayout.Button("Create Level Tiles"))
            {
                Debug.Log($"Check if the level prefab is created: {tilePrefab.name}");

                CreateRootObject();

               
            }

            if(root != null)
            {
                FillCharacterMapData();
                FillObjectiveMapData();

                if (characterList.Count > 0 && objectiveList.Count > 0)
                {

                    if (GUILayout.Button("Save to Prefab"))
                    {
                        //Save data to the root level for characters and objectives
                        OnSaveButton();
                        Debug.Log("Game prefab will be saved at this location");
                    }
                }
            }

            
        }

        /// <summary>
        /// Creates the root Game Level Object with Game Level component attached to it
        /// </summary>
        private void CreateRootObject()
        {
            if(root != null)
            {
                DestroyImmediate(root);
            }

            root = new GameObject($"Game Level {levelId}");
            root.transform.position = Vector3.zero;
            root.AddComponent<GameLevel>();

            CreateTileCreator();
        }


        /// <summary>
        /// Creates the tile creator to the root object and attach Level Tile Creator script to it
        /// Set reference on root for the Level Tiles we create
        /// </summary>
        private void CreateTileCreator()
        {
            var levelTileCreator = new GameObject("Tile Creator");
            levelTileCreator.transform.SetParent(root.transform);

            tileCreator = levelTileCreator.AddComponent<LevelTileCreator>();

            GameLevel.LevelTiles = tileCreator;

            TileCreatorInit();

            CreateLevelTiles();
        }

        /// <summary>
        /// Set the default tile prefab on the Level Tile Creator object
        /// </summary>
        private void TileCreatorInit()
        {
            tileCreator.SetDefaultTile(tilePrefab);
            tileCreator.SetGridSize(tileSize);
        }

        /// <summary>
        /// Creates the level tiles
        /// </summary>
        private void CreateLevelTiles()
        { 
            tileCreator.CreateGrid();
        }

        #region Character Map Section

        private void FillCharacterMapData()
        {
            GUILayout.Label("Character Data List Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Add New Character"))
            {
                characterList.Add(new CharacterData());
            }

            GUILayout.Space(10);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Iterate and draw each CharacterData in the list
            for (int i = 0; i < characterList.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label($"Character #{i + 1}", EditorStyles.boldLabel);

                var character = characterList[i];

                character.CharacterID = EditorGUILayout.TextField("Character ID", character.CharacterID);
                character.CharacterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", character.CharacterPrefab, typeof(GameObject), false);

                // TileID struct input: integer fields for x and y
                GUILayout.Label("Start Tile ID");
                character.StartTileID.x = EditorGUILayout.IntField("  X", character.StartTileID.x);
                character.StartTileID.y = EditorGUILayout.IntField("  Y", character.StartTileID.y);

                character.FacingDirection = (Enum_GridDirection)EditorGUILayout.EnumPopup("Facing Direction", character.FacingDirection);
                character.UICardPrefab = (GameObject)EditorGUILayout.ObjectField("UI Card Prefab", character.UICardPrefab, typeof(GameObject), false);

                if (GUILayout.Button("Remove Character"))
                {
                    characterList.RemoveAt(i);
                    i--; // Adjust index since we removed an element
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
        }

        #endregion

        #region Objective Map Section

        private void FillObjectiveMapData()
        {
            GUILayout.Label("Game Objective Data List Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Add New Objective"))
            {
                objectiveList.Add(new GameObjectiveData()
                {
                    ObjectiveTargets = new List<string>() // initialize list to avoid null refs
                });
            }

            GUILayout.Space(10);

            objectiveScrollPos = EditorGUILayout.BeginScrollView(objectiveScrollPos);

            for (int i = 0; i < objectiveList.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label($"Objective #{i + 1}", EditorStyles.boldLabel);

                var objective = objectiveList[i];

                objective.ObjectiveName = EditorGUILayout.TextField("Objective Name", objective.ObjectiveName);

                // Edit list of strings for ObjectiveTargets
                GUILayout.Label("Objective Targets:");
                if (objective.ObjectiveTargets == null)
                    objective.ObjectiveTargets = new List<string>();

                // Show each target with text field and remove button
                for (int t = 0; t < objective.ObjectiveTargets.Count; t++)
                {
                    EditorGUILayout.BeginHorizontal();
                    objective.ObjectiveTargets[t] = EditorGUILayout.TextField($"Target {t + 1}", objective.ObjectiveTargets[t]);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        objective.ObjectiveTargets.RemoveAt(t);
                        t--;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Target"))
                {
                    objective.ObjectiveTargets.Add("");
                }

                // Objective Type enum
                objective.Type = (ObjectiveType)EditorGUILayout.EnumPopup("Objective Type", objective.Type);

                // Show LocationTileID only if Type == LOCATION
                if (objective.Type == ObjectiveType.LOCATION)
                {
                    GUILayout.Label("Location Tile ID");
                    objective.LocationTileID.x = EditorGUILayout.IntField("  X", objective.LocationTileID.x);
                    objective.LocationTileID.y = EditorGUILayout.IntField("  Y", objective.LocationTileID.y);
                }

                if (GUILayout.Button("Remove Objective"))
                {
                    objectiveList.RemoveAt(i);
                    i--;
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
        }

        #endregion


        /// <summary>
        /// Save Button Logic Handling
        /// </summary>
        private void OnSaveButton()
        {
            //Populate Character Map in Game Level based on local variable Character List
            GameLevel.LoadChaacterMap(characterList);
            GameLevel.LoadObjectiveMap(objectiveList);

            // 2. Prepare the directory path to save prefab
            string prefabDirectory = "Assets/Team/Game Levels";
            if (!AssetDatabase.IsValidFolder(prefabDirectory))
            {
                // Create folder if it does not exist
                AssetDatabase.CreateFolder("Assets/Team", "Game Levels");
            }

            // 3. Generate prefab path using levelId
            string prefabPath = $"{prefabDirectory}/GameLevel_{levelId}.prefab";

            // 4. Save the root GameObject as prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

            if (prefab != null)
            {
                Debug.Log($"Prefab saved successfully at: {prefabPath}");
                // Optionally, focus the Project window and select the new prefab asset
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = prefab;
            }
            else
            {
                Debug.LogError("Failed to save prefab.");
            }
        }
    }
}
