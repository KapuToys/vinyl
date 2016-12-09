using UnityEngine;
using System.Collections;

public class VinylLeadDebug : MonoBehaviour {

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawLine(-transform.up * 3f + transform.position, -transform.up * 14f + transform.position);
	}
}
