using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
	#region Fields

	[SerializeField] GameObject _landingPagePanel;
	[SerializeField] TMP_InputField _addressInput;
	[SerializeField] Button _joinButton;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}

	void OnEnable()
	{
		RTSNetworkManager.ClientOnConnected += HandleClientConnected;
		RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
	}

	void OnDisable()
	{
		RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
		RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
	}

	#endregion

	#region Public Methods

	public void Join()
	{
		string address = _addressInput.text;

		NetworkManager.singleton.networkAddress = address;
		NetworkManager.singleton.StartClient();

		_joinButton.interactable = false;
	}
	#endregion

	#region Private Methods

	void HandleClientConnected()
	{
		_joinButton.interactable = true;
		gameObject.SetActive(false);
		_landingPagePanel.SetActive(false);

	}

	void HandleClientDisconnected()
	{
		_joinButton.interactable = true;
	}
	#endregion
}
