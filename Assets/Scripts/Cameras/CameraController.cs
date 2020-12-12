using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
	#region Fields

	[SerializeField] Transform _playerCameraTransform;
	[SerializeField] float _speed = 20f;
	[SerializeField] float _screenBorderThickness = 10f;
	[SerializeField] Vector2 _screenXLimits;
	[SerializeField] Vector2 _screenZLimits;

	Vector2 _previousInput;

	Controls _controls;

	#endregion

	#region MonoBehaviour Methods

	[ClientCallback]
	void Update() 
	{
		if (!hasAuthority || !Application.isFocused) return;

		UpdateCameraPosition();
	}
	#endregion

	public override void OnStartAuthority()
	{
		_playerCameraTransform.gameObject.SetActive(true);
		_controls = new Controls();

		_controls.Player.MoveCamera.performed += SetPreviousInput;
		_controls.Player.MoveCamera.canceled += SetPreviousInput;

		_controls.Enable();
	}

	#region Public Methods


	#endregion

	#region Private Methods

	void SetPreviousInput(InputAction.CallbackContext ctx)
	{
		_previousInput = ctx.ReadValue<Vector2>();
	}

	void UpdateCameraPosition()
	{
		Vector3 pos = _playerCameraTransform.position;

		if (_previousInput == Vector2.zero)	//no keyboard input
		{
			Vector3 cursorMovement = Vector3.zero;
			Vector2 cursorPosition = Mouse.current.position.ReadValue();

			if (cursorPosition.y >= Screen.height - _screenBorderThickness)
			{
				cursorMovement.z += 1;

			}
			else if (cursorPosition.y <= _screenBorderThickness)
			{
				cursorMovement.z -= 1;
			}
			if (cursorPosition.x >= Screen.width - _screenBorderThickness)
			{
				cursorMovement.x += 1;

			}
			else if (cursorPosition.x <= _screenBorderThickness)
			{
				cursorMovement.x -= 1;
			}

			pos += cursorMovement.normalized * _speed * Time.deltaTime;
		}
		else
		{
			pos += new Vector3(_previousInput.x, 0f, _previousInput.y) * _speed * Time.deltaTime;
		}

		pos.x = Mathf.Clamp(pos.x, _screenXLimits.x, _screenXLimits.y);
		pos.z = Mathf.Clamp(pos.z, _screenZLimits.x, _screenZLimits.y);

		_playerCameraTransform.position = pos;
	}
	#endregion
}
