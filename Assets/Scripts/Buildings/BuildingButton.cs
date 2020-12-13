using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	#region Fields

	[SerializeField] Building _building;
	[SerializeField] Image _iconImage;
	[SerializeField] TMP_Text _priceText;
	[SerializeField] LayerMask _floorMask;

	Camera _mainCamera;
	BoxCollider _buildingCollider;

	RTSPlayer _player;
	GameObject _buildingPreviewInstance;
	Renderer _buildingRendererInstance;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_mainCamera = Camera.main;
		_buildingCollider = _building.GetComponent<BoxCollider>();
		_iconImage.sprite = _building.GetIcon();
		_priceText.SetText(_building.GetPrice().ToString());
		_player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
	}
	
	void Update() 
	{
		if(_player==null)
			_player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

		if (_buildingPreviewInstance == null) return;

		UpdateBuildiingPreview();
	}
	#endregion

	#region Public Methods

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;

		if (_player.GetResources() < _building.GetPrice()) return;

		_buildingPreviewInstance = Instantiate(_building.GetBuildingPreview());
		_buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();
		_buildingPreviewInstance.SetActive(false);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (_buildingPreviewInstance == null) return;

		Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
		{
			//place building...
			_player.CmdTryPlaceBuilding(_building.GetId(), hit.point);
		}

		Destroy(_buildingPreviewInstance);

	}
	#endregion

	#region Private Methods

	void UpdateBuildiingPreview()
	{
		Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

		if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask)) return;

		_buildingPreviewInstance.transform.position = hit.point;

		if (!_buildingPreviewInstance.activeSelf)
			_buildingPreviewInstance.SetActive(true);

		//Debug.Log("In UBP...");

		Color color = _player.CanPlaceBuilding(_buildingCollider, hit.point) ? Color.green : Color.red;

		//Debug.Log("Color: " + color);

		_buildingRendererInstance.material.SetColor("_BaseColor", color);
	}
	#endregion
}
