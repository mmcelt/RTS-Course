using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement: NetworkBehaviour
{
	#region Fields

	[SerializeField] NavMeshAgent _navAgent;

	Camera _mainCamera;

	#endregion

	#region Server Methods

	[Command]
	void CmdMove(Vector3 position)
	{
		if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;
		_navAgent.SetDestination(hit.position);
	}
	#endregion

	#region Client Methods

	public override void OnStartAuthority()
	{
		_mainCamera = Camera.main;
	}

	[ClientCallback]	//prevents the server from running this code
	void Update()
	{
		if (!hasAuthority) return;	//if not the local player, return
		if (!Mouse.current.rightButton.wasPressedThisFrame) return;

		Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

		if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;

		CmdMove(hit.point);
	}
	#endregion
}
