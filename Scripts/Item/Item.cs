using Godot;
using System;

public abstract partial class Item : Node3D
{
	private float DeltaTime;

	private GameManager GameManager;
	private InputManager InputManager;
	private Console Console;
	private RandomNumberGenerator Rng;
	private ProjectileManager ProjectileManager;
	private DestructionManager DestructionManager;
	private Hud Hud;
	private Viewmodel Viewmodel;
	
	[Export] public string Id = "ItemId";
	
	private bool Active = false;
	private float ActionTimer = 0f;
	private ItemAction CurrentAction;
	private ItemAction QueuedAction;
	private ItemAction EmptyAction = new ItemAction();
	
	public override void _Ready()
	{
		GameManager = (GameManager)GetTree().Root.FindChild("GameManager", true, false);
		InputManager = GameManager.InputManager;
		Console = GameManager.Console;
		Rng = GameManager.Rng;
		ProjectileManager = GameManager.ProjectileManager;
		DestructionManager = GameManager.DestructionManager;
		Hud = GameManager.Hud;
		Viewmodel = GameManager.Viewmodel;
		
		CurrentAction = EmptyAction;
		QueuedAction = EmptyAction;
	}
	
	public override void _Process(double delta)
	{
		DeltaTime = GameManager.GlobalDeltaTime;
		
		Update();
		
		ProgressAction();
	}
	
	protected virtual void Update()
	{
		
	}
	
	private void ProgressAction()
	{
		ActionTimer += DeltaTime;
		
		//if (CurrentAction.HasEvents)
		//if (CurrentAction.Events.Length != 0 )
		if (CurrentAction.Events != null && CurrentAction.Events != EmptyAction.Events)
		{
			foreach (ItemEvent Event in CurrentAction.Events)
			{
				if (!Event.Triggered)
				{
					if (ActionTimer <= Event.TriggerTime)
					{
						EventCallback(Event.Name);
						Event.Triggered = true;
					}
				}
			}
		}
		
		if (ActionTimer >= CurrentAction.Duration) 
		{
			if (QueuedAction.Name != "") PlayAction(QueuedAction);
			else PlayAction(EmptyAction);
		}
	}
	
	public void SetActive(bool NewActive)
	{
		Active = NewActive;
	}
	
	public bool IsActive()
	{
		return Active;
	}
	
	public void QueueAction(ItemAction NewAction)
	{
		QueuedAction = NewAction;
	}
	
	public void PlayAction(ItemAction NewAction)
	{
		CurrentAction = NewAction;
		ActionTimer = 0f;
		if (NewAction.Override != null && NewAction.Animation != "") NewAction.Override.Play(NewAction.Animation);
		QueuedAction = EmptyAction;
	}
	
	protected virtual void EventCallback(string EventName)
	{
		
	}
}
