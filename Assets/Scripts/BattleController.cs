using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState{ START, PLAYER, ENEMY, WON, LOSE, MESSAGE, END }

public class BattleController : MonoBehaviour
{

	const float TEXTSPEED = 0.005f;

	public event Action OnBattleEnter;
	public event Action OnBattleExit;
	public event Action OnPlayerDeath;

	//[SerializeField] private GameController _gameController;
	[SerializeField] private PlayerController _playerController;

	[SerializeField] public BattleEnemy battleEnemy;
	[SerializeField] private Image _menuSelectionIcon;

	[Header("INTERFACE")]
	[SerializeField] public Canvas battleCanvas;
	[SerializeField] public Image _backgroundImage;
	[SerializeField] private TMP_Text _battleMessageText;

	[SerializeField] private TMP_Text _playerMPText;
	[SerializeField] private TMP_Text _playerHPText;

	[SerializeField] private TMP_Text playerNumberText;
	[SerializeField] private TMP_Text enemyNumberText;

	[Header("ANIMATORS")]
	[SerializeField] private Animator animator;
	[SerializeField] private Animator animatorPlayerNumbers;
	[SerializeField] private Animator animatorEnemyNumbers;
	[SerializeField] private Animator animatorEnemy;
	[SerializeField] private Animator animatorPlayer;
	 [SerializeField] public Fader _fader;

	[Header("AUDIO")]
	public AudioController _audioController;


	[Header("DEBUG")]
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
	private bool moveLeft = false;


	void Start()
    {
		_backgroundImage.rectTransform.position = new Vector3(0, 0, 0);
	}


	private void Update() {

		if(_backgroundImage.rectTransform.anchoredPosition.x >= 4000 && !moveLeft)
				moveLeft = true;
		else if (_backgroundImage.rectTransform.anchoredPosition.x <= -4000 && moveLeft)
			moveLeft = false;
			
		if(moveLeft) _backgroundImage.rectTransform.anchoredPosition = new Vector2(_backgroundImage.rectTransform.anchoredPosition.x - 0.3f, 0);
		else if(!moveLeft) _backgroundImage.rectTransform.anchoredPosition = new Vector2(_backgroundImage.rectTransform.anchoredPosition.x + 0.3f, 0);
		
		


		//if(_gameController.State == GameState.BATTLE){
		if(State != BattleState.END){
			UserInput();
			UpdatePlayerText();
		}

		if(_isGameOver){
			if(_finishedBattleMessage && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))){
				_audioController.Play("Menu Select");
				animator.SetBool("isShowing", false);
				StartCoroutine(WaitForBoxExit(0.5f));
				StartCoroutine(ExitBattle());
			}
		}

	}

	IEnumerator ExitBattle(){
		
		StartCoroutine(_fader.FadeIn(1f));
		yield return new WaitForSeconds(1f);

		if(_playerController.HP <= 0){
			Debug.Log("Player dies here");
			OnPlayerDeath();
			_isGameOver = true;
		} else {
			OnBattleExit();
		}

		State = BattleState.END;
		_isGameOver = false;
		battleCanvas.enabled = false;

		StartCoroutine(_fader.FadeOut(1f));
		yield return new WaitForSeconds(1f);
	}

	public void FalseStartBattle(Enemy encounter){
		StartCoroutine(BeginBattle(encounter));
	}

	
	IEnumerator BeginBattle(Enemy encounter){
		OnBattleEnter();
		StartCoroutine(_fader.FadeIn(1f));
		yield return new WaitForSeconds(1f);
		animatorEnemy.SetBool("isDeath", false);
		SwitchBattleState(BattleState.START);
		_battleEnemy = encounter;
		battleCanvas.enabled = true;
		_finishedBattleMessage = false;
		_isGameOver = false;
		_battleStart = true;
		StartCoroutine(_fader.FadeOut(1f));
		yield return new WaitForSeconds(1f);
		StartCoroutine(SetupBattle());
		yield return null;
	}
	
	public void UserInput(){

		if(State == BattleState.ENEMY){
			StartCoroutine(EnemyTurn());
		}

		if(State == BattleState.PLAYER){
			_menuSelectionIcon.enabled = true;
		} else {
			_menuSelectionIcon.enabled = false;
		}

		if(State == BattleState.PLAYER){

			if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)){
				
				if(_menuSelection < 3 ){
					_menuSelection++;
					_audioController.Play("Menu Move");
					_menuSelectionIcon.rectTransform.anchoredPosition = 
							new Vector2(_menuSelectionIcon.rectTransform.anchoredPosition.x, 38 - (25 * _menuSelection));
				}
			}
	
			if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
				
				if(_menuSelection > 0){
					_menuSelection--;
					_audioController.Play("Menu Move");
					_menuSelectionIcon.rectTransform.anchoredPosition = 
							new Vector2(_menuSelectionIcon.rectTransform.anchoredPosition.x, 38 - (25 * _menuSelection));
				}
			}
	
			if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)){
				_audioController.Play("Menu Select");
				DoPlayerAction(_menuSelection);
			}

		}

		if(State == BattleState.MESSAGE){
			DebugUI();
			if(!_isGameOver && _finishedBattleMessage && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))){
				_audioController.Play("Menu Select");
				animator.SetBool("isShowing", false);
				StartCoroutine(WaitForBoxExit(0.5f));
				
				if(battleEnemy.Enemy.HP <= 0){
					_isGameOver = true;
					animatorEnemy.SetBool("isDeath", true);
					StartCoroutine(ShowBattleMessage(battleEnemy.Enemy.EnemySO.DeathText));
					return;
				}

				if(_playerController.HP <= 0){
					_isGameOver = true;
					_audioController.FadeOut("BATTLE MUSIC", 1f, 0f);
					_audioController.Play("DEATH MUSIC");
					StartCoroutine(ShowBattleMessage("You woke up."));
					return;
				}

				StartCoroutine(SetTurn());
				ResetNumberAnimations();

			} else if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)){
				_textSpeed = 0.0001f;
			}

		}
		
	}


	IEnumerator SetTurn(){
		yield return new WaitForSeconds(0.25f);
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

		animatorEnemy.SetBool("isCrit", false);
		animatorEnemy.SetBool("isHit", false);
		animatorEnemy.SetBool("isHeal", false);

		animatorPlayer.SetBool("isCrit", false);
		animatorPlayer.SetBool("isHit", false);
		animatorPlayer.SetBool("isHeal", false);
		animatorPlayer.SetBool("isShield", false);
	}

	private void DoPlayerAction(int action){

		SwitchBattleState(BattleState.MESSAGE);

		switch(action){
			case 0:
				StartCoroutine(ActionWake());
				break;
			case 1:
				StartCoroutine(ActionPsychosis());
				break;
			case 2:
				StartCoroutine(ActionSleep());
				break;
			case 3:
				StartCoroutine(ActionDream());
				break;
		}

	}


	// =======================================================================================================

	//			PLAYER ACTIONS

	// =======================================================================================================

	IEnumerator ActionWake(){

		bool crit = false;
		int damage = _playerController.Attack + UnityEngine.Random.Range(1, 2);
		string message = "Player tries to wake.";

		if(UnityEngine.Random.Range(1,100) < 50){
			crit = true;
			damage = damage * 2;
			message = "Player wakes from a deep slumber!";
		}
		
		int dmg = damage - Mathf.FloorToInt(battleEnemy.Enemy.Defense / damage);
		if(dmg < 0) dmg = 0;
		
		battleEnemy.Enemy.TakeDamage(dmg);
		StartCoroutine(ShowBattleMessage(message));
		
		enemyNumberText.text = $"-{dmg}";
		yield return StartCoroutine(DisplayDamage(0, crit));
		
	}

	IEnumerator ActionPsychosis(){
		bool crit = false;
		int damage = _playerController.Attack + UnityEngine.Random.Range(1, 3);
		string message = "Player enters a state of psychosis.";
		int damageToPlayer = damage;
		if(UnityEngine.Random.Range(1,100) < 50){
			crit = true;
			damage = damage * 2;
			message = "Player experiences euphoria!";
		}

		int dmg = damage - Mathf.FloorToInt(battleEnemy.Enemy.Defense / damage);
		if(dmg < 0) dmg = 0;

		battleEnemy.Enemy.TakeDamage(dmg);
		_playerController.TakeDamage(damageToPlayer);
		StartCoroutine(ShowBattleMessage(message));
		

		enemyNumberText.text = $"-{dmg}";
		playerNumberText.text = $"-{damageToPlayer}";
		yield return StartCoroutine(DisplayDamage(4, crit));

	}

	IEnumerator ActionSleep(){
		_playerController.Defense += _playerController.Level / 2 *  5;
		Debug.Log(_playerController.Defense);
		StartCoroutine(ShowBattleMessage("Player feels asleep. Defense went up."));

		yield return StartCoroutine(DisplayDamage(-1, false));
	}

	IEnumerator ActionDream(){
		int heal = Mathf.FloorToInt(UnityEngine.Random.Range(5, 10) * _playerController.Level);
		_playerController.HP += heal;
		if(_playerController.HP > _playerController.MaxHP) _playerController.HP = _playerController.MaxHP;

		StartCoroutine(ShowBattleMessage("Player starts dreaming."));

		playerNumberText.text = $"+{heal}";

		yield return StartCoroutine(DisplayDamage(2, false));
	}



	// =======================================================================================================

	//			EMEMY ACTIONS

	// =======================================================================================================

	IEnumerator EnemyAttack(){ 

		bool crit = false;
		int damage = battleEnemy.Enemy.Attack + UnityEngine.Random.Range(1, 2);
		string message = battleEnemy.Enemy.EnemySO.AttackText;

		if(UnityEngine.Random.Range(1,100) < 20){
			crit = true;
			damage = damage * 2;
		}

		int dmg = damage - Mathf.FloorToInt(_playerController.Defense / damage);
		if(dmg < 0) dmg = 0;

		_playerController.TakeDamage(dmg);
		
		StartCoroutine(ShowBattleMessage(message));

		playerNumberText.text = $"-{dmg}";
		yield return StartCoroutine(DisplayDamage(1, crit));
	}

	IEnumerator EnemyHeal(){
		int heal = Mathf.FloorToInt(UnityEngine.Random.Range(1, 3) * battleEnemy.Enemy.Level / 2);
		battleEnemy.Enemy.HealHP(heal);

		StartCoroutine(ShowBattleMessage(battleEnemy.Enemy.EnemySO.HealText));

		enemyNumberText.text = $"+{heal}";
		yield return StartCoroutine(DisplayDamage(3, false));
	}

	IEnumerator EnemyTurn(){
		yield return new WaitForSeconds(0.5f);
		if(UnityEngine.Random.Range(1,100) < 20){
			StartCoroutine(EnemyHeal());
		} else {
			StartCoroutine(EnemyAttack());
		}
		
	}


	IEnumerator DisplayDamage(int source, bool crit){
		yield return new WaitForSeconds(0.75f);
		if(source == 0){
			if(crit){
				_audioController.Play("Crit");
				animatorEnemyNumbers.SetBool("isCrit", true);
				animatorEnemy.SetBool("isCrit", true);
			} else{
				_audioController.Play("Hit");
				animatorEnemyNumbers.SetBool("isDamage", true);
				animatorEnemy.SetBool("isHit", true);
			}

		} else if (source == 1){
			if(crit){
				_audioController.Play("Crit");
				animatorPlayerNumbers.SetBool("isCrit", true);
				animatorPlayer.SetBool("isCrit", true);
			} else{
				_audioController.Play("Hit");
				animatorPlayerNumbers.SetBool("isDamage", true);
				animatorPlayer.SetBool("isHit", true);
			}
		} else if (source == 2){
			_audioController.Play("Heal");
			animatorPlayerNumbers.SetBool("isHeal", true);
			animatorPlayer.SetBool("isHeal", true);
		} else if (source == 3){
			_audioController.Play("Heal");
			animatorEnemyNumbers.SetBool("isHeal", true);
			animatorEnemy.SetBool("isHeal", true);
		} else if (source == 4){
			if(crit){
				_audioController.Play("Crit");
				animatorEnemyNumbers.SetBool("isCrit", true);
				animatorEnemy.SetBool("isCrit", true);
			} else{
				_audioController.Play("Hit");
				animatorEnemyNumbers.SetBool("isDamage", true);
				animatorEnemy.SetBool("isHit", true);
			}
			_audioController.Play("Hit");
			animatorPlayerNumbers.SetBool("isDamage", true);
			animatorPlayer.SetBool("isHit", true);
		} else if (source == -1){
			// _audioController.Play("Shield");
			// animatorPlayer.SetBool("isShield", true);
		}

		yield return null;
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
		yield return StartCoroutine(WaitForBoxEnter(0.5f, message));
	}

	IEnumerator SetBattleText(string battleMessage){
		_textSpeed = TEXTSPEED;
		foreach(char letter in battleMessage.ToCharArray()){
			_audioController.Play("Typing");
			_battleMessageText.text += letter;
			yield return new WaitForSeconds(_textSpeed);
		}

		yield return new WaitForSeconds(0.75f);
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
		_debugEnemyHP.text = "Enemy HP: " + battleEnemy.Enemy.HP;
	}

	private void UpdatePlayerText(){
		_playerHPText.text = "HP " + _playerController.HP + " / " + _playerController.MaxHP;
	}

}
