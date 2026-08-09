using Godot;
using System;

public partial class Pocion : Node2D
{
	[Export] public TextureButton UsarColor { get; set; }
	[Export] public Color color = Colors.White;
	[Export] public Sprite2D sprite;
	[Export] public Contenedor contenedor;

	private Mezclador _mezclador;
	private bool botonPresionado = false;

	private T BuscarNodo<T>(Node nodo) where T : Node
	{
		if (nodo is T resultado)
			return resultado;

		foreach (Node hijo in nodo.GetChildren())
		{
			T encontrado = BuscarNodo<T>(hijo);

			if (encontrado != null)
				return encontrado;
		}

		return null;
	}

	public override void _Ready()
	{
		_mezclador = BuscarNodo<Mezclador>(GetTree().CurrentScene);

		if (_mezclador == null)
			throw new Exception("No se encontraron Mezclador");

		if (sprite == null)
		{
			sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		}

		ActualizarColor();

		if (UsarColor != null)
		{
			UsarColor.ButtonDown += OnBotonApretado;
			UsarColor.ButtonUp += OnBotonSoltado;
		}
	}

	public override void _Process(double delta)
	{
		if (botonPresionado && Input.IsKeyPressed(Key.Space))
		{
			_mezclador.CambiarObjetivo(color);
		}
	}

	private void OnBotonApretado()
	{
		botonPresionado = true;
	}

	private void OnBotonSoltado()
	{
		botonPresionado = false;
		_mezclador.DetenerMezcla();
	}

	private void ActualizarColor()
	{
		if (sprite != null)
		{
			sprite.Modulate = color;
		}
	}

	public void CambiarColor(Color nuevoColor)
	{
		color = nuevoColor;
		ActualizarColor();
	}
}
