using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public enum GameState { MENU, BATTLE, GAME, DEAD, CREDITS, PAUSE }

public class GameController : MonoBehaviour
{
	const int CREDITS = 11;

	//private GameController _gameController;
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private DialogueController _dialogueController;
	[SerializeField] private BattleController _battleController;
	[SerializeField] private AudioController _audioController;

	private GameObject _player;

	[SerializeField] private Camera _mainCamera;
	[SerializeField] private GameObject _titleScreen;
	[SerializeField] private GameObject _gameOver;

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
		StartCoroutine(WaitForFadeOut(2f,1f));

		State = GameState.PAUSE;
		_battleController.battleCanvas.enabled = false;

		_battleController.OnBattleEnter += OnBattleEnter;
		_battleController.OnBattleExit += OnBattleExit;
		_battleController.OnPlayerDeath += OnPlayerDeath;

		_dialogueController.OnDialogueEnter += OnDialogueEnter;
		_dialogueController.OnDialogueExit += OnDialogueExit;
		_audioController.FadeIn("NORMAL MUSIC", 20f, 0.20f);
		
	}



    void Update()
    {
		if(State == GameState.DEAD){
			
			if(Input.GetKeyDown(KeyCode.Escape)){
				//ReturnToTitle();
			}

		}

		DebugUI();
	
		// if(Input.GetKeyDown(KeyCode.KeypadEnter)){
		// 	UpdateGameState(State);
		// }

		// if(Input.GetKeyDown(KeyCode.Backspace)){
		// 	_battleController.State = BattleState.START;
		// }
		
		// if(State == GameState.GAME){
                
        //     if(Input.GetKeyDown("escape")){
        //         QuitGame();
        //     }
        // } 
        

            
    }

  

	private void DebugUI(){
		DebugGameState.text = "GameState: " + State.ToString();
		DebugAction.text = "Action: " + _playerController.action;
		DebugMenuSelection.text = "MenuSelection: " + _battleController._menuSelection;

		DebugBattleState.text = "BATTLE STATE: " + _battleController.State.ToString();
		//DebugEnemyHP.text = "ENEMY HP: " + _battleController.battleEnemy.Enemy.HP;		
		
	}

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        #if UNITY_64
            Application.Quit();
        #endif
    }

    public void UpdateGameState(GameState state){

		switch(state){
			case GameState.MENU:
				state = GameState.GAME;
				break;
			case GameState.GAME:
				state = GameState.BATTLE;
				break;
			case GameState.BATTLE:
				state = GameState.DEAD;
				break;
			case GameState.DEAD:
				state = GameState.CREDITS;
				break;
			case GameState.CREDITS:
				state = GameState.MENU;
				break;
			default:
				state = GameState.GAME;
				break;
		}

        State = state;
	}



    IEnumerator SwitchScene(int scene){
		State = GameState.PAUSE;
		yield return _fader.FadeIn(1f);
		yield return SceneManager.LoadSceneAsync(scene);
        State = GameState.PAUSE;
		_playerStart = GameObject.Find("PLAYER_START");
		_playerController.transform.position = _playerStart.transform.position;
		yield return _fader.FadeOut(1f);
    }

	public void SwitchLevel(int levelNumber){
		StartCoroutine(SwitchScene(levelNumber));
	}


	IEnumerator WaitForFadeOut(float fadeTime, float waitTime){
		yield return new WaitForSeconds(waitTime);
		StartCoroutine(FadeOut(fadeTime));
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




	public void OnBattleEnter(){
		_audioController.FadeOut("NORMAL MUSIC", 1f, 0f);
		_playerController.canMove = false;
		State = GameState.BATTLE;
		_audioController.FadeIn("BATTLE MUSIC", 5f, 0.2f);
	}

	public void OnPlayerDeath(){
		_audioController.FadeOut("BATTLE MUSIC", 1f, 0f);
		_audioController.FadeOut("NORMAL MUSIC", 20f, 0.2f);
		_playerController.canMove = false;
		State = GameState.DEAD;
		_gameOver.SetActive(true);
	}

	public void OnBattleExit(){
		_audioController.FadeOut("BATTLE MUSIC", 1f, 0f);
		_playerController.canMove = true;
		State = GameState.GAME;
		_audioController.FadeIn("NORMAL MUSIC", 20f, 0.2f);
	}

	public void OnDialogueEnter(){
		_playerController.canMove = false;
		State = GameState.PAUSE;
	}

	public void OnDialogueExit(){
		_playerController.canMove = true;
		State = GameState.GAME;
	}


}



