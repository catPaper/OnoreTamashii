using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageInformation : MonoBehaviour {


	public enum enum_GroundType{
		WALL,
		ROOM,
		AILSLE
	}

	public enum enum_ObjectType{
		STAIRS,
		ITEM,
		TRAP,
		NULL
	}

	public enum enum_CharacterType{
		PLAYER,
		ENEMY,
		NULL
	}

	public struct struct_roomInfo{
		int roomNO;
		//int roomSize;	//はかるとこも可能
		Vector2 roomPos;

		public int RoomNO(){
			return roomNO;
		}

		public Vector2 RoomPos(){
			return roomPos;
		}

		public int RoomPosX(){
			return (int)roomPos.x;
		}

		public int RoomPosZ(){
			return (int)roomPos.y;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StageInformation+struct_roomInfo"/> struct.
		/// </summary>
		/// <param name="num">部屋NO</param>
		/// <param name="pos">タイルポジション</param>
		public struct_roomInfo(int num,Vector2 pos){
			roomNO = num;
			roomPos = pos;
		}
	}


	public int StageLevel;		//これをもとに敵のレベル調整や種類の変更をデータベーススクリプトで行う
	//グラウンドタイプはジェネレータよりリストに情報がセットされる
	public List<enum_GroundType> _groundTypeList;
	private List<enum_ObjectType> _objectTypeList;
	private List<enum_CharacterType> _characterTypeList;
	//ルームインフォはジェネレータよりリストに情報がセットされる
	public List<struct_roomInfo> _roomInfoList;

	//敵キャラクターのリスト
	private List<GameObject> _enemyList;



	private StageObjectDataBase _stgObjDtbs;
	private int stageLength;

	public int StageLength()
	{
		return stageLength;
	}

	void Start()
	{
		_stgObjDtbs = GameObject.FindGameObjectWithTag ("StageManager").GetComponent<StageObjectDataBase> ();
		if (transform.childCount == 0) {
			stageLength = (int)Mathf.Sqrt(_groundTypeList.Count);

			//objectTypeListとcharacterTypeListをステージの大きさだけNULLで初期化
			_objectTypeList = new List<enum_ObjectType>();
			_characterTypeList = new List<enum_CharacterType>();
			for(int i= 0;i<_groundTypeList.Count;i++){
				_objectTypeList.Add(enum_ObjectType.NULL);
				_characterTypeList.Add(enum_CharacterType.NULL);
			}

			ShowStageObjects ();
		}
	}



	//キャラクターを指定方向に移動させる（リストの座標のみ）
	public void CharacterMove(Vector3 targetPos,Character.enum_Direction targetDirection){
		//座標の誤差を丸める
		targetPos.x = Mathf.RoundToInt(targetPos.x);
		targetPos.z = Mathf.RoundToInt(targetPos.z);
		//入れ替え先座標
		Vector3 swapPos = new Vector3();
		swapPos.x = targetPos.x +((int)targetDirection%3 - 1);	//（X%3-1）→これで-1,0,1になりx座標をちょうせつ　できる
		swapPos.z = targetPos.z -((int)targetDirection/3 - 1);	// (X/3-1) →これで-1,0,1になりz座標を調節できる
		//入れ替え
		enum_CharacterType tmpType = _characterTypeList[(int)targetPos.x + ((int)targetPos.z*stageLength)];
		_characterTypeList[(int)targetPos.x + ((int)targetPos.z * stageLength)] = _characterTypeList[(int)swapPos.x + ((int)swapPos.z*stageLength)];
		_characterTypeList[(int)swapPos.x+((int)swapPos.z*stageLength)] = tmpType;
	}

	private void ShowStageObjects()
	{
		//_stageの下の階層
		GameObject groundLayer = new GameObject();
		groundLayer.name = "groundLayer";
		groundLayer.transform.SetParent (transform);
		GameObject objectLayer = new GameObject();
		objectLayer.name = "objectLayer";
		objectLayer.transform.SetParent (transform);
		GameObject characterLayer = new GameObject();
		characterLayer.name = "characterLayer";
		characterLayer.transform.SetParent (transform);
		//groundLayerの下の階層
		GameObject wall = new GameObject();
		wall.name = "wall";
		wall.transform.SetParent (groundLayer.transform);
		GameObject room = new GameObject();
		room.name = "room";
		room.transform.SetParent (groundLayer.transform);
		GameObject aisle = new GameObject();
		aisle.name = "aisle";
		aisle.transform.SetParent (groundLayer.transform);

		//オブジェクトの格納

		//groundLayerに格納
		for (int i = 0; i < stageLength; i++) {
			for (int j = 0; j < stageLength; j++) {
				switch (_groundTypeList [(i * stageLength) + j]) {
				case enum_GroundType.WALL:
					GameObject wallPrefab = Instantiate(_stgObjDtbs.GetWallPrefab());
					wallPrefab.name = "wallPrefab";
					wallPrefab.transform.SetParent (wall.transform);
					wallPrefab.transform.position = new Vector3 (j, 0, i);
					break;
				case enum_GroundType.ROOM:
					GameObject roomPrefab = Instantiate (_stgObjDtbs.GetFloorPrefab ());
					roomPrefab.name = "roomPrefab";
					roomPrefab.transform.SetParent (room.transform);
					roomPrefab.transform.position = new Vector3 (j, 0, i);
					break;
				case enum_GroundType.AILSLE:
					GameObject aislePrefab = Instantiate (_stgObjDtbs.GetFloorPrefab ());
					aislePrefab.name = "aislePrefab";
					aislePrefab.transform.SetParent (aisle.transform);
					aislePrefab.transform.position = new Vector3 (j,0,i);
					break;
				}
			}
		}

		InitSetCharacters();

		//characterLayerに格納
		_enemyList = new List<GameObject>();
		for(int i=0;i<stageLength;i++){
			for(int j= 0;j<stageLength;j++){
				switch(_characterTypeList[(i*stageLength)+j]){
				case enum_CharacterType.PLAYER:
					GameObject playerPrefab = Instantiate (_stgObjDtbs.GetCharacterPrefab(StageObjectDataBase.enum_CharacterType.PLAYER));
					//--プレイヤースクリプトの初期化
					Player playerScript = playerPrefab.GetComponent<Player>();
					playerScript.GetMyStageInfo(this);
					//--
					playerPrefab.name = "playerPrefab";
					playerPrefab.transform.SetParent (characterLayer.transform);
					playerPrefab.transform.position = new Vector3 (j,0.5f,i);
					break;
				case enum_CharacterType.ENEMY:
					//TODO　敵の種類を変える処理
					GameObject enemyPrefab = Instantiate(_stgObjDtbs.GetCharacterPrefab(StageObjectDataBase.enum_CharacterType.ENEMY_A));
					//---エネミースクリプトの初期化
					Enemy enemyScript = enemyPrefab.GetComponent<Enemy>();
					enemyScript.GetMyStageInfo(this);
					//---
					enemyPrefab.name = "enemyPrefab";
					enemyPrefab.transform.SetParent(characterLayer.transform);
					enemyPrefab.transform.position = new Vector3(j,0.5f,i);
					_enemyList.Add(enemyPrefab);
					break;
				case enum_CharacterType.NULL:
					//Nothing
					break;
				}
			}
		}



		InitSetObjects();

		//objectLayerに格納
		for(int i= 0;i< stageLength;i++){
			for(int j= 0;j< stageLength;j++){
				switch(_objectTypeList[(i*stageLength)+j]){
				case enum_ObjectType.ITEM:
					//TODO
					break;
				case enum_ObjectType.STAIRS:
					//TODO
					break;
				case enum_ObjectType.TRAP:
					//TODO
					break;
				case enum_ObjectType.NULL:
					//Nothing
					break;
				}
			}
		}
			
	}

	//プレイヤーの行動が終了し、敵のプレイヤーの行動へ移る
	public void PlayerActEnd(){
		for(int i= 0;i<_enemyList.Count;i++){
			_enemyList[i].GetComponent<Enemy>().EnemyAction();
		}
	}
		

	/// <summary>
	/// オブジェクトの初期配置
	/// </summary>
	private void InitSetObjects(){
		//TODO
	}

	/// <summary>
	/// キャラクターの初期配置
	/// 配置方法（部屋の数の1.5倍の敵キャラクターをランダム配置する）敵キャラの復活は現在考えていない
	/// </summary>
	private void InitSetCharacters(){
		//座標管理
		int x,z;
		//配列番号管理
		int randPos;
		//敵キャラクターの登録
		int enemyCharaCount = (int)((_roomInfoList[_roomInfoList.Count-1].RoomNO()+1)*1.5);
		for(int i= 0;i<enemyCharaCount;i++){
			randPos = Random.Range(0,_roomInfoList.Count-1);
			x = _roomInfoList[randPos].RoomPosX();
			z = _roomInfoList[randPos].RoomPosZ();
			//キャラクターリストへ登録
			_characterTypeList[x+(z*stageLength)] = enum_CharacterType.ENEMY;
			//ルームリストから削除
			_roomInfoList.RemoveAt(randPos);
		}
		//プレイヤーの登録
		randPos = Random.Range(0,_roomInfoList.Count-1);
		x = _roomInfoList[randPos].RoomPosX();
		z = _roomInfoList[randPos].RoomPosZ();
		//キャラクターリストへ登録
		_characterTypeList[x+(z*stageLength)] = enum_CharacterType.PLAYER;
		//ルームリストから削除
		_roomInfoList.RemoveAt(randPos);
	}

	/// <summary>
	/// ポジションに一致する敵キャラプレファブを返す
	/// </summary>
	/// <returns>The prefab.</returns>
	/// <param name="pos">Position.</param>
	public GameObject EnemyPrefab(Vector3 targetPos){
		int enemyPosX;
		int enemyPosZ;
		for (int i = 0; i < _enemyList.Count; i++) {
			enemyPosX = Mathf.RoundToInt (_enemyList [i].transform.position.x);
			enemyPosZ = Mathf.RoundToInt (_enemyList [i].transform.position.z);
			if (targetPos.x == enemyPosX && targetPos.z == enemyPosZ) {
				return _enemyList [i];
			}
		}

		Debug.Log ("error:Nothing to targetObject");
		return null;
	}

	/// <summary>
	/// 指定座標のグラウンドタイプを返す
	/// </summary>
	/// <returns>The is the object.</returns>
	/// <param name="_xzPos">Xz position.</param>
	public enum_GroundType WhatIsTheGroundType(Vector3 _xzPos){
		return _groundTypeList[(int)_xzPos.x + ((int)_xzPos.z * stageLength)];
	}

	/// <summary>
	/// 指定座標のオブジェクトタイプを返す
	/// </summary>
	/// <returns>The is T he object type.</returns>
	/// <param name="_xzPos">Xz position.</param>
	private enum_ObjectType WhatIsTheObjectType(Vector3 _xzPos){
		return _objectTypeList[(int)_xzPos.x +((int)_xzPos.z*stageLength)];
	}

	/// <summary>
	/// 指定座標のキャラクタータイプを返す
	/// </summary>
	/// <returns>The is the character type.</returns>
	/// <param name="_xzPos">Xz position.</param>
	public enum_CharacterType WhatIsTheCharacterType(Vector3 _xzPos){
		return _characterTypeList[(int)_xzPos.x +((int)_xzPos.z*stageLength)];
	}
}
