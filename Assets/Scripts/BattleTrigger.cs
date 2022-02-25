using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
	[SerializeField] Enemy _encounter;
	[SerializeField] public bool isDead = false;
	//[SerializeField] EnemySO encounter;
	//[SerializeField] int level;

	public void TriggerBattle(){
		if(!isDead){
			isDead = true;
			_encounter.Init();
			FindObjectOfType<BattleController>().FalseStartBattle(_encounter);
		}
	}
}
