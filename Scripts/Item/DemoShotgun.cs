using Godot;
using System;

public partial class DemoShotgun : Weapon
{
	[Export] private ViewmodelOverride Override;
	
	float LRMouseHang = 0f;
	float UDMouseHang = 0f;
	float LRMoveHang = 0f;
	float FBMoveHang = 0f;
	float WalkAngle = 0f;
	(Vector3 Position, Vector3 Rotation) ViewmodelPivot = (Vector3.Zero, Vector3.Zero);
	float CrouchLerp = 0f;
	float BreatheTimer = 0f;
	float JitterTimer = 0f;
	Vector3 JitterRotTarget = Vector3.Zero;
	Vector3 JitterRot = Vector3.Zero;
	Vector3 JitterPosTarget = Vector3.Zero;
	Vector3 JitterPos = Vector3.Zero;
	Vector3 JumpHangRot = Vector3.Zero;
	Vector3 JumpHangPos = Vector3.Zero;
	
	bool Sprinting = false;
	
	protected override void Setup()
	{
		Override.LayerCount(8);
		
		Viewmodel.Register(Override);
		
		Override.Play("Idle");
	}
	
	protected override void Shoot()
	{
		float PalletSpread = 0f;//10f;
		float PalletRange = 32f;
		
		Player.SpreadR.Rotation = new Vector3(0f, 0f, Mathf.DegToRad(Rng.RandfRange(0f, 360f)));
		Player.Director.Rotation = new Vector3(Mathf.DegToRad(Rng.RandfRange(0f, PalletSpread)), 0f, 0f);
		
		var Result = GameManager.Raycast(
			Player.CameraOrientation.GlobalPosition, 
			Player.CameraOrientation.GlobalPosition - Player.Director.GlobalBasis.Z * PalletRange);
		if (Result.DidHit)
			DestructionManager.QueueDestruction(Result.Position, "DestructionBlob");
		
		Override.ForcePlay("Shot");
		
		Player.RecoilImpulse(new Vector2(RecoilX, RecoilY));
		//Viewmodel.Recoil(new Vector3(RNG.RandfRange(-0.015f, 0.015f), 0f, CurrentWeapon.RecoilImpulse.Length() * 0.025f), new Vector3(CurrentWeapon.RecoilImpulse.Y * 2.5f, CurrentWeapon.RecoilImpulse.X * -2.5f, CurrentWeapon.RecoilImpulse.X * -2.5f));
		//Viewmodel.Recoil(new Vector3(0f, 0f, 0.125f), new Vector3(10f, -5f, -5f));
	}
	
	protected override void Animation()
	{
		/*
		if (GameManager.InputManager.Hold("sprint") && Player.WalkTimer > 0f && !Sprinting)
		{
			Override.Play("Sprint", 0.2f);
			Sprinting = true;
		}
		else if ((!GameManager.InputManager.Hold("sprint") || Player.WalkTimer <= 0f) && Sprinting)
		{
			Override.Play("Idle", 0.2f);
			Sprinting = false;
		}
		
		if (GameManager.InputManager.Hold("primary") && !Sprinting)
		{
			Override.ForcePlay("Shot");
		}
		*/
		
		/*if (GameManager.InputManager.Down("rmb"))
		{
			Override.Play("Ads", 0.2f);
		}
		else if (GameManager.InputManager.Up("rmb"))
		{
			Override.Play("Idle", 0.2f);
		}*/
		
		LRMouseHang += Mathf.DegToRad(Player.DeltaCameraRot.Y * 0.5f);
		LRMouseHang = Mathf.Clamp(Mathf.Lerp(LRMouseHang, 0f, DeltaTime * 12f), -0.5f, 0.5f);
		
		UDMouseHang += Mathf.DegToRad(Player.DeltaCameraRot.X * 0.5f);
		UDMouseHang = Mathf.Clamp(Mathf.Lerp(UDMouseHang, 0f, DeltaTime * 12f), -0.5f, 0.5f);
		
		CrouchLerp = Mathf.Lerp(CrouchLerp, (Player.IsCrouching ? 1f : 0f), DeltaTime * 12f);
		
		LRMoveHang = Mathf.Lerp(LRMoveHang, Player.InputDirection.X, DeltaTime * (Player.WalkTimer > 0f ? 8f : 1f));
		FBMoveHang = Mathf.Lerp(FBMoveHang, Player.InputDirection.Y, DeltaTime * (Player.WalkTimer > 0f ? 8f : 1f));
		
		float YVelocity = Mathf.Clamp(Player.Velocity.Y - (Player.IsGrounded ? Player.Force.GroundGravity : 0f), -6.66f, 10f);
		JumpHangRot = new Vector3(
			Mathf.Lerp(JumpHangRot.X, Mathf.DegToRad(YVelocity * -0.75f), DeltaTime * 8f), 
			0f, 
			0f
		);
		JumpHangPos = new Vector3(
			0f, 
			Mathf.Lerp(JumpHangPos.Y, YVelocity * -0.005f, DeltaTime * 16f), 
			Mathf.Lerp(JumpHangPos.Z, Mathf.Abs(YVelocity) * 0.002f, DeltaTime * 12f)
		);
		
		// breathe
		BreatheTimer += DeltaTime * 1.5f;
		JitterTimer -= DeltaTime;
		float JitterStrengthRot = 1.333f;
		float JitterStrengthPos = 0.666f;
		if (JitterTimer <= 0f)
		{
			JitterRotTarget = new Vector3(
				Mathf.DegToRad(Rng.RandfRange(-JitterStrengthRot, JitterStrengthRot)),  
				Mathf.DegToRad(Rng.RandfRange(-JitterStrengthRot, JitterStrengthRot)), 
				Mathf.DegToRad(Rng.RandfRange(-JitterStrengthRot, JitterStrengthRot)));
			JitterPosTarget = new Vector3(
				Mathf.DegToRad(Rng.RandfRange(-JitterStrengthPos, JitterStrengthPos)), 
				Mathf.DegToRad(Rng.RandfRange(-JitterStrengthPos, JitterStrengthPos)), 
				Mathf.DegToRad(Rng.RandfRange(-JitterStrengthPos, JitterStrengthPos)));
			JitterTimer = 0.125f;
		}
		
		JitterRot = JitterRot.Lerp(JitterRotTarget, DeltaTime * 8f);
		JitterPos = JitterPos.Lerp(JitterPosTarget, DeltaTime * 8f);
		
		// pivot
		Override.Get.Transform = Override.Skeleton.GetBoneGlobalPose(Override.Skeleton.FindBone("viewmodel_pivot"));
		ViewmodelPivot = (Override.Get.Position, Override.Get.Rotation);
		Override.RigTransform.Position = -ViewmodelPivot.Position;
		Override.Layers[0].Inn.Position = ViewmodelPivot.Position;
		
		// jump/fall
		Override.Layers[5].Inn.Rotation = Override.Layers[5].Inn.Rotation.Lerp(JumpHangRot, DeltaTime * 32f);
		
		Override.Layers[5].Mid.Position = Override.Layers[5].Mid.Position.Lerp(JumpHangPos, DeltaTime * 32f);
		
		// up/down perspective
		Override.Layers[1].Out.Position = Override.Layers[1].Out.Position.Lerp(new Vector3(
			0f, 
			Sprinting ? 0f : Mathf.Abs(Mathf.DegToRad(Player.CurrentCameraRot.X)) * (Player.CurrentCameraRot.X > 0 ? -0.03f : -0.025f), 
			Sprinting ? 0f : Mathf.Abs(Mathf.DegToRad(Player.CurrentCameraRot.X)) * (Player.CurrentCameraRot.X > 0 ? 0.125f : 0.125f)
		), DeltaTime * 4f);
			
		Override.Layers[1].Out.Rotation = Override.Layers[1].Out.Rotation.Lerp(new Vector3(
			0f, 
			0f,
			Sprinting ? 0f : Mathf.DegToRad(Player.CurrentCameraRot.X) * (Player.CurrentCameraRot.X > 0 ? 0f : -0.25f)
		), DeltaTime * 4f);
		
		// breathing
		Override.Layers[1].Inn.Position = Override.Layers[1].Inn.Position.Lerp(new Vector3(
			JitterPos.X, 
			JitterPos.Y, 
			JitterPos.Z
		), DeltaTime * 4f);
		
		Override.Layers[1].Inn.Rotation = Override.Layers[1].Inn.Rotation.Lerp(new Vector3(
			JitterRot.X, 
			JitterRot.Y, 
			JitterRot.Z
		), DeltaTime * 4f);
		
		Override.Layers[1].Mid.Position = Override.Layers[1].Mid.Position.Lerp(new Vector3(
			Sprinting ? 0f : LRMoveHang * -0.025f, 
			Mathf.Sin(BreatheTimer) * 0.005f, 
			Sprinting ? 0f : FBMoveHang * 0.03f
		), DeltaTime * 4f);
		
		// sway/hang
		Override.Layers[3].Inn.Rotation = Override.Layers[3].Inn.Rotation.Lerp(new Vector3(
			UDMouseHang * -0.333f, 
			LRMouseHang * -0.25f, 
			LRMouseHang * 0.333f
		), DeltaTime * 8f);
		
		Override.Layers[1].Mid.Rotation = Override.Layers[1].Mid.Rotation.Lerp(new Vector3(
			UDMouseHang * 0.5f, 
			LRMouseHang * 0.5f, 
			0f
		), DeltaTime * 16f);
		
		// locomotion
		if (Player.WalkTimer > 0f)
		{
			float WalkSpeed = 10f;
			float WalkTimer = Player.WalkTimer;
			
			if (InputManager.Hold("sprint"))
			{
				
				Override.Layers[2].Inn.Rotation = new Vector3(
					Mathf.Sin((WalkTimer - 0.333f) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer - 0.333f) * WalkSpeed)) * Mathf.DegToRad(10f), 
					0f, 
					Mathf.Sin((WalkTimer + 0.666f) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer + 0.666f) * WalkSpeed)) * Mathf.DegToRad(10f)
				);
				
				Override.Layers[2].Mid.Rotation = new Vector3(
					Mathf.Abs(Mathf.Sin((WalkTimer - 0.25f) * (WalkSpeed * 2f))) * Mathf.DegToRad(Mathf.Sin((WalkTimer - 0.25f) * (WalkSpeed * 2f)) > 0f ? 0f : 7.5f), 
					(Mathf.Sin((WalkTimer + 0.25f) * WalkSpeed) - 1f) * Mathf.DegToRad(15f), 
					0f
				);
			}
			else
			{
				Override.Layers[2].Inn.Rotation = Override.Layers[2].Inn.Rotation.Lerp(new Vector3(
					Mathf.Sin((WalkTimer + 0.25f) * (WalkSpeed * 2f)) * Mathf.Abs(Mathf.Sin((WalkTimer + 0.25f) * (WalkSpeed * 2f))) * Mathf.DegToRad(1f), 
					Mathf.Sin((WalkTimer + 0.333f) * (WalkSpeed * 2f)) * Mathf.Abs(Mathf.Sin((WalkTimer + 0.333f) * (WalkSpeed * 2f))) * Mathf.DegToRad(0.5f), 
					Mathf.Sin((WalkTimer + 0.5f) * WalkSpeed) * Mathf.Abs(Mathf.Sin((WalkTimer + 0.5f) * WalkSpeed)) * Mathf.DegToRad(7.5f) + LRMoveHang * Mathf.DegToRad(-7.5f)
				), DeltaTime * 16f);
				
				Override.Layers[2].Mid.Rotation = Override.Layers[2].Mid.Rotation.Lerp(new Vector3(
					Mathf.Sin((WalkTimer + 0f) * (WalkSpeed * 2f)) * Mathf.Abs(Mathf.Sin((WalkTimer + 0f) * (WalkSpeed * 2f)))  * Mathf.DegToRad(1.25f), 
					Mathf.Sin((WalkTimer + 0.666f) * WalkSpeed) * Mathf.DegToRad(2f), 
					0f
				), DeltaTime * 16f);
			}
		}
		else
		{
			Override.Layers[2].Inn.Rotation = Override.Layers[2].Inn.Rotation.Lerp(new Vector3(
				0f, 
				0f, 
				0f
			), DeltaTime * 8f);
			
			Override.Layers[2].Mid.Rotation = Override.Layers[2].Mid.Rotation.Lerp(new Vector3(
				0f, 
				0f, 
				0f
			), DeltaTime * 8f);
		}
	}
}
