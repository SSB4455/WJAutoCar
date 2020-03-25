using Barracuda;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class WJAutoCarAgent : Agent
{
	public NNModel[] brains;
	public Dropdown dropdown;
	private WheelDrive wd;



	void Start()
	{
		Monitor.SetActive(true);

		//GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.01f, -0.02f);//左右 上下 前后
		wd = transform.GetComponent<WheelDrive>();
		wd.scriptControl = true;

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		for (int i = 0; i < brains.Length; i++)
		{
			options.Add(new Dropdown.OptionData(brains[i].name));
		}
		dropdown.options = options;
		dropdown.value = brains.Length - 1;
	}

	public override void AgentReset()
	{
		Debug.Log("AgentReset");

		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		transform.position = new Vector3(0, 1, 0);
		transform.rotation = Quaternion.Euler(0, 0, 0);

		wd.Drive(0, 0);

		//hr.brakeTorque = 10000;

		lastCollisionTime = Time.realtimeSinceStartup;
	}

	public override void CollectObservations()
	{
		// car 3D Speed and ForwardSpeed
		AddVectorObs(GetComponent<Rigidbody>().velocity.x);
		AddVectorObs(GetComponent<Rigidbody>().velocity.y);
		AddVectorObs(wd.ForwardSpeed);

		// car rotation
		AddVectorObs(transform.rotation.eulerAngles.y);
	}

	public void ChangeBrain()
	{
		if (dropdown.value < brains.Length)
		{
			GiveModel("WJAutoCar", brains[dropdown.value]);
		}
	}

	public override void AgentAction(float[] vectorAction)
	{
		wd.Drive(vectorAction[0], vectorAction[1]);
		AddReward(-0.001f);

		float[] desplayTorque = new float[] { vectorAction[1] };
		Monitor.Log("torque", desplayTorque, transform);
		Monitor.Log("steer", vectorAction[0], transform);
		//Monitor.Log("vectorAction", vectorAction[0], null);
		Monitor.Log("CumulativeReward", this.GetCumulativeReward(), null);
		Monitor.Log("time left", (this.maxStep - this.GetStepCount()) / (float)this.maxStep, null);

		//Debug.Log("torque = " + torque + " forwardSpeed = " + forwardSpeed);
	}

	void Update()
	{
		float angle_z = transform.rotation.eulerAngles.z > 180 ? 360 - transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z;
		//翻车
		if (Mathf.Abs(angle_z) > 20)
		{
			SetReward(-0.4f);
			Done();
		}

		// Fell off platform或卡边
		if (transform.position.y < 0.3f)
		//if (transform.position.y < 0)
		{
			SetReward(-0.4f);
			Done();
		}

		Monitor.Log("forwardSpeed", wd.ForwardSpeed / 30, transform);
		AddReward(wd.ForwardSpeed / 1000);
		
	}

	public override float[] Heuristic()
	{
		var action = new float[2];

		action[0] = Input.GetAxis("Horizontal");
		action[1] = Input.GetAxis("Vertical");
		return action;
	}

	Collider lastCollider = null;
	float lastCollisionTime = 0;
	private void OnTriggerEnter(Collider collider)
	{
		Debug.Log("OnTriggerEnter " + collider.gameObject.name);
		if (collider.tag == "RewardWall")
		{
			if (collider != lastCollider)
			{
				if (lastCollider != null)
				{
					AddReward(0.5f / (1 + Time.realtimeSinceStartup - lastCollisionTime));
				}
				lastCollider = collider;
				lastCollisionTime = Time.realtimeSinceStartup;
			}
			else
			{
				AddReward(-0.2f);
			}
		}
	}

}
