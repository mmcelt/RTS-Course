using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
	#region Fields

	[SerializeField] GameObject _unitBasePrefab;
	[SerializeField] GameOverHandler _gameOverHandlerPrefab;

	public static Action ClientOnConnected;
	public static Action ClientOnDisconnected;

	#endregion

	#region Server Methods

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		base.OnServerAddPlayer(conn);

		RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

		player.SetTeamColor(new Color(
			UnityEngine.Random.Range(0f, 1f),
			UnityEngine.Random.Range(0f, 1f),
			UnityEngine.Random.Range(0f, 1f)
		));
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);
		ClientOnConnected?.Invoke();
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect(conn);
		ClientOnDisconnected?.Invoke();
	}

	public override void OnServerSceneChanged(string sceneName)
	{
		if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
		{
			GameOverHandler gameOverHandlerInstance = Instantiate(_gameOverHandlerPrefab);
			NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
		}
	}
	#endregion
}
