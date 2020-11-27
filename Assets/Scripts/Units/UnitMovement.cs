using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement: NetworkBehaviour
{
	#region Fields

	[SerializeField] NavMeshAgent _navAgent;

	#endregion

	#region Server Methods

	[Command]
	public void CmdMove(Vector3 position)
	{
		if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;
		_navAgent.SetDestination(hit.position);
	}
	#endregion

	#region Client Methods


	#endregion
}
