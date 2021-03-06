﻿using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement: NetworkBehaviour
{
	#region Fields

	[SerializeField] NavMeshAgent _navAgent;
	[SerializeField] Targeter _targeter;

	[SerializeField] float _chaseRange = 10f;

	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		GameOverHandler.ServerOnGameOver += ServerHandleOnGameOver;
	}

	public override void OnStopServer()
	{
		GameOverHandler.ServerOnGameOver -= ServerHandleOnGameOver;
	}

	[ServerCallback]
	void Update()
	{
		if (_targeter.GetTarget() != null)
		{
			Targetable target = _targeter.GetTarget();

			if ((target.transform.position - transform.position).sqrMagnitude > _chaseRange * _chaseRange)
			{
				//chase
				_navAgent.SetDestination(target.transform.position);
			}
			else if (_navAgent.hasPath)
			{
				//stop chasing
				_navAgent.ResetPath();
			}

			return;
		}

		if (!_navAgent.hasPath) return;
		if (_navAgent.remainingDistance > _navAgent.stoppingDistance) return;

		_navAgent.ResetPath();
	}

	[Command]
	public void CmdMove(Vector3 position)
	{
		ServerMove(position);
	}

	[Server]
	public void ServerMove(Vector3 position)
	{
		_targeter.ClearTarget();

		if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;
		_navAgent.SetDestination(hit.position);
	}

	[Server]
	void ServerHandleOnGameOver()
	{
		_navAgent.ResetPath();
	}
	#endregion

	#region Client Methods


	#endregion
}
