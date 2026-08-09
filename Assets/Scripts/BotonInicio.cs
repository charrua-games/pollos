using Godot;
using System;

public partial class BotonInicio : Button
{
	public override void _Ready()
	{
		Pressed += OnPressed;
	}
	private async void OnPressed()
	{
		await CanvasTransition.Instance.ChangeSceneAsync("res://Assets/Scenes/MezclarScene.tscn");
	}
}
