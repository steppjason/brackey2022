using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public enum GameState { TITLE, BATTLE, GAME, DEAD, CREDITS, PAUSE }

public class GameController : MonoBehaviour
{
	const int CREDITS = 11;

	//private GameController _gameController;
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private DialogueController _dialogueController;
	[SerializeField] private BattleController _battleController;
	[SerializeField] private AudioController _audioController;
	[SerializeField] private WarpController _warpController;

	private GameObject _player;

	[SerializeField] private Camera _mainCamera;
	[SerializeField] private GameObject _titleScreen;
	[SerializeField] private GameObject _gameOver;
	[SerializeField] private GameObject _creditsScreen;

	[SerializeField] public TMP_Text DebugGameState;
	[SerializeField] public TMP_Text DebugMenuSelection;
	[SerializeField] public TMP_Text DebugBattleState;
	[SerializeField] public TMP_Text DebugAction;
	[SerializeField] public TMP_Text DebugEnemyHP;

	public GameState State {get;set;}

    [SerializeField] public Fader _fader;
    private GameObject _playerStart;

    private void Awake() {
		_mainCamera.enabled = true;
	}

    void Start()
    {
		State = GameState.PAUSE;
		StartCoroutine(WaitForFadeOut(2f,1f));

		_battleController.battleCanvas.enabled = false;
		_battleController.OnBattleEnter += OnBattleEnter;
		_battleController.OnBattleExit += OnBattleExit;
		_battleController.OnPlayerDeath += OnPlayerDeath;

		_dialogueController.OnDialogueEnter += OnDialogueEnter;
		_dialogueController.OnDialogueExit += OnDialogueExit;

		_warpController.OnWarpEnter += OnWarpEnter;
		//_warpController.OnWarpExit += OnWarpExit;

		//_audioController.FadeIn("CREEP MUSIC", 5f, 0.20f);
		
	}



    void Update()
    {
		if(State == GameState.DEAD){
			if(Input.GetKeyDown(KeyCode.Escape)){
				GameObject game = GameObject.Find("GameCore");
				SceneManager.LoadScene(0);
				Destroy(game);
			}
		}

		if(State == GameState.TITLE){

			if(Input.GetKeyDown(KeyCode.Return)){
				_audioController.Play("Menu Select");
				StartCoroutine(SwitchScene(2));
			}
		}

		if(State == GameState.PAUSE){
			_playerController.canMove = false;
		}

		if(State == GameState.CREDITS){
			
		}

		DebugUI();
    }

  
	private void DebugUI(){
		DebugGameState.text = "GameState: " + State.ToString();
		DebugAction.text = "Action: " + _playerController.action;
		DebugMenuSelection.text = "MenuSelection: " + _battleController._menuSelection;

		DebugBattleState.text = "BATTLE STATE: " + _battleController.State.ToString();
		//DebugEnemyHP.text = "ENEMY HP: " + _battleController.battleEnemy.Enemy.HP;		
		
	}


    IEnumerator SwitchScene(int scene){
		State = GameState.PAUSE;
		yield return _fader.FadeIn(1.5f);
		State = GameState.GAME;
		yield return SceneManager.LoadSceneAsync(scene);
		_titleScreen.SetActive(false);
		_playerStart = GameObject.Find("PLAYER_START");
		_playerController.transform.position = _playerStart.transform.position;
		_playerController.canMove = true;
		yield return _fader.FadeOut(1.5f);
    }

	public void SwitchLevel(int levelNumber){
		StartCoroutine(SwitchScene(levelNumber));
	}

	public void LoadLevelMusic(string music, float fadetime, float volume){
		_audioController.StopAll();
		_audioController.FadeIn(music, fadetime, volume);
	}

	public void StartCredits(){
		State = GameState.CREDITS;
		_playerController.canMove = false;
		_creditsScreen.SetActive(true);
	}


	IEnumerator WaitForFadeOut(float fadeTime, float waitTime){
		yield return new WaitForSeconds(waitTime);
		StartCoroutine(FadeOut(fadeTime));
		State = GameState.TITLE;
	}

	IEnumerator WaitForFadeIn(float fadeTime, float waitTime){
		StartCoroutine(FadeIn(fadeTime));
		yield return new WaitForSeconds(waitTime);
	}

	IEnumerator FadeOut(float fadeout){
		yield return _fader.FadeOut(fadeout);
	}

	IEnumerator FadeIn(float fadein){
		yield return _fader.FadeIn(fadein);
	}




	public void OnBattleEnter(bool boss){
		_audioController.FadeOut("NORMAL MUSIC", 1f, 0f);
		_audioController.FadeOut("GLITCH MUSIC", 1f, 0f);
		_playerController.canMove = false;
		State = GameState.BATTLE;
		if(!boss)
			_audioController.FadeIn("BATTLE MUSIC", 5f, 0.10f);
	}

	public void OnPlayerDeath(){
		_audioController.StopAll();
		StartCoroutine(FadeOut(1.5f));
		_audioController.Play("DEATH MUSIC");
		_playerController.canMove = false;
		State = GameState.DEAD;
		_gameOver.SetActive(true);
		
	}

	public void OnBattleExit(int loadLevel){
		_audioController.FadeOut("BATTLE MUSIC", 1f, 0f);
		SwitchLevel(loadLevel);
	}

	public void OnDialogueEnter(){
		_playerController.canMove = false;
		State = GameState.PAUSE;
	}

	public void OnDialogueExit(){
		_playerController.canMove = true;
		State = GameState.GAME;
	}

	public void OnWarpEnter(GameObject warp){
		StartCoroutine(StartWarp(warp));
	}

	IEnumerator StartWarp(GameObject warp){
		_playerController.canMove = false;
		StartCoroutine(_fader.FadeIn(1f));
		yield return new WaitForSeconds(1f);
		StartCoroutine(_fader.FadeOut(1f));
		//yield return new WaitForSeconds(1f);
		_playerController.transform.position = warp.transform.position;
		_playerController.canMove = true;
	}

	public void OnWarpExit(){

	}

}



