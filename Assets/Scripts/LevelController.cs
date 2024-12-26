using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pacman"))
        {
            string gameMode = PlayerPrefs.GetString("GameMode", "Campaign");

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log(gameMode);
            Debug.Log(currentSceneIndex);

            if (gameMode == "Campaign")
            {
                if (currentSceneIndex == 4) 
                {
                    GameObject[] dontDestroyObjects = GameObject.FindObjectsOfType<GameObject>();

                    foreach (GameObject obj in dontDestroyObjects)
                    {
                        if (obj.scene.buildIndex == -1)
                        {
                            Destroy(obj);
                        }
                    }
                    SceneManager.LoadScene(0);
                }
                else
                {
                    SceneManager.LoadScene(currentSceneIndex + 1); 
                }
            }
            else if (gameMode == "Endless")
            {
                if (currentSceneIndex == 4)
                {
                    SceneManager.LoadScene(1);
                }
                else
                {
                    SceneManager.LoadScene(currentSceneIndex + 1); 
                }
            }
        }
    }
}

