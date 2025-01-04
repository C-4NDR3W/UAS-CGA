using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance;
    public GameObject menuUIPanel;
    public GameObject battleUIPanel;
    public GameObject doctorUIPanel;
    public GameObject gameOverUIPanel;
    public TMP_Text floor;

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
            UpdateFloorText();
        }
    }
    public void OnMainMenuButtonPressed()
    {
        StartCoroutine(LoadSceneAsync(0));
    }

    public void OnRetryButtonPressed()
    {
        StartCoroutine(LoadSceneAsync(1));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        DestroyAllDontDestroyOnLoadObjects();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        yield return 1.5f;
    }

    public void UpdateFloorText()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;

        if (buildIndex == 4)
        {
            floor.text = "Floor - Boss";
        }
        else
        {
            floor.text = $"Floor - {buildIndex}";
        }
    }

    public void DestroyAllDontDestroyOnLoadObjects()
    {

        var go = new GameObject("Sacrificial Lamb");
        DontDestroyOnLoad(go);

        foreach (var root in go.scene.GetRootGameObjects())
            Destroy(root);
    }
}