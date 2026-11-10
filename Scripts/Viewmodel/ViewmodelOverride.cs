using Godot;
using System;

[Tool]
public partial class ViewmodelOverride : Node3D
{
	private float DeltaTime = 0f;
	
	GameManager GameManager;
	InputManager InputManager;
	Console Console;
	Player Player;
	RandomNumberGenerator Rng;
	Viewmodel Viewmodel;
	
	[Export] public string Id;
	[ExportToolButton("Configure")]
	public Callable ConfigureButton => Callable.From(Configure);
	[ExportCategory("")]
	[Export] public AnimationPlayer Animator;
	[Export] public Skeleton3D Skeleton;
	[Export] public Node3D RigTransform;
	[Export] public Node3D InnTransform;
	[Export] public Node3D MidTransform;
	[Export] public Node3D OutTransform;
	[Export] public Node3D Get;
	
	public ((Vector3 Rotation, Vector3 Position) Inn, 
			(Vector3 Rotation, Vector3 Position) Mid, 
			(Vector3 Rotation, Vector3 Position) Out)[] Layers = 
			{
				((Vector3.Zero, Vector3.Zero), (Vector3.Zero, Vector3.Zero), (Vector3.Zero, Vector3.Zero))
			};
	
	public override void _Ready()
	{
		if (Engine.IsEditorHint()) return;
		
		GameManager = (GameManager)GetTree().Root.FindChild("GameManager", true, false);
		InputManager = GameManager.InputManager;
		Console = GameManager.Console;
		Player = GameManager.Player;
		Rng = GameManager.Rng;
		Viewmodel = GameManager.Viewmodel;
	}
	
	public void LayerCount(int NewCount)
	{
		Array.Resize(ref Layers, Mathf.Clamp(NewCount, 1, 100));
		
		for(int I = 0; I < Layers.Length; I++)
		{
			Layers[I] = ((Vector3.Zero, Vector3.Zero), (Vector3.Zero, Vector3.Zero), (Vector3.Zero, Vector3.Zero));
		}
	}
	
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint()) return;
		
		DeltaTime = GameManager.GlobalDeltaTime;
		
		Vector3 InnRotation = Vector3.Zero;
		Vector3 InnPosition = Vector3.Zero;
		Vector3 MidRotation = Vector3.Zero;
		Vector3 MidPosition = Vector3.Zero;
		Vector3 OutRotation = Vector3.Zero;
		Vector3 OutPosition = Vector3.Zero;
		
		foreach (((Vector3 Rotation, Vector3 Position) Inn, 
			(Vector3 Rotation, Vector3 Position) Mid, 
			(Vector3 Rotation, Vector3 Position) Out) Layer in Layers)
		{
			InnRotation += Layer.Inn.Rotation;
			InnPosition += Layer.Inn.Position;
			MidRotation += Layer.Mid.Rotation;
			MidPosition += Layer.Mid.Position;
			OutRotation += Layer.Out.Rotation;
			OutPosition += Layer.Out.Position;
		}
		
		InnTransform.Rotation = InnRotation;
		InnTransform.Position = InnPosition;
		MidTransform.Rotation = MidRotation;
		MidTransform.Position = MidPosition;
		OutTransform.Rotation = OutRotation;
		OutTransform.Position = OutPosition;
	}
	
	public void Play(string Track)
	{
		Animator.Play(Track);
	}
	
	public void Play(string Track, float Lerp)
	{
		Animator.Play(Track, Lerp);
	}
	
	public (Vector3 Position, Vector3 Rotation) GetBoneLocals(string BoneName)
	{
		Transform3D GlobalBoneTransform = Skeleton.GetBoneGlobalPose(Skeleton.FindBone(BoneName));
		
		return (GlobalBoneTransform.Origin, GlobalBoneTransform.Basis.GetRotationQuaternion().GetEuler());
	}
	
	public (Vector3 Position, Vector3 Rotation) GetBoneGlobals(string BoneName)
	{
		Transform3D GlobalBoneTransform = Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(Skeleton.FindBone(BoneName));
		
		return (GlobalBoneTransform.Origin, GlobalBoneTransform.Basis.GetRotationQuaternion().GetEuler());
	}
	
	public ViewmodelState GetState()
	{
		ViewmodelState State = new ViewmodelState();
		
		// bones
		Array.Resize(ref State.Left.Bones, Viewmodel.HandBones.Length);
		Array.Resize(ref State.Right.Bones, Viewmodel.HandBones.Length);
		
		for (int I = 0; I < Viewmodel.HandBones.Length; I++)
		{
			int BoneIdLeft = Skeleton.FindBone(Viewmodel.HandBones[I] + ".L");
			int BoneIdRight = Skeleton.FindBone(Viewmodel.HandBones[I] + ".R");
			
			State.Left.Bones[I] = (Skeleton.GetBonePosePosition(BoneIdLeft), Skeleton.GetBonePoseRotation(BoneIdLeft));
			State.Right.Bones[I] = (Skeleton.GetBonePosePosition(BoneIdRight), Skeleton.GetBonePoseRotation(BoneIdRight));
		}
		
		// ik
		State.Left.Ik = (Skeleton.GetBonePosePosition(Skeleton.FindBone("ik_pole.L")), Skeleton.GetBonePosePosition(Skeleton.FindBone("ik_target.L")));
		State.Right.Ik = (Skeleton.GetBonePosePosition(Skeleton.FindBone("ik_pole.R")), Skeleton.GetBonePosePosition(Skeleton.FindBone("ik_target.R")));
		
		// orientation
		State.Left.RigTransform = (RigTransform.Position, RigTransform.Rotation);
		State.Left.InnTransform = (InnTransform.Position, InnTransform.Rotation);
		State.Left.MidTransform = (MidTransform.Position, MidTransform.Rotation);
		State.Left.OutTransform = (OutTransform.Position, OutTransform.Rotation);
		
		State.Right.RigTransform = (RigTransform.Position, RigTransform.Rotation);
		State.Right.InnTransform = (InnTransform.Position, InnTransform.Rotation);
		State.Right.MidTransform = (MidTransform.Position, MidTransform.Rotation);
		State.Right.OutTransform = (OutTransform.Position, OutTransform.Rotation);
		
		// control
		State.Left.Control = Mathf.Clamp(Skeleton.GetBonePosePosition(Skeleton.FindBone("control.L")).Y, 0f, 1f);
		State.Right.Control = Mathf.Clamp(Skeleton.GetBonePosePosition(Skeleton.FindBone("control.R")).Y, 0f, 1f);
		
		return State;
	}
	
	private void Configure()
	{
		if (!Engine.IsEditorHint()) return;
		
		var SceneRoot = EditorInterface.Singleton.GetEditedSceneRoot();
		AnimationPlayer NewAnimator = (AnimationPlayer)this.FindChild("AnimationPlayer", true, false);
		Skeleton3D NewSkeleton = (Skeleton3D)this.FindChild("Skeleton3D", true, false);
		Node3D NewRigTransform = (Node3D)this.FindChild("Rig", true, false);
		
		Node3D NewOutTransform = new Node3D();
		Node3D NewMidTransform = new Node3D();
		Node3D NewInnTransform = new Node3D();
		Node3D NewGet = new Node3D();
		
		if (Id.StripEdges() != "") this.Name = Id;
		NewOutTransform.Name = "OutTransform";
		NewMidTransform.Name = "MidTransform";
		NewInnTransform.Name = "InnTransform";
		NewGet.Name = "Get";
		//NewAnimator.Name = "Animator";
		//NewSkeleton.Name = "Skeleton";
		//NewRigTransform.Name = "RigTransform";
		
		this.AddChild(NewOutTransform);
		NewOutTransform.AddChild(NewMidTransform);
		NewMidTransform.AddChild(NewInnTransform);
		NewRigTransform.AddChild(NewGet);
		
		NewRigTransform.Reparent(NewInnTransform, true);
		NewAnimator.Reparent(NewInnTransform, true);
		
		this.GetChild(0).QueueFree(); 
		
		NewOutTransform.Owner = SceneRoot;
		NewMidTransform.Owner = SceneRoot;
		NewInnTransform.Owner = SceneRoot;
		NewGet.Owner = SceneRoot;
		
		this.Animator = NewAnimator;
		this.Skeleton = NewSkeleton;
		this.RigTransform = NewRigTransform;
		this.OutTransform = NewOutTransform;
		this.MidTransform = NewMidTransform;
		this.InnTransform = NewInnTransform;
		this.Get = NewGet;
		
		EditorInterface.Singleton.MarkSceneAsUnsaved();
	}
}
