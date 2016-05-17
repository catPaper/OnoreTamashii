using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageInformation : MonoBehaviour {


	public enum enum_GroundType{
		WALL,
		ROOM,
		AILSLE
	}

	public List<enum_GroundType> _groundTypeList;

	private StageObjectDataBase _stgObjDtbs;

	void Start()
	{
		_stgObjDtbs = GameObject.FindGameObjectWithTag ("StageManager").GetComponent<StageObjectDataBase> ();
		if (transform.childCount == 0) {
			ShowStageObjects ();
		}
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
		int stageLength = (int)Mathf.Sqrt(_groundTypeList.Count);

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

	}
}
