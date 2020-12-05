using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
	#region Fields

	[SerializeField] GameObject _unitPrefab;
	[SerializeField] Transform _unitSpawnPoint;
	[SerializeField] Health _health;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}
	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		_health.ServerOnDie += ServerHandleOnDie;
	}

	public override void OnStopServer()
	{
		_health.ServerOnDie -= ServerHandleOnDie;
	}

	[Server]
	void ServerHandleOnDie()
	{
		//NetworkServer.Destroy(gameObject);
	}

	[Command]
	void CmdSpawnUnit()
	{
		GameObject unitInstance = Instantiate(_unitPrefab, _unitSpawnPoint.position, _unitSpawnPoint.rotation);

		NetworkServer.Spawn(unitInstance, connectionToClient);
	}
	#endregion

	#region Client Methods

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		if (!hasAuthority) return;

		CmdSpawnUnit();
	}
	#endregion
}
