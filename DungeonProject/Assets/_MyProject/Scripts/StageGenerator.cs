using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StageGenerator : MonoBehaviour {

	public enum enum_StageSize
	{
		SMALL = 12,
		MIDIUM = 24,
		LARGE = 48,
		HUGE = 96
	}

	public enum enum_JointSetting{
		ONE = 0,
		ALL = 1,
		RANDOM = 2
	}

	public struct struct_rooomInfo
	{
		int roomsx;		//部屋の開始座標x
		int roomsy;		//部屋の開始座標y
		int roomex;		//部屋の終了座標x
		int roomey;		//部屋の終了座標y
		int roomwidth;	//部屋の横幅
		int roomheight;	//部屋の縦幅

		int framesx;	//枠の開始座標x
		int framesy;	//枠の開始座標y
		int frameex;	//枠の終了座標x
		int frameey;	//枠の終了座標y
		int framewidth;	//枠の横幅
		int frameheight;//枠の縦幅


		public int Roomsx(){
			return roomsx;
		}
		public int Roomsy(){
			return roomsy;
		}
		public int Roomex(){
			return roomex;
		}
		public int Roomey(){
			return roomey;
		}
		public int Roomwidth(){
			return roomwidth;
		}
		public int Roomheight(){
			return roomheight;
		}

		public int Framesx(){
			return framesx;
		}
		public int Framesy(){
			return framesy;
		}
		public int Frameex(){
			return frameex;
		}
		public int Frameey(){
			return frameey;
		}
		public int Framewidth(){
			return framewidth;
		}
		public int FrameHeight(){
			return frameheight;
		}


		/// <summary>
		/// ルーム情報
		/// </summary>
		/// <param name="rsx">部屋の開始座標x</param>
		/// <param name="rsy">部屋の開始座標y</param>
		/// <param name="rw">部屋の横幅</param>
		/// <param name="rh">部屋の縦幅</param>
		/// <param name="fsx">枠の開始座標x</param>
		/// <param name="fsy">枠の開始座標y</param>
		/// <param name="fw">枠の横幅</param>
		/// <param name="fh">枠の縦幅</param>
		public struct_rooomInfo(int rsx,int rsy,int rw,int rh,int fsx,int fsy,int fw,int fh){
			roomsx = rsx;
			roomsy = rsy;
			roomwidth = rw;
			roomheight = rh;
			roomex = roomsx + rw;
			roomey = roomsy + rh;
			framesx = fsx;
			framesy = fsy;
			framewidth = fw;
			frameheight = fh;
			frameex = framesx + framewidth;
			frameey = framesy + frameheight;
		}
	};


	public struct struct_borderInfo{

		int sx;		//開始座標x
		int sy;		//開始座標y
		int ex;		//終了座標x
		int ey;		//終了座標y
		int length;	//長さ


		public int Sx(){ return sx; }

		public int Sy(){ return sy; }

		/// <summary>
		/// 水平線の時に使用
		/// </summary>
		public int Ex(){ return ex; }

		/// <summary>
		/// 緯線の時に使用
		/// </summary>
		public int Ey(){ return ey; }

		public int Length(){ return length; }

		/// <summary>
		/// ボーダー情報
		/// </summary>
		/// <param name="x">開始座標x</param>
		/// <param name="y">開始座標y</param>
		/// <param name="len">長さ</param>
		public struct_borderInfo(int x,int y,int len){
			sx = x;
			sy = y;
			length = len;
			ex = sx + length;
			ey = sy + length;
		}
	};
			

	[Header("ゲーム内変更用")]
	[SerializeField]
	private enum_StageSize _stageSize;
	[Header("通路接続設定（ONE...一つのみ,ALL...すべて連結,RANDOM...一つ～最大まで乱数で接続)")][SerializeField]
	private enum_JointSetting _jointSetting;




	/// <summary>
	/// ステージの縦横の長さ（正方形)
	/// </summary>
	private int stageLength;
	/// <summary>
	/// 部屋の最小の大きさ
	/// </summary>
	private int MinRoomLength = 8;
	/// <summary>
	/// 部屋の外周は壁とする
	/// </summary>
	private int Margin = 4;

	private GameObject _stage;

	private List<StageInformation.enum_GroundType> _groundTypeList;
	private List<struct_rooomInfo> _roomInfoList;
	private List<struct_borderInfo> _horizontalBorderInfoList;
	private List<struct_borderInfo> _verticalBorderInfoList;

	/// <summary>
	/// ステージのサイズを設定する
	/// </summary>
	/// <param name="_size">Size.</param>
	public void TestSetStageSize(int _sizeNumber){
		switch (_sizeNumber) {
		case 0:
			_stageSize = enum_StageSize.SMALL;
			break;
		case 1:
			_stageSize = enum_StageSize.MIDIUM;
			break;
		case 2:
			_stageSize = enum_StageSize.LARGE;
			break;
		case 3:
			_stageSize = enum_StageSize.HUGE;
			break;
		}
	}

	public void TestSetAisleSetting(int _typeNumber){
		_jointSetting = (enum_JointSetting)_typeNumber;
	}

	/// <summary>
	/// ステージを生成し、情報を渡す
	/// </summary>
	public GameObject GenerateStage()
	{
		SetStageLength ();
		InitStage();
		GenerateWall ();
		GenerateFloor ();
		StageInformation stgInfo = _stage.GetComponent<StageInformation>();
		stgInfo._groundTypeList = _groundTypeList;
		return _stage;
	}

	/// <summary>
	/// フロア（部屋・通路）の生成
	/// </summary>
	private void GenerateFloor()
	{
		//分割し、部屋を生成
		DividedIntoVertical(0,0,stageLength,stageLength);

		//通路の作成
		GenerateAisle();
	}

	/// <summary>
	/// 縦に分割する
	/// </summary>
	private void DividedIntoVertical(int sx,int sy,int width,int height)
	{
		//作成できない
		if ((2 * (Margin + MinRoomLength) + 1) > width) {
			GenerateRoom (sx, sy, width, height);
			return;
		}

		int dividedPoint = Random.Range (MinRoomLength + Margin + 1, width - MinRoomLength - Margin);
		int leftWidth = (dividedPoint -1);
		int rightWidth = width - dividedPoint;

		//境界線情報を格納(通路生成に使用)
		struct_borderInfo _thisBorderInfo = new struct_borderInfo(sx + dividedPoint,sy,height);	//TODO もしかするとdividedPoint -1の可能性
		_verticalBorderInfoList.Add(_thisBorderInfo);

		//更に分割
		DividedIntoHorizontal (sx, sy, leftWidth, height);
		DividedIntoHorizontal (sx + dividedPoint, sy, rightWidth, height);


	}

	/// <summary>
	/// 横に分割する
	/// </summary>
	private void DividedIntoHorizontal(int sx,int sy,int width,int height)
	{
		//作成できない地点にまで到達したので部屋を作成
		if ((2 * (Margin + MinRoomLength) + 1) > height) {
			GenerateRoom (sx, sy, width, height);
			return;
		}

		int dividedPoint = Random.Range (MinRoomLength + Margin + 1, height - MinRoomLength - Margin);
		int downHeight = (dividedPoint - 1);
		int upHeight = height - dividedPoint;

		//境界線情報を格納(通路生成に使用)
		struct_borderInfo _thisBorderInfo = new struct_borderInfo(sx,sy + dividedPoint,width);	//TODO もしかするとdividedPoint -1の可能性
		_horizontalBorderInfoList.Add(_thisBorderInfo);

		//さらに分割
		DividedIntoVertical (sx, sy, width, downHeight);
		DividedIntoVertical (sx, sy + dividedPoint, width, upHeight);
	}

	/// <summary>
	/// 部屋を生成(Margin含む）
	/// </summary>
	private void GenerateRoom(int sx,int sy,int width,int height)
	{
		
		//片方のマージン
		int otherMargin = Margin / 2;
		int leftx = Random.Range (otherMargin, width - (otherMargin + MinRoomLength));
		int roomWidth = Random.Range (MinRoomLength, width - (otherMargin + leftx));
		int downy = Random.Range (otherMargin, height - (otherMargin + MinRoomLength));
		int roomHeight = Random.Range (MinRoomLength, height - (otherMargin + downy));

		if (roomWidth < MinRoomLength || roomHeight < MinRoomLength) {
			Debug.Log ("error:部屋の生成に失敗しました。");
			return;
		}

		int roomsx = sx + leftx;
		int roomsy = sy + downy;

		//部屋生成
		for (int i = 0; i < roomHeight; i++) {
			for (int j = 0; j < roomWidth; j++) {
				_groundTypeList [(roomsx + j) + ((roomsy + i) * stageLength)] = StageInformation.enum_GroundType.ROOM;
			}
		}

		//部屋情報を格納(通路生成に使用)
		struct_rooomInfo thisRoomInfo = new struct_rooomInfo(roomsx,roomsy,roomWidth,roomHeight,sx,sy,width,height);	//数値を調整
		_roomInfoList.Add(thisRoomInfo);
	}

	/// <summary>
	/// 通路の生成
	/// </summary>
	private void GenerateAisle()
	{
		if (_roomInfoList.Count < 0) {
			Debug.Log ("通路を生成するための条件をみたしていません。");
			return;
		}

		//つなげるための部屋情報
		List<struct_rooomInfo> _jointRoomInfoList_first  = new List<struct_rooomInfo>();	//境界線のsecondの反対側のルームリスト	
		List<struct_rooomInfo> _jointRoomInfoList_second  = new List<struct_rooomInfo>();	//境界線のfirstの反対側のルームリスト

		//まずは水平ボーダーから連結
		for (int i = 0; i < _horizontalBorderInfoList.Count; i++) {
			for (int j = 0; j < _roomInfoList.Count; j++) {
				//Debug.Log ("borderY=" + _horizontalBorderInfoList [i].Sy() + "borderX=" + _horizontalBorderInfoList[i].Sx()+"borderLength="+_horizontalBorderInfoList[i].Length()+":frameSy=" + _roomInfoList [j].Framesy () + ":frameEy=" + _roomInfoList [j].Frameey () + ":frameSx=" + _roomInfoList[i].Framesx() + ":frameEx=" + _roomInfoList[i].Frameex());
				//対象ボーダーのx座標において範囲内かどうか判定
				if (_horizontalBorderInfoList [i].Sx() <= _roomInfoList [j].Framesx() && _horizontalBorderInfoList [i].Ex () >= _roomInfoList [j].Frameex ()) {
					//境界線の上に属するかチェック
					if (_horizontalBorderInfoList [i].Sy ()-1 <= _roomInfoList [j].Framesy () && _horizontalBorderInfoList[i].Sy()+1 >= _roomInfoList[j].Framesy()) {
						_jointRoomInfoList_first.Add (_roomInfoList [j]);
					}
					//境界線の下に属するかチェック
					if (_horizontalBorderInfoList [i].Sy ()-1 <= _roomInfoList [j].Frameey () && _horizontalBorderInfoList[i].Sy()+1 >= _roomInfoList[j].Frameey()) {
						_jointRoomInfoList_second.Add (_roomInfoList [j]);
					}
				}
			}
			//部屋をつなげる
			JointRoom (true, _jointRoomInfoList_first, _jointRoomInfoList_second, _horizontalBorderInfoList [i]);
			_jointRoomInfoList_first.Clear();
			_jointRoomInfoList_second.Clear ();
		}

		_jointRoomInfoList_first.Clear ();
		_jointRoomInfoList_second.Clear ();
		//次に垂直ボーダを連結
		for (int i = 0; i < _verticalBorderInfoList.Count; i++) {

			for (int j = 0; j < _roomInfoList.Count; j++) {
				Debug.Log ("borderSy=" + _verticalBorderInfoList [i].Sy () + "～" + _verticalBorderInfoList [i].Ey () + ":roomSy=" + _roomInfoList [j].Framesy () + "～" + _roomInfoList [j].Frameey ());

				//対象ボーダーのy座標において範囲内かどうか判定
				if (_verticalBorderInfoList [i].Sy() <= _roomInfoList [j].Framesy () && _verticalBorderInfoList [i].Ey () >= _roomInfoList [j].Frameey ()) {
					//境界線の右に属するかチェック
					if (_verticalBorderInfoList [i].Sx ()-1 <= _roomInfoList [j].Framesx () && _verticalBorderInfoList[i].Sx()+1 >= _roomInfoList[j].Framesx()) {
						_jointRoomInfoList_first.Add (_roomInfoList [j]);
					}
					//境界線の左に属するかチェック
					if (_verticalBorderInfoList [i].Sx ()-1 <= _roomInfoList [j].Frameex () && _verticalBorderInfoList[i].Sx()+1 >= _roomInfoList[j].Frameex()) {
						_jointRoomInfoList_second.Add (_roomInfoList [j]);
					}
				}
			}
			//部屋をつなげる
			JointRoom (false, _jointRoomInfoList_first, _jointRoomInfoList_second,_verticalBorderInfoList[i]);			
			_jointRoomInfoList_first.Clear();
			_jointRoomInfoList_second.Clear ();
		}

	}


	/// <summary>
	/// 壁を生成
	/// </summary>
	private void GenerateWall()
	{
		for (int i = 0; i < (stageLength * stageLength); i++) {
			_groundTypeList.Add (StageInformation.enum_GroundType.WALL);
		}
	}

	/// <summary>
	/// 部屋をつなげる
	/// </summary>
	/// <param name="isHorizon">境界線は水平か</param>
	/// <param name="_firstRoomList">境界線をまたいで他方の部屋リスト</param>
	/// <param name="_secondRoomList">境界線をまたいでもう一方の部屋リスト</param>
	/// <param name="_borderInfo">境界線情報</param>
	private void JointRoom(bool isHorizon, List<struct_rooomInfo> _firstRoomList,List<struct_rooomInfo> _secondRoomList,struct_borderInfo _borderInfo){

		List<struct_rooomInfo> jointRoomList_first = new List<struct_rooomInfo> ();
		List<struct_rooomInfo> jointRoomList_second = new List<struct_rooomInfo> ();

		int targetIndex;

		Debug.Log ("firstRoomCount=" + _firstRoomList.Count + ":secondRoomCount=" + _secondRoomList.Count);


		switch (_jointSetting) {
		case enum_JointSetting.ONE:
			//一つ目
			targetIndex = Random.Range (0, _firstRoomList.Count - 1);
			jointRoomList_first.Add (_firstRoomList [targetIndex]);
			//二つ目
			targetIndex = Random.Range(0,_secondRoomList.Count-1);
			jointRoomList_second.Add (_secondRoomList [targetIndex]);
			break;
		case enum_JointSetting.ALL:
			jointRoomList_first = _firstRoomList;
			jointRoomList_second = _secondRoomList;
			break;
		case enum_JointSetting.RANDOM:
			//一つ目
			int pickCounter = Random.Range (1, _firstRoomList.Count);
			for (int i = 0; i < pickCounter; i++) {
				targetIndex = Random.Range (0, _firstRoomList.Count - 1);
				jointRoomList_first.Add (_firstRoomList [targetIndex]);
				_firstRoomList.RemoveAt (targetIndex);
			}
			//二つ目
			pickCounter = Random.Range (1, _secondRoomList.Count);
			for (int i = 0; i < pickCounter; i++) {
				targetIndex = Random.Range (0, _secondRoomList.Count - 1);
				jointRoomList_second.Add (_secondRoomList [targetIndex]);
				_secondRoomList.RemoveAt (targetIndex);
			}
			break;
		default:
			Debug.Log ("未実装");
			break;
		}


		List<int> aisleStartPointList = new List<int> ();
		//TODO 現状、すべてをつないでいるが、後に別々につなげる方法も実装
		if (isHorizon) {
			//境界線より上の部屋をつなげる
			for (int i = 0; i < jointRoomList_first.Count; i++) {
				aisleStartPointList.Add(Random.Range (jointRoomList_first [i].Roomsx(), jointRoomList_first [i].Roomex()));
				//通路の生成
				for(int j= jointRoomList_first[i].Roomsy()-1;j>= _borderInfo.Sy();j--){
					_groundTypeList [aisleStartPointList [i] + (j * stageLength)] = StageInformation.enum_GroundType.AILSLE;
				}
			}
			//境界線より下の部屋をつなげる
			for (int i = 0; i < jointRoomList_second.Count; i++) {
				aisleStartPointList.Add (Random.Range (jointRoomList_second [i].Roomsx (), jointRoomList_second [i].Roomex ()));
				//通路の生成
				for (int j = jointRoomList_second [i].Roomey (); j <= _borderInfo.Sy (); j++) {
					_groundTypeList [aisleStartPointList [i + jointRoomList_first.Count] + (j * stageLength)] = StageInformation.enum_GroundType.AILSLE;
				}
			}
			//境界線上の通路をつなげる
			aisleStartPointList.Sort();
			for (int i = aisleStartPointList [0]; i < aisleStartPointList [aisleStartPointList.Count - 1]; i++) {
				_groundTypeList [i + (_borderInfo.Sy () * stageLength)] = StageInformation.enum_GroundType.AILSLE;
			}

		} else {
			//境界線より右の部屋をつなげる
			for (int i = 0; i < jointRoomList_first.Count; i++) {
				aisleStartPointList.Add(Random.Range (jointRoomList_first [i].Roomsy(), jointRoomList_first [i].Roomey()));
				//通路の生成
				for(int j= jointRoomList_first[i].Roomsx()-1;j>= _borderInfo.Sx();j--){
					_groundTypeList [j + (aisleStartPointList[i] * stageLength)] = StageInformation.enum_GroundType.AILSLE;
				}
			}
			//境界線より下の部屋をつなげる
			for (int i = 0; i < jointRoomList_second.Count; i++) {
				aisleStartPointList.Add (Random.Range (jointRoomList_second [i].Roomsy (), jointRoomList_second [i].Roomey ()));
				//通路の生成
				for (int j = jointRoomList_second [i].Roomex (); j <= _borderInfo.Sx (); j++) {
					_groundTypeList [j + (aisleStartPointList [i + jointRoomList_first.Count] * stageLength)] = StageInformation.enum_GroundType.AILSLE;
				}
			}
			//境界線上の通路をつなげる
			aisleStartPointList.Sort();
			for (int i = aisleStartPointList [0]; i < aisleStartPointList [aisleStartPointList.Count - 1]; i++) {
				_groundTypeList [_borderInfo.Sx() + (i * stageLength)] = StageInformation.enum_GroundType.AILSLE;
			}
		}
	}

	/// <summary>
	/// ステージの初期化設定
	/// </summary>
	private void InitStage()
	{
		_stage = new GameObject();
		_stage.name = "stage";
		_stage.AddComponent<StageInformation> ();
		_groundTypeList = new List<StageInformation.enum_GroundType> ();
		_roomInfoList = new List<struct_rooomInfo> ();
		_horizontalBorderInfoList = new List<struct_borderInfo> ();
		_verticalBorderInfoList = new List<struct_borderInfo> ();
	}

	/// <summary>
	/// ステージの長さを設定する
	/// </summary>
	private void SetStageLength()
	{
		stageLength = (int)_stageSize;
	}


}
