using UnityEngine;
using System.Collections;

public static class AudioClipExtension 
{

	public static AudioClip Combine(this AudioClip thisClip, params AudioClip[] clips)
	{
		if (clips == null || clips.Length == 0)
		{
			return null;
		}
			

		int length = 0;
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i] == null)
			{
				continue;
			}
				

			length += clips[i].samples * clips[i].channels;
		}

		float[] data = new float[length];
		length = 0;
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i] == null)
			{
				continue;
			}
				

			float[] buffer = new float[clips[i].samples * clips[i].channels];
			clips[i].GetData(buffer, 0);
			//System.Buffer.BlockCopy(buffer, 0, data, length, buffer.Length);
			buffer.CopyTo(data, length);
			length += buffer.Length;
		}

		if (length == 0)
		{
			return null;
		}
			

		AudioClip result = AudioClip.Create("Combine", length / 2, 2, 44100, false, false);
		result.SetData(data, 0);

		return result;
	}
}
