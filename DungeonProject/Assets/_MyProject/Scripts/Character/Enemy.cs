using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Character {

	//キャラクターが属するステージインフォメーション
	private StageInformation _myStageInfo;


	/// <summary>
	/// Gets my stage info.
	/// </summary>
	/// <param name="stageInfo">Stage info.</param>
	public void GetMyStageInfo(StageInformation stageInfo){
		_myStageInfo = stageInfo;
	}

	/// <summary>
	/// 行動を行う
	/// </summary>
	public void EnemyAction()
	{
		//TODO 待機したり、攻撃したりする処理

		List<enum_Direction> _mySpaceDirections = _myStageInfo.SpaceDirections(gameObject.transform.position);

		if(_mySpaceDirections.Count == 0){
			//攻撃処理がなければ待機
			return;
		}

		int randNumber = Random.Range(0,_mySpaceDirections.Count-1);

		//移動処理
		_myStageInfo.CharacterMove(gameObject.transform.position,_mySpaceDirections[randNumber]);
		SetDirection(_mySpaceDirections[randNumber]);
		SetState(enum_State.MOVE);

		Action();
	}
}
