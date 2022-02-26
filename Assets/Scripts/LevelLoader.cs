using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelLoader : MonoBehaviour
{
	[SerializeField] public string _musicTrack;
	[SerializeField] public float _fadeTime;
	[SerializeField] public float _volume;

	[Header("PLAYER")]
	public int level;
	public int attack;
	public int defense;
	

	void Start()
    {
		FindObjectOfType<GameController>().LoadLevelMusic(_musicTrack, _fadeTime, _volume);
		FindObjectOfType<PlayerController>().SetStats(level, attack, defense);
	}

}
