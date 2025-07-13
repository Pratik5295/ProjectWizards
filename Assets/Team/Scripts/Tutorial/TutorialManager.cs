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

        private TutorialStep currentStep = null;

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
            ClearTutorialSteps();

            tutorialSteps = new List<TutorialData>(_steps);
        }

        public void ClearTutorialSteps()
        {
            if (tutorialSteps != null)
            {
                tutorialSteps.Clear();
            }
        }

        public void StartTutorial()
        {
            currentIndex = 0;
            var step = tutorialSteps[0];

            currentStep = GetElementByType(step.uiElement);

            currentStep.gameObject.SetActive(true);
        }

        public void GoToNextStep()
        {
            currentStep.gameObject.SetActive(false);
            currentIndex++;

            if (currentIndex >= tutorialSteps.Count)
            {
                Debug.Log("Tutorial has ended");
                UIManager.Instance.OnTutorialCompleted();

                ClearTutorialSteps();
            }
            else
            {
                var step = tutorialSteps[currentIndex];

                currentStep = GetElementByType(step.uiElement);

                currentStep.gameObject.SetActive(true);
            }
        }

        private TutorialStep GetElementByType(TutUIElement _type)
        {
            return tutorialsUIMap[_type];
        }
        
    }
}
