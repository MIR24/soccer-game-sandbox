using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Messages;

public class SoccerBallLogic : MonoBehaviour {

    public float smoothTime = 0.5F;

    //Set the moratorium for ball control regain before ball and player Triggers left each other (if TRUE must wait)
    public bool isJustReleased = false;

    //Just released 
    public int justReleasedMaxTimeout = 20;
    //Set timeout after Trigers diverged to avoid occasional triggering
    public int justReleasedTimeout = 0;


    public Vector3 ballNewPosition;
    public bool isUnderControl;

    //Player who takes ball control, NULL if neither
    public GameObject playerControlling;

    private Vector3 velocity = Vector3.forward;
    private Vector3 controllableOffset;
    private Vector3 kickDirection = Vector3.zero;
    private SphereCollider ballCollider;
    private Rigidbody ballRigidbody;
    private GameObject ballPhysics;
    private GameObject ballControllerBodyPart;
    private float ballRadiusScaled;

    private Vector3 dribblingDirection;

    private const float ControllerMinDistance = 0.4f;

    // Use this for initialization
    void Start () {
        Debug.Log("Started");

        ballPhysics = gameObject.transform.parent.gameObject;
        ballCollider = ballPhysics.GetComponent<SphereCollider>();
        ballRigidbody = ballPhysics.GetComponent<Rigidbody>();

        ballRadiusScaled = ballCollider.radius * ballPhysics.transform.localScale.x;
        controllableOffset.y = ballRadiusScaled;
        ballNewPosition.y = ballRadiusScaled;
    }
	
	// Update is called once per frame
	void Update () {
        if (isUnderControl && ballControllerBodyPart && playerControlling)
        {
            ballNewPosition.x = ballControllerBodyPart.transform.position.x + controllableOffset.x;
            ballNewPosition.z = ballControllerBodyPart.transform.position.z + controllableOffset.z;
            dribblingDirection = Vector3.Normalize(ballPhysics.transform.position - playerControlling.transform.position);
            
            //ballPhysics.transform.position = Vector3.SmoothDamp(ballPhysics.transform.position, ballNewPosition, ref velocity, smoothTime);
            ballPhysics.transform.position = ballNewPosition;
            Vector3 contactPoint = ballControllerBodyPart.gameObject.GetComponent<Collider>().ClosestPointOnBounds(ballPhysics.transform.position);
            float distance = Vector3.Distance(contactPoint, ballPhysics.transform.position) - ballCollider.radius;

            Debug.DrawLine(contactPoint, ballPhysics.transform.position, Color.red);
            Debug.DrawRay(ballControllerBodyPart.transform.position, controllableOffset, Color.yellow);
            Debug.DrawRay(ballPhysics.transform.position, dribblingDirection, Color.green);
            //Debug.Log("Controller position:" + ballController.transform.position);
            //Debug.Log("Offset" + controllableOffset);
            //Debug.Log("Controller to Ball distance " + distance);
        }
        if (justReleasedTimeout > 0) justReleasedTimeout--;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enters " + other.gameObject.name);

        if (!isUnderControl && !isJustReleased && justReleasedTimeout == 0 && other.gameObject.tag == "Ball Controller")
        {
            if (SetControllable(true, other.gameObject))
                MessageDispatcher.SendMessage("ball_controlled_by", other.transform.root.name, 0);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exits " + other.gameObject.name);

        if (ballControllerBodyPart && other.gameObject.GetInstanceID() == ballControllerBodyPart.GetInstanceID()) {
            ballControllerBodyPart = null;
            playerControlling = null;
            isJustReleased = false;
        };
    }

    private void OnTriggerStay(Collider other)
    {


    }

    public bool SetControllable(bool controllable, GameObject newBallController)
    {

        if (controllable)
        {
            //Turn off PhysX
            ballRigidbody.isKinematic = true;
            ballRigidbody.useGravity = false;
            ballCollider.enabled = false;

            //Setup ball current owner
            ballControllerBodyPart = newBallController;
            playerControlling = newBallController.transform.root.gameObject;
            dribblingDirection = Vector3.Normalize(ballPhysics.transform.position - playerControlling.transform.position);

            //Calculate starting ball offset relative to player
            controllableOffset.x = ballPhysics.transform.position.x - ballControllerBodyPart.transform.position.x;
            controllableOffset.z = ballPhysics.transform.position.z - ballControllerBodyPart.transform.position.z;

            isUnderControl = true;
            Debug.Log("Set "+ newBallController.gameObject.transform.root.name +" as ball controller");
        }
        else {
            //Turn on PhysX
            ballRigidbody.isKinematic = false;
            ballRigidbody.useGravity = true;
            ballCollider.enabled = true;

            //Stop control logic
            isUnderControl = false;
            isJustReleased = true;
            justReleasedTimeout = justReleasedMaxTimeout;

            Debug.Log("Set ball control OFF");
        }

        return true;
    }

    public void kick(Vector3 dribblingDirection, int kickPower) {
        Debug.Log("Ball got the kick in direction" + dribblingDirection);
        Vector3 bodyPartControllerDirection = Vector3.Normalize(ballPhysics.transform.position - ballControllerBodyPart.transform.position);
        Vector3 kickDirection = (dribblingDirection + bodyPartControllerDirection) / 2;
        ballRigidbody.AddForce(bodyPartControllerDirection * kickPower);
        //ballRigidbody.AddForce(kickDirection * kickPower);
        //ballRigidbody.AddForce(dribblingDirection * kickPower);
    }
}
