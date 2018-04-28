using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBallPhysics : MonoBehaviour {

    private Rigidbody ballPhysics;
    public float dribbleForceMultiplier = 1F;
    public Vector3 lstAccelerationPoint;
    public Vector3 lstAccelerationDirection;
    public Vector3 goalDirection;
    public Vector3 interimDirection;
    public Vector3 accelerationVectorAlongWay;
    public float lerpDirectionFactor = 0.3F;
    public Vector3 testForce;

    public GameObject ballTarget;
	// Use this for initialization
	void Start () {
        ballPhysics = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        Debug.DrawRay(transform.position, goalDirection, Color.yellow);
        Debug.DrawRay(lstAccelerationPoint, lstAccelerationDirection, Color.green);
        Debug.DrawRay(lstAccelerationPoint, accelerationVectorAlongWay, Color.red);
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

            accelerationVectorAlongWay = (lstAccelerationPoint - collision.gameObject.transform.root.gameObject.transform.position);

            //Remove vertical difference between Player and Ball positions from acceleration
            accelerationVectorAlongWay.y = 0;
            //Normalize acceleration
            accelerationVectorAlongWay = accelerationVectorAlongWay.normalized;
            //Remove Verticale from Acceleration
            accelerationVectorAlongWay.y = transform.position.y;


            float objectMoveSpeed = collision.transform.root.gameObject.GetComponent<SoccerPlayer>().moveSpeed;

            goalDirection = ballTarget.transform.position - transform.position;
            interimDirection = Vector3.Slerp(accelerationVectorAlongWay, goalDirection.normalized, lerpDirectionFactor);

            lstAccelerationDirection = interimDirection * dribbleForceMultiplier;

            ballPhysics.AddForce(lstAccelerationDirection, ForceMode.VelocityChange);
            Debug.Log("Pushing ball forwards" + lstAccelerationDirection);
        }
    }


}
