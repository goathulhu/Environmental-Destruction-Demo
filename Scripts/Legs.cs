using Godot;
using System;

[Tool]
public partial class Legs : Node3D
{
	[Export] public string Id;
	[ExportToolButton("Configure")]
	public Callable ConfigureButton => Callable.From(Configure);
	[ExportCategory(" ")]
	[Export] public Skeleton3D Skeleton;
	[ExportCategory(" ")]
	[Export] public SkeletonIK3D LeftIk;
	[Export] public SkeletonIK3D RightIk;
	
	public override void _Ready()
	{
		if (Engine.IsEditorHint()) return;
		
		LeftIk.TargetNode = new NodePath("../../../../FootPivot/FootLeft");
		LeftIk.UseMagnet = true;
		LeftIk.Start();
		
		RightIk.TargetNode = new NodePath("../../../../FootPivot/FootRight");
		RightIk.UseMagnet = true;
		RightIk.Start();
		
		this.Position = new Vector3(0f, -1f, 0f);
	}
	
	private void Configure()
	{
		if (!Engine.IsEditorHint()) return;
		
		var SceneRoot = EditorInterface.Singleton.GetEditedSceneRoot();
		Skeleton3D NewSkeleton = (Skeleton3D)this.FindChild("Skeleton3D", true, false);
		Node3D NewRigTransform = (Node3D)this.FindChild("Rig", true, false);
		
		SkeletonIK3D NewLeftIk = new SkeletonIK3D();
		SkeletonIK3D NewRightIk = new SkeletonIK3D();
		
		if (Id.StripEdges() != "") this.Name = Id;
		NewLeftIk.Name = "LeftIk";
		NewRightIk.Name = "RightIk";
		
		NewSkeleton.AddChild(NewLeftIk);
		NewSkeleton.AddChild(NewRightIk);
		
		NewSkeleton.Reparent(this, true);
		
		this.GetChild(0).QueueFree(); 
		
		NewLeftIk.Owner = SceneRoot;
		NewRightIk.Owner = SceneRoot;
		
		NewLeftIk.RootBone = "upper_leg.L";
		NewLeftIk.TipBone = "foot.L";
		NewRightIk.RootBone = "upper_leg.R";
		NewRightIk.TipBone = "foot.R";
		
		this.Skeleton = NewSkeleton;
		this.LeftIk = NewLeftIk;
		this.RightIk = NewRightIk;
		
		EditorInterface.Singleton.MarkSceneAsUnsaved();
	}
}
