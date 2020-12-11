using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
	#region Fields

	[SerializeField] Health _health;
	[SerializeField] Unit _unitPrefab;
	[SerializeField] Transform _unitSpawnPoint;
	[SerializeField] TMP_Text _remainingUnitsText;
	[SerializeField] Image _unitProgressImage;
	[SerializeField] int _maxUnitQueue = 5;
	[SerializeField] float _spawnMoveRange = 7f;
	[SerializeField] float _unitSpawnDuration = 5f;

	[SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
	int _queuedUnits;
	[SyncVar]
	float _unitTimer;
	float progressImageVelocity;

	RTSPlayer _player;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}

	void Update()
	{
		if (isServer)
			ProduceUnits();

		if (isClient)
			UpdateTimerDisplay();

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
	void ProduceUnits()
	{
		if (_queuedUnits == 0) return;

		_unitTimer += Time.deltaTime;

		if (_unitTimer < _unitSpawnDuration) return;

		GameObject unitInstance = Instantiate(_unitPrefab.gameObject, _unitSpawnPoint.position, _unitSpawnPoint.rotation);

		NetworkServer.Spawn(unitInstance, connectionToClient);

		Vector3 spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
		spawnOffset.y = _unitSpawnPoint.position.y;
		UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
		unitMovement.ServerMove(_unitSpawnPoint.position + spawnOffset);

		_queuedUnits--;
		_unitTimer = 0f;
	}

	[Server]
	void ServerHandleOnDie()
	{
		NetworkServer.Destroy(gameObject);
	}

	[Command]
	void CmdSpawnUnit()
	{
		if (_queuedUnits == _maxUnitQueue) return;

		if (_player == null)
			_player = connectionToClient.identity.GetComponent<RTSPlayer>();

		if (_player.GetResources() < _unitPrefab.GetResourceCost()) return;

		_queuedUnits++;

		_player.SetResources(_player.GetResources() - _unitPrefab.GetResourceCost());
	}
	#endregion

	#region Client Methods

	[Client]
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		if (!hasAuthority) return;

		CmdSpawnUnit();
	}

	[Client]
	void ClientHandleQueuedUnitsUpdated(int oldAmount, int newAmount)
	{
		_remainingUnitsText.SetText(newAmount.ToString());
	}

	[Client]
	void UpdateTimerDisplay()
	{
		float newProgress = _unitTimer / _unitSpawnDuration;

		if (newProgress < _unitProgressImage.fillAmount)
			_unitProgressImage.fillAmount = newProgress;
		else
			_unitProgressImage.fillAmount = Mathf.SmoothDamp(_unitProgressImage.fillAmount, newProgress, ref progressImageVelocity, 0.1f);
	}
	#endregion
}
