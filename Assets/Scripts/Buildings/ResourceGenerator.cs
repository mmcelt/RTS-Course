using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
	#region Fields

	[SerializeField] Health _health;
	[SerializeField] int _resourcesPerInterval = 10;
	[SerializeField] float _interval = 2f;

	float _timer;
	RTSPlayer _player;

	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		_timer = _interval;
		_player = connectionToClient.identity.GetComponent<RTSPlayer>();

		_health.ServerOnDie += ServerHandleDie;
		GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
	}

	public override void OnStopServer()
	{
		_health.ServerOnDie -= ServerHandleDie;
		GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
	}

	[ServerCallback]
	void Update()
	{
		_timer -= Time.deltaTime;

		if (_timer <= 0)
		{
			//add resources...
			_player.SetResources(_player.GetResources() + _resourcesPerInterval);

			_timer += _interval;
		}
	}

	[Server]
	void ServerHandleDie()
	{
		NetworkServer.Destroy(gameObject);
	}

	[Server]
	void ServerHandleGameOver()
	{
		enabled = false;
	}
	#endregion

	#region Client Methods


	#endregion
}
