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
            ballPhysics.AddForce(testForce, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DebugPanel.Log("Ball collided with ", collision.gameObject.tag);
        if (collision.gameObject.tag == "RightFoot" || collision.gameObject.tag == "LeftFoot") {
            lstAccelerationPoint = transform.position;
            Vector3 accelerationVector = (lstAccelerationPoint - collision.transform.root.gameObject.transform.position).normalized;
            accelerationVector = new Vector3(accelerationVector.x, transform.position.y, accelerationVector.z);
            lstAccelerationDirection = accelerationVector * collision.transform.root.gameObject.GetComponent<SoccerPlayer>().moveSpeed * dribbleForceMultiplier;
            ballPhysics.AddForce(lstAccelerationDirection, ForceMode.VelocityChange);
            Debug.Log("Pushing ball forwards");
        }
    }


}
