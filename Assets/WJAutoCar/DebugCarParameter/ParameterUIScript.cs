using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ParameterUIScript : MonoBehaviour
{
    public GameObject carObject;

	public Dropdown deiveTypeInput;
	public Text textPrefab;
	public InputField inputFieldPrefab;
    public Slider[] allSliders;
	Dictionary<string, InputField> slidersMultiples;

	Rigidbody carBodyRigidbody;
	WheelDrive wd;
	WheelCollider[] m_Wheels;
	Hashtable parameterJson;



	// Find all the WheelColliders down in the hierarchy.
	void Start()
	{
		carBodyRigidbody = carObject.GetComponent<Rigidbody>();
		wd = carObject.GetComponent<WheelDrive>();
		m_Wheels = carObject.GetComponentsInChildren<WheelCollider>();

		Debug.Log(Application.persistentDataPath + Path.DirectorySeparatorChar + "parameter.json");
		string jsonStr = "";
		if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "parameter.json"))
		{
			jsonStr = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "parameter.json");
		} else {
			jsonStr = Resources.Load<TextAsset>("parameter").text;
		}
		parameterJson = MiniJSON.jsonDecode(jsonStr) as Hashtable;
		slidersMultiples = new Dictionary<string, InputField>();
		foreach (Slider slider in allSliders)
		{
			InputField inputField = Instantiate(inputFieldPrefab);
			inputField.gameObject.SetActive(true);
			inputField.transform.parent = slider.transform;
			inputField.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			(inputField.placeholder as Text).text = "倍数";
			if (parameterJson.ContainsKey(slider.name))
			{
				slider.value = (float)parameterJson[slider.name];
				inputField.text = parameterJson[slider.name + "_Times"].ToString();
			}
			slidersMultiples.Add(slider.name, inputField);

			Text text = Instantiate(textPrefab);
			text.gameObject.SetActive(true);
			text.transform.parent = slider.transform;
			text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			text.text = GetParameterAnnotation(slider.name) + "(" + slider.value * float.Parse(slidersMultiples[slider.name].text) + ")";
		}
		inputFieldPrefab.gameObject.SetActive(false);
		textPrefab.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SliderValueChange()
    {
		foreach (Slider slider in allSliders)
		{
			float value = slider.value * float.Parse(slidersMultiples[slider.name].text);
			switch (slider.name)
			{
				case "maxTorqueSlider":
					wd.maxTorque = value;
					break;
				case "bodyMassSlider":
					carBodyRigidbody.mass = value;
					break;
				case "wheelMassSlider":
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.mass = value;
					}
					break;
				case "suspensionDistionSlider":
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.suspensionDistance = value;
					}
					break;
				case "suspensionSpringSlider":
					JointSpring js = m_Wheels[0].suspensionSpring;
					js.spring = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.suspensionSpring = js;
					}
					break;
				case "suspensionDamperSlider":
					js = m_Wheels[0].suspensionSpring;
					js.damper = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.suspensionSpring = js;
					}
					break;
				case "forwardFrictionExtremumSlipSlider":
					WheelFrictionCurve wfc = m_Wheels[0].forwardFriction;
					wfc.extremumSlip = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.forwardFriction = wfc;
					}
					break;
				case "forwardFrictionExtremumValueSlider":
					wfc = m_Wheels[0].forwardFriction;
					wfc.extremumValue = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.forwardFriction = wfc;
					}
					break;
				case "forwardFrictionAsymptoteSlipSlider":
					wfc = m_Wheels[0].forwardFriction;
					wfc.asymptoteSlip = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.forwardFriction = wfc;
					}
					break;
				case "forwardFrictionAsymptoteValueSlider":
					wfc = m_Wheels[0].forwardFriction;
					wfc.asymptoteValue = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.forwardFriction = wfc;
					}
					break;
				case "sidewaysFrictionExtremumSlipSlider":
					wfc = m_Wheels[0].sidewaysFriction;
					wfc.extremumSlip = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.sidewaysFriction = wfc;
					}
					break;
				case "sidewaysFrictionExtremumValueSlider":
					wfc = m_Wheels[0].sidewaysFriction;
					wfc.extremumValue = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.sidewaysFriction = wfc;
					}
					break;
				case "sidewaysFrictionAsymptoteSlipSlider":
					wfc = m_Wheels[0].sidewaysFriction;
					wfc.asymptoteSlip = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.sidewaysFriction = wfc;
					}
					break;
				case "sidewaysFrictionAsymptoteValueSlider":
					wfc = m_Wheels[0].sidewaysFriction;
					wfc.asymptoteValue = value;
					foreach (WheelCollider wheel in m_Wheels)
					{
						wheel.sidewaysFriction = wfc;
					}
					break;
			}
			if (!parameterJson.ContainsKey(slider.name))
			{
				parameterJson.Add(slider.name, slider.value);
			}
			if (!parameterJson.ContainsKey(slider.name + "_Times"))
			{
				parameterJson.Add(slider.name + "_Times", slidersMultiples[slider.name].text);
			}
			parameterJson[slider.name] = slider.value;
			parameterJson[slider.name + "_Times"] = slidersMultiples[slider.name].text;
		}

		File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "parameter.json", parameterJson.toJson());
    }

	string GetParameterAnnotation(string name)
	{





		return name.Replace("Slider", "");
	}
}
