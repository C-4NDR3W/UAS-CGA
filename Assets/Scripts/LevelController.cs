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

            if (gameMode == "Campaign")
            {
                if (currentSceneIndex == 4)
                {
                    SceneManager.LoadScene(0);
                    DestroyDontDestroyOnLoadObjects();
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

    IEnumerator DestroyDontDestroyOnLoadObjects()
    {
        yield return null;

        // Find the objects you want to remove or destroy
        GameObject pacman = GameObject.Find("Pacman(Clone)");
        GameObject inGameUI = GameObject.Find("In Game UI");

        // Check if the objects are found
        if (pacman != null)
        {
            // Move Pacman to the current scene to remove it from DontDestroyOnLoad
            SceneManager.MoveGameObjectToScene(pacman, SceneManager.GetActiveScene());
            Destroy(pacman); // Destroy it after moving to the active scene
        }

        if (inGameUI != null)
        {
            // Move In Game UI to the current scene to remove it from DontDestroyOnLoad
            SceneManager.MoveGameObjectToScene(inGameUI, SceneManager.GetActiveScene());
            Destroy(inGameUI); // Destroy it after moving to the active scene
        }
    }

}