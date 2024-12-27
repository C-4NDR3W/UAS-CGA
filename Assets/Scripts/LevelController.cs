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