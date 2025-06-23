using UnityEngine;

public class Locked_Cards : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KillKids()
    {
        foreach(Transform child in transform)
        {

            Destroy(child.gameObject);

        }



    }


}
