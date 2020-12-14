using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	#region Fields

	[SerializeField] GameObject _landingPagePanel;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}
	
	//void Update() 
	//{
		
	//}
	#endregion

	#region Public Methods

	public void HostLobby()
	{
		_landingPagePanel.SetActive(false);

		NetworkManager.singleton.StartHost();

	}
	#endregion

	#region Private Methods


	#endregion
}
