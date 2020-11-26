using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler: MonoBehaviour
{
	#region Fields

	[SerializeField] LayerMask layerMask = new LayerMask();

	Camera _mainCamera;

	List<Unit> _selectedUnits = new List<Unit>();

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_mainCamera = Camera.main;
	}
	
	void Update() 
	{
		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			//start the selection area...
			foreach(Unit selectedUnit in _selectedUnits)
			{
				selectedUnit.Deselect();
			}
			_selectedUnits.Clear();
		}
		else if (Mouse.current.leftButton.wasReleasedThisFrame)
		{
			ClearSelectionArea();
		}
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	void ClearSelectionArea()
	{
		Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

		if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

		if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;

		if (!unit.hasAuthority) return;

		_selectedUnits.Add(unit);
		foreach(Unit selectedUnit in _selectedUnits)
		{
			selectedUnit.Select();
		}
	}
	#endregion
}
