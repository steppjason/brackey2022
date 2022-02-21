using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState{ START, PLAYER, ENEMY, WON, LOSE }

public class BattleController : MonoBehaviour
{
	public BattleState state;

	public GameObject player;
	public GameObject enemy;

	public Transform playerStart;
	public Transform enemyStart;

	void Start()
    {
		state = BattleState.START;
		SetupBattle();
	}

	void SetupBattle(){
		Instantiate(player, playerStart);
		Instantiate(enemy, enemyStart);
	}

}
