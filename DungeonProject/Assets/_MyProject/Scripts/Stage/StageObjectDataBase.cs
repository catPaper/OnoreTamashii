using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageObjectDataBase : MonoBehaviour {

	public enum enum_WallType
	{
		TYPE_A,
		TYPE_B,
		TYPE_C
	}

	public enum enum_FloorType
	{
		TYPE_A,
		TYPE_B,
		TYPE_C
	}

	public enum enum_CharacterType{
		PLAYER,
		ENEMY_A,		//いずれ名前を付ける
		ENEMY_B
	}

	[Header("ゲーム内変更用")]
	[SerializeField]
	private enum_WallType _wallType;
	[SerializeField]
	private enum_FloorType _floorType;
	[Header("事前設定項目")]
	[SerializeField]
	private List<GameObject> _wallList;
	[SerializeField]
	private List<GameObject> _floorList;
	[SerializeField]
	private List<GameObject> _charaList;

	/// <summary>
	/// 壁のプレファブを取得
	/// </summary>
	/// <returns>The wall prefab.</returns>
	public GameObject GetWallPrefab()
	{
		ExistResourceCheck ();

		if(!ExistWallType(_wallType)){
			_wallType = enum_WallType.TYPE_A;
		}

		return _wallList [(int)_wallType];
	}

	/// <summary>
	/// 床のプレファブを取得
	/// </summary>
	/// <returns>The floor prefab.</returns>
	public GameObject GetFloorPrefab()
	{
		ExistResourceCheck ();


		if (!ExistFloorType (_floorType)) {
			_floorType = enum_FloorType.TYPE_A;
		}

		return _floorList [(int)_floorType];
	}

	public GameObject GetCharacterPrefab(enum_CharacterType _charaType)
	{
		ExistCharacterCheck(_charaType);

		if(!ExistCharacterType(_charaType)){
			_charaType = enum_CharacterType.ENEMY_A;
		}

		return _charaList[(int)_charaType];
	}

	/// <summary>
	/// 指定したキャラクタープレファブが存在するかチェック
	/// </summary>
	private void ExistCharacterCheck(enum_CharacterType _charaType)
	{
		if(!ExistCharacterType(_charaType)){
			Debug.Log("設定したキャラは存在しません");
		}
	}

	/// <summary>
	/// Exists the type of the character.
	/// </summary>
	/// <returns><c>true</c>, if character type was existed, <c>false</c> otherwise.</returns>
	private bool ExistCharacterType(enum_CharacterType charaType){
		return (_charaList[(int)charaType] != null);
	}

	/// <summary>
	/// 壁、床等の指定したオブジェクトが存在するかチェック
	/// </summary>
	private void ExistResourceCheck()
	{
		if (!ExistWallType(_wallType)) {
			Debug.Log ("設定した壁は存在しません。");
		}
		if (!ExistFloorType (_floorType)) {
			Debug.Log("設定した床は存在しません。");
		}
	}

	/// <summary>
	/// 指定した壁の種類が存在するかどうか
	/// </summary>
	/// <returns><c>true</c>, if wall type was existed, <c>false</c> otherwise.</returns>
	/// <param name="wallType">Wall type.</param>
	private bool ExistWallType(enum_WallType wallType)
	{
		return (_wallList [(int)wallType] != null);
	}

	/// <summary>
	/// 指定した床の種類が存在するかどうか
	/// </summary>
	/// <returns><c>true</c>, if floor type was existed, <c>false</c> otherwise.</returns>
	/// <param name="floorType">Floor type.</param>
	private bool ExistFloorType(enum_FloorType floorType)
	{
		return (_floorList [(int)floorType] != null);
	}

}
