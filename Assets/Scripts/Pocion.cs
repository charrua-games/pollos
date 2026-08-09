using Godot;
using System;

public partial class Pocion : Node2D
{
	[Export] public string IdPocion { get; set; } = ""; 
	[Export] public TextureButton UsarColor { get; set; }
	[Export] public Color color = Colors.White;
	[Export] public Sprite2D sprite;
	[Export] public Contenedor contenedor;

	private Mezclador _mezclador;
	private DesbloqueoPociones _desbloqueoPociones; // Nombre corregido
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
		// 1. Obtener el Autoload de forma segura
		_desbloqueoPociones = GetNodeOrNull<DesbloqueoPociones>("/root/DesbloqueoPociones");

		if (_desbloqueoPociones == null)
		{
			GD.PushWarning("No se encontró el autoload DesbloqueoPociones");
		}
		else if (string.IsNullOrEmpty(IdPocion))
		{
			GD.PushWarning($"{Name}: no tiene IdPocion asignado, se asume desbloqueada por defecto");
		}

		// 2. Inicializar componentes obligatorios (Deben configurarse SIEMPRE)
		_mezclador = BuscarNodo<Mezclador>(GetTree().CurrentScene);
		if (_mezclador == null)
			throw new Exception("No se encontró el Mezclador en la escena actual.");

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

		// 3. Comprobar estado de desbloqueo al final
		if (_desbloqueoPociones != null && !string.IsNullOrEmpty(IdPocion))
		{
			if (!_desbloqueoPociones.EstaDesbloqueada(IdPocion))
			{
				// Si está bloqueada, la ocultamos y desactivamos el proceso
				Visible = false;
				SetProcess(false);
			}
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

	public void Desbloquear()
	{
		if (_desbloqueoPociones != null && !string.IsNullOrEmpty(IdPocion))
		{
			_desbloqueoPociones.Desbloquear(IdPocion);
			Visible = true;
			SetProcess(true); // Volvemos a activar el _Process al desbloquear
		}
	}
}
