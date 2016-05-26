using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour {

	public enum enum_Direction{
		UPPERLEFT=0,
		UP=1,
		UPPERRIGHT=2,
		LEFT=3,
		//(処理の都合上4は空ける)
		RIGHT = 5,
		UNDERLEFT = 6,
		UNDER = 7,
		UNDERRIGHT = 8
	}

	public enum enum_State{
		STAY,
		MOVE,
		ATTACK
	}

	private float actSpeed = .25f;
	private bool isAnimate;
	private bool isITweenMove;

	private enum_Direction _direction;
	private enum_State _state;
	private Animator _myAnim;


	Vector3 moveAmount = Vector3.zero;

	private void Awake()
	{
		isITweenMove = false;
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
		isITweenMove = true;
		iTween.MoveBy(this.gameObject,iTween.Hash("x",moveAmount.x,"z",moveAmount.z,"time",actSpeed,"oncomplete","ITweenMoveEndHandler","oncompleteTarget",this.gameObject));
		//iTween.MoveBy(this.gameObject,moveAmount,actSpeed);
	}

	private void ITweenMoveEndHandler(){
		isITweenMove = false;
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


	/// <summary>
	/// iTweenによる移動中であるかどうか
	/// </summary>
	/// <returns><c>true</c> if this instance is I tween move; otherwise, <c>false</c>.</returns>
	public bool IsITweenMove()
	{
		return isITweenMove;
	}

	/// <summary>
	/// キャラクターが自分の上下左右[searchVolume]マス分に存在するかどうか（上下左右にsearchVolume分伸ばした正方形内）
	/// </summary>
	/// <param name="charaType">Chara type.</param>
	/// <param name="searchVolume">Search volume.</param>
	public List<Vector3> CharacterExistPosition(StageInformation.enum_CharacterType charaType,StageInformation targetStageInfo,int searchVolume){

		List<Vector3> list_charaPos = new List<Vector3> ();

		Vector3 myPos = this.gameObject.transform.position;
		//誤差を丸める
		myPos.x = Mathf.RoundToInt(myPos.x);
		myPos.z = Mathf.RoundToInt (myPos.z);

		Vector3 startPos = this.gameObject.transform.position;
		//誤差を丸める
		startPos.x = Mathf.RoundToInt(startPos.x);
		startPos.z = Mathf.RoundToInt (startPos.z);

		int searchMax = (2 * searchVolume + 1);
		//右側がsearchVolumeの範囲に収まらないかチェック
		if ((targetStageInfo.StageLength () - startPos.x) <= searchVolume) {
			startPos.x = Mathf.Max (0, (targetStageInfo.StageLength () - searchMax));	//2*searchVolume -1 でサーチできるギリギリ左側に設定できる
		} else {
			//左側がsearchVolumeの範囲に収まるかチェック
			startPos.x = Mathf.Max (0, startPos.x - searchVolume);
		}
		//下側がsearchVolumeの範囲に収まらないかチェック
		if (startPos.z <= searchVolume) {
			startPos.x = Mathf.Min (targetStageInfo.StageLength(), searchMax);	//2*searchVolume +1 でサーチできるギリギリ上側に設定できる
		} else {
			//上側がsearchVolumeの範囲に収まるかチェック
			startPos.z = Mathf.Min (targetStageInfo.StageLength()-searchMax, startPos.z + searchVolume);
		}
		//毎回xのポジションを設定し直すため値を保存しておく
		int savedStartPosX = (int)startPos.x;
		for (int zCounter = 0; zCounter < searchMax; zCounter++) {
			for (int xCounter = 0; xCounter < searchMax; xCounter++) {
				Debug.Log ("SearchPos=" + startPos);
				//自身はサーチ対象外
				if (myPos.x == startPos.x && myPos.z == startPos.z) {
					break;
				}
				if (targetStageInfo.WhatIsTheCharacterType (startPos) == charaType) {
					list_charaPos.Add (startPos);
				}

				startPos.x++;
				//範囲外参照回避
				if (startPos.x > targetStageInfo.StageLength ()) {
					break;
				}
			}
			startPos.x = savedStartPosX;
			startPos.z--;
			//範囲外参照回避
			if (startPos.z < 0) {
				break;
			}
		}


		return list_charaPos;
	}

	/// <summary>
	/// キャラクターの座標から指定方向に1マス移動した座標
	/// </summary>
	/// <returns>The around character direction.</returns>
	/// <param name="targetPos">Target position.</param>
	/// <param name="targetStageInfo">Target stage info.</param>
	/// <param name="targetDirection">Target direction.</param>
	public Vector3 PosAroundCharacterDirection(Vector3 targetPos,StageInformation targetStageInfo,enum_Direction targetDirection){
		//誤差を丸める
		targetPos.x = Mathf.RoundToInt(targetPos.x);
		targetPos.z = Mathf.RoundToInt (targetPos.z);

		int addX;
		int addZ;

		addX = ((int)targetDirection % 3) - 1;
		addZ = (-(int)targetDirection / 3) + 1;

		Vector3 movePos = new Vector3 (targetPos.x + addX, targetPos.y, targetPos.z + addZ);
		return movePos;
	}

	/// <summary>
	/// 指定方向に障害物（キャラクター、ブロック）が存在するか
	/// </summary>
	/// <returns><c>true</c> if this instance is exist obstacle the specified ownPos direction; otherwise, <c>false</c>.</returns>
	/// <param name="ownPos">Own position.</param>
	/// <param name="direction">Direction.</param>
	public bool IsExistObstacle(Vector3 ownPos,StageInformation targetStageInfo, enum_Direction direction){
		Vector3 targetPos = PosAroundCharacterDirection (ownPos, targetStageInfo, direction);

		//キャラクターチェック
		if (targetStageInfo.WhatIsTheCharacterType (targetPos) != StageInformation.enum_CharacterType.NULL) {
			return true;
		}
		//ブロックチェック
		if (targetStageInfo.WhatIsTheGroundType (targetPos) == StageInformation.enum_GroundType.WALL) {
			return true;
		}

		return false;
	}

	/// <summary>
	/// ターゲットの座標から空いているマスを返す(斜め方向の場合は縦横に障害物チェックを行う）
	/// </summary>
	/// <returns>The directions.</returns>
	/// <param name="targetPos">Target position.</param>
	public List<Character.enum_Direction> SpaceDirections(Vector3 targetPos,StageInformation targetStageInfo){
		//座標の誤差を丸める
		targetPos.x = Mathf.RoundToInt(targetPos.x);
		targetPos.z = Mathf.RoundToInt(targetPos.z);

		List<Character.enum_Direction> spaceDirections = new List<Character.enum_Direction>();
		bool[] existDirectionObstalce = new bool[9];	//depend on enum_Direction
		//まずは障害物が存在するか判定
		for (int i = 0; i < 9; i++) {
			if (i != 4) {
				existDirectionObstalce[i] = IsExistObstacle (targetPos, targetStageInfo, (enum_Direction)i);
			}
		}
		//上下左右はtrueなら格納
		if (!existDirectionObstalce [(int)enum_Direction.UP]) {
			spaceDirections.Add (enum_Direction.UP);
			//左、右上の判定
			if (!existDirectionObstalce [(int)enum_Direction.LEFT] && !existDirectionObstalce [(int)enum_Direction.UPPERLEFT]) {
				spaceDirections.Add (enum_Direction.UPPERLEFT);
			}
			if (!existDirectionObstalce [(int)enum_Direction.RIGHT] && !existDirectionObstalce [(int)enum_Direction.UPPERRIGHT]) {
				spaceDirections.Add (enum_Direction.UPPERRIGHT);
			}
		}
		if (!existDirectionObstalce [(int)enum_Direction.UNDER]) {
			spaceDirections.Add (enum_Direction.UNDER);
			//左、右上の判定
			if (!existDirectionObstalce [(int)enum_Direction.LEFT] && !existDirectionObstalce [(int)enum_Direction.UNDERLEFT]) {
				spaceDirections.Add (enum_Direction.UNDERLEFT);
			}
			if (!existDirectionObstalce [(int)enum_Direction.RIGHT] && !existDirectionObstalce [(int)enum_Direction.UNDERRIGHT]) {
				spaceDirections.Add (enum_Direction.UNDERRIGHT);
			}
		}
		if (!existDirectionObstalce [(int)enum_Direction.LEFT]) {
			spaceDirections.Add (enum_Direction.LEFT);
		}
		if (!existDirectionObstalce [(int)enum_Direction.RIGHT]) {
			spaceDirections.Add (enum_Direction.RIGHT);
		}




		return spaceDirections;
	}

}
