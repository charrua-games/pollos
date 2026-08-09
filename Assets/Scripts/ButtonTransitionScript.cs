using Godot;
using System;

public partial class ButtonTransitionScript : Button
{
	[Export(PropertyHint.File, "*.tscn")]
	public string EscenaDestino;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private async void OnPressed()
	{
		if (string.IsNullOrEmpty(EscenaDestino))
		{
			GD.PrintErr($"{Name}: EscenaDestino no está asignado.");
			return;
		}

		if (CanvasTransition.Instance == null || !IsInstanceValid(CanvasTransition.Instance))
		{
			GD.PrintErr($"{Name}: CanvasTransition.Instance no es válido.");
			return;
		}

		await CanvasTransition.Instance.ChangeSceneAsync(EscenaDestino);
	}
}
