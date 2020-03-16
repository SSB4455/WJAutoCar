using UnityEngine;
using MLAgents;

public class WJAutoCarAgent : Agent
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

		GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.01f, -0.02f);//左右 上下 前后

		Monitor.SetActive(true);


		//GetComponentInChildren<Collider>().private void OnTriggerEnter(Collider other) {
			
		//}

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

		fl.steerAngle = 0;
		fr.steerAngle = 0;
		
		fl.motorTorque = 0;
		fr.motorTorque = 0;
		hl.motorTorque = 0;
		hr.motorTorque = 0;

		//hr.brakeTorque = 10000;


		forward = Vector3.Normalize(fl.transform.position - hl.transform.position);
		
		lastCollisionTime = Time.realtimeSinceStartup;
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

		Monitor.Log("vectorAction", vectorAction, transform);
		//Monitor.Log("vectorAction", vectorAction[0], null);
		Monitor.Log("CumulativeReward", this.GetCumulativeReward(), null);
		Monitor.Log("time left", this.maxStep - this.GetStepCount(), null);
		
		//this.rew
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

		float steer = vectorAction[0] * steerMax;
		float torque = motorMax * -vectorAction[1];
		hl.motorTorque = torque;
		hr.motorTorque = torque;
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
		AddReward(forwardSpeed / 1000);
		//Debug.Log("torque = " + torque + " forwardSpeed = " + forwardSpeed);
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
        if(collider.tag == "RewardWall")
        {
            if (collider != lastCollider)
			{
				if (lastCollider != null)
				{
					AddReward(1 / (1 + Time.realtimeSinceStartup - lastCollisionTime));
				}
				lastCollider = collider;
				lastCollisionTime = Time.realtimeSinceStartup;
			} else {
				AddReward(-0.2f);
			}
        }
    }

}
