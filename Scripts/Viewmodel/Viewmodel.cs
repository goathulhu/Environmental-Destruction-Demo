using Godot;
using System;

public partial class Viewmodel : Node3D
{
	private float DeltaTime = 0f;
	
	GameManager GameManager;
	InputManager InputManager;
	Player Player;
	RandomNumberGenerator Rng;
	
	[Export] private ViewmodelArms ArmLeft;
	[Export] private ViewmodelArms ArmRight;
	
	[Export] private ViewmodelOverride BaseOverride;
	
	private ViewmodelOverride[] OverrideStack;
	private ViewmodelArms[] ArmsStack;
	
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
	float JumpHangRotTarget = 0f;
	float JumpHangRot = 0f;
	float JumpHangPosTarget = 0f;
	float JumpHangPos = 0f;
	
	public readonly string[] HandBones =
	{
		"hand",
		"palm.01",
		"palm.02",
		"palm.03",
		"palm.04",
		"f_index.01",
		"f_index.02",
		"f_index.03",
		"thumb.01",
		"thumb.02",
		"thumb.03",
		"f_middle.01",
		"f_middle.02",
		"f_middle.03",
		"f_ring.01",
		"f_ring.02",
		"f_ring.03",
		"f_pinky.01",
		"f_pinky.02",
		"f_pinky.03",
	};
	
	public override void _Ready()
	{
		GameManager = (GameManager)GetTree().Root.FindChild("GameManager", true, false);
		InputManager = GameManager.InputManager;
		Player = GameManager.Player;
		Rng = GameManager.Rng;
		
		Array.Resize(ref ArmsStack, 2);
		ArmsStack[0] = ArmLeft;
		ArmsStack[1] = ArmRight;
		
		Array.Resize(ref OverrideStack, 1);
		OverrideStack[0] = BaseOverride;
	}
	
	public override void _Process(double delta)
	{
		DeltaTime = GameManager.GlobalDeltaTime;
		
		PseudoItem();
		
		ViewmodelState CurrentState = OverrideStack[0].GetState();
		
		if (OverrideStack.Length > 1)
		{
			for (int I = 1; I < OverrideStack.Length; I++)
			{
				CurrentState = CurrentState.InterpolateWith(OverrideStack[I].GetState());
			}
		}
		
		foreach (ViewmodelArms Arms in ArmsStack)
		{
			if (Arms.IsLeft)
			{
				// bones
				for (int I = 0; I < HandBones.Length; I++)
				{
					int BoneIdLeft = Arms.Skeleton.FindBone(HandBones[I] + ".L");
					
					Arms.Skeleton.SetBonePosePosition(BoneIdLeft, CurrentState.Left.Bones[I].Position);
					Arms.Skeleton.SetBonePoseRotation(BoneIdLeft, CurrentState.Left.Bones[I].Rotation);
				}
				
				// ik
				Arms.IkTarget.Position = CurrentState.Left.Ik.Target;
				Arms.Ik.Magnet = CurrentState.Left.Ik.Pole;
				
				// orientation
				Arms.RigTransform.Position = CurrentState.Left.RigTransform.Position;
				Arms.RigTransform.Rotation = CurrentState.Left.RigTransform.Rotation;
				Arms.InnTransform.Position = CurrentState.Left.InnTransform.Position;
				Arms.InnTransform.Rotation = CurrentState.Left.InnTransform.Rotation;
				Arms.MidTransform.Position = CurrentState.Left.MidTransform.Position;
				Arms.MidTransform.Rotation = CurrentState.Left.MidTransform.Rotation;
				Arms.OutTransform.Position = CurrentState.Left.OutTransform.Position;
				Arms.OutTransform.Rotation = CurrentState.Left.OutTransform.Rotation;
			}
			else
			{
				// bones
				for (int I = 0; I < HandBones.Length; I++)
				{
					int BoneIdRight = Arms.Skeleton.FindBone(HandBones[I] + ".R");
					
					Arms.Skeleton.SetBonePosePosition(BoneIdRight, CurrentState.Right.Bones[I].Position);
					Arms.Skeleton.SetBonePoseRotation(BoneIdRight, CurrentState.Right.Bones[I].Rotation);
				}
				
				// ik
				Arms.IkTarget.Position = CurrentState.Right.Ik.Target;
				Arms.Ik.Magnet = CurrentState.Right.Ik.Pole;
				
				// orientation
				Arms.RigTransform.Position = CurrentState.Right.RigTransform.Position;
				Arms.RigTransform.Rotation = CurrentState.Right.RigTransform.Rotation;
				Arms.InnTransform.Position = CurrentState.Right.InnTransform.Position;
				Arms.InnTransform.Rotation = CurrentState.Right.InnTransform.Rotation;
				Arms.MidTransform.Position = CurrentState.Right.MidTransform.Position;
				Arms.MidTransform.Rotation = CurrentState.Right.MidTransform.Rotation;
				Arms.OutTransform.Position = CurrentState.Right.OutTransform.Position;
				Arms.OutTransform.Rotation = CurrentState.Right.OutTransform.Rotation;
			}
		}
	}
	
	public void Register(ViewmodelOverride Override)
	{
		foreach (ViewmodelOverride Ride in OverrideStack)
		{
			if (Ride.Id == Override.Id) return;
		}
		Array.Resize(ref OverrideStack, OverrideStack.Length + 1);
		OverrideStack[OverrideStack.Length - 1] = Override;
	}
	
	public void Unregister(ViewmodelOverride Override)
	{
		for (int I = 0; I < OverrideStack.Length; I++)
		{
			if (OverrideStack[I].Id == Override.Id)
			{
				for (int J = I; J < OverrideStack.Length; J++)
				{
					if (J + 1 >= OverrideStack.Length)
					{
						Array.Resize(ref OverrideStack, OverrideStack.Length - 1);
						return;
					}
					
					OverrideStack[J] = OverrideStack[Mathf.Clamp(J + 1, I, OverrideStack.Length - 1)];
				}
				
				Array.Resize(ref OverrideStack, OverrideStack.Length - 1);
				return;
			}
		}
	}
	
	public void Land()
	{
		//JumpHangRotTarget = Mathf.DegToRad(-2f);
		//JumpHangPosTarget = -0.05f;
	}	
	
	private void PseudoItem()
	{
		//if (InputManager.Down("sprint")) BaseOverride.Play("Walk", 0.2f);
		//else if (InputManager.Up("sprint")) BaseOverride.Play("Default", 0.2f);
	}
}
