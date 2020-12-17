using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	#region Fields

	[SerializeField] GameObject _landingPagePanel;
	[SerializeField] bool _useSteam;

	protected Callback<LobbyCreated_t> _lobbyCreated;
	protected Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
	protected Callback<LobbyEnter_t> _lobbyEntered;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		if (!_useSteam) return;

		_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		_gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
		_lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnterd);
	}
	
	//void Update() 
	//{
		
	//}
	#endregion

	#region Public Methods

	public void HostLobby()
	{
		_landingPagePanel.SetActive(false);

		if (_useSteam)
		{
			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
			return;
		}

		NetworkManager.singleton.StartHost();
	}
	#endregion

	#region Private Methods

	void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult != EResult.k_EResultOK)
		{
			_landingPagePanel.SetActive(true);
			return;
		}

		NetworkManager.singleton.StartHost();

		SteamMatchmaking.SetLobbyData(
			new CSteamID(callback.m_ulSteamIDLobby),
			"HostAddress",
			SteamUser.GetSteamID().ToString());
	}

	void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
	{
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}

	void OnLobbyEnterd(LobbyEnter_t callback)
	{
		if (NetworkServer.active) return;

		string hostAddress = SteamMatchmaking.GetLobbyData(
			new CSteamID(callback.m_ulSteamIDLobby),
			"HostAddress");

		NetworkManager.singleton.networkAddress = hostAddress;
		NetworkManager.singleton.StartClient();

		_landingPagePanel.SetActive(false);
	}
	#endregion
}
