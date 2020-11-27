using Mirror;
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

	#region Public Client Methods

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

	#region Private Client Methods


	#endregion
}
