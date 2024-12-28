using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance;
    public GameObject menuUIPanel;
    public GameObject battleUIPanel;
    public GameObject doctorUIPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuUIPanel.SetActive(!menuUIPanel.activeSelf);
        }
    }
    public void OnMainMenuButtonPressed()
    {
        Debug.Log("Main Menu");
        SceneManager.LoadScene(0);
    }
}