using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterGameManager : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        Vector2 randomViewportPos = new Vector2(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));
        Vector3 randomWorldPos = Camera.main.ViewportToWorldPoint(randomViewportPos);
        randomWorldPos = new Vector3(randomWorldPos.x, randomWorldPos.y, 0);
        PhotonNetwork.Instantiate(playerPrefab.name, randomWorldPos, Quaternion.identity);
    }
}
