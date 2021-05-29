using System;
using UnityEngine;

public interface IState
{
	//パックマンに接触したときの処理を登録する。
	event EventHandler OnTouch;
	//毎フレームの実行する処理。
	IState Excute();
	//恐慌状態になった軒の処理。
	IState Frighten(float span);
	//恐慌状態が終わったときの処理。
	IState Calm();
	//パックマンに接触したときにOnTouchイベントを実行。
	IState Eaten();
	//ワープしたときの処理。
	void Warp(Vector2 pos, float span);
	//アニメーションの処理。
	void Animate();
}
