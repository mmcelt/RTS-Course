using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
	#region Fields

	[SerializeField] Rigidbody _rb;
	[SerializeField] float _launchForce = 10f;
	[SerializeField] float _lifetime = 5f;

	#endregion

	#region NetworkBehaviour Methods

	void Start() 
	{
		_rb.velocity = transform.forward * _launchForce;
	}

	public override void OnStartServer()
	{
		Invoke(nameof(SelfDestruct), _lifetime);
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	[Server]
	void SelfDestruct()
	{
		NetworkServer.Destroy(gameObject);
	}
	#endregion
}
