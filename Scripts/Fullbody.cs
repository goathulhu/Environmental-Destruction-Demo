using Godot;
using System;

public partial class Fullbody : Node3D
{
	private float DeltaTime;
	
	GameManager GameManager;
	InputManager InputManager;
	Player Player;
	Console Console;
	RandomNumberGenerator Rng;
	
	[Export] Node3D HipPivot;
	[Export] Node3D FootPivot;
	[Export] Node3D FootLeft;
	[Export] Node3D FootRight;
	
	private float LRHang = 0f;
	private float LRHangLimit = 1.5f;
	private float LRHangLerp = 0f;
	
	public override void _Ready()
	{
		GameManager = (GameManager)GetTree().Root.FindChild("GameManager", true, false);
		InputManager = GameManager.InputManager;
		Player = GameManager.Player;
		Console = GameManager.Console;
		Rng = GameManager.Rng;
	}
	
	public override void _Process(double delta)
	{
		DeltaTime = GameManager.GlobalDeltaTime;
		
		FootPivot.GlobalPosition = new(Player.PlayerView.GlobalPosition.X, Player.Controller.GlobalPosition.Y, Player.PlayerView.GlobalPosition.Z);
		
		LRHang += Mathf.DegToRad(Player.DeltaCameraRot.Y);
		LRHang = Mathf.Clamp(Mathf.Lerp(LRHang, 0f, DeltaTime * LRHangLerp), -LRHangLimit, LRHangLimit);
		
		if (Player.WalkTimer > 0f)
		{
			float WalkSpeed = 10f;
			float WalkTimer = Player.WalkTimer;
			
			if (InputManager.Hold("sprint"))
			{
				LRHangLerp = 6f;
				LRHangLimit = Mathf.Lerp(LRHangLimit, 2f, DeltaTime * 8f);
				
				FootLeft.Position = new Vector3(
					-0.15f, 
					Mathf.Clamp((Mathf.Sin((WalkTimer) * WalkSpeed)) * 0.25f, 0f, 99f), 
					(Mathf.Sin((WalkTimer - 0.45f) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer - 0.45f) * WalkSpeed))) * 0.6f + 0.2f
				);
				
				FootRight.Position = new Vector3(
					0.15f, 
					Mathf.Clamp((Mathf.Sin((WalkTimer + 0.333f) * WalkSpeed)) * 0.25f, 0f, 99f), 
					(Mathf.Sin((WalkTimer - 0.45f + 0.333f) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer - 0.45f + 0.333f) * WalkSpeed))) * 0.6f + 0.2f
				);
				
				FootPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f) + Mathf.Sin((WalkTimer + 0.2f) * WalkSpeed) * Mathf.DegToRad(10f), 0f);
				HipPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f) + Mathf.Sin((WalkTimer + 0.2f) * WalkSpeed) * Mathf.DegToRad(30f), 0f);
			}
			else
			{
				LRHangLerp = 6f;
				LRHangLimit = Mathf.Lerp(LRHangLimit, 2f, DeltaTime * 8f);
				
				FootLeft.Position = new Vector3(
					-0.15f, 
					Mathf.Clamp((Mathf.Sin((WalkTimer) * WalkSpeed)) * 0.25f, 0f, 99f), 
					(Mathf.Sin((WalkTimer - 0.45f) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer - 0.45f) * WalkSpeed))) * 0.5f + 0.25f
				);
				
				FootRight.Position = new Vector3(
					0.15f, 
					Mathf.Clamp((Mathf.Sin((WalkTimer + 0.333f) * WalkSpeed)) * 0.25f, 0f, 99f), 
					(Mathf.Sin((WalkTimer - 0.45f + 0.333f) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer - 0.45f + 0.333f) * WalkSpeed))) * 0.5f + 0.25f
				);
				
				FootPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f), 0f);
				HipPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f), 0f);
			}
		}
		else
		{
			LRHangLerp = 0f;
			LRHangLimit = Mathf.Lerp(LRHangLimit, 1.5f, DeltaTime * 8f);
			
			if (InputManager.Hold("crouch"))
			{
				FootLeft.Position = FootLeft.Position.Lerp(new Vector3(-0.3f, 0f, -0.15f), DeltaTime * 8f);
				FootRight.Position = FootRight.Position.Lerp(new Vector3(0.3f, 0f, 0.15f), DeltaTime * 8f);
			}
			else
			{
				FootLeft.Position = FootLeft.Position.Lerp(new Vector3(-0.25f, 0f, -0.1f), DeltaTime * 8f);
				FootRight.Position = FootRight.Position.Lerp(new Vector3(0.25f, 0f, 0.1f), DeltaTime * 8f);
			}
			
			FootPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f), 0f);
			HipPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f), 0f);
		}
		
		
	}
}
