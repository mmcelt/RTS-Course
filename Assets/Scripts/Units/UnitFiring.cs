using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
	#region Fields

	[SerializeField] Targeter _targeter;
	[SerializeField] GameObject _projectilePrefab;
	[SerializeField] Transform _firePoint;
	[SerializeField] float _fireRange = 5f, _fireRate = 1f, _rotationSpeed = 20f;

	float _lastFireTime;

	#endregion

	#region Server Methods

	[ServerCallback]
	void Update()
	{
		Targetable target = _targeter.GetTarget();

		if ( target == null) return;
		if (!CanFireAtTarget()) return;

		Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

		if(Time.time > (1 / _fireRate) + _lastFireTime)
		{
			//we can fire now...

			Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimPoint().position - _firePoint.position);

			GameObject projectileGO = Instantiate(_projectilePrefab, _firePoint.position, projectileRotation);

			NetworkServer.Spawn(projectileGO, connectionToClient);

			_lastFireTime = Time.time;
		}
	}

	[Server]
	bool CanFireAtTarget()
	{
		//distance check...
		return (_targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= _fireRange * _fireRange;
	}
	#endregion

	#region Private Methods


	#endregion
}
