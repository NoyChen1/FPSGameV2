using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;
    public int currentZombiesPerWave;

    public float SpawnDelay = 0.5f; //delay between each spawn

    public int currentWave = 0;
    public float waveCoolDown = 10f; // delay between waves

    public bool inCoolDown;
    public float coolDownCounter = 0;

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    public int zombiesKilled = 0;
    public TextMeshProUGUI ZombiesKilledUI;
    //UI
    //public TextMeshProUGUI waveOverUI;
    // public TextMeshProUGUI coolDownCounterUI;

    public int zombieIncreaser = 2;


    private void Start()
    {
        ZombiesKilledUI.text = $"Zombies YOU Killed: {GlobalRefrences.instance.zombiesKilled}";

        currentZombiesPerWave = initialZombiesPerWave;

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();
        currentWave++;

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            //Genarate random offset
            Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            //Initiates the zombie
            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            //Get Enemy script
            Enemy enemyScript = zombie.GetComponent<Enemy>();

            //track this zombie
            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(SpawnDelay);
        }
    }

    private void Update()
    {
        List<Enemy> zombiesToRemove = new List<Enemy>();
        foreach(Enemy zombie in currentZombiesAlive)
        {
            if (zombie.isDead)
            {
                GlobalRefrences.instance.zombiesKilled++;
                ZombiesKilledUI.text = $"Zombies YOU Killed: {GlobalRefrences.instance.zombiesKilled}";
                zombiesToRemove.Add(zombie);
            }
        }

        //remove all the dead zombies
        foreach (Enemy zombie in zombiesToRemove)
        {
            if (zombie.isDead)
            {
                currentZombiesAlive.Remove(zombie);
            }
        }

        //GlobalRefrences.instance.zombiesKilled += this.zombiesKilled;
        zombiesToRemove.Clear();

        if(currentZombiesAlive.Count == 0 && !inCoolDown)
        {
            //start cooldown
            StartCoroutine(StartCoolDown());
        }

        //Run the CoolDown counter on the UI
        if (inCoolDown)
        {
            coolDownCounter -= Time.deltaTime;
        }
        else
        {
            coolDownCounter = waveCoolDown;
        }
       // coolDownCounterUI.text = coolDownCounter.ToString("F0");
    }

    private IEnumerator StartCoolDown()
    {
        inCoolDown = true;
       // waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCoolDown);
        inCoolDown = false;
       // waveOverUI.gameObject.SetActive(false);

        currentZombiesPerWave *= zombieIncreaser; //first wave:5, second wave:10, thirs wave:20 ....
        StartNextWave();
    }
}
