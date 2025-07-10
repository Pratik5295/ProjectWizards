using System;
using System.Collections.Generic;
using Team.Gameplay.Tutorial;
using UnityEngine;

namespace Team.Managers
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance = null;

        public List<TutorialStep> tutorialSteps = new List<TutorialStep> ();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
