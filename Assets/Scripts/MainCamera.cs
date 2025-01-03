using UnityEngine;
using Cinemachine;

public class MainCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public string targetTag = "Pacman";

    void Update()
    {
        // Cari karakter Pacman secara otomatis dengan tag
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);

        // Jika karakter ditemukan, atur sebagai target Follow dan LookAt
        if (player != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
            Debug.Log("Player found and camera follows");
        }
    }
}
