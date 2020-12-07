using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
	#region Fields

	[SerializeField] GameObject _buildingPreview;
	[SerializeField] Sprite _icon;
	[SerializeField] int _id = -1;
	[SerializeField] int _price = 100;

	public static event Action<Building> ServerOnBuildingSpawned;
	public static event Action<Building> ServerOnBuildingDespawned;

	public static event Action<Building> AuthorityOnBuildingSpawned;
	public static event Action<Building> AuthorityOnBuildingDespawned;

	#endregion

	#region Getters

	public GameObject GetBuildingPreview()
	{
		return _buildingPreview;
	}

	public Sprite GetIcon()
	{
		return _icon;
	}

	public int GetId()
	{
		return _id;
	}

	public int GetPrice()
	{
		return _price;
	}
	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		ServerOnBuildingSpawned?.Invoke(this);
	}

	public override void OnStopServer()
	{
		ServerOnBuildingDespawned?.Invoke(this);
	}
	#endregion

	#region Client Methods

	public override void OnStartAuthority()
	{
		AuthorityOnBuildingSpawned?.Invoke(this);
	}

	public override void OnStopClient()
	{
		if (!hasAuthority) return;

		AuthorityOnBuildingDespawned?.Invoke(this);
	}

	#endregion
}
