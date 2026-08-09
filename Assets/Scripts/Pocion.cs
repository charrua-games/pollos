using Godot;
using System;

public partial class Pocion : Node2D
{
	[Export] public string IdPocion { get; set; } = ""; 
	[Export] public TextureButton UsarColor { get; set; }
	[Export] public Color color = Colors.White;
	[Export] public Sprite2D sprite;
	[Export] public Contenedor contenedor;

	// NUEVO: texturas para estado bloqueado/desbloqueado
	[Export] public Texture2D TexturaVacia { get; set; }
	[Export] public Texture2D TexturaLlena { get; set; }

	private Mezclador _mezclador;
	private DesbloqueoPociones _desbloqueoPociones;
	private bool botonPresionado = false;
	private bool _desbloqueada = true; // NUEVO: estado actual

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
			_desbloqueada = _desbloqueoPociones.EstaDesbloqueada(IdPocion);
			ActualizarEstadoVisual();
		}
	}

	public override void _Process(double delta)
	{
		// Solo permitimos mezclar si está desbloqueada
		if (_desbloqueada && botonPresionado && Input.IsKeyPressed(Key.Space))
		{
			_mezclador.CambiarObjetivo(color);
		}
	}

	private void OnBotonApretado()
	{
		if (!_desbloqueada) return; // NUEVO: ignorar clicks si está bloqueada
		botonPresionado = true;
	}

	private void OnBotonSoltado()
	{
		botonPresionado = false;
		_mezclador.DetenerMezcla();
	}

	private void ActualizarColor()
	{
		if (sprite != null && _desbloqueada)
		{
			sprite.Modulate = color;
		}
	}

	// NUEVO: cambia la textura según el estado, en vez de Visible
	private void ActualizarEstadoVisual()
	{
		if (sprite == null) return;

		if (_desbloqueada)
		{
			if (TexturaLlena != null)
				sprite.Texture = TexturaLlena;
			sprite.Modulate = color;
		}
		else
		{
			if (TexturaVacia != null)
				sprite.Texture = TexturaVacia;
			sprite.Modulate = Colors.White; // sin tinte en el estado vacío
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
			_desbloqueada = true;
			ActualizarEstadoVisual(); // en vez de Visible = true / SetProcess(true)
		}
	}
}
