using Barracuda;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class WJAutoCarAgent : Agent
{
	public GameObject[] checkPointSeq;
	int checkIndex;
	public NNModel[] brains;
	public Dropdown dropdown;
	public Text LapText;
	float lapStartTime = 0;
	float[] lapTimeTops = new float[3];
	WheelDrive wd;



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
		lapStartTime = 0;
	}

	public override void CollectObservations()
	{
		// car 3D Speed and ForwardSpeed
		//AddVectorObs(GetComponent<Rigidbody>().velocity.x);
		//AddVectorObs(GetComponent<Rigidbody>().velocity.y);
		//AddVectorObs(wd.ForwardSpeed);

		// car rotation
		//AddVectorObs(transform.rotation.eulerAngles.y / 360);

		// 剩余活动时间
		//AddVectorObs(GetStepCount() / (float)maxStep);
	}

	public void ChangeBrain()
	{
		if (dropdown.value < brains.Length)
		{
			GiveModel("WJAutoCar", brains[dropdown.value]);
		}
		lapTimeTops = new float[3];
		Done();
	}

	public override void AgentAction(float[] vectorAction)
	{
		wd.Drive(vectorAction[0], vectorAction[1]);

		float[] desplayTorque = new float[] { vectorAction[1] };
		Monitor.Log("torque", desplayTorque, transform);
		//Debug.Log("torque = " + torque + " forwardSpeed = " + forwardSpeed);
		Monitor.Log("steer", vectorAction[0], transform);
		//Monitor.Log("vectorAction", vectorAction[0], null);
		Monitor.Log("CumulativeReward", this.GetCumulativeReward() / 1000, null);
		Monitor.Log("time left", 1 - (GetStepCount() / (float)maxStep), null);

		AddReward((wd.ForwardSpeed - 3) / 100);
	}

	void Update()
	{
		float angle_z = transform.rotation.eulerAngles.z > 180 ? 360 - transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z;
		//翻车
		if (Mathf.Abs(angle_z) > 20)
		{
			SetReward(-1);
			Done();
		}

		// Fell off platform或卡边
		if (transform.position.y < 0.3f)
		//if (transform.position.y < 0)
		{
			SetReward(-1);
			Done();
		}

		//Monitor.Log("forwardSpeed", wd.ForwardSpeed / 30, transform);

		string lapStr = (lapStartTime > 0 ? "lap time(s):" + (Time.realtimeSinceStartup - lapStartTime).ToString("f2") : "") + "\t\t|" + wd.ForwardSpeed.ToString("f2") + "m/s\n";
		for (int i = 0; i < lapTimeTops.Length; i++)
		{
			if (lapTimeTops[i] == 0)
			{
				break;
			}
			lapStr += "<size=" + (14 + (lapTimeTops.Length - i) * 2) + ">Top" + (i + 1) + ":" + lapTimeTops[i].ToString("f2") + "</size>\n";
		}
		LapText.text = lapStr;
	}

	public override float[] Heuristic()
	{
		var action = new float[2];

		action[0] = Input.GetAxis("Horizontal");
		action[1] = Input.GetAxis("Vertical");
		return action;
	}

	private void OnTriggerEnter(Collider collider)
	{
		Debug.Log("OnTriggerEnter " + collider.gameObject.name + " checkIndex " + checkIndex);
		if (collider.tag == "RewardWall")
		{
			if (collider.gameObject == checkPointSeq[checkIndex])
			{
				if (checkIndex == 0)
				{
					float lapTime = Time.realtimeSinceStartup - lapStartTime;
					for (int i = lapTimeTops.Length - 1; lapStartTime > 0 && i >= 0; i--)
					{
						if (lapTimeTops[i] == 0 || lapTime < lapTimeTops[i])
						{
							if (i < lapTimeTops.Length - 1)
							{
								lapTimeTops[i + 1] = lapTimeTops[i];
							}
							lapTimeTops[i] = lapTime;
						}
					}
					lapStartTime = Time.realtimeSinceStartup;
				}
				checkIndex = (checkIndex + 1) % checkPointSeq.Length;
			}
			else
			{
				AddReward(-0.5f);
			}
		}
	}

}
