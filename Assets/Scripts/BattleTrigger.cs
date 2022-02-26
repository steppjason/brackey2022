using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
	[SerializeField] Enemy _encounter;
	[SerializeField] public bool isDead = false;
	public bool isFinalBoss = false;
	[SerializeField] public int level;
	//[SerializeField] EnemySO encounter;
	

	public void TriggerBattle(){
		if(!isDead){
			isDead = true;
			_encounter.Init(level);
			FindObjectOfType<BattleController>().FalseStartBattle(_encounter, isFinalBoss);
		}
	}
}
