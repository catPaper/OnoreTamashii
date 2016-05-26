using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Character {

	//キャラクターが属するステージのインフォメーション
	[SerializeField]
	private StageInformation _myStageInfo;

	private enum_Direction _myDirection;

	//敵の最大探索範囲（大きすぎると処理が重くなる)
	private int _enemyMaxSearchDistance = 5;


	/// <summary>
	/// Gets my stage info.
	/// </summary>
	/// <param name="stageInfo">Stage info.</param>
	public void GetMyStageInfo(StageInformation stageInfo){
		_myStageInfo = stageInfo;
	}

	void Update()
	{
		//Debug
		SetState(enum_State.STAY);

		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.X)){
			_myDirection = enum_Direction.UNDER;
		}else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
			_myDirection = enum_Direction.UP;
		}else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
			_myDirection = enum_Direction.RIGHT;
		}else if(Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.A)){
			_myDirection = enum_Direction.LEFT;
		}else if(Input.GetKeyDown(KeyCode.Q)){
			_myDirection = enum_Direction.UPPERLEFT;
		}else if(Input.GetKeyDown(KeyCode.E)){
			_myDirection = enum_Direction.UPPERRIGHT;
		}else if(Input.GetKeyDown(KeyCode.Z)){
			_myDirection = enum_Direction.UNDERLEFT;
		}else if(Input.GetKeyDown(KeyCode.C)){
			_myDirection = enum_Direction.UNDERRIGHT;
		}else{
			//Nothing 
		}

		SetDirection(_myDirection);

		if(Input.GetKeyDown(KeyCode.Space) && !IsITweenMove()){
			List<enum_Direction> spaceDirections = SpaceDirections(gameObject.transform.position,_myStageInfo);
			for(int i= 0;i<spaceDirections.Count;i++){
				if(_myDirection == spaceDirections[i]){
					_myStageInfo.CharacterMove(gameObject.transform.position,_myDirection);
					SetState(enum_State.MOVE);
					EnemySearchCheck ();
					_myStageInfo.PlayerActEnd ();
					break;
				}
			}	
		}

		Action();
		//Debug
	}

	/// <summary>
	/// 敵の探索内かどうかチェック
	/// </summary>
	private void EnemySearchCheck()
	{
		List<Vector3> enemyPosList = CharacterExistPosition (StageInformation.enum_CharacterType.ENEMY, _myStageInfo, _enemyMaxSearchDistance);

		if (enemyPosList.Count == 0) {
			return;
		}
		Debug.Log ("enemyCount=" + enemyPosList.Count);
		GameObject enemyObject;
		for (int i = 0; i < enemyPosList.Count; i++) {
			enemyObject = _myStageInfo.EnemyPrefab (enemyPosList [i]);
			if (enemyObject != null) {
				enemyObject.GetComponent<Enemy> ().isPlayerInMyRange = true;
			}
		}
	}
}
