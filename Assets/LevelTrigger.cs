using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelTrigger : MonoBehaviour
{
	[SerializeField] public int _levelNumber;

	private GameController _gameController;

	private void Start() {
		_gameController = FindObjectOfType<GameController>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		_gameController.SwitchLevel(_levelNumber);
	}
}
