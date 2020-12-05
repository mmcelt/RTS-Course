using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
	#region Fields

	[SerializeField] GameObject _gameOverDisplayParent;
	[SerializeField] TMP_Text _winnerNameText;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
	}

	void OnDestroy()
	{
		GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
	}
	#endregion

	#region Public Methods

	public void LeaveGame()
	{
		if(NetworkServer.active && NetworkClient.isConnected)
		{
			//stop Hosting...
			NetworkManager.singleton.StopHost();
		}
		else
		{
			//stop Client...
			NetworkManager.singleton.StopClient();
		}
	}
	#endregion

	#region Private Methods

	void ClientHandleGameOver(string winner)
	{
		_winnerNameText.SetText($"{winner} Has Won!!");
		_gameOverDisplayParent.SetActive(true);
	}
	#endregion
}
