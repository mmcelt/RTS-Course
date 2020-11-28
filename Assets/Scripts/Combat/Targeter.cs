using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
	#region Fields

	Targetable _target;

	#endregion

	#region Getters

	public Targetable GetTarget()
	{
		return _target;
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
}
