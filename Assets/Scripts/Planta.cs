using Godot;
using System;

public partial class Planta
{
	public string Nombre { get; private set; }
	private Color color { get; set; }

	public Planta(string nombre, Color color)
	{
		Nombre = nombre;
		this.color = color;
	}

	public Color ObtenerColor()
	{
		return color;
	}
}
