using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MiniMap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
	#region Fields

	[SerializeField] RectTransform _minimapRect;
	[SerializeField] float _mapScale = 20f;   //This is assuming a square mini map
	[SerializeField] float _offset = -6f;

	Transform _playerCameraTransform;

	#endregion

	#region MonoBehaviour Methods
	
	void Update() 
	{
		if (_playerCameraTransform != null) return;

		if (NetworkClient.connection.identity == null) return;

		_playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();
	}
	#endregion

	#region Public Methods

	public void OnPointerDown(PointerEventData eventData)
	{
		MoveCamera();
	}

	public void OnDrag(PointerEventData eventData)
	{
		MoveCamera();
	}
	#endregion

	#region Private Methods

	void MoveCamera()
	{
		Vector2 mousPos = Mouse.current.position.ReadValue();

		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
			_minimapRect,
			mousPos,
			null,
			out Vector2 localPoint
			)) return;

		Vector2 lerp = new Vector2(
			(localPoint.x - _minimapRect.rect.x) / _minimapRect.rect.width,
			(localPoint.y - _minimapRect.rect.y) / _minimapRect.rect.height);

		Vector3 newCameraPos = new Vector3(
			Mathf.Lerp(-_mapScale, _mapScale, lerp.x),
			_playerCameraTransform.position.y,
			Mathf.Lerp(-_mapScale, _mapScale, lerp.y));

		_playerCameraTransform.position = newCameraPos + new Vector3(0f, 0f, _offset);
	}
	#endregion
}
