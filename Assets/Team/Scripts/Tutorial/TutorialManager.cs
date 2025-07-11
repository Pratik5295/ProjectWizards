using System;
using System.Collections.Generic;
using Team.Gameplay.Tutorial;
using UnityEngine;
using static Team.GameConstants.MetaConstants;

namespace Team.Managers
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance = null;

        public List<TutorialData> tutorialSteps; //The tutorial steps to be loaded

        public List<TutorialStep> uiTutorialList = new List<TutorialStep>();

        public Dictionary<TutUIElement, TutorialStep> tutorialsUIMap = new Dictionary<TutUIElement, TutorialStep>();

        private int currentIndex = 0;

        public TutorialManager InitializeTutorialsUI()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            tutorialsUIMap.Clear();
            foreach (var uiElement in uiTutorialList)
            {
                tutorialsUIMap.Add(uiElement.ElementID, uiElement);
            }

            Debug.Log($"[TutMan] Initialize Complete with Element Count: {tutorialsUIMap.Count}");
            return Instance;
        }

        public void LoadTutorialSteps(List<TutorialData> _steps)
        {
            if (tutorialSteps != null)
            {
                tutorialSteps.Clear();
            }

            tutorialSteps = new List<TutorialData>(_steps);
        }

        public void GoToNextStep()
        {

        }

        
    }
}
