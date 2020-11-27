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
	}
	
	void Update() 
	{
		if (!Mouse.current.rightButton.wasPressedThisFrame) return;

		Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

		if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) return;

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
	#endregion
}
