using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBallPhysics : MonoBehaviour {

    private Rigidbody ballPhysics;
    public float dribbleForceMultiplier = 1F;
    public Vector3 lstAccelerationPoint;
    public Vector3 lstAccelerationDirection;
    public Vector3 testForce;
	// Use this for initialization
	void Start () {
        ballPhysics = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
            Debug.DrawLine(lstAccelerationPoint, lstAccelerationDirection, Color.red);
	}

    void OnGUI()
    {
         if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
            ballPhysics.AddForce(lstAccelerationDirection, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ball collided with " + collision.gameObject.tag+ ", collider root owner " + collision.gameObject.transform.root.gameObject.name + " at" + transform.position);

        if (collision.gameObject.transform.root.gameObject.tag == "Player") {
            lstAccelerationPoint = transform.position;

            Vector3 accelerationVector = (lstAccelerationPoint - collision.gameObject.transform.root.gameObject.transform.position);

            //Remove Verticale difference between Player and Ball from acceleration
            accelerationVector.y = 0;
            //Normalize acceleration
            accelerationVector = accelerationVector.normalized;
            //Remove Verticale from Acceleration
            accelerationVector.y = transform.position.y;

            float objectMoveSpeed = collision.transform.root.gameObject.GetComponent<SoccerPlayer>().moveSpeed;
            lstAccelerationDirection = accelerationVector * dribbleForceMultiplier;
            ballPhysics.AddForce(lstAccelerationDirection, ForceMode.VelocityChange);
            Debug.Log("Pushing ball forwards" + lstAccelerationDirection);
        }
    }


}
