using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMove { ATTACK, DEFEND, HEAL }

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/New Enemy" )]
public class EnemySO : ScriptableObject
{
	[SerializeField] string enemyName;

	[TextArea]
	[SerializeField] string description;

	[SerializeField] string appearText;
	[SerializeField] string attackText;
	[SerializeField] string healText;
	[SerializeField] string deathText;


	[SerializeField] Sprite enemySprite;

	[SerializeField] int maxHP;
	[SerializeField] int attack;
	[SerializeField] int defense;
	[SerializeField] int speed;

	[SerializeField] EnemyMove[] moves;

	public string Name{
		get { return enemyName; }
	}

	public string Description{
		get { return description; }
	}

	public string Appearance{
		get { return appearText; }
	}

	public string AttackText{
		get { return attackText; }
	}

	public string HealText{
		get { return healText; }
	}

	public string DeathText{
		get { return deathText; }
	}

	public int MaxHP{
		get { return maxHP; }
	}

	public int Attack{
		get { return attack; }
	}

	public int Defense{
		get { return defense; }
	}

	public int Speed{
		get { return speed; }
	}

	public Sprite EnemySprite{
		get { return enemySprite; }
	}
	
}
