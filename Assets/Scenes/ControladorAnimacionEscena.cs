using Godot;
using System;

public partial class ControladorAnimacionEscena : Node2D
{
	private AnimatedSprite2D _morteroGolpes;
	private AnimatedSprite2D agarrarMortero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		agarrarMortero = GetNode<AnimatedSprite2D>("../AgarrarMortero");
		agarrarMortero.Play();

		_morteroGolpes = GetNode<AnimatedSprite2D>("../MorteroGolpes");
		_morteroGolpes.Visible = false;
		_morteroGolpes.Play();
	}



	private void _on_agarrar_mortero_animation_finished()
	{
		_morteroGolpes.Visible = true;
		agarrarMortero.Visible = false;
	}
}
