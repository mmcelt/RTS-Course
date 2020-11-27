using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
	#region Fields

	[SerializeField] Transform _aimAtPoint;

	#endregion

	#region Getters

	public Transform GetAimPoint()
	{
		return _aimAtPoint;
	}
	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}
	
	void Update() 
	{
		
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods


	#endregion
}
