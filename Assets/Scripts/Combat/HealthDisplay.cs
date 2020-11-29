using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
	#region Fields

	[SerializeField] Health _health;
	[SerializeField] GameObject _healthbarParent;
	[SerializeField] Image _healthbarImage;

	#endregion

	#region MonoBehaviour Methods

	void Awake() 
	{
		_health.ClientOnHealthUpdated += HandleHealthUpdated;
	}
	
	void OnDestroy() 
	{
		_health.ClientOnHealthUpdated -= HandleHealthUpdated;
	}

	void OnMouseEnter()
	{
		_healthbarParent.SetActive(true);
	}

	void OnMouseExit()
	{
		_healthbarParent.SetActive(false);
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	void HandleHealthUpdated(int currentHealth,int maxHealth)
	{
		_healthbarImage.fillAmount = (float)currentHealth / maxHealth;
	}
	#endregion
}
