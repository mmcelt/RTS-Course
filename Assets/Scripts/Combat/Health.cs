using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
	#region Fields

	[SerializeField] int _maxHealth = 100;

	[SyncVar(hook = nameof(HandleHealthUpdated))]
	int _currentHealth;

	public event Action ServerOnDie;
	public event Action<int, int> ClientOnHealthUpdated;

	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		_currentHealth = _maxHealth;
		UnitBase.ServerOnPlayerDie += ServerHandleOnPlayerDie;
	}

	public override void OnStopServer()
	{
		UnitBase.ServerOnPlayerDie -= ServerHandleOnPlayerDie;
	}

	[Server]
	public void DealDamage(int damageAmount)
	{
		if (_currentHealth == 0) return;

		_currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

		if (_currentHealth != 0) return;

		//DEAD...
		ServerOnDie?.Invoke();
	}

	[Server]
	void ServerHandleOnPlayerDie(int deadID)
	{
		if (connectionToClient.connectionId != deadID) return;

		DealDamage(_currentHealth);
	}
	#endregion

	#region Client Methods

	void HandleHealthUpdated(int oldHealth,int newHealth)
	{
		ClientOnHealthUpdated?.Invoke(newHealth, _maxHealth);
	}
	#endregion
}
