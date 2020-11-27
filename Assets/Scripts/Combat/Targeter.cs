using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
	#region Fields

	[SerializeField] Targetable _target;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}
	
	void Update() 
	{
		
	}
	#endregion

	#region Server Methods

	[Command]
	public void CmdSetTarget(GameObject targetGO)
	{
		if (!targetGO.TryGetComponent(out Targetable target)) return;

		_target = target;
	}

	[Server]
	public void ClearTarget()
	{
		_target = null;
	}
	#endregion

	#region Client Methods


	#endregion
}
