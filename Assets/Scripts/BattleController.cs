using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState{ START, PLAYER, ENEMY, WON, LOSE, MESSAGE }

public class BattleController : MonoBehaviour
{

	const float TEXTSPEED = 0.05f;

	private GameController _gameController;
	private PlayerController _playerController;
	private DialogueController _dialogueController;
	//private BattleController _battleController;

	[SerializeField] private BattleEnemy battleEnemy;
	[SerializeField] private Image _menuSelectionIcon;

	[Header("User Interface")]
	[SerializeField] public Canvas battleCanvas;
	[SerializeField] private TMP_Text _battleMessageText;

	[SerializeField] private TMP_Text _playerMPText;
	[SerializeField] private TMP_Text _playerHPText;

	[SerializeField] private TMP_Text playerNumberText;
	[SerializeField] private TMP_Text enemyNumberText;

	[SerializeField] private Animator animator;
	[SerializeField] private Animator animatorPlayerNumbers;
	[SerializeField] private Animator animatorEnemyNumbers;

	[Header("Debug Fields")]
	[SerializeField] private TMP_Text _debugBattleState;
	[SerializeField] private TMP_Text _debugEnemyHP;

	[HideInInspector] public int _menuSelection = 0;
	[HideInInspector] public BattleState State;

	private float _textSpeed;
	private bool _finishedBattleMessage = false;
	private bool _isGameOver = false;

	private Enemy _battleEnemy;

	private bool _battleStart = false;
	private bool _playersTurn = false;


	void Start()
    {
		_dialogueController = FindObjectOfType<DialogueController>();
		_gameController = FindObjectOfType<GameController>();
		_playerController = FindObjectOfType<PlayerController>();
	}

	private void Update() {

		if(Input.GetKeyDown(KeyCode.Backspace)){
			SwitchBattleState(State);
		}

		if(_gameController.State == GameState.BATTLE){
			UserInput();
			DebugUI();
			UpdatePlayerText();
		}

		if(_isGameOver){
			if(_finishedBattleMessage && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))){
				animator.SetBool("isShowing", false);
				StartCoroutine(WaitForBoxExit(0.75f));
				ExitBattle();
			}
		}

	}

	public void ExitBattle(){

		if(_playerController.HP <= 0){
			Debug.Log("Player dies here");
			// Do something? Send to next day?
		}

		SwitchBattleState(BattleState.START);
		battleCanvas.enabled = false;
		_gameController.State = GameState.GAME;
	}

	
	public void BeginBattle(Enemy encounter){
		SwitchBattleState(BattleState.START);
		_battleEnemy = encounter;
		battleCanvas.enabled = true;
		_finishedBattleMessage = false;
		_isGameOver = false;
		_gameController.State = GameState.BATTLE;
		_battleStart = true;
		StartCoroutine(SetupBattle());
	}
	
	public void UserInput(){

		if(State == BattleState.ENEMY){
			
			if(Random.Range(1,100) < 20){
				EnemyHeal();
			} else {
				EnemyAttack();
			}

		}

		if(State == BattleState.PLAYER){

			if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)){
				
				if(_menuSelection < 3 ){
					_menuSelection++;
	
					_menuSelectionIcon.rectTransform.anchoredPosition = 
							new Vector2(_menuSelectionIcon.rectTransform.anchoredPosition.x, 38 - (25 * _menuSelection));
				}
			}
	
			if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
				
				if(_menuSelection > 0){
					_menuSelection--;
	
					_menuSelectionIcon.rectTransform.anchoredPosition = 
							new Vector2(_menuSelectionIcon.rectTransform.anchoredPosition.x, 38 - (25 * _menuSelection));
				}
			}
	
			if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)){
				DoPlayerAction(_menuSelection);
			}

		}

		if(State == BattleState.MESSAGE){
			if(!_isGameOver && _finishedBattleMessage && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))){
				animator.SetBool("isShowing", false);
				StartCoroutine(WaitForBoxExit(0.75f));
				
				if(battleEnemy.Enemy.HP <= 0){
					_isGameOver = true;
					StartCoroutine(ShowBattleMessage(battleEnemy.Enemy.EnemySO.DeathText));
					return;
				}

				if(_playerController.HP <= 0){
					_isGameOver = true;
					StartCoroutine(ShowBattleMessage("You woke up."));
					return;
				}

				SetTurn();
				ResetNumberAnimations();

				// if(_playerController.HP <= 0)
				// 	State = BattleState.LOSE;
					
				// if(battleEnemy.Enemy.HP <= 0)
				// 	State = BattleState.WON;

			} else if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)){
				_textSpeed = 0.0001f;
			}

		}
		
	}



	private void SetTurn(){
		if(_battleStart){

			_battleStart = false;

			if(_playerController.Speed > battleEnemy.Enemy.Speed){
				State = BattleState.PLAYER;
				_playersTurn = true;
			} else {
				State = BattleState.ENEMY;
				_playersTurn = false;
			}

		} else {
			if(_playersTurn){
				_playersTurn = false;
				State = BattleState.ENEMY;
			} else if (!_playersTurn){
				_playersTurn = true;
				State = BattleState.PLAYER;
			}
		}
	}

	private void ResetNumberAnimations(){
		animatorPlayerNumbers.SetBool("isHeal", false);
		animatorPlayerNumbers.SetBool("isCrit", false);
		animatorPlayerNumbers.SetBool("isDamage", false);

		animatorEnemyNumbers.SetBool("isHeal", false);
		animatorEnemyNumbers.SetBool("isDamage", false);
		animatorEnemyNumbers.SetBool("isCrit", false);
	}

	private void DoPlayerAction(int action){

		SwitchBattleState(BattleState.MESSAGE);

		switch(action){
			case 0:
				ActionWake();
				break;
			case 1:
				ActionPsychosis();
				break;
			case 2:
				ActionSleep();
				break;
			case 3:
				ActionDream();
				break;
		}

	}


	// =======================================================================================================

	//			PLAYER ACTIONS

	// =======================================================================================================

	private void ActionWake(){

		bool crit = false;
		int damage = _playerController.Attack + Random.Range(0, 5);
		string message = "Player tries to wake.";

		if(Random.Range(1,100) < 15){
			crit = true;
			damage = damage * 2;
			message = "Player wakes from a deep slumber!";
		}

		battleEnemy.Enemy.TakeDamage(damage, 0);
		StartCoroutine(ShowBattleMessage(message));

		enemyNumberText.text = $"-{damage}";
		if(crit)
			animatorEnemyNumbers.SetBool("isCrit", true);
		else
			animatorEnemyNumbers.SetBool("isDamage", true);
	}

	private void ActionPsychosis(){
		bool crit = false;
		int damage = _playerController.Attack + Random.Range(0, 5);
		string message = "Player enters a state of psychosis.";

		if(Random.Range(1,100) < 10){
			crit = true;
			damage = damage * 2;
			message = "Player experiences euphoria!";
		}

		battleEnemy.Enemy.TakeDamage(damage, 1);
		StartCoroutine(ShowBattleMessage(message));

		enemyNumberText.text = $"-{damage}";
		if(crit)
			animatorEnemyNumbers.SetBool("isCrit", true);
		else
			animatorEnemyNumbers.SetBool("isDamage", true);
	}

	private void ActionSleep(){
		_playerController.Defense += _playerController.Level + 1;
		StartCoroutine(ShowBattleMessage("Player feels asleep. Defense went up."));
	}

	private void ActionDream(){
		int heal = Mathf.FloorToInt(Random.Range(1, 10) * _playerController.Level);
		_playerController.HP += heal;
		if(_playerController.HP > _playerController.MaxHP) _playerController.HP = _playerController.MaxHP;

		StartCoroutine(ShowBattleMessage("Player starts dreaming."));
		playerNumberText.text = $"+{heal}";
		animatorPlayerNumbers.SetBool("isHeal", true);
	}



	// =======================================================================================================

	//			EMEMY ACTIONS

	// =======================================================================================================

	public void EnemyAttack(){ 

		bool crit = false;
		int damage = battleEnemy.Enemy.Attack + Random.Range(0, 5);
		string message = battleEnemy.Enemy.EnemySO.AttackText;

		if(Random.Range(1,100) < 10){
			crit = true;
			damage = damage * 2;
		}

		_playerController.TakeDamage(damage);
		
		StartCoroutine(ShowBattleMessage(message));

		playerNumberText.text = $"-{damage}";
		if(crit)
			animatorPlayerNumbers.SetBool("isCrit", true);
		else
			animatorPlayerNumbers.SetBool("isDamage", true);


	}

	public void EnemyHeal(){
		int heal = Mathf.FloorToInt(Random.Range(1, 10) * battleEnemy.Enemy.Level / 2);
		battleEnemy.Enemy.HealHP(heal);
		StartCoroutine(ShowBattleMessage(battleEnemy.Enemy.EnemySO.HealText));
		enemyNumberText.text = $"+{heal}";
		animatorEnemyNumbers.SetBool("isHeal", true);
	}



	IEnumerator SetupBattle(){
		
		battleEnemy.Setup(_battleEnemy);

		SwitchBattleState(BattleState.MESSAGE);
		yield return StartCoroutine(ShowBattleMessage(battleEnemy.Enemy.EnemySO.Appearance));

	}

	IEnumerator ShowBattleMessage(string message){
		_finishedBattleMessage = false;
		_battleMessageText.text = "";
		SwitchBattleState(BattleState.MESSAGE);

		animator.SetBool("isShowing", true);
		StopAllCoroutines();
		yield return StartCoroutine(WaitForBoxEnter(0.75f, message));
	}

	IEnumerator SetBattleText(string battleMessage){

		Debug.Log("DOES IT GET HERE");
		_textSpeed = TEXTSPEED;
		foreach(char letter in battleMessage.ToCharArray()){
			_battleMessageText.text += letter;
			yield return new WaitForSeconds(_textSpeed);
		}

		_finishedBattleMessage = true;
		
	}

	private void SwitchBattleState(BattleState state){
        State = state;
	}

	IEnumerator WaitForBoxEnter(float seconds, string text){
		yield return new WaitForSeconds(seconds);
		yield return StartCoroutine(SetBattleText(text));
	}

	IEnumerator WaitForBoxExit(float seconds){
		yield return new WaitForSeconds(seconds);
	}

	private void DebugUI(){
		_debugBattleState.text = "BATTLE STATE: " + State.ToString();
		_debugEnemyHP.text = "ENEMY HP: " + battleEnemy.Enemy.HP;		
	}

	private void UpdatePlayerText(){
		_playerHPText.text = "HP " + _playerController.HP + " / " + _playerController.MaxHP;
	}

}
