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
    public GameObject myTarget;

    private bool controllingBall;
    private Vector3 dribblingDirection;
    private SoccerWorld world;
    private Animator anim;
    private int moveSpeedHash;
    private int moveDirectionHash;
    private int abruptStopHash;
    
	// Use this for initialization
	void Start () {
        //Cache Components and Objects
        anim = GetComponent<Animator>();
        world = GameObject.FindGameObjectWithTag("World").GetComponent<SoccerWorld>();

        //Cache Animator Parameters
        moveSpeedHash = Animator.StringToHash("Speed");
        moveDirectionHash = Animator.StringToHash("Direction");
        abruptStopHash = Animator.StringToHash("AbruptStop");

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
        //Manage Animator States
        anim.SetFloat(moveSpeedHash, moveSpeed);
        anim.SetFloat(moveDirectionHash, moveDirection);
        if (abruptStop && !anim.GetBool(abruptStopHash))
        {
            anim.SetBool(abruptStopHash, true);
        }
        if (!abruptStop && anim.GetBool(abruptStopHash)) anim.SetBool(abruptStopHash, false);
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
