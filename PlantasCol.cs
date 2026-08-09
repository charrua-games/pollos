using Godot;
using System;
using System.Collections.Generic;

public partial class PlantasCol : Node
{

	public static readonly List<Planta> Plantas = new()
	{
		new("Abedul", Colors.Red),
		new("Romero", Colors.Green),
		new("Poleo", Colors.Yellow),
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
