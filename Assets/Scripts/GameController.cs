using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState { MENU, BATTLE, GAME, DEAD, CREDITS, PAUSE }

public class GameController : MonoBehaviour
{
	const int CREDITS = 11;

	//private GameController _gameController;
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private DialogueController _dialogueController;
	[SerializeField] private BattleController _battleController;

	private GameObject _player;

	[SerializeField] private Camera _mainCamera;

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
		StartCoroutine(WaitForFade());

		State = GameState.PAUSE;
		_battleController.battleCanvas.enabled = false;

		_battleController.OnBattleEnter += OnBattleEnter;
		_battleController.OnBattleExit += OnBattleExit;

		_dialogueController.OnDialogueEnter += OnDialogueEnter;
		_dialogueController.OnDialogueExit += OnDialogueExit;
	}



    void Update()
    {
		DebugUI();

	
		if(Input.GetKeyDown(KeyCode.KeypadEnter)){
			UpdateGameState(State);
		}

		if(Input.GetKeyDown(KeyCode.Backspace)){
			_battleController.State = BattleState.START;
		}
			

		// if(State == GameState.StartScreen){

		//     if(Input.GetKeyDown("return")){
		//         startSound.Play();
		//         StartCoroutine(SwitchScene(LevelIndex, GameState.Game));
		//     }

		//     if(Input.GetKeyDown("escape")){
		//         QuitGame();
		//     }
		// }

		if(State == GameState.GAME){
            
            // if(CheckForWin()){
            //     UpdateGameState(GameState.Win);
            //     LevelIndex += 1;
                
            //     if(LevelIndex == CREDITS)
            //         StartCoroutine(NextLevel(LevelIndex, GameState.Credits));
            //     else
            //         StartCoroutine(NextLevel(LevelIndex, GameState.Game));
            // }
                
            if(Input.GetKeyDown("escape")){
                QuitGame();
            }
        } 
        
        // if(State == GameState.Game || State == GameState.Dead) {

        //     if(Input.GetKeyDown("r") && !_playerController.isMoving ){
        //         Debug.Log("Game Reset");
        //         ResetLevel();
        //         // StartCoroutine(gametext.FadeOut(0f));
        //     }

        //     if(Input.GetKeyDown("escape")){
        //         QuitGame();
        //     }
        // }
            
    }

    // public bool CheckForWin(){
        
    //     if((_player.transform.position - Player_1_End.transform.position).sqrMagnitude < Mathf.Epsilon){
    //         player_1_win = true;
    //     } else {
    //         player_1_win = false;
    //     }

    //     if((player_2.transform.position - Player_2_End.transform.position).sqrMagnitude < Mathf.Epsilon){
    //         player_2_win = true;
    //     } else {
    //         player_2_win = false;
    //     }

    //     if(player_1_win && player_2_win){
    //         winSound.Play();
    //         return true;
    //     }
            

    //     return false;
    // }

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


	IEnumerator WaitForFade(){
		yield return new WaitForSeconds(3);
		StartCoroutine(StartFade());
	}

	IEnumerator StartFade(){
		yield return _fader.FadeOut(2f);
	}





	public void OnBattleEnter(){
		_playerController.canMove = false;
		State = GameState.BATTLE;
	}

	public void OnBattleExit(){
		_playerController.canMove = true;
		State = GameState.GAME;
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



