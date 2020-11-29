using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
	#region Fields

	[SerializeField] List<Unit> _myUnits = new List<Unit>();

	#endregion

	#region Getter Methods

	public List<Unit> GetMyUnits()
	{
		return _myUnits;
	}
	#endregion

	#region MonoBehaviour Methods

	void Start()
	{

	}
	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
		Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
	}

	public override void OnStopServer()
	{
		Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
		Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
	}

	void ServerHandleUnitSpawned(Unit unit)
	{
		if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
		_myUnits.Add(unit);
	}

	void ServerHandleUnitDespawned(Unit unit)
	{
		if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
		_myUnits.Remove(unit);
	}
	#endregion

	#region Client Methods

	public override void OnStartAuthority()
	{
		if (NetworkServer.active) return;
		//if (isServer) return;

		Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
		Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitDespawned;
	}

	public override void OnStopClient()
	{
		if (!isClientOnly || !hasAuthority) return;

		Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
		Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitDespawned;
	}

	void AuthorityHandleUnitSpawned(Unit unit)
	{
		_myUnits.Add(unit);
	}

	void AuthorityHandleUnitDespawned(Unit unit)
	{
		_myUnits.Remove(unit);
	}
	#endregion
}
