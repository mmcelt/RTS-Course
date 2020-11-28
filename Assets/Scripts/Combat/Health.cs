using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
	#region Fields

	[SerializeField] int _maxHealth = 100;

	[SyncVar]
	int _currentHealth;

	public event Action ServerOnDie;

	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		_currentHealth = _maxHealth;
	}

	[Server]
	public void DealDamage(int damageAmount)
	{
		if (_currentHealth == 0) return;

		_currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

		if (_currentHealth != 0) return;

		//DEAD...
		ServerOnDie?.Invoke();

		Debug.Log("WE BE DEAD!");
	}
	#endregion

	#region Client Methods


	#endregion
}
