using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VinylTrack : MonoBehaviour 
{
	public float Radius;
	public PlayMakerFSM Controller;
	public Slider SpinSlider;

	AudioSource audioSource;

	AudioClip masterClip;

	float perDegree;

	Transform spinner;

	float currentSpin;
	float currentPitch;
	float currentTime;

	float minSlider;
	float maxSlider;
	float centerSlider;
	float factor;


	void Start()
	{
		spinner = transform.parent;

		string clipName = gameObject.name + "master clip";
		int frequency = 44100;

		float normalSpeed = SpinSlider.maxValue;

		int clipLength = (int)( (2f * Mathf.PI / (normalSpeed * Mathf.Deg2Rad)) * frequency);//frequency * 10;


		perDegree = clipLength / 360f;
			
		masterClip = AudioClip.Create(clipName, clipLength, 1, frequency, false, false);


		audioSource = GetComponent<AudioSource>();
		audioSource.clip = masterClip;

		audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Fixed;

		minSlider = SpinSlider.minValue;
		maxSlider = SpinSlider.maxValue;

		centerSlider = (maxSlider - minSlider) / 2f;

		factor = 1f / centerSlider; 
	}

	void Update()
	{
		if (Mathf.Abs(currentSpin - SpinSlider.value) < float.Epsilon) { return; }

		currentSpin = SpinSlider.value;




		currentPitch = factor * currentSpin;

		audioSource.pitch = currentPitch;

		if (currentSpin < float.Epsilon && audioSource.isPlaying)	
		{
			audioSource.Stop();
			currentTime = audioSource.time;
		}
		else if ( !audioSource.isPlaying )
		{
			audioSource.time = currentTime;
			audioSource.Play();
		}
	}


	public void AttachClipToTrack(GameObject clipObject)
	{
		Debug.Log (" Updating clip for " + gameObject.name);


		AudioClip clip = clipObject.GetComponent<ClipSample>().Clip;


		Vector2 dist = clipObject.transform.position - transform.position;
		dist.Normalize();

		Vector2 normalized = dist;

		clipObject.transform.SetParent(gameObject.transform);
		clipObject.transform.up = -normalized;
		clipObject.transform.position = normalized * Radius + (Vector2)transform.position;

		normalized.y = -normalized.y;
		float cos   = Vector2.Dot(Vector2.right, normalized);
		float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;


		float adjustedAngle = spinner.eulerAngles.z  + angle - 90f;
		Debug.Log (" adjustedAngle " + adjustedAngle);

		if (adjustedAngle < 0)
		{
			adjustedAngle += 360f;
		}


		int clipSamples = Mathf.Clamp(clip.samples, 0, masterClip.samples);


		int offset = (int)(perDegree * adjustedAngle);

		if (offset + clipSamples < masterClip.samples)
		{
			float[] clipData = new float[clipSamples];
			float[] masterClipData = new float[clipSamples];


			Debug.Log (" clip.samples " + clipSamples);

			masterClip.GetData(masterClipData, offset);
			clip.GetData(clipData, 0);

			int i = 0;
			while(i < masterClipData.Length)
			{
				masterClipData[i] = masterClipData[i] + clipData[i];
				++i;
			}

			masterClip.SetData(clipData, offset);
		}


		// clip is on the edge of master clip
		else
		{

			float[] clipData = new float[clipSamples];
			float[] masterClipDataTail   = new float[masterClip.samples - offset];
			float[] masterClipDataStart = new float[clipSamples - masterClipDataTail.Length];


			Debug.Log (" clip.samples " + clipSamples);

			masterClip.GetData(masterClipDataTail, offset);
			masterClip.GetData(masterClipDataStart, 0);
			clip.GetData(clipData, 0);

			int i = 0;
			while(i < masterClipDataTail.Length)
			{
				masterClipDataTail[i] = masterClipDataTail[i] + clipData[i];
				++i;
			}

			int j = 0;
			while(j < masterClipDataStart.Length)
			{
				masterClipDataStart[j] = masterClipDataStart[j] + clipData[i];
				++i;
				++j;
			}

			masterClip.SetData(masterClipDataTail, offset);
			masterClip.SetData(masterClipDataStart, 0);


		}

	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, Radius);
	}
}
