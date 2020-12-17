using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
	#region Fields

	[SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
	[SerializeField] int _resources = 500;

	[SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] 
	bool _isPartyOwner;

	[SerializeField] LayerMask _buildingBlockLayer;
	[SerializeField] float _buildingRangeLimit = 5f;
	[SerializeField] Building[] _buildings;
	[SerializeField] Transform _cameraTransform;

	List <Unit> _myUnits = new List<Unit>();
	List<Building> _myBuildings = new List<Building>();

	Color _teamColor;

	public event Action<int> ClientOnResourcesUpdated;
	public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

	#endregion

	#region Getter Methods

	public int GetResources()
	{
		return _resources;
	}

	public bool GetPartyOwner()
	{
		return _isPartyOwner;
	}

	public List<Unit> GetMyUnits()
	{
		return _myUnits;
	}

	public List<Building> GetMyBuildings()
	{
		return _myBuildings;
	}

	public Color GetTeamColor()
	{
		return _teamColor;
	}
	
	public Transform GetCameraTransform()
	{
		return _cameraTransform;
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

	[Server]
	public void SetPartyOwner(bool state)
	{
		_isPartyOwner = state;
	}

	[Server]
	public void SetResources(int amount)
	{
		_resources = amount;
	}

	[Server]
	public void SetTeamColor(Color newColor)
	{
		_teamColor = newColor;
	}

	[Command]
	public void CmdStartGame()
	{
		if (!_isPartyOwner) return;

		((RTSNetworkManager)NetworkManager.singleton).StartGame();
	}

	[Command]
	public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
	{
		Building buildingToPlace = null;
		//ensure the building is a valid building...
		foreach(Building building in _buildings)
		{
			if (building.GetId() == buildingId)
			{
				buildingToPlace = building;
				break;
			}
		}

		if (buildingToPlace == null) return;
		//ensure we have sufficient resources...
		if (_resources < buildingToPlace.GetPrice()) return;

		BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
		//check that we're not overlapping anything...
		if (!CanPlaceBuilding(buildingCollider, point))
		{
			//Debug.Log("IN CMD false");
			return;
		}

		//spawn the building...
		GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, point, Quaternion.identity);

		NetworkServer.Spawn(buildingInstance, connectionToClient);
		//take the required resources...
		SetResources(_resources - buildingToPlace.GetPrice());
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

	void ClientHandleResourcesUpdated(int oldAmount, int newAmount)
	{
		ClientOnResourcesUpdated?.Invoke(newAmount);
	}

	public override void OnStartAuthority()
	{
		if (NetworkServer.active) return;
		//if (isServer) return;

		Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
		Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

		Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
		Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
	}

	public override void OnStartClient()
	{
		if (NetworkServer.active) return;

		((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
	}

	public override void OnStopClient()
	{
		if (!isClientOnly) return;

		((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);

		if (!hasAuthority) return;

		Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
		Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

		Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
		Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
	}

	void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
	{
		if (!hasAuthority) return;

		AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
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

	public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
	{
		//Debug.Log("In CPB...");

		if (Physics.CheckBox(
			point + buildingCollider.center, 
			buildingCollider.size / 2, 
			Quaternion.identity, 
			_buildingBlockLayer))
		{
			//Debug.Log("This is false due to interference...");

			return false;
		}

		//bool inRange = false;
		//check if we're placing this building within range on one of our other buildings...
		foreach (Building building in _myBuildings)
		{
			if ((point - building.transform.position).sqrMagnitude <= _buildingRangeLimit * _buildingRangeLimit)
			{
				//Debug.Log("This is true...");

				return true;
			}
		}
		//Debug.Log("This is false due to not in range...");

		return false;
	}
}
