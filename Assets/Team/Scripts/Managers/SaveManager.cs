using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static Team.GameConstants.LevelConstants;

namespace Team.Managers
{
    [System.Serializable]
    public class SaveData
    {
        public List<LevelID> CompletedLevels;
    }

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        public List<LevelID> CompletedLevels = new List<LevelID>();

        [SerializeField]
        private SaveData localSaveData;

        private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            localSaveData = new SaveData();
            DontDestroyOnLoad(gameObject);
            Load();
        }

        [ContextMenu("Save Completed Levels")]
        public void Save()
        {
            localSaveData.CompletedLevels = CompletedLevels = new List<LevelID>(CompletedLevels);

            string json = JsonUtility.ToJson(localSaveData, true);
            File.WriteAllText(SavePath, json);
            Debug.Log("Game saved.");
        }

        [ContextMenu("Load Completed Levels")]
        public void Load()
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                CompletedLevels = data.CompletedLevels ?? new List<LevelID>();
                Debug.Log("Game loaded.");
            }
            else
            {
                Debug.Log("No save file found. Starting fresh.");
                CompletedLevels = new List<LevelID>();
            }
        }

        public void Delete()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted.");
            }
            CompletedLevels.Clear();
        }
    }
}
