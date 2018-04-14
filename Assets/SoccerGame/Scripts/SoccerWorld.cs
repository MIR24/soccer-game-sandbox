using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerWorld : MonoBehaviour
{

    public float timeScale = 1f;
    public GameObject ball;
    public GameObject playableChar;

    //Midpoint beetwen ball and controllable char to point camera
    public GameObject spectateMediane;

    // Use this for initialization
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        spectateMediane.transform.position = (playableChar.transform.position + ball.transform.position) / 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(spectateMediane.transform.position, 0.1F);
    }

}