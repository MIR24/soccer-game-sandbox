using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Messages;

public class SoccerPlayer : MonoBehaviour {

    public int kickPower = 100;
    public float moveSpeed = 0;

    private bool controllingBall;
    private Vector3 dribblingDirection;
    private SoccerWorld world;
    private Animator anim;
    private int moveSpeedHash;
    
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        moveSpeedHash = Animator.StringToHash("Speed");
        world = GameObject.FindGameObjectWithTag("World").GetComponent<SoccerWorld>();
        MessageDispatcher.AddListener("ball_controlled_by", this.gameObject.name, takeBallControl, true);
    }
	
	// Update is called once per frame
	void Update () {
        anim.SetFloat(moveSpeedHash, moveSpeed);    
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
