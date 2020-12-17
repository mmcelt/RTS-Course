using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
	#region Fields

	[SerializeField] GameObject _lobbyUI;
	[SerializeField] Button _startGameButton;
	[SerializeField] TMP_Text[] _playerNameTexts;

	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		RTSNetworkManager.ClientOnConnected += HandleClientConnected;
		RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
		RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
	}

	void OnDisable()
	{
		RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
		RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
		RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
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

	void ClientHandleInfoUpdated()
	{
		List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).Players;

		for (int i = 0; i < players.Count; i++)
		{
			_playerNameTexts[i].text = players[i].GetDisplayName();
		}

		for (int i = players.Count; i < _playerNameTexts.Length; i++)
		{
			_playerNameTexts[i].text = "Waiting For Player...";
		}

		_startGameButton.interactable = players.Count > 1;
	}
	#endregion
}
