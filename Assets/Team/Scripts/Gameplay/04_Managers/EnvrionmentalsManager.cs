using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class EnvrionmentalsManager : MonoBehaviour
{
    public static EnvrionmentalsManager Instance;

    
    [SerializeField] private GameObject Rocket;

    [SerializeField] private GameObject[] QuadraticCurves;

    [SerializeField] private float minDelay = 2f;

    [SerializeField] private float maxDelay = 10f;

    private System.Random rng = new System.Random();
    public bool isRunning = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void ExecuteRocketsSpawnLoop()
    {
        if (isRunning) return;
        else
        {
            isRunning = true;
            _ = RocketsSpawnLoopAsync();
        }
    }

    private async Task RocketsSpawnLoopAsync()
    {
        while (isRunning)
        {
            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            await Task.Delay(TimeSpan.FromSeconds(delay));

            int spawnChance = rng.Next(0, 100);
            if(spawnChance < 40)
            {
                SpawnRocket();
            }
        }
    }

    void SpawnRocket()
    {
        isRunning = false;

        int RandomChoice = UnityEngine.Random.Range(0, QuadraticCurves.Length);
        GameObject RandomCurve = Instantiate(QuadraticCurves[RandomChoice], QuadraticCurves[RandomChoice].transform.position, Quaternion.identity, transform.parent);

        GameObject RocketInstance = Instantiate(Rocket, Vector3.zero, Quaternion.identity, transform);
        RocketInstance.GetComponent<Racoon>().curve = RandomCurve.GetComponent<QuadraticCurve>();
    }

    private void OnDestroy()
    {
        isRunning = false;
    }
}
