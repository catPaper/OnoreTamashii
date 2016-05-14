using UnityEngine;
using System.Collections;

[RequireComponent(typeof(StageGenerator))]
public class StageManager : MonoBehaviour {

	private StageGenerator _stageGenerator;

	private GameObject testStage;

	void Awake()
	{
		_stageGenerator = GetComponent<StageGenerator> ();
	}

	/// <summary>
	/// ボタン等でステージ生成するのに使用
	/// </summary>
	public void TestGenerateStage()
	{
		if (testStage != null)
			Destroy (testStage);
		testStage = _stageGenerator.GenerateStage ();
	}
}
