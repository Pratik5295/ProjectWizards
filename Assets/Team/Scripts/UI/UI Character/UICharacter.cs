using Team.Data;
using TMPro;
using UnityEngine;

public class UICharacter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI characterNameText;


    public void PopulateCharacterUI(string _characterName, CharacterReskinData _reskinData)
    {
        characterNameText.text = _characterName;
        characterNameText.color = _reskinData.CharacterColor;
    }
}
