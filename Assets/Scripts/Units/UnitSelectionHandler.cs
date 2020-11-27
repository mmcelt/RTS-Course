using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler: MonoBehaviour
{
	#region Fields

	[SerializeField] LayerMask _layerMask = new LayerMask();

	Camera _mainCamera;

	public List<Unit> SelectedUnits { get; } = new List<Unit>();

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
			foreach(Unit selectedUnit in SelectedUnits)
			{
				selectedUnit.Deselect();
			}
			SelectedUnits.Clear();
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

		if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) return;

		if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;

		if (!unit.hasAuthority) return;

		SelectedUnits.Add(unit);
		foreach(Unit selectedUnit in SelectedUnits)
		{
			selectedUnit.Select();
		}
	}
	#endregion
}
