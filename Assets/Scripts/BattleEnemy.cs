using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnemy : MonoBehaviour
{
	public Enemy Enemy { get; set; }

	public void Setup(Enemy enemy)
    {
		Enemy = enemy;
		GetComponent<Image>().sprite = Enemy.EnemySO.EnemySprite;
	}
}
