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
	[SerializeField] int _damageToDeal = 20;

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

	[ServerCallback]
	void OnTriggerEnter(Collider other)
	{
		if(other.TryGetComponent(out NetworkIdentity networkIdentity))
		{
			//we hit out own unit...
			if (networkIdentity.connectionToClient == connectionToClient) return;
		}

		if(other.TryGetComponent(out Health health))
		{
			health.DealDamage(_damageToDeal);
		}

		SelfDestruct();
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
