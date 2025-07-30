using System;
using Team.Managers;
using TMPro;
using UnityEngine;

public class CharacterNumberHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI character_Number_Text;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GameTurnManager.Instance.OnCharacterNumberVisibilityChanged += ToggleNumberActive;
        ToggleNumberActive(false);
    }

    private void OnDisable()
    {
        GameTurnManager.Instance.OnCharacterNumberVisibilityChanged -= ToggleNumberActive;
        
    }

    // Update is called once per frame


    public void UpdateCharacterNumberText(int characterNumber)
    {
        character_Number_Text.text = characterNumber.ToString();
    }

    private void ToggleNumberActive(bool isActive)
    {

        character_Number_Text.gameObject.SetActive(isActive);


    }






}
