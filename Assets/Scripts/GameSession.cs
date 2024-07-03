using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{
    [SerializeField] int key = 0;
    [SerializeField] float cooldown = 0;
    [SerializeField] TextMeshProUGUI keyText;
    [SerializeField] TextMeshProUGUI cooldownText;
    // Start is called before the first frame update
    void Start()
    {
        keyText.text = key.ToString();
        cooldownText.text = cooldown.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        cooldown -= Time.deltaTime;
        if (cooldown > 0) {
            cooldownText.text = ((int)cooldown).ToString();
        }

    }
    // void Awake(){
    //     int numGameSessions = FindObjectsOfType<GameSession>().Length;
    //     if(numGameSessions >= 1){
    //         Destroy(gameObject);
    //     }
    //     elae{
    //         DontDestroyOnLand(gameObject);
    //     }
    // }
    // public void ProcessPlayerDeath(){
    //     if(playerLives >= 2){
    //         playerLives--;
    //         int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    //         SceneManager.LoadScene(currentSceneIndex);
    //     }
    //     else{
    //         SceneManager.LoadScene(currentSceneIndex);
    //         Destroy(gameObject);
    //     }
    // }
    public void AddToKey(int numkey){
        key += numkey;
        keyText.text = key.ToString();
    }

    public void RemoveKey(int numkey){
        key -= numkey;
        keyText.text = key.ToString();
    }

    public void ReduceCooldown(float timer){
        cooldown = timer;
    }
}
