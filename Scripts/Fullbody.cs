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
	private float DiaRot = 0f;
	
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
		
		Vector2 InputVector = InputManager.GetMovementInputVector();
		DiaRot = Mathf.Lerp(DiaRot, InputVector.X * -InputVector.Y * Mathf.DegToRad(45f), DeltaTime * 4f);
		
		LRHang += Mathf.DegToRad(Player.DeltaCameraRot.Y);
		LRHang = Mathf.Clamp(Mathf.Lerp(LRHang, 0f, DeltaTime * LRHangLerp), -LRHangLimit, LRHangLimit);
		
		if (Player.WalkTimer > 0f)
		{
			float WalkSpeed = InputVector.Y != 0f ? 10f : 12.5f;
			//float WalkTimer = Player.WalkTimer;
			float WalkTimer = (InputVector.Y != 0f ? (InputManager.Hold("backward") ? -Player.WalkTimer : Player.WalkTimer) : (InputManager.Hold("right") ? -Player.WalkTimer : Player.WalkTimer)) + 0.333f;
			
			
			float FootLerp = DeltaTime * 64f;
			
			float UpDownOffset = -0.45f;
			float LeftRightOffset = 0.333f;
			
			if (InputManager.Hold("sprint"))
			{
				LRHangLerp = 5f;
				LRHangLimit = Mathf.Lerp(LRHangLimit, 2f, DeltaTime * 8f);
				
				FootLeft.Position = FootLeft.Position.Lerp(new Vector3(
					-0.15f, 
					Mathf.Clamp((Mathf.Sin((WalkTimer) * WalkSpeed)) * 0.3f, 0f, 99f), 
					(Mathf.Sin((WalkTimer + UpDownOffset) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer + UpDownOffset) * WalkSpeed))) * 0.6f + 0.4f
				), FootLerp);
				
				FootRight.Position = FootRight.Position.Lerp(new Vector3(
					0.15f, 
					Mathf.Clamp((Mathf.Sin((WalkTimer + LeftRightOffset) * WalkSpeed)) * 0.3f, 0f, 99f), 
					(Mathf.Sin((WalkTimer + UpDownOffset + LeftRightOffset) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer + UpDownOffset + LeftRightOffset) * WalkSpeed))) * 0.6f + 0.4f
				), FootLerp);
				
				FootPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f) + Mathf.Sin((WalkTimer + 0.2f) * WalkSpeed) * Mathf.DegToRad(7.5f) + DiaRot, 0f);
				HipPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f) + Mathf.Sin((WalkTimer + 0.2f) * WalkSpeed) * Mathf.DegToRad(15f) + DiaRot, 0f);
				//FootPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f), 0f);
				//HipPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f), 0f);
			}
			else
			{
				LRHangLerp = 5f;
				LRHangLimit = Mathf.Lerp(LRHangLimit, 2f, DeltaTime * 8f);
				
				if (InputVector.Y != 0f)
				{
					FootLeft.Position = FootLeft.Position.Lerp(new Vector3(
						-0.125f, 
						Mathf.Clamp((Mathf.Sin((WalkTimer) * WalkSpeed)) * 0.15f, 0f, 99f), 
						(Mathf.Sin((WalkTimer + UpDownOffset) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer + UpDownOffset) * WalkSpeed))) * 0.35f + 0.35f
					), FootLerp);
					
					FootRight.Position = FootRight.Position.Lerp(new Vector3(
						0.125f, 
						Mathf.Clamp((Mathf.Sin((WalkTimer + LeftRightOffset) * WalkSpeed)) * 0.15f, 0f, 99f), 
						(Mathf.Sin((WalkTimer + UpDownOffset + LeftRightOffset) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer + UpDownOffset + LeftRightOffset) * WalkSpeed))) * 0.35f + 0.35f
					), FootLerp);
					
					FootPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f) + Mathf.Sin((WalkTimer + 0.2f) * WalkSpeed) * Mathf.DegToRad(5f) + DiaRot, 0f);
					HipPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f) + Mathf.Sin((WalkTimer + 0.2f) * WalkSpeed) * Mathf.DegToRad(10f) + DiaRot, 0f);
				}
				else
				{
					FootLeft.Position = FootLeft.Position.Lerp(new Vector3(
						(Mathf.Sin((WalkTimer + UpDownOffset) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer + UpDownOffset) * WalkSpeed))) * 0.25f - 0.2f, 
						Mathf.Clamp((Mathf.Sin((WalkTimer) * WalkSpeed)) * 0.125f, 0f, 99f), 
						InputVector.X > 0f ? 0.25f : 0.15f
					), FootLerp);
					
					FootRight.Position = FootRight.Position.Lerp(new Vector3(
						(Mathf.Sin((WalkTimer + UpDownOffset + LeftRightOffset) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer + UpDownOffset + LeftRightOffset) * WalkSpeed))) * 0.25f + 0.2f, 
						Mathf.Clamp((Mathf.Sin((WalkTimer + LeftRightOffset) * WalkSpeed)) * 0.125f, 0f, 99f), 
						InputVector.X > 0f ? 0.15f : 0.25f
					), FootLerp);
					
					FootPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f) + DiaRot, 0f);
					HipPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f) + DiaRot, 0f);
				}
			}
		}
		else
		{
			LRHangLerp = 0f;
			LRHangLimit = Mathf.Lerp(LRHangLimit, 1.5f, DeltaTime * 8f);
			
			if (InputManager.Hold("crouch"))
			{
				FootLeft.Position = FootLeft.Position.Lerp(new Vector3(-0.3f, 0f, -0.15f), DeltaTime * 10f);
				FootRight.Position = FootRight.Position.Lerp(new Vector3(0.3f, 0f, 0.15f), DeltaTime * 10f);
			}
			else
			{
				FootLeft.Position = FootLeft.Position.Lerp(new Vector3(-0.25f, 0f, -0.1f), DeltaTime * 10f);
				FootRight.Position = FootRight.Position.Lerp(new Vector3(0.25f, 0f, 0.1f), DeltaTime * 10f);
			}
			
			FootPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f), 0f);
			HipPivot.Rotation = new Vector3(0f, Mathf.DegToRad(LRHang * 45f), 0f);
		}
		
		
	}
}
