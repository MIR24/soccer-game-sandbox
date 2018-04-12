using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Messages;
using RootMotion.FinalIK;

public class SoccerPlayer : MonoBehaviour {

    public int kickPower = 100;
    public float moveSpeed = 0;
    public float moveDirection = 0;
    public bool abruptStop = false;
    public bool turnToTarget = false;
    public GameObject myTarget;
    public Vector3 myTargetDirection;
    public float myTargetDirectionAngle;
    public float maxFieldOfView = 200F;
    public float visionAngle = 30F;
    public bool turnToLeft;
    public bool turnToRight;

    private bool controllingBall;
    private Vector3 dribblingDirection;
    private SoccerWorld world;
    private Animator anim;
    private int moveSpeedHash;
    private int moveDirectionHash;
    private int abruptStopHash;
    private int turnLeftInPlaceHash;
    private int turnRightInPlaceHash;
    
	// Use this for initialization
	void Start () {
        //Cache Components and Objects
        anim = GetComponent<Animator>();
        world = GameObject.FindGameObjectWithTag("World").GetComponent<SoccerWorld>();

        //Cache Animator Parameters
        moveSpeedHash = Animator.StringToHash("Speed");
        moveDirectionHash = Animator.StringToHash("Direction");
        abruptStopHash = Animator.StringToHash("AbruptStop");
        turnLeftInPlaceHash = Animator.StringToHash("TurnLeftInPlace");
        turnRightInPlaceHash = Animator.StringToHash("TurnRightInPlace");

        //Setup Listeners
        MessageDispatcher.AddListener("ball_controlled_by", this.gameObject.name, takeBallControl, true);

        //Initial setup player target
        if (myTarget) {
            LookAtIK myLookTarget = GetComponent<LookAtIK>();
            myLookTarget.solver.target = myTarget.transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Setup movement speed and direction
        anim.SetFloat(moveSpeedHash, moveSpeed);
        anim.SetFloat(moveDirectionHash, moveDirection);
        
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

        //Calculate target heading angle
        myTargetDirection = myTarget.transform.position - transform.position;
        myTargetDirectionAngle = Vector3.SignedAngle(myTargetDirection, transform.forward, Vector3.up);

        //Check if should turn left
        if (myTargetDirectionAngle > maxFieldOfView / 2 + visionAngle && !anim.GetBool(turnLeftInPlaceHash)) {
            Debug.Log("Have to turn left");
            anim.SetBool(turnLeftInPlaceHash, true);
        }
        //Check if should turn right
        if (myTargetDirectionAngle < -maxFieldOfView / 2 - visionAngle && !anim.GetBool(turnRightInPlaceHash)) {
            Debug.Log("Have to turn right");
            anim.SetBool(turnRightInPlaceHash, true);
        }

        Debug.Log("Do I turning in place left" + anim.GetCurrentAnimatorStateInfo(0).IsName("Left_turn_in_place"));
	}

    void takeBallControl(IMessage rMessage) {
        dribblingDirection = Vector3.Normalize(world.ball.transform.position - transform.position);
        controllingBall = true;
        Debug.Log("Start dribbling");
    }

    void kickTheBall() {
        Debug.Log(this.gameObject.name + " kicks the ball");
        SoccerBallLogic soccerBallLogic = world.ball.GetComponent<SoccerBallLogic>();
        soccerBallLogic.setControllable(false, null);
        soccerBallLogic.kick(dribblingDirection, kickPower);
    }
}
