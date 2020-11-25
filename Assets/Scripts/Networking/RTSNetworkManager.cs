using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
	#region Fields

	[SerializeField] GameObject _unitSpawnerPrefab;

	#endregion

	#region MonoBehaviour Methods


	#endregion

	#region Server Methods

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		base.OnServerAddPlayer(conn);

		GameObject unitSpawnerInstance = Instantiate(_unitSpawnerPrefab, conn.identity.transform.position, Quaternion.identity);

		NetworkServer.Spawn(unitSpawnerInstance, conn);
	}
	#endregion

	#region Private Methods


	#endregion
}
