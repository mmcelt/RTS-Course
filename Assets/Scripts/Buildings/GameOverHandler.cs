using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
	#region Fields

	public static event Action ServerOnGameOver;
	public static event Action<string> ClientOnGameOver;

	List<UnitBase> _bases = new List<UnitBase>();

	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
		UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
	}

	public override void OnStopServer()
	{
		UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
		UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
	}

	[Server]
	void ServerHandleBaseSpawned(UnitBase unitBase)
	{
		_bases.Add(unitBase);
	}

	[Server]
	void ServerHandleBaseDespawned(UnitBase unitBase)
	{
		_bases.Remove(unitBase);

		if (_bases.Count != 1) return;

		//only 1 Player left...
		int playerID = _bases[0].connectionToClient.connectionId;

		RpcGameOver($"Player {playerID}");

		ServerOnGameOver?.Invoke();
	}
	#endregion

	#region Client Methods

	[ClientRpc]
	void RpcGameOver(string winner)
	{
		ClientOnGameOver?.Invoke(winner);
	}
	#endregion
}
