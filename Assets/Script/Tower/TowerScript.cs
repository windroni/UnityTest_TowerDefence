using UnityEngine;
using System.Collections;

public class TowerScript : MonoBehaviour 
{

	public GameObject currentTarget = null;
	RoateLookTarget roateLookTarget = null;

	// Use this for initialization
	void Start () 
	{
		roateLookTarget = GetComponentInChildren<RoateLookTarget>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(currentTarget == null)
			return;

		roateLookTarget.RotateToTarget(currentTarget.transform);
	}

	void OnTriggerEnter(Collider other)
	{
		if(currentTarget == null)
			currentTarget = other.gameObject;
	}
}
