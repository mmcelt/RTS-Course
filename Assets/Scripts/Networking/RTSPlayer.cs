using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
	#region Fields

	List<Unit> _myUnits = new List<Unit>();
	List<Building> _myBuildings = new List<Building>();

	#endregion

	#region Getter Methods

	public List<Unit> GetMyUnits()
	{
		return _myUnits;
	}

	public List<Building> GetMyBuildings()
	{
		return _myBuildings;
	}
	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
		Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

		Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
		Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
	}

	public override void OnStopServer()
	{
		Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
		Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

		Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
		Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
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
	
	void ServerHandleBuildingSpawned(Building building)
	{
		if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
		_myBuildings.Add(building);
	}

	void ServerHandleBuildingDespawned(Building building)
	{
		if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
		_myBuildings.Add(building);
	}
	#endregion

	#region Client Methods

	public override void OnStartAuthority()
	{
		if (NetworkServer.active) return;
		//if (isServer) return;

		Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
		Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitDespawned;

		Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
		Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingDespawned;
	}

	public override void OnStopClient()
	{
		if (!isClientOnly || !hasAuthority) return;

		Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
		Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitDespawned;

		Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
		Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingDespawned;
	}

	void AuthorityHandleUnitSpawned(Unit unit)
	{
		_myUnits.Add(unit);
	}

	void AuthorityHandleUnitDespawned(Unit unit)
	{
		_myUnits.Remove(unit);
	}

	void AuthorityHandleBuildingSpawned(Building building)
	{
		_myBuildings.Add(building);
	}

	void AuthorityHandleBuildingDespawned(Building building)
	{
		_myBuildings.Remove(building);
	}
	#endregion
}
