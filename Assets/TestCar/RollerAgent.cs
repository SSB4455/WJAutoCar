using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
	public Transform Target;
	Rigidbody rBody;



	void Start()
	{
		rBody = GetComponent<Rigidbody>();
		Debug.Log("Start");

	}

	public override void AgentReset()
	{
		if (this.transform.position.y < 0)
		{
			// If the Agent fell, zero its momentum
			this.rBody.angularVelocity = Vector3.zero;
			this.rBody.velocity = Vector3.zero;
			this.transform.position = new Vector3(0, 0.5f, 0);
		}

		// Move the target to a new spot
		Target.position = new Vector3(Random.value * 8 - 4,
									  0.5f,
									  Random.value * 8 - 4);
		Debug.Log("AgentReset");
	}
    
    public float speed = 10;
	public override void AgentAction(float[] vectorAction)
	{
		// Actions, size = 2
		Vector3 controlSignal = Vector3.zero;
		controlSignal.x = vectorAction[0];
		controlSignal.z = vectorAction[1];
		rBody.AddForce(controlSignal * speed);

		// Rewards
		float distanceToTarget = Vector3.Distance(this.transform.position,
												  Target.position);

		// Reached target
		if (distanceToTarget < 1.42f)
		{
			SetReward(1.0f);
			Done();
		}

		// Fell off platform
		if (this.transform.position.y < 0)
		{
			Done();
		}


	}
    
	public override void CollectObservations()
	{
		// Target and Agent positions
		AddVectorObs(Target.position);
		AddVectorObs(this.transform.position);

		// Agent velocity
		AddVectorObs(rBody.velocity.x);
		AddVectorObs(rBody.velocity.z);
	}

	public override float[] Heuristic()
	{
		var action = new float[2];

		action[0] = -Input.GetAxis("Horizontal");
		action[1] = Input.GetAxis("Vertical");
		return action;
	}
}
