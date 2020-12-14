using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
	#region Fields

	[SerializeField] Renderer[] _colorRenderers;

	[SyncVar(hook = nameof(ClientHandleTeamColorUpdated))]
	Color _teamColor;

	#endregion

	#region Server Methods

	public override void OnStartServer()
	{
		RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

		_teamColor = player.GetTeamColor();
	}
	#endregion

	#region Client Methods

	void ClientHandleTeamColorUpdated(Color oldColor, Color newColor)
	{
		foreach(Renderer renderer in _colorRenderers)
		{
			foreach (Material material in renderer.materials)
				material.color = newColor;

			renderer.material.SetColor("_BaseColor", newColor);
		}
	}
	#endregion
}
