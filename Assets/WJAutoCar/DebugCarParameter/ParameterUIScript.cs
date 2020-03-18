using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ParameterUIScript : MonoBehaviour
{
	public GameObject carObject;

	public Transform panel;
	public Text displayParameterButtonText;
	public Text speedText;
	public Dropdown deiveTypeInput;
	public Toggle autoForwardToggle;
	public Toggle autoSteerToggle;
	public Text textPrefab;
	public InputField inputFieldPrefab;
	public Slider[] allSliders;
	Dictionary<string, InputField> slidersMultiples;

	Rigidbody carBodyRigidbody;
	WheelDrive wd;
	WheelCollider[] m_Wheels;
	Hashtable parameterJson;

	bool displayParameter = false;



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
			slider.value = -1;
			InputField inputField = Instantiate(inputFieldPrefab);
			inputField.gameObject.SetActive(true);
			inputField.transform.parent = slider.transform;
			inputField.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			inputField.onValueChanged.AddListener(delegate
			{
				slider.maxValue = float.Parse(inputField.text);
				parameterJson[slider.name + "_Times"] = inputField.text;
				SaveParameter();
			});
			(inputField.placeholder as Text).text = "倍数";
			slidersMultiples.Add(slider.name, inputField);

			Text text = Instantiate(textPrefab);
			text.name = slider.name + "Text";
			text.gameObject.SetActive(true);
			text.transform.parent = slider.transform;
			text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

			slider.onValueChanged.AddListener(delegate
			{
				SliderValueChange(slider.name, slider.value);
				parameterJson[slider.name] = slider.value;
				text.text = GetParameterAnnotation(slider.name) + "(" + slider.value + ") = " + slider.value + "  ";
				SaveParameter();
			});
			if (!parameterJson.ContainsKey(slider.name))
			{
				parameterJson.Add(slider.name, slider.value);
			}
			if (!parameterJson.ContainsKey(slider.name + "_Times"))
			{
				parameterJson.Add(slider.name + "_Times", inputField.text);
			}
			inputField.text = parameterJson[(slider.name + "_Times")].ToString();
			slider.maxValue = float.Parse(inputField.text);
			slider.value = float.Parse(parameterJson[slider.name].ToString());
		}
		inputFieldPrefab.gameObject.SetActive(false);
		textPrefab.gameObject.SetActive(false);

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		for (int i = 0; i < 3; i++)
		{
			options.Add(new Dropdown.OptionData(((DriveType)i).ToString()));
		}
		deiveTypeInput.options = options;
		deiveTypeInput.onValueChanged.AddListener(delegate
		{
			DriveTypeChange();
		});
		deiveTypeInput.value = 0;

		ParameterShowableChange();
	}

	// Update is called once per frame
	void Update()
	{
		speedText.text = "speed = " + wd.ForwardSpeed;
		
		wd.Drive(autoSteerToggle.isOn ? (Time.realtimeSinceStartup / 10) % 2 : 0, 
				autoForwardToggle.isOn ? 1 : 0);

	}

	public void SliderValueChange(string sliderName, float value)
	{
		switch (sliderName)
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
	}

	public void ResetDefaultParameter()
	{
		string jsonStr = Resources.Load<TextAsset>("parameter").text;
		parameterJson = MiniJSON.jsonDecode(jsonStr) as Hashtable;
		foreach (Slider slider in allSliders)
		{
			InputField inputField = slidersMultiples[slider.name];
			if (parameterJson.ContainsKey(slider.name + "_Times"))
			{
				inputField.text = parameterJson[(slider.name + "_Times")].ToString();
				slider.maxValue = float.Parse(inputField.text);
			}
			if (parameterJson.ContainsKey(slider.name))
			{
				slider.value = float.Parse(parameterJson[slider.name].ToString());
			}
		}
	}

	public void DriveTypeChange()
	{
		wd.driveType = (DriveType)deiveTypeInput.value;
	}

	public void ParameterShowableChange()
	{
		displayParameter = !displayParameter;
		panel.gameObject.SetActive(displayParameter);
		displayParameterButtonText.text = displayParameter ? "关闭参数设置" : "打开参数设置";
	}

	bool SaveParameter()
	{
		File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "parameter.json", parameterJson.toJson());
		return true;
	}

	string GetParameterAnnotation(string name)
	{





		return name.Replace("Slider", "");
	}
}
