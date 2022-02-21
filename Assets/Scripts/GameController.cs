using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    const int CREDITS = 11;

    // public GameObject startScreen;
    // public GameObject gameOverScreen;
    // public GameObject creditsScreen;
    public GameObject _player;

    // public AudioSource startSound;
    // public AudioSource levelWarpSound;
    // public AudioSource winSound;
    // public AudioSource deathSound;

    public GameState State {get;set;}
    public int LevelIndex {get; set;}

    private Fader fader;
    // private GameOverText gametext;

    private GameObject _playerStart;

    private PlayerController _playerController;
    

    private bool _playerWin;

    private void Awake() {
        this.LevelIndex = 1;
        fader = FindObjectOfType<Fader>();
        // gametext = FindObjectOfType<GameOverText>();
    }

    void Start()
    {
        _playerController = _player.GetComponent<PlayerController>();
		StartCoroutine(WaitForFade());
	}

	IEnumerator WaitForFade(){
		yield return new WaitForSeconds(3);
		StartCoroutine(StartFade());
	}

	IEnumerator StartFade(){
		yield return fader.FadeOut(3f);
	}

    void Update()
    {
       
        // if(State == GameState.StartScreen){

        //     if(Input.GetKeyDown("return")){
        //         startSound.Play();
        //         StartCoroutine(SwitchScene(LevelIndex, GameState.Game));
        //     }

        //     if(Input.GetKeyDown("escape")){
        //         QuitGame();
        //     }
        // }
        
        if(State == GameState.Game){
            
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

		ResetGame();

		// if(state == GameState.StartScreen)
		//     startScreen.SetActive(true);

			// else if(state == GameState.Dead){
			//     gameOverScreen.SetActive(true);
			//     // StartCoroutine(gametext.FadeIn(2f));
			// }

			// else if(state == GameState.Credits)
			//     creditsScreen.SetActive(true);

		//else 
		if(state == GameState.Game || state == GameState.Win){
            _player.SetActive(true);
            
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
        
        UpdateGameState(GameState.Game);
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

public enum GameState{
    StartScreen,
    Menu,
    Game,
    Win,
    Credits,
    Dead
}



