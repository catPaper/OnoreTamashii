using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour {

	public enum enum_Direction{
		UP,
		UPPERRIGHT,
		RIGHT,
		UNDERRIGHT,
		UNDER,
		UNDERLEFT,
		LEFT,
		UPPERLEFT
	}

	public enum enum_State{
		STAY,
		MOVE,
		ATTACK
	}

	private float actSpeed = 1f;
	private bool isAnimate;

	private enum_Direction _direction;
	private enum_State _state;
	private Animator _myAnim;

	Vector3 moveAmount = Vector3.zero;

	private void Awake()
	{
		_myAnim = GetComponent<Animator>();
	}

	/// <summary>
	/// 行動を行う
	/// </summary>
	public void Action(){
		
		//TODO 全体の行動が終えるまで次の行動を起こさない

		switch(_state){
		case enum_State.STAY:
			LookDirection();
			break;
		case enum_State.MOVE:
			LookDirection();
			Move();
			break;
		case enum_State.ATTACK:
			Attack();
			break;
		}
	}

	/// <summary>
	/// 指定の方向を向く
	/// </summary>
	private void LookDirection()
	{
		if(_state != enum_State.MOVE){
			//_myAnim.speed = 0;
			//debug
			_myAnim.speed = 1;
		}else{
			_myAnim.speed = 1;
		}

		SetAnim();
	}

	/// <summary>
	/// 移動処理
	/// </summary>
	private void Move(){

		switch(_direction){
		case enum_Direction.UNDER:
			moveAmount = new Vector3(0,0,-1);
			TweenMove();
			break;
		case enum_Direction.UNDERLEFT:
			moveAmount = new Vector3(-1,0,-1);
			TweenMove();
			break;
		case enum_Direction.UNDERRIGHT:
			moveAmount = new Vector3(1,0,-1);
			TweenMove();
			break;
		case enum_Direction.UP:
			moveAmount = new Vector3(0,0,1);
			TweenMove();
			break;
		case enum_Direction.UPPERLEFT:
			moveAmount = new Vector3(-1,0,1);
			TweenMove();
			break;
		case enum_Direction.UPPERRIGHT:
			moveAmount = new Vector3(1,0,1);
			TweenMove();
			break;
		case enum_Direction.LEFT:
			moveAmount = new Vector3(-1,0,0);
			TweenMove();
			break;
		case enum_Direction.RIGHT:
			moveAmount = new Vector3(1,0,0);
			TweenMove();
			break;
		default:
			break;
		}

	}

	/// <summary>
	/// iTweenによる移動
	/// </summary>
	private void TweenMove()
	{
		iTween.MoveBy(this.gameObject,moveAmount,actSpeed);
	}

	/// <summary>
	/// 攻撃処理
	/// </summary>
	private void Attack(){
		//TODO 攻撃処理
	}

	/// <summary>
	/// 方角アニメーションを設定する
	/// </summary>
	/// <param name="direction">Direction.</param>
	private void SetAnim(){
		_myAnim.SetBool("isUnder",(_direction == enum_Direction.UNDER));
		_myAnim.SetBool("isUnderLeft",(_direction == enum_Direction.UNDERLEFT));
		_myAnim.SetBool("isUnderRight",(_direction == enum_Direction.UNDERRIGHT));
		_myAnim.SetBool("isUpperLeft",(_direction == enum_Direction.UPPERLEFT));
		_myAnim.SetBool("isUpperRight",(_direction == enum_Direction.UPPERRIGHT));
		_myAnim.SetBool("isUp",(_direction == enum_Direction.UP));
		_myAnim.SetBool("isRight",(_direction == enum_Direction.RIGHT));
		_myAnim.SetBool("isLeft",(_direction == enum_Direction.LEFT));

	}

	/// <summary>
	/// Sets the direction.
	/// </summary>
	/// <param name="direction">Direction.</param>
	public void SetDirection(enum_Direction direction){
		_direction = direction;
	}

	/// <summary>
	/// Sets the state.
	/// </summary>
	/// <param name="state">State.</param>
	public void SetState(enum_State state){
		_state = state;
	}





}
