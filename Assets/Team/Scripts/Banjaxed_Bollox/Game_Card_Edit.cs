using UnityEngine;
using UnityEngine.UI;

public class Game_Card_Edit : MonoBehaviour
{

    private Image card_Image;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        card_Image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {

        card_Image.color = Color.gray;

    }
}
