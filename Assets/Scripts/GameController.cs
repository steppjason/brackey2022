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

	public GameState State {get;set;}
    public int LevelIndex {get; set;}

    private Fader fader;
    private GameObject _playerStart;
	private bool _playerWin;


    // public GameObject startScreen;
    // public GameObject gameOverScreen;
    // public GameObject creditsScreen;
	// public AudioSource startSound;
	// public AudioSource levelWarpSound;
	// public AudioSource winSound;
	// public AudioSource deathSound;


    private void Awake() {
        this.LevelIndex = 1;
        fader = FindObjectOfType<Fader>();
		// gametext = FindObjectOfType<GameOverText>();
		_mainCamera.enabled = true;

	}

    void Start()
    {
        
//		_dialogueController = FindObjectOfType<DialogueController>();
//		_playerController = FindObjectOfType<PlayerController>();
//		_battleController = FindObjectOfType<BattleController>();
		
		StartCoroutine(WaitForFade());

		State = GameState.GAME;
		_battleController.battleCanvas.enabled = false;
	}

	IEnumerator WaitForFade(){
		yield return new WaitForSeconds(3);
		StartCoroutine(StartFade());
	}

	IEnumerator StartFade(){
		yield return fader.FadeOut(3f);
	}

	private void DebugUI(){
		DebugGameState.text = "GameState: " + State.ToString();
		DebugMenuSelection.text = "MenuSelection: " + _battleController._menuSelection;
	}

    void Update()
    {
		DebugUI();

	
		if(Input.GetKeyDown(KeyCode.KeypadEnter)){
			UpdateGameState(State);
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

    public void ResetGame(){
        // startScreen.SetActive(false);
        // gameOverScreen.SetActive(false);
    }

    public void ResetLevel(){
        //_player.GetComponent<PlayerController>().Reset();
        
        _player.transform.position = _playerStart.transform.position;
        
        UpdateGameState(GameState.GAME);
    }

    IEnumerator SwitchScene(int scene, GameState gameState){
        // yield return fader.FadeIn(1f);
        yield return SceneManager.LoadSceneAsync(scene);
        UpdateGameState(gameState);
        _playerStart = GameObject.Find("Player_1_Start");
        ResetLevel();
        // yield return fader.FadeOut(1f);
    }

    IEnumerator NextLevel(int scene, GameState gameState){
        _playerWin = false;
        
        yield return new WaitForSeconds(1f);
        //levelWarpSound.Play();
        // yield return fader.FadeIn(1f);
        yield return SceneManager.LoadSceneAsync(scene);
        UpdateGameState(gameState);
        _playerStart = GameObject.Find("Player_1_Start");
        ResetLevel();
        // yield return fader.FadeOut(1f);
    }
}



