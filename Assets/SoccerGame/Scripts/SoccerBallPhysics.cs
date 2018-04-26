using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBallPhysics : MonoBehaviour {

    private Rigidbody ballPhysics;
	// Use this for initialization
	void Start () {
        ballPhysics = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        DebugPanel.Log("Ball collided with ", collision.gameObject.tag);
        if (collision.gameObject.tag == "RightFoot" || collision.gameObject.tag == "LeftFoot") {
            ballPhysics.velocity = Vector3.zero;
            ballPhysics.angularVelocity = Vector3.zero;

            ballPhysics.AddForce((transform.position - collision.transform.position).normalized*10, ForceMode.Force);
        }
    }


}
