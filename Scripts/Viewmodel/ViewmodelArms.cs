using Godot;
using System;

[Tool]
public partial class ViewmodelArms : Node3D
{
	[Export] public string Id;
	[Export] public bool IsLeft = true;
	[ExportToolButton("Configure")]
	public Callable ConfigureButton => Callable.From(Configure);
	[ExportCategory(" ")]
	[Export] public Skeleton3D Skeleton;
	[Export] public Node3D RigTransform;
	[Export] public Node3D InnTransform;
	[Export] public Node3D MidTransform;
	[Export] public Node3D OutTransform;
	[ExportCategory(" ")]
	[Export] public SkeletonIK3D Ik;
	[Export] public Node3D IkTarget;
	
	public override void _Ready()
	{
		if (Engine.IsEditorHint()) return;
		
		Ik.Start();
	}
	
	private void Configure()
	{
		if (!Engine.IsEditorHint()) return;
		
		var SceneRoot = EditorInterface.Singleton.GetEditedSceneRoot();
		Skeleton3D NewSkeleton = (Skeleton3D)this.FindChild("Skeleton3D", true, false);
		Node3D NewRigTransform = (Node3D)this.FindChild("Rig", true, false);
		
		Node3D NewOutTransform = new Node3D();
		Node3D NewMidTransform = new Node3D();
		Node3D NewInnTransform = new Node3D();
		SkeletonIK3D NewIk = new SkeletonIK3D();
		Node3D NewIkTarget = new Node3D();
		
		if (Id.StripEdges() != "") this.Name = Id;
		NewOutTransform.Name = "OutTransform";
		NewMidTransform.Name = "MidTransform";
		NewInnTransform.Name = "InnTransform";
		NewIk.Name = "Ik";
		NewIkTarget.Name = "IkTarget";
		//NewSkeleton.Name = "Skeleton";
		//NewRigTransform.Name = "RigTransform";
		
		this.AddChild(NewOutTransform);
		NewOutTransform.AddChild(NewMidTransform);
		NewMidTransform.AddChild(NewInnTransform);
		NewSkeleton.AddChild(NewIk);
		this.AddChild(NewIkTarget);
		
		NewRigTransform.Reparent(NewInnTransform, true);
		
		this.GetChild(0).QueueFree(); 
		
		NewOutTransform.Owner = SceneRoot;
		NewMidTransform.Owner = SceneRoot;
		NewInnTransform.Owner = SceneRoot;
		NewIk.Owner = SceneRoot;
		NewIkTarget.Owner = SceneRoot;
		
		if (IsLeft)
		{
			NewSkeleton.SetBonePoseScale(NewSkeleton.FindBone("hand.R"), Vector3.Zero);
			NewIk.RootBone = "forearm.L";
			NewIk.TipBone = "upper_arm.L_leaf";
		}
		else
		{
			NewSkeleton.SetBonePoseScale(NewSkeleton.FindBone("hand.L"), Vector3.Zero);
			NewIk.RootBone = "forearm.R";
			NewIk.TipBone = "upper_arm.R_leaf";
		}
		
		NewIk.UseMagnet = true;
		NewIk.TargetNode = new NodePath("../../../../../../IkTarget");
		
		this.Skeleton = NewSkeleton;
		this.RigTransform = NewRigTransform;
		this.OutTransform = NewOutTransform;
		this.MidTransform = NewMidTransform;
		this.InnTransform = NewInnTransform;
		this.Ik = NewIk;
		this.IkTarget = NewIkTarget;
		
		EditorInterface.Singleton.MarkSceneAsUnsaved();
	}
}
