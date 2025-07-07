using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static Team.GameConstants.LevelConstants;
using System.Linq;

namespace Team.Managers
{
    [System.Serializable]
    public class SavePacket
    {
        public ChapterID ChapterID;
        public List<LevelID> CompletedLevels;

        public SavePacket()
        {
            CompletedLevels = new List<LevelID>();
        }
    }

    [System.Serializable]
    public class SaveHolder
    {
        public List<SavePacket> dataPackets;

        public SaveHolder()
        {
            dataPackets = new List<SavePacket>();
        }

        //Check and get the relevant chapter id if it exists
        public SavePacket GetPacketWithChapterID(ChapterID chapterID)
        {
           return dataPackets.FirstOrDefault(chapter =>  chapter.ChapterID == chapterID);
        }

        public SavePacket CreateDataPacket(ChapterID chapterID)
        {
            SavePacket packet = new SavePacket() { ChapterID = chapterID };
            dataPackets.Add(packet);

            return packet;
        }

        public void AddOrUpdateCompletedLevelList(SavePacket _data, LevelID _levelID)
        {
            if (!_data.CompletedLevels.Contains(_levelID))
            {
                _data.CompletedLevels.Add(_levelID);
            }
        }
    }

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [SerializeField]
        private SaveHolder localSaveData;

        private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
        }

        private void Start()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            localSaveData = new SaveHolder();
            Load();
        }

        #region Main Functions Section

        [ContextMenu("Save Completed Levels")]
        public void Save()
        {
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
                localSaveData = JsonUtility.FromJson<SaveHolder>(json);
                Debug.Log("Game loaded.");
            }
            else
            {
                Debug.Log("No save file found. Starting fresh.");
            }
        }

        public void Delete()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted.");
            }
        }

        #endregion

        #region Save Packet Handling Section

        public void UpdateLevelCompletedOnChapter(ChapterID _chapterID, LevelID _levelID)
        {
            //Check if the Chapter Packet exists in the local cache
            var savePacket = localSaveData.GetPacketWithChapterID(_chapterID);

            if(savePacket == null)
            {
                //Create a chapter packet if the id doesnt exist
                savePacket = localSaveData.CreateDataPacket(_chapterID);
            }

            localSaveData.AddOrUpdateCompletedLevelList(savePacket,_levelID);
        }

        #endregion


    }
}
