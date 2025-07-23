using Team.Managers;
using UnityEngine;

public class ToggleNumbersButton : MonoBehaviour
{


    public void ToggleCharacterNumbers()
    {
        bool toggle = !GameTurnManager.Instance._areCharNumbersActive;


        GameTurnManager.Instance.ToggleCharacterNumberVisibility(toggle);
    }





}
