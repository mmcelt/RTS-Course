using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
	#region Fields

	[SerializeField] UnitSelectionHandler _unitSelectionHandler;
	[SerializeField] LayerMask _layerMask;

	Camera _mainCamera;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_mainCamera = Camera.main;
		GameOverHandler.ClientOnGameOver += ClientHandleOnGameOver;
	}

	void OnDestroy()
	{
		GameOverHandler.ClientOnGameOver -= ClientHandleOnGameOver;
	}

	void Update() 
	{
		if (!Mouse.current.rightButton.wasPressedThisFrame) return;

		Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

		if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) return;

		if(hit.collider.TryGetComponent(out Targetable target))
		{
			if (target.hasAuthority)
			{
				TryMove(hit.point);
				return;
			}

			TryTarget(target);
			return;
		}
		TryMove(hit.point);
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	void TryMove(Vector3 destination)
	{
		foreach(Unit unit in _unitSelectionHandler.SelectedUnits)
		{
			unit.GetUnitMovement().CmdMove(destination);
		}
	}

	void TryTarget(Targetable target)
	{
		foreach (Unit unit in _unitSelectionHandler.SelectedUnits)
		{
			unit.GetTargeter().CmdSetTarget(target.gameObject);
		}
	}

	void ClientHandleOnGameOver(string winnerName)
	{
		enabled = false;
	}
	#endregion
}
