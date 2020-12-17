using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
	#region Fields

	[SerializeField] GameObject _lobbyUI;
	[SerializeField] Button _startGameButton;

	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		RTSNetworkManager.ClientOnConnected += HandleClientConnected;
		RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
	}

	void OnDisable()
	{
		RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
		RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
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

	public void StartGame()
	{
		NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
	}

	#endregion

	#region Private Methods

	void HandleClientConnected()
	{
		_lobbyUI.SetActive(true);
	}

	void AuthorityHandlePartyOwnerStateUpdated(bool state)
	{
		_startGameButton.gameObject.SetActive(state);
	}
	#endregion
}
