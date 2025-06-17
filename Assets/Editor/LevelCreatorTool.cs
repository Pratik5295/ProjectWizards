using System.Collections.Generic;
using Team.Data;
using Team.Gameplay.Characters;
using Team.Gameplay.GameLevelSystem;
using Team.Gameplay.GridSystem;
using Team.Gameplay.ObjectiveSystem;
using UnityEditor;
using UnityEngine;

namespace Team.Tool
{

    public class LevelCreatorTool : EditorWindow
    {
        private GameObject root;
        private GameObject charactersRoot;  //Root game object for visualizing characters
        private GameLevel GameLevel => root.GetComponent<GameLevel>();


        private LevelTileCreator tileCreator;

        private string levelId;


        private GameObject tilePrefab = null;
        private Vector2 tileSize = Vector2.one; // Default to (1,1)

        //Characters
        private List<CharacterData> characterList = new List<CharacterData>();

        //Objectives
        private List<GameObjectiveData> objectiveList = new List<GameObjectiveData>();

        private bool showCharacters = false;

        private Vector2 scrollPos;
        private Vector2 objectiveScrollPos;


        [MenuItem("Tools/Level Creator")]
        public static void ShowWindow()
        {
            GetWindow<LevelCreatorTool>("Level Creator").Show();
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

                if(characterList.Count > 0)
                {
                    //Visualize characters button
                    CharacterVisualizerButton();
                }

                if (characterList.Count > 0 && objectiveList.Count > 0)
                {
                    

                    //Save to prefab button
                    if (GUILayout.Button("Save to Prefab"))
                    {
                        //Save data to the root level for characters and objectives
                        OnSaveButton();
                        Debug.Log("Game prefab will be saved at this location");
                    }
                }
            }

            if(GUILayout.Button("Reset Tool Data"))
            {
                ResetAllValues();
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

        #region Character Map & Visualizing Section

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


                // RESKIN SECTION
                GUILayout.Label("Character Reskin Data", EditorStyles.miniBoldLabel);
                if (character.CharacterSkin == null)
                {
                    character.CharacterSkin = new CharacterReskinData(); // Ensure it's not null
                }

                character.CharacterSkin.CharacterCode = (CharacterColorCode)EditorGUILayout.EnumPopup("Character Code", character.CharacterSkin.CharacterCode);
                character.CharacterSkin.SkinMaterial = (Material)EditorGUILayout.ObjectField("Skin Material", character.CharacterSkin.SkinMaterial, typeof(Material), false);
                character.CharacterSkin.CharacterColor = EditorGUILayout.ColorField("Character Color", character.CharacterSkin.CharacterColor);
                character.CharacterSkin.CharacterColor.a = 1f;

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

        private void CharacterVisualizerButton()
        {
            if (showCharacters)
            {
                //Hide character buttons
                if(GUILayout.Button("Hide Characters"))
                {
                    showCharacters = false;
                    DestroyImmediate(charactersRoot);
                }
            }
            else
            {
                //Show character button
                if(GUILayout.Button("Show Characters"))
                {
                    showCharacters = true;

                    OnVisualizeCharacters();
                }
            }
        }

        private void OnVisualizeCharacters()
        {
            if (characterList.Count == 0) return;

            charactersRoot = new GameObject("Characters");
            charactersRoot.transform.position = Vector3.zero;

            foreach (var character in characterList)
            {
                var characterObject = Instantiate(character.CharacterPrefab);
                characterObject.name = $"{character.CharacterID}";
                characterObject.transform.SetParent(charactersRoot.transform);

                TileID tileID = new TileID((int)character.StartTileID.x, (int)character.StartTileID.y);

                var baseCharacterRef = characterObject.GetComponent<Base_Ch>();

                //Set Position
                var tile = tileCreator.GetTile(tileID);
                characterObject.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + baseCharacterRef.YSpawnOffset, tile.transform.position.z);

                //Set Rotation
                characterObject.transform.eulerAngles = RotateToFaceDir(character.FacingDirection);

                //Skin the character
                if (characterObject.TryGetComponent<CharacterReskinner>(out var characterReskinner))
                {
                    characterReskinner.ToolSetCharacterSkin(character.CharacterSkin);

                    characterReskinner.UICharacter.PopulateCharacterUI(character.CharacterID, character.CharacterSkin);
                }

            }
        }

        public Vector3 RotateToFaceDir(Enum_GridDirection dir)
        {
            switch (dir)
            {
                case Enum_GridDirection.EAST:
                    return new Vector3(0, 90, 0);

                case Enum_GridDirection.WEST:
                    return new Vector3(0, 270, 0);

                case Enum_GridDirection.NORTH:
                    return new Vector3(0, 0, 0);

                case Enum_GridDirection.SOUTH:
                    return new Vector3(0, 180, 0);
            }

            return Vector3.zero;
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

        #region Save & Data Section
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

                ResetAllValues();
            }
            else
            {
                Debug.LogError("Failed to save prefab.");
            }
        }

        private void ResetAllValues()
        {
            characterList.Clear();
            objectiveList.Clear();

            DestroyImmediate(root);
            DestroyImmediate(charactersRoot);

            tileSize = Vector2.one;
            levelId = string.Empty;

            tileCreator = null;
            root = null;

            showCharacters = false;
        }

        #endregion
    }
}
