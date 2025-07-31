using Team.Data;
using Team.GameConstants;
using TMPro;
using UnityEngine;
using DG.Tweening;


namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float BarkHideAfter = 1f;
    }
}


namespace Team.UI
{

    public class UICharacter : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI characterNameText;

        [SerializeField]
        private GameObject barkObject;

        [SerializeField]
        private TextMeshProUGUI characterBarkText;

        private Vector3 initScale;

        private bool isBarking = false;

        public void Awake()
        {
            initScale = transform.localScale;
        }



        public void PopulateCharacterUI(string _characterName, CharacterReskinData _reskinData)
        {
            characterNameText.text = _characterName;
            characterNameText.color = _reskinData.CharacterColor;
            
        }

        private void ShowBarkUI()
        {

            if (!isBarking)
            {
                isBarking = true;

                barkObject.SetActive(true);

                Invoke(nameof(HideBarkUI), MetaConstants.BarkHideAfter);

                transform.localScale = Vector3.zero;

                transform.DOScale(initScale, 0.25f);
            }




            
        }

        private void HideBarkUI()
        {
            if (isBarking)
            {
                transform.DOScale(Vector3.zero, 0.25f).OnComplete
                            (() => { barkObject.SetActive(false); isBarking = false; });
            }

        }

        public void UpdateBark(string _barkMessage)
        {
            characterBarkText.text = _barkMessage;

            ShowBarkUI();
        }
    }
}
