using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTargetPositioning : MonoBehaviour {

    public Vector3 playerDirection;
    public Vector3 ballDirection;
    public GameObject soccerWorld;
    public GameObject soccerPlayer;
    public GameObject soccerBall;

    public float distanceToBall = 0.1F;

	// Use this for initialization
	void Start () {
        soccerPlayer = GameObject.FindGameObjectWithTag("Player");
        soccerBall = GameObject.FindGameObjectWithTag("Ball");
	}
	
	// Update is called once per frame
	void Update () {
        playerDirection = soccerPlayer.transform .position - gameObject.transform.position;
        ballDirection = soccerBall.transform.position - gameObject.transform.position;
        Debug.DrawRay(gameObject.transform.position, playerDirection, Color.yellow);
        Debug.DrawRay(gameObject.transform.position, ballDirection, Color.yellow);

        transform.position = soccerBall.transform.position + (soccerPlayer.transform.position - soccerBall.transform.position).normalized * distanceToBall;
	}
}
