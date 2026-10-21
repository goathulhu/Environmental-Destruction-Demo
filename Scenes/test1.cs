using Godot;
using System;

public partial class test1 : AnimationPlayer
{
	GameManager GameManager;
	
	public override void _Ready()
	{
		GameManager = (GameManager)GetTree().Root.FindChild("GameManager", true, false);
	}
	
	public override void _Process(double delta)
	{
		if (GameManager.InputManager.Down("rmb"))
		{
			//Stop();
			Play("IdleToAds", 0.2f);
		}
		else if (GameManager.InputManager.Up("rmb"))
		{
			//Stop();
			Play("Idle", 0.2f);
		}
	}
}
