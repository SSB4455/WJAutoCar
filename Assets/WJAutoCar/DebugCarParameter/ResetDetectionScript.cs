using UnityEngine;
using MLAgents;

public class ResetDetectionScript : MonoBehaviour
{



	void Start()
	{
	}

	public void Reset()
	{
		Debug.Log("Reset");

		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		transform.position = new Vector3(0, 1, 0);
		transform.rotation = Quaternion.Euler(0, 0, 0);

		//hr.brakeTorque = 10000;

	}

	void Update()
	{
		float angle_z = transform.rotation.eulerAngles.z > 180 ? 360 - transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z;
		//翻车
		if (Mathf.Abs(angle_z) > 20)
		{
			Reset();
		}

		// Fell off platform或卡边
		if (transform.position.y < 0.3f)
		//if (transform.position.y < 0)
		{
			Reset();
		}
	}

}
