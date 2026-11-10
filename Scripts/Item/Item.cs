using Godot;
using System;

public abstract partial class Item : Node3D
{
	protected float DeltaTime;
	
	protected GameManager GameManager;
	protected InputManager InputManager;
	protected Player Player;
	protected Console Console;
	protected RandomNumberGenerator Rng;
	protected ProjectileManager ProjectileManager;
	protected DestructionManager DestructionManager;
	protected Hud Hud;
	protected Viewmodel Viewmodel;
	
	[Export] public string Id = "ItemId";
	
	private bool Active = false;
	
	public override void _Ready()
	{
		GameManager = (GameManager)GetTree().Root.FindChild("GameManager", true, false);
		InputManager = GameManager.InputManager;
		Player = GameManager.Player;
		Console = GameManager.Console;
		Rng = GameManager.Rng;
		ProjectileManager = GameManager.ProjectileManager;
		DestructionManager = GameManager.DestructionManager;
		Hud = GameManager.Hud;
		Viewmodel = GameManager.Viewmodel;
		
		Setup();
	}
	
	public override void _Process(double delta)
	{
		DeltaTime = GameManager.GlobalDeltaTime;
		
		Update();
	}
	
	protected virtual void Setup()
	{
		
	}
	
	protected virtual void Update()
	{
		
	}
	
	public void SetActive(bool NewActive)
	{
		Active = NewActive;
	}
	
	public bool IsActive()
	{
		return Active;
	}
}
