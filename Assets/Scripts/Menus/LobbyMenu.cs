using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
	#region Fields

	[SerializeField] GameObject _lobbyUI;

	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		RTSNetworkManager.ClientOnConnected += HandleClientConnected;
	}

	void OnDisable()
	{
		RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
	}
	#endregion

	#region Public Methods

	public void LeaveLobby()
	{
		if (NetworkServer.active && NetworkClient.isConnected)	//you are the host
		{
			NetworkManager.singleton.StopHost();
		}
		else
		{
			NetworkManager.singleton.StopClient(); //you are just a client

			SceneManager.LoadScene(0);
		}
	}
	#endregion

	#region Private Methods

	void HandleClientConnected()
	{
		_lobbyUI.SetActive(true);
	}
	#endregion
}
