using Godot;
using System;

public partial class BotonSalir : Button
{
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		GetTree().Quit();
	}
}
