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
	[SerializeField] private Animator _animatorActionBubble;
	[SerializeField] private Animator _animatorPlayer;

	public Rigidbody2D _rb;
	public SpriteRenderer _sprite;

	public List<Sprite> nSprites;
	public List<Sprite> neSprites;
	public List<Sprite> eSprites;
	public List<Sprite> seSprites;
	public List<Sprite> sSprites;
	
	public float _walkSpeed;
	public float _framesRate;
	private float _idleTime;

	//public event Action OnDialogue;
	//public event Action OnBattle;


	[SerializeField] public Camera _camera;
    [SerializeField] public float _moveSpeed = 5;
	[SerializeField] public GameObject _actionIcon;

	private Vector3 direction;

	public bool action = false;
	public bool canMove = true;

	public int level;
	public int maxhp;
	public int hp;
	public int attack;
	public int defense;

	public int Level { get; set; }
	public int MaxHP { get; set; }
	public int HP { get; set; }
	public int MP { get; set; }
	public int Attack { get; set; } 
	public int Speed { get; set; }
	public int Defense { get; set; }

	private void Awake() {
		//_animatorActionBubble = GetComponent<Animator>();
	}


    // Start is called before the first frame update
    void Start()
    {
		// _gameController = FindObjectOfType<GameController>();

		this.Level = 1;
		this.MaxHP = 25 * this.Level;
		this.HP = this.MaxHP;
		this.Attack = 100;
		this.Defense = 250;
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

		if (canMove){
			_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
			_rb.velocity = _input * _walkSpeed;
			HandleSpriteFlip();
			SetSprite();
		}
		

	}

	void SetSprite(){
		List<Sprite> dirSprite = GetSpriteDirection();

		if(dirSprite != null){
			float playTime = Time.time - _idleTime;
			int totalFrames = (int)(playTime * _framesRate);
			int frame = totalFrames  % dirSprite.Count;
			_sprite.sprite = dirSprite[frame];
		} else {
			_idleTime = Time.time;
			//_sprite.sprite = dirSprite[0];
		}
	}

	List<Sprite> GetSpriteDirection(){

		List<Sprite> selSprite = null;

		if(_input.y > 0){
			if(Mathf.Abs(_input.x) > 0){
				selSprite = neSprites;
			} else {
				selSprite = nSprites;
			}
		} else if (_input.y < 0){
			if(Mathf.Abs(_input.x) > 0){
				selSprite = seSprites;
			} else {
				selSprite = sSprites;
			}
		} else {
			if(Mathf.Abs(_input.x) > 0){
				selSprite = eSprites;
			}
		}

		return selSprite;
	}

	void HandleSpriteFlip(){
		if(!_sprite.flipX && _input.x < 0){
			_sprite.flipX = true;
		} else if (_sprite.flipX && _input.x > 0){
			_sprite.flipX = false;
		}
	}

	private void FixedUpdate() {

		// //if (_gameController.State == GameState.GAME)
		// if (canMove)
		// {
		// 	_input.x = Input.GetAxisRaw("Horizontal");
		// 	_input.y = Input.GetAxisRaw("Vertical");

		// 	direction = new Vector3(_input.x, _input.y, 0).normalized;
		// 	transform.position = transform.position + direction * _moveSpeed * Time.deltaTime;

		// 	_animatorPlayer.SetInteger("Direction", GetDirection());
		// }

	}

	private int GetDirection(){
		if(direction.y == -1){
            return 0;
        } else if(direction.y == 1){
            return 2;
        } else if(direction.x == -1){
            return 3;
        } else if(direction.x == 1){
            return 1;
		} else if(direction.y < 0 && direction.x < 0){
            return 3;
        } else if(direction.y > 0 && direction.x < 0){
            return 3;
        } else if(direction.y > 0 && direction.x > 0){
            return 1;
        } else if(direction.y < 0 && direction.x > 0){
            return 1;
        } else {
            return -1;
        }
	}


	private void OnTriggerStay2D(Collider2D other) {

		if(_collider.GetComponent<DialogueTrigger>()){
			_actionIcon.SetActive(true);
			_animatorActionBubble.SetBool("isShowing", true);
		}

	}

	private void OnTriggerEnter2D(Collider2D other) {
		action = true;
		_collider = other.gameObject;
	}

	private void OnTriggerExit2D(Collider2D other) {
		_animatorActionBubble.SetBool("isShowing", false);
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
