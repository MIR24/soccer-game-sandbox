using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages Final IK Target Pose position.
/// </summary>

public class IKTargetPositioning : MonoBehaviour {

    public GameObject soccerPlayer;
    public GameObject soccerBall;

    public Vector3 lookAtOffset;

	// Use this for initialization
	void Start () {
        soccerPlayer = GameObject.FindGameObjectWithTag("Player");
        soccerBall = GameObject.FindGameObjectWithTag("Ball");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = soccerBall.transform.position;
        transform.LookAt(new Vector3(soccerPlayer.transform.position.x + lookAtOffset.x, transform.position.y+lookAtOffset.y, soccerPlayer.transform.position.z+lookAtOffset.z));

    }
}
