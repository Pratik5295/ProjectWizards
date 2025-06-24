using Team.Managers;
using Team.UI.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reload_Level : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {

    }


    public void LoadLevel()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(activeScene.name);

    }

    public void LoadGameLevel()
    {

        LevelManager.Instance.LoadCurrentLevel();



    }

    public void ResetGameCardTracker()
    {

        UIGameCard.tracker = 0;

    }


}
