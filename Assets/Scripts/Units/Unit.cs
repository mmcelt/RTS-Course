using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
	#region Fields

	[SerializeField] UnitMovement _unitMovement;
	[SerializeField] Targeter _targeter;
	[SerializeField] Health _health;

	[SerializeField] UnityEvent OnSelected;
	[SerializeField] UnityEvent OnDeselected;

	public static event Action<Unit> ServerOnUnitSpawned;
	public static event Action<Unit> ServerOnUnitDespawned;
	public static event Action<Unit> AuthorityOnUnitSpawned;
	public static event Action<Unit> AuthorityOnUnitDespawned;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}
	#endregion

	#region Getters

	public UnitMovement GetUnitMovement()
	{
		return _unitMovement;
	}

	public Targeter GetTargeter()
	{
		return _targeter;
	}
	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		ServerOnUnitSpawned?.Invoke(this);
		_health.ServerOnDie += ServerHandleOnDie;
	}

	public override void OnStopServer()
	{
		ServerOnUnitDespawned?.Invoke(this);
		_health.ServerOnDie -= ServerHandleOnDie;
	}

	[Server]
	void ServerHandleOnDie()
	{
		NetworkServer.Destroy(gameObject);
	}
	#endregion

	#region Client Methods

	public override void OnStartAuthority()
	{
		AuthorityOnUnitSpawned?.Invoke(this);
	}

	public override void OnStopClient()
	{
		if (!hasAuthority) return;

		AuthorityOnUnitDespawned?.Invoke(this);
	}

	[Client]
	public void Select()
	{
		if (!hasAuthority) return;

		OnSelected?.Invoke();
	}

	[Client]
	public void Deselect()
	{
		if (!hasAuthority) return;

		OnDeselected?.Invoke();
	}
	#endregion
}
