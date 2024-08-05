using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    string nextSceneName = "MainMenuScene";

    [Header("Player's details")]
    public int HP = 100;
    private PlayerState state;


    [Header("UI")]
    public TextMeshProUGUI playerHealthUI;
    public TextMeshProUGUI zombiesKilledUI;
    public GameObject crossHairUI;
    public GameObject GameOverUI;
    public GameObject bloodyScreen;

    [Header("Components")]
    public MouseMovement mouseMovement; 
    public PlayerMovement playerMovement;
    public Animator animator;
    public ScreenFader screenFader;

    //public bool isDead;


    private enum PlayerState
    {
        None,
        Dead
    }

    private void Start()
    {
        playerHealthUI.text = $"Health: {HP}"; 
        
        mouseMovement = GetComponent<MouseMovement>();
        playerMovement = GetComponent<PlayerMovement>();
        screenFader = GetComponent<ScreenFader>();
    }
    public void TakeDemage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            print("Player Dead");

            PlayerDead();
            state = PlayerState.Dead;
            //isDead = true;
            SoundManager.instance.PlayerChannel.PlayOneShot(SoundManager.instance.PlayerDeath);
            StartCoroutine(ShowGameOverUI());
            //Game Over
            //Dying animation
        }
        else
        {
            print("Player Hit");
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health: {HP}";
            SoundManager.instance.PlayerChannel.PlayOneShot(SoundManager.instance.PlayerHurt);

        }
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        GameOverUI.gameObject.SetActive(true);

        int zombiesKilled = GlobalRefrences.instance.zombiesKilled;
        if (zombiesKilled > SaveLoadManager.instance.LoadHighScore()){
            SaveLoadManager.instance.SaveHighScore(zombiesKilled);
        }

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(nextSceneName);
    }

    private void PlayerDead()
    {
        //GetComponent<MouseMovement>().enabled = false;
        //GetComponent<PlayerMovement>().enabled = false;
        mouseMovement.enabled = false;
        playerMovement.enabled = false;

        //Dying Animation
        GetComponentInChildren<Animator>().enabled = true;
        playerHealthUI.gameObject.SetActive(false);
        zombiesKilledUI.gameObject.SetActive(false);
        crossHairUI.gameObject.SetActive(false);

        screenFader.StartFade();
       // GetComponent<ScreenFader>().StartFade();

    }

    private IEnumerator BloodyScreenEffect()
    {
        if (!bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();

        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;

            yield return null; ; // Wait for the next frame.
        }

        if (bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.gameObject.name);

        if (other.CompareTag("ZombieHand"))
        {
            Debug.Log("hit player");
            if(state != PlayerState.Dead)
            //if (!isDead)
            {
                TakeDemage(other.gameObject.GetComponent<ZombieHand>().damage);
            }
        }
    }
}
