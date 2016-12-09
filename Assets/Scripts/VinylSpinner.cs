using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class VinylSpinner : MonoBehaviour 
{
	public Slider SpinSlider;

	void Update () 
	{
		transform.Rotate(0f, 0f, SpinSlider.value * Time.deltaTime);
	}
}
