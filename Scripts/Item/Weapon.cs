using Godot;
using System;

public abstract partial class Weapon : Item
{
	[Export] protected int ShotsPerShot = 1;
	[Export] protected float Firerate = 1f;
	[Export] protected float BurstFirerate = 1f;
	[Export] protected float ReloadDuration = 1f;
	[Export] protected int ChamberCount = 1;
	[Export] protected int AmmoMax = 1;
	[Export] protected int ProjectileCount = 1;
	protected float ReloadTimer = 0f;
	protected float ShotDelay = 0f;
	protected int AmmoCurrent = 0;
	protected int ShotQueue = 0;
	/// FLAGS
	[Export] protected bool HoldFire = false;
	protected bool IsShooting = false;
	protected bool CanShoot = true;
	protected bool IsReloading = false;
	protected bool IsShotsQueued = false;
	
	protected override void Update()
	{
		Animation();
		
		/// SHOOTING
		ShotDelay -= DeltaTime;
		IsShooting = ShotDelay > 0f;
		IsShotsQueued = ShotQueue > 0;
		
		if (CanShoot && ReloadTimer <= 0f && ShotQueue <= 0 && ShotDelay <= 0f && AmmoCurrent > 0)
		{
			if ((HoldFire && InputManager.Hold("primary")) || (!HoldFire && InputManager.Down("primary")))
			{
				ShotQueue = Mathf.Clamp(ShotsPerShot, 0, AmmoCurrent);
			}
		}
		
		if (ShotQueue > 0 && ShotDelay <= 0f)
		{
			for (int i = 0; i < (ProjectileCount * Mathf.Clamp(ChamberCount, 0, AmmoCurrent)); i++)
			{
				Shoot();
			}
			
			ShotQueue--;
			AmmoCurrent -= Mathf.Clamp(ChamberCount, 0, AmmoCurrent);
			/// UPDATE AMMO UI HERE IG
			
			/// RECOIL STUFF WOULD GO HERE
			
			if (ShotQueue <= 0) ShotDelay = 1f / Firerate;
			else ShotDelay = 1f / BurstFirerate;
		}
		
		/// RELOADING
		if (IsReloading) ReloadTimer -= DeltaTime;
		
		if (!IsReloading && !IsShotsQueued && InputManager.Down("reload") && AmmoCurrent < AmmoMax)
		{
			ReloadTimer = ReloadDuration;
			IsReloading = true;
		}
		
		if (IsReloading && ReloadTimer <= 0f)
		{
			AmmoCurrent = AmmoMax;
			/// UPDATE AMMO UI HERE IG
			IsReloading = false;
		}
	}
	
	protected virtual void Shoot()
	{
		/// SIMPLEMENTATION
		float PalletSpread = 10f;
		float PalletRange = 16f;
		
		Player.SpreadR.Rotation = new Vector3(0f, 0f, Mathf.DegToRad(Rng.RandfRange(0f, 360f)));
		Player.Director.Rotation = new Vector3(Mathf.DegToRad(Rng.RandfRange(0f, PalletSpread)), 0f, 0f);
		
		var Result = GameManager.Raycast(
			Player.CameraOrientation.GlobalPosition, 
			Player.CameraOrientation.GlobalPosition - Player.Director.GlobalBasis.Z * PalletRange);
		if (Result.DidHit) DestructionManager.QueueDestruction(Result.Position, "DestructionBlob");
		//Console.Log(Name, "...and then, johnathan said, the gun shot...");
		/*
		float PalletSpread = 10f;
		float PalletRange = 16f;
		int PalletCount = 1;
		
		(Vector3 At, string Type, float ScaleMult)[] Hits = new (Vector3 At, string Type, float ScaleMult)[PalletCount];
		int Landed = 0;
		
		for (int I = PalletCount; I > 0; I--)
		{
			Player.SpreadR.Rotation = new Vector3(0f, 0f, Mathf.DegToRad(Rng.RandfRange(0f, 360f)));
			Player.Director.Rotation = new Vector3(Mathf.DegToRad(Rng.RandfRange(0f, PalletSpread)), 0f, 0f);
			
			var Result = I == PalletCount ? GameManager.Raycast(Player.CameraOrientation.GlobalPosition, Player.CameraOrientation.GlobalPosition - Player.CameraOrientation.GlobalBasis.Z * PalletRange) : 
				GameManager.Raycast(Player.CameraOrientation.GlobalPosition, Player.CameraOrientation.GlobalPosition - Player.Director.GlobalBasis.Z * PalletRange);
			if (Result.DidHit)
			{
				//Hits[Landed] = (Result.Position, "DestructionBlob", (PalletRange - Result.Position.DistanceTo(CameraOrientation.GlobalPosition)) / PalletRange);
				Hits[Landed] = (Result.Position, "DestructionBlob", 1f);
				Landed += 1;
			}
		}
		
		if (Landed > 0)
		{
			Array.Resize(ref Hits, Landed);
			DestructionManager.QueueDestruction(Hits);
		}
		*/
	}
	
	protected virtual void Animation() { }
}
