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

	public UnitMovement GetUnitMovement()
	{
		return _unitMovement;
	}

	#region Server Methods

	public override void OnStartServer()
	{
		ServerOnUnitSpawned?.Invoke(this);
	}

	public override void OnStopServer()
	{
		ServerOnUnitDespawned?.Invoke(this);
	}
	#endregion

	#region Client Methods

	public override void OnStartClient()
	{
		//IF you are the HOST OR the unit isn't yours - RETURN
		if (!isClientOnly || !hasAuthority) return;

		AuthorityOnUnitSpawned?.Invoke(this);
	}

	public override void OnStopClient()
	{
		if (!isClientOnly || !hasAuthority) return;

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
