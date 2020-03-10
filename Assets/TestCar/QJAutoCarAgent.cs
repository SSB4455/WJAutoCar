using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class QJAutoCarAgent : Agent
{
	public float motorMax, steerMax;

	private Vector3 forward;
	private WheelCollider fl, fr, hl, hr;



	void Start()
	{
		Debug.Log("Start");

		fl = transform.Find("Alloys01").Find("fl").GetComponent<WheelCollider>();
		fr = transform.Find("Alloys01").Find("fr").GetComponent<WheelCollider>();
		hl = transform.Find("Alloys01").Find("hl").GetComponent<WheelCollider>();
		hr = transform.Find("Alloys01").Find("hr").GetComponent<WheelCollider>();
	}

	public override void AgentReset()
	{
		Debug.Log("AgentReset");

		//DestroyImmediate(car);
		//car = Instantiate(prefab, new Vector3(0.0f, 1.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));

		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		transform.position = new Vector3(0, 1, 0);
		transform.rotation = Quaternion.Euler(0, 0, 0);


		forward = Vector3.Normalize(fl.transform.position - hl.transform.position);
		
	}

	public override void CollectObservations()
	{
		// Target and Agent positions
		//AddVectorObs(Target.position);
		//AddVectorObs(this.transform.position);

		// Agent velocity
		//AddVectorObs(rBody.velocity.x);
		//AddVectorObs(rBody.velocity.z);
	}
    
	public override void AgentAction(float[] vectorAction)
	{
		float angle_z = transform.rotation.eulerAngles.z > 180 ? 360 - transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z;
	
		//翻车
		if (Mathf.Abs(angle_z) > 10)
		{
			SetReward(-0.4f);
			Done();
		}

		// Reached target
		//if (goal.GetComponent<reachGoal>().goal == 1)
		{
			//goal.GetComponent<reachGoal>().goal = 0;

			//SetReward(1.0f);
			//done = true;
			//return;
		}

		// Fell off platform或卡边
		if (transform.position.y < 0.4f)
		{
			SetReward(-0.4f);
			Done();
		}

		float steer = vectorAction[0] * steerMax;
		float motor = -vectorAction[1];
		hl.motorTorque = motor * motorMax;
		hr.motorTorque = motor * motorMax;
		Vector3 position;
		Quaternion rotation;

		fl.steerAngle = steer;
		fr.steerAngle = steer;
		fl.GetWorldPose(out position, out rotation);
		fl.transform.rotation = rotation;
		fr.transform.rotation = rotation;
		hr.GetWorldPose(out position, out rotation);
		hl.transform.rotation = rotation;
		hr.transform.rotation = rotation;
		forward = Vector3.Normalize(fl.transform.position - hl.transform.position);

		float forwardSpeed = Vector3.Dot(forward, GetComponent<Rigidbody>().velocity);
		SetReward(forwardSpeed / 100);
	}

	public override float[] Heuristic()
	{
		var action = new float[2];

		action[0] = Input.GetAxis("Horizontal");
		action[1] = Input.GetAxis("Vertical");
		return action;
	}
}
