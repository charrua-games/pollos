using Godot;
using System;

public partial class Contenedor : Node2D
{
	[Export]
	public Sprite2D sprite;

	public override void _Ready()
	{
	}

	public void RecibirColor(Color nuevoColor)
	{
		sprite.Modulate = nuevoColor;
	}

	public Color ObtenerColor()
	{
		return sprite.Modulate;
	}
}
