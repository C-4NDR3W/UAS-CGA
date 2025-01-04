using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFloor : MonoBehaviour
{
    public Vector3 pacmanSpawnPosition = new Vector3(1.530671f, 0.04309654f, 9.002954f);

    void Start()
    {

        GameObject existingPacman = GameObject.FindGameObjectWithTag("Pacman");

        if (existingPacman != null)
        {
            existingPacman.transform.position = pacmanSpawnPosition;
        }
    }
}
