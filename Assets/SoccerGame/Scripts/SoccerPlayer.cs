using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using com.ootii.Messages;
using RootMotion.FinalIK;

public class SoccerPlayer : MonoBehaviour {

    //Components cached
    public GameObject myTarget;
    public Vector3 targetDirection;
    private SoccerWorld world;
    private Animator anim;

    //Char characteristics
    public float headingAngle = 200F;
    public float visionAngle = 30F;
    public int kickPower = 100;

    //Motion states params & flags
    public float moveSpeed = 0;
    public float moveDirectionAngle = 0;
    public bool abruptStop = false;

    //Animator cached parameters
    private int moveSpeedHash;
    private int moveDirectionHash;
    private int targetDistanceHash;
    private int abruptStopHash;
    private int turnLeftInPlaceHash;
    private int turnRightInPlaceHash;
    private int followTargetHash;
    
    //Other params & options
    public float targetDirectionAngle;
    public float targetDistance;
    private bool controllingBall;
    private Vector3 dribblingDirection;
    public float moveDirectionDeviation = 5F; //Move direction acceptable deviation
    public float temporarySlowDown = 0F;
    public float accelerationLerp = 0.1F; 

    // Use this for initialization
    void Start () {
        //Cache Components and Objects
        anim = GetComponent<Animator>();
        world = GameObject.FindGameObjectWithTag("World").GetComponent<SoccerWorld>();

        //Cache Animator Parameters
        moveSpeedHash = Animator.StringToHash("Speed");
        moveDirectionHash = Animator.StringToHash("MoveDirection");
        targetDistanceHash = Animator.StringToHash("TargetDistance");
        abruptStopHash = Animator.StringToHash("AbruptStop");
        turnLeftInPlaceHash = Animator.StringToHash("TurnLeftInPlace");
        turnRightInPlaceHash = Animator.StringToHash("TurnRightInPlace");
        followTargetHash = Animator.StringToHash("FollowTarget");

        //Setup Listeners
        MessageDispatcher.AddListener("ball_controlled_by", this.gameObject.name, takeBallControl, true);

        //Initial setup player target
        myTarget = GameObject.FindGameObjectWithTag("Ball");
        if (myTarget) {
            LookAtIK myLookTarget = GetComponent<LookAtIK>();
            myLookTarget.solver.target = myTarget.transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Setup move direction (move direction and target direction are not the same, but sometime this happens)
        moveDirectionAngle = targetDirectionAngle;

        //Adjust move direction
        if (moveSpeed > 0)
        {
            if (moveDirectionAngle > 1)
            {
                DebugPanel.Log("Last Correction ", "Left");
                transform.Rotate(Vector3.up * -Time.deltaTime * Mathf.Lerp(0, moveDirectionAngle, 0.5F), Space.World);
            }
            if (moveDirectionAngle < -1)
            {
                DebugPanel.Log("Last Correction ", "Right");
                transform.Rotate(Vector3.up * -Time.deltaTime * Mathf.Lerp(0, moveDirectionAngle, 0.5F), Space.World);
            }
        }

        //Slowdown for turn
        if (Math.Abs(moveDirectionAngle) > 45)
        {
            if (temporarySlowDown == 0) temporarySlowDown = moveSpeed;
            if(moveSpeed > 0.1F) moveSpeed = Mathf.Lerp(moveSpeed, 0.1F, accelerationLerp);
        }
        else if (temporarySlowDown > 0) {
            if (moveSpeed < temporarySlowDown) moveSpeed = Mathf.Lerp(moveSpeed, temporarySlowDown, accelerationLerp);
            else temporarySlowDown = 0;
        }

        //Setup movement speed and direction
        anim.SetFloat(moveSpeedHash, moveSpeed);
        anim.SetFloat(moveDirectionHash, moveDirectionAngle);
        anim.SetFloat(targetDistanceHash, targetDistance);
        
        //Flush animation flags
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Left_turn_in_place") && anim.GetBool(turnLeftInPlaceHash)) {
            anim.SetBool(turnLeftInPlaceHash, false);
        }

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Right_turn_in_place") && anim.GetBool(turnRightInPlaceHash)) {
            anim.SetBool(turnRightInPlaceHash, false);
        }

        //Trigger abrupt stop during sprint
        if (abruptStop && !anim.GetBool(abruptStopHash))
        {
            anim.SetBool(abruptStopHash, true);
        }
        if (!abruptStop && anim.GetBool(abruptStopHash)) anim.SetBool(abruptStopHash, false);

        //Calculate target heading angle and distance
        targetDirection = myTarget.transform.position - transform.position;
        targetDirectionAngle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);
        targetDistance = Vector3.Distance(myTarget.transform.position, transform.position);

        //Check if should turn left in place
        if (targetDirectionAngle > headingAngle / 2 + visionAngle && !anim.GetBool(turnLeftInPlaceHash)) {
            anim.SetBool(turnLeftInPlaceHash, true);
        }
        //Check if should turn right place
        if (targetDirectionAngle < -headingAngle / 2 - visionAngle && !anim.GetBool(turnRightInPlaceHash)) {
            anim.SetBool(turnRightInPlaceHash, true);
        }

        DebugPanel.Log("Target Distance ", targetDistance);
        DebugPanel.Log("Target Direction Angle ", targetDirectionAngle);
        DebugPanel.Log("MoveSpeed", moveSpeed);
	}

    void takeBallControl(IMessage rMessage) {
        dribblingDirection = Vector3.Normalize(world.ball.transform.position - transform.position);
        controllingBall = true;
    }

    void kickTheBall() {
        SoccerBallLogic soccerBallLogic = world.ball.GetComponent<SoccerBallLogic>();
        soccerBallLogic.setControllable(false, null);
        soccerBallLogic.kick(dribblingDirection, kickPower);
    }
}
