using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler: MonoBehaviour
{
	#region Fields

	[SerializeField] RectTransform _unitSelectionArea;
	[SerializeField] LayerMask _layerMask = new LayerMask();

	Vector2 _startPosition;

	RTSPlayer _player;
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
		if(_player==null)
			_player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			StartSelectionArea();
		}
		else if (Mouse.current.leftButton.wasReleasedThisFrame)
		{
			ClearSelectionArea();
		}
		else if (Mouse.current.leftButton.isPressed)
		{
			UpdateSelectionArea();
		}
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	void StartSelectionArea()
	{
		if (!Keyboard.current.leftShiftKey.isPressed)
		{
			foreach (Unit selectedUnit in SelectedUnits)
			{
				selectedUnit.Deselect();
			}
			SelectedUnits.Clear();
		}

		_unitSelectionArea.gameObject.SetActive(true);
		_startPosition = Mouse.current.position.ReadValue();

		UpdateSelectionArea();
	}

	void ClearSelectionArea()
	{
		_unitSelectionArea.gameObject.SetActive(false);

		//click with no drag...
		if (_unitSelectionArea.sizeDelta.magnitude == 0)
		{
			Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

			if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) return;

			if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;

			if (!unit.hasAuthority) return;

			SelectedUnits.Add(unit);
			foreach (Unit selectedUnit in SelectedUnits)
			{
				selectedUnit.Select();
			}

			return;
		}

		Vector2 min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
		Vector2 max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

		foreach(Unit unit in _player.GetMyUnits())
		{
			if (SelectedUnits.Contains(unit)) continue;	//prevents multiples

			Vector3 screenPositon = _mainCamera.WorldToScreenPoint(unit.transform.position);

			if (screenPositon.x > min.x && screenPositon.x < max.x && screenPositon.y > min.y && screenPositon.y < max.y)
			{
				SelectedUnits.Add(unit);
				unit.Select();
			}
		}
	}

	void UpdateSelectionArea()
	{
		Vector2 mousePosition = Mouse.current.position.ReadValue();
		float areaWidth = mousePosition.x - _startPosition.x;
		float areaHeight = mousePosition.y - _startPosition.y;

		_unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
		_unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
	}
	#endregion
}
