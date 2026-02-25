using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

/*
This script uses a queue system to spawn the Health Unit UI, the core idea being first, we check the difference between the the new and old health. then we set that that amount
to queue, after that, we spawn/despawn those healths based on a timer (HandleSpawnTimer)
*/
public class PlayerStat : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;
    [SerializeField] int currentHealth;

    [SerializeField] GameObject healthUnit;
    [SerializeField] RectTransform initialHealthUnitSpawnPoint;
    [SerializeField] float nextHealthUnitSpawnDist = 1f;
    [SerializeField] float spawnDelay = 0.1f;

    [SerializeField] private HealthPopup healthPopup;



    float timer = 0f;
    int unitsToSpawn = 0;
    int unitsToDespawn = 0;

    [SerializeField] Image HealthEffect;



    [SerializeField] AudioClip healthSpawnSFX;
    [SerializeField] AudioClip playerDamageSFX;
    [SerializeField] AudioClip playerDeathSFX;
    AudioSource audioSource;
    


    List<GameObject> spawnedHealthUnits = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();


        currentHealth = maxHealth;
        unitsToSpawn = currentHealth;

        HealthEffect.color = new Color(1f, 1f, 1f, 0f);
    }


    void Update()
    {
        HandleSpawnTimer(); // based on the difference in Unit to spawn/despawn, it automatically spawns/despawns
    }

    // if you want to modify player health, these are the 3 function you need to interact with HealHealth, ReduceHealth and SetMaxHealth
    public void HealHealth(int amount)
    {
        int previousHealth = currentHealth;

        if((maxHealth - currentHealth) > amount)
            currentHealth+= amount;
        else
            currentHealth = maxHealth;
        
        // First we see how many health points gained then we spwan health; 
        int gained = currentHealth - previousHealth;
        unitsToSpawn += gained;
        FlashEffect(Color.green);
        if (gained > 0) healthPopup?.ShowHeal();

    }
    public void ReduceHealth(int amount)
    {
        int previousHealth = currentHealth;

        if(currentHealth >= amount)
            currentHealth-=amount;
        else
            currentHealth = 0;
        
        // First we see how many health points lost then we destroy health; 
        int lost = previousHealth - currentHealth;

        unitsToDespawn+= lost;
        FlashEffect(Color.red);
        if (lost > 0) healthPopup?.ShowDamage();

        // check for death
        if (currentHealth <= 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        //TODO: Another dev (TWAS DEV 5!) prolly gotta hook this up to some sort of Game Manager to handle respawns or resets
        PlayerRagdoll.Instance.ActivateRagdoll(); //This also needs to have its camera fixed.. unless we like it LOL
        audioSource.PlayOneShot(playerDeathSFX);

        // me likey the way it FLIES LMAOOO
        StartCoroutine(ShowUI());

    }
    public IEnumerator ShowUI(){
        yield return new WaitForSeconds(2f);
        GameOverUI.Instance?.Show();

    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        FlashEffect(Color.yellow);

        if(currentHealth < maxHealth)
        {
            int difference = maxHealth - currentHealth;
            unitsToSpawn += difference;
            currentHealth = maxHealth;
        }
        else if (currentHealth > maxHealth)
        {
            int difference = currentHealth - maxHealth;
            unitsToDespawn += difference;
            currentHealth = maxHealth;
        }
    }

    private void FlashEffect(Color color)
    {
        HealthEffect.DOKill();

        Sequence seq = DOTween.Sequence();

        color.a = 0f;
        HealthEffect.color = color;
        seq.Append(HealthEffect.DOFade(0.5f, 0.15f));
        seq.AppendInterval(0.2f);
        seq.Append(HealthEffect.DOFade(0f, 0.4f));

        seq.SetLink(gameObject);
    }

    void HandleSpawnTimer() // This function handles the spawning and the despawning based on timer
    {
        if (unitsToSpawn == 0 && unitsToDespawn == 0) 
            return;
        
        timer += Time.deltaTime;

        if(timer >= spawnDelay)
        {
            timer = 0f;

            if(unitsToDespawn > 0)
            {
                DespawnLastHealthUnit();
                unitsToDespawn--;
            }
            else if (unitsToSpawn > 0)
            {
                SpawnHealthUnit(spawnedHealthUnits.Count);
                unitsToSpawn--;
            }
        }
    }

    void SpawnHealthUnit(int index)
    {
        Vector2 spawnPosition = new Vector2(initialHealthUnitSpawnPoint.localPosition.x + (nextHealthUnitSpawnDist * index), initialHealthUnitSpawnPoint.localPosition.y); 
        // spawning the health unit side by side using a distance
        GameObject spawnedHealthUnit = Instantiate(healthUnit, spawnPosition, Quaternion.identity);
        spawnedHealthUnit.transform.SetParent(initialHealthUnitSpawnPoint.parent, false);
        spawnedHealthUnit.transform.localScale = Vector3.one;

        spawnedHealthUnits.Add(spawnedHealthUnit);
        audioSource.PlayOneShot(healthSpawnSFX);

    }

    private void DespawnLastHealthUnit()
    {
        if (spawnedHealthUnits.Count == 0)
        {
            Debug.LogError("You already ded boi");
            return;
        }

        int lastIndex = spawnedHealthUnits.Count - 1;
        spawnedHealthUnits[lastIndex].GetComponent<HealthUnit>().DestroyObject();
        spawnedHealthUnits.RemoveAt(lastIndex);
        audioSource.PlayOneShot(playerDamageSFX);
    }
}
