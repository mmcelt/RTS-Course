using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
	#region Fields

	[SerializeField] GameObject _unitSpawnerPrefab;
	[SerializeField] GameOverHandler _gameOverHandlerPrefab;

	#endregion

	#region Server Methods

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		base.OnServerAddPlayer(conn);

		RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

		player.SetTeamColor(new Color(
			Random.Range(0f, 1f), 
			Random.Range(0f, 1f), 
			Random.Range(0f, 1f)
		));

		GameObject unitSpawnerInstance = Instantiate(_unitSpawnerPrefab, conn.identity.transform.position, Quaternion.identity);

		NetworkServer.Spawn(unitSpawnerInstance, conn);
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
