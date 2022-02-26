using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
	[SerializeField] EnemySO enemybase;
	
	[SerializeField] public int loadLevel;

	int _currentHP;
	int level;

	public EnemySO EnemyBase {
		get { return enemybase; }
	}

	public int Level {
		get { return level;}
	}

	public EnemySO EnemySO{
		get { return EnemyBase; }
	}

	public int HP{
		get { return _currentHP; }
	}

	public int Attack {
		get { return EnemyBase.Attack; }
	}

	public int Defense{
		get { return EnemyBase.Defense; }
	}

	public int Speed{
		get { return EnemyBase.Speed; }
	}

	public int MaxHP{
		get { return EnemyBase.MaxHP; }
	}



	public void Init(int lvl){
		_currentHP = MaxHP;
		level = lvl;
	}

	public void TakeDamage(int damage){
		_currentHP -= damage;
		if(_currentHP < 0) _currentHP = 0;
	}

	public void HealHP(int heal){
		_currentHP += heal;
		if(_currentHP > MaxHP) _currentHP = MaxHP;
	}

	
}
