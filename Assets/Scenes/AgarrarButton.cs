using Godot;
using System;

public partial class AgarrarButton : Button
{
	[Export]
	private AnimatedSprite2D animacion;

	[Export]
	private int indicePlanta;

	private InventarioPlantas inventario = new InventarioPlantas();

	public override void _Ready()
	{
		animacion.Visible = false;

		Pressed += IniciarAnimacion;
		animacion.AnimationFinished += TerminarAnimacion;
	}

	private void IniciarAnimacion()
	{
	  
		Disabled = true;

		Planta planta = PlantasCol.Plantas[indicePlanta];

	  
		inventario.RecibirPlanta(planta);

		
		animacion.Visible = true;
		animacion.Frame = 0;
		animacion.Play("default");
	}

	private void TerminarAnimacion()
	{
		animacion.Stop();
		animacion.Visible = false;
	}
}
