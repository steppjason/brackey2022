using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	//[SerializeField] private GameController _gameController; 
	[SerializeField] private AudioController _audioController;
	
	private GameObject _collider;
	private Vector2 _input;
	private Animator _animator;

	//public event Action OnDialogue;
	//public event Action OnBattle;


	[SerializeField] public Camera _camera;
    [SerializeField] public float _moveSpeed = 5;
	[SerializeField] public GameObject _actionIcon;
	
	public bool action = false;
	public bool canMove = true;

	public int Level { get; set; }
	public int MaxHP { get; set; }
	public int HP { get; set; }
	public int MP { get; set; }
	public int Attack { get; set; }
	public int Speed { get; set; }
	public int Defense { get; set; }

	private void Awake() {
		_animator = GetComponent<Animator>();
	}


    // Start is called before the first frame update
    void Start()
    {
		// _gameController = FindObjectOfType<GameController>();

		this.Level = 3;
		this.MaxHP = 25 * this.Level;
		this.HP = this.MaxHP;
		this.Attack = 10;
		this.Defense = 10;
		this.Speed = 10;
	
	}

    // Update is called once per frame
    void Update()
    {
		_camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

		if(action && canMove && (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl))){
			
			if(_collider.GetComponent<DialogueTrigger>()){				
				_collider.GetComponent<DialogueTrigger>().TriggerDialogue();
			}
				
			if(_collider.GetComponent<BattleTrigger>()){
				_collider.GetComponent<BattleTrigger>().TriggerBattle();
			}
				
		}

	}

	private void FixedUpdate() {

		//if (_gameController.State == GameState.GAME)
		if (canMove)
		{
			_input.x = Input.GetAxisRaw("Horizontal");
			_input.y = Input.GetAxisRaw("Vertical");

			transform.position = transform.position +
						new Vector3(_input.x, _input.y, 0).normalized * _moveSpeed * Time.deltaTime;
		}

	}


	private void OnTriggerStay2D(Collider2D other) {

		if(!_collider.GetComponent<BattleTrigger>()){
			_actionIcon.SetActive(true);
			_animator.SetBool("isShowing", true);
		}

	}

	private void OnTriggerEnter2D(Collider2D other) {
		action = true;
		_collider = other.gameObject;
	}

	private void OnTriggerExit2D(Collider2D other) {
		_animator.SetBool("isShowing", false);
		action = false;
		_collider = null;
	}

	private void HideActionIcon(){
		_actionIcon.SetActive(false);
	}

	public void TakeDamage(int damage){
		this.HP = this.HP - damage;
		if(this.HP < 0) this.HP = 0;
	}
}
