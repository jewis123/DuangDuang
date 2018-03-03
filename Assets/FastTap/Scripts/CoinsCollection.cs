using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsCollection : MonoBehaviour {
    Transform player;
    float distance;
    GameManager gameManager;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameManager =  GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void FixedUpdate()
    {
        distance = Vector2.Distance(transform.position, player.position);

        if (distance < 1.2 + PlayerPrefs.GetFloat("level"))
        {
            gameManager.Coins = PlayerPrefs.GetInt("coinNum") + 2;
            gameObject.SetActive(false);
        }
    }
}
