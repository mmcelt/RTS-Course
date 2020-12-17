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

	public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

	bool _isGameInProgress;

	#endregion

	#region Server Methods

	public override void OnServerConnect(NetworkConnection conn)
	{
		if (!_isGameInProgress) return;

		conn.Disconnect();
	}

	public override void OnServerDisconnect(NetworkConnection conn)
	{
		RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

		Players.Remove(player);

		base.OnServerDisconnect(conn);
	}

	public override void OnStopServer()
	{
		Players.Clear();
		_isGameInProgress = false;
	}

	public void StartGame()
	{
		if (Players.Count < 2) return;

		_isGameInProgress = true;

		ServerChangeScene("Scene_Map_01");
	}

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		base.OnServerAddPlayer(conn);

		RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

		Players.Add(player);

		player.SetTeamColor(new Color(
			UnityEngine.Random.Range(0f, 1f),
			UnityEngine.Random.Range(0f, 1f),
			UnityEngine.Random.Range(0f, 1f)
		));

		player.SetPartyOwner(Players.Count == 1);
	}

	public override void OnServerSceneChanged(string sceneName)
	{
		if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
		{
			GameOverHandler gameOverHandlerInstance = Instantiate(_gameOverHandlerPrefab);
			NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

			foreach(RTSPlayer player in Players)
			{
				//spawn in each player's base...
				GameObject baseInstance = Instantiate(_unitBasePrefab, GetStartPosition().position, Quaternion.identity);

				NetworkServer.Spawn(baseInstance, player.connectionToClient);
			}
		}
	}
	#endregion

	#region Client Methods

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

	public override void OnStopClient()
	{
		Players.Clear();
	}
	#endregion
}
