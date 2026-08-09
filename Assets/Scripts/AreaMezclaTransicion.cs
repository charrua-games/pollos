using Godot;
using System;

public partial class AreaMezclaTransicion : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnPressed;
	}


	private async void OnPressed()
	{
		await CanvasTransition.Instance.ChangeSceneAsync("res://Assets/Scenes/Overworld.tscn");
	}
}
