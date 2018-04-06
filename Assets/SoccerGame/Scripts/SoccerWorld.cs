using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerWorld : MonoBehaviour
{

    public float timeScale = 1f;
    public GameObject ball;

    // Use this for initialization
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
    }
}