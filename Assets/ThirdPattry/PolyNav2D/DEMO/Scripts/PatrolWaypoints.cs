using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PolyNavAgent))]
public class PatrolWaypoints : MonoBehaviour {

	public List<Vector2> WPoints = new List<Vector2>();
	private int currentIndex = -1;

	private PolyNavAgent _agent;
	public PolyNavAgent agent{
		get
		{
			if (_agent == null)
				_agent = GetComponent<PolyNavAgent>();
			return _agent;			
		}
	}

	void OnEnable(){
		agent.OnDestinationReached += MoveNext;
		agent.OnDestinationInvalid += MoveNext;
	}

	void OnDisable(){
		agent.OnDestinationReached -= MoveNext;
		agent.OnDestinationInvalid -= MoveNext;
	}

	void Start(){
		if (WPoints.Count > 0)
			MoveNext();
	}

	void MoveNext(){
		currentIndex = (int)Mathf.Repeat(currentIndex + 1, WPoints.Count);
		agent.SetDestination(WPoints[currentIndex]);
	}

	void OnDrawGizmosSelected(){
		for ( int i = 0; i < WPoints.Count; i++)
			Gizmos.DrawSphere(WPoints[i], 0.1f);			
	}
}