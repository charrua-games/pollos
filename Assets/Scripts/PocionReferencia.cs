using Godot;
using System;

public partial class PocionReferencia : Node
{
	
   
	[Export]
	public Color color = Colors.White;


	[Export]
	public Sprite2D sprite;

	public override void _Ready()
	{
		if (sprite == null)
		{
			sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		}

		ActualizarColor();
	}
	public void CambiarColor(Color nuevoColor)
	{
		color = nuevoColor;
		ActualizarColor();
	}
	private void ActualizarColor()
	{
		if (sprite != null)
		{
			sprite.Modulate = color;
		}
	}

  
	public Color ObtenerColor()
	{
		return color;
	}
}
