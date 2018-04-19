using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBallPhysics : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedWith = collision.transform.gameObject;

        if (collidedWith.transform.root.tag != "Player") return;

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.yellow, 3F);
            Debug.Log(contact.point);
        }   
    }
}
