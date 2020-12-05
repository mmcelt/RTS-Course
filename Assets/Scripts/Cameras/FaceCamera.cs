using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	#region Fields

	Transform _mainCameraTransform;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_mainCameraTransform = Camera.main.transform;
	}
	
	void LateUpdate() 
	{
		transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward, _mainCameraTransform.rotation * Vector3.up);
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods


	#endregion
}
