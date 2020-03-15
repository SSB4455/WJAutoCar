using UnityEngine;

public class ControlCarScript : MonoBehaviour
{
	public float motorMax, steerAngleMax;

	private WheelCollider fl, fr, hl, hr;



	void Start()
	{
		Debug.Log("Start");

		fl = transform.Find("Alloys01").Find("fl").GetComponent<WheelCollider>();
		fr = transform.Find("Alloys01").Find("fr").GetComponent<WheelCollider>();
		hl = transform.Find("Alloys01").Find("hl").GetComponent<WheelCollider>();
		hr = transform.Find("Alloys01").Find("hr").GetComponent<WheelCollider>();

		GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.01f, -0.02f);//左右 上下 前后
	}

	public void CarReset(Vector3 resetPosition)
	{
		Debug.Log("Car Reset");

		//DestroyImmediate(car);
		//car = Instantiate(prefab, new Vector3(0.0f, 1.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));

		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		transform.position = resetPosition;
		transform.rotation = Quaternion.Euler(0, 0, 0);

		fl.steerAngle = 0;
		fr.steerAngle = 0;
		
		fl.motorTorque = 0;
		fr.motorTorque = 0;
		hl.motorTorque = 0;
		hr.motorTorque = 0;
	}

	void Update()
	{
		if (this.GetComponent<MLAgents.Agent>() == null)
		{
			ControlCar(new float[2] { Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") }, false);
		}
	}
    
	public void ControlCar(float[] vectorAction, bool wd4)
	{
		float steer = vectorAction[0] * steerAngleMax;
		float torque = motorMax * -vectorAction[1];
		if (wd4)
		{
			fl.motorTorque = torque;
			fr.motorTorque = torque;
		}
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
	}
    
	public void ResetDetection()
	{
		float angle_z = transform.rotation.eulerAngles.z > 180 ? 360 - transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z;
		//翻车
		if (Mathf.Abs(angle_z) > 20)
		{
			CarReset(new Vector3(0, 0.5f, 0));
		}

		// Fell off platform或卡边
		if (transform.position.y < 0.3f)
		//if (transform.position.y < 0)
		{
			CarReset(new Vector3(0, 0.5f, 0));
		}
	}

	public float GetForwardSpeed()
	{
		Vector3 forward = Vector3.Normalize(fl.transform.position - hl.transform.position);
		float forwardSpeed = Vector3.Dot(forward, GetComponent<Rigidbody>().velocity);
		//SetReward(forwardSpeed / 100);
		Debug.Log("forwardSpeed = " + forwardSpeed);
		return forwardSpeed;
	}
}
