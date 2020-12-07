using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
	#region Fields

	[SerializeField] TMP_Text _resourcesText;

	RTSPlayer _player;
	bool _subscribed;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}
	
	void Update() 
	{
		if (_player == null)
			_player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

		if (_player != null && !_subscribed)
		{
			ClientHandleResourcesUpdated(_player.GetResources());

			_player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
			_subscribed = true;
		}
	}

	void OnDestroy()
	{
		_player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	void ClientHandleResourcesUpdated(int amount)
	{
		_resourcesText.SetText($"Resources: {amount}");
	}
	#endregion
}
