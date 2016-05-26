using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Character {

	//キャラクターが属するステージインフォメーション
	private StageInformation _myStageInfo;

	[SerializeField][Header("自分の場所から上下左右に広がるサーチマス数")]
	private int SearchVolume = 5;

	//プレイヤーが自分のサーチ範囲内にいる可能性があるかどうか
	public bool isPlayerInMyRange = false;

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

		List<enum_Direction> _mySpaceDirections = SpaceDirections(gameObject.transform.position,_myStageInfo);
		if(_mySpaceDirections.Count == 0){
			//攻撃処理がなければ待機
			return;
		}

		int randNumber;

		//移動処理
		//プレイヤーがサーチ範囲内かどうか
		if (isPlayerInMyRange) {
			List<Vector3> playerPosList = CharacterExistPosition (StageInformation.enum_CharacterType.PLAYER, _myStageInfo, SearchVolume);
			//範囲内の場合近づけるDirectionのみを残しほかは削除（今回は近づける方向がない場合何もしない）
			if (playerPosList.Count != 0) {
				//指定方向にひとます移動した座標
				Vector3 directionPos;
				for (int i = 0; i < _mySpaceDirections.Count; i++) {
					directionPos = PosAroundCharacterDirection (this.gameObject.transform.position, _myStageInfo, _mySpaceDirections [i]);
					//移動後の方が距離が近くならない場合削除
					if (Vector3.Distance (playerPosList[0], directionPos) > (Vector3.Distance (playerPosList[0], this.gameObject.transform.position))) {
						_mySpaceDirections.RemoveAt (i);
						i--;	//取り除いたため調節
					}
				}
				if (_mySpaceDirections.Count == 0) {
					//攻撃処理がなければ待機
					return;
				}
			} else {
				isPlayerInMyRange = false;
			}
		}
		randNumber = Random.Range (0, _mySpaceDirections.Count - 1);
		_myStageInfo.CharacterMove(gameObject.transform.position,_mySpaceDirections[randNumber]);
		SetDirection(_mySpaceDirections[randNumber]);
		SetState(enum_State.MOVE);

		Action();
	}
}
