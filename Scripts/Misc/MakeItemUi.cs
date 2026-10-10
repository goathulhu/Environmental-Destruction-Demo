using Godot;

[Tool]
public partial class MakeItemUi : Node
{
	[Export] PackedScene THING;
	[ExportToolButton("Generate")]
	public Callable ButtonGenerate => Callable.From(DoThing);

	private void DoThing()
	{
		Node NewThingy = THING.Instantiate();
		AddChild(NewThingy);
		NewThingy.Owner = GetTree().EditedSceneRoot;
	}
}
