using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
	#region Fields

	[SerializeField] Health _health;

	public static event Action<int> ServerOnPlayerDie;
	public static event Action<UnitBase> ServerOnBaseSpawned;
	public static event Action<UnitBase> ServerOnBaseDespawned;

	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		_health.ServerOnDie += ServerHandleOnDie;
		ServerOnBaseSpawned?.Invoke(this);
	}

	public override void OnStopServer()
	{
		ServerOnBaseDespawned?.Invoke(this);
		_health.ServerOnDie -= ServerHandleOnDie;
	}

	void ServerHandleOnDie()
	{
		ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

		NetworkServer.Destroy(gameObject);
	}
	#endregion

	#region Client Methods


	#endregion
}
