using Godot;
using System;
using System.Collections.Generic;

public partial class PruebaColoresGrilla : Node
{
	// Lista de paneles de la grilla (asignar en el Inspector, mismo orden que _labels)
	[Export] private Panel[] _paneles;

	// Lista de labels debajo de cada panel, que muestran el % de similitud
	[Export] private Label[] _labels;

	// Panel opcional que muestra el color "original" a adivinar/comparar
	[Export] private Panel _panelOriginal;

	// Botón opcional para volver a generar todo con colores nuevos
	[Export] private Button _botonRegenerar;

	private CalcularPorcentajeAciertoColor _calculadora;
	private Random _random = new Random();

	// Color original en escala 0-255, guardado para comparar
	private int _origR, _origG, _origB;

	public override void _Ready()
	{
		_calculadora = new CalcularPorcentajeAciertoColor();

		if (_botonRegenerar != null)
			_botonRegenerar.Pressed += RegenerarGrilla;

		RegenerarGrilla();
	}

	public override void _ExitTree()
	{
		if (_botonRegenerar != null)
			_botonRegenerar.Pressed -= RegenerarGrilla;
	}

	// Genera un nuevo color original y repinta toda la grilla comparando contra él
	private void RegenerarGrilla()
	{
		if (!ValidarNodos()) return;

		// 1) Color original random (escala 0-255)
		Color colorOriginal = Color.FromHsv(ColorRandomRaw(), 1, 1);

		if (_panelOriginal != null)
		{
			StyleBoxFlat estiloOriginal = new StyleBoxFlat();
			estiloOriginal.BgColor = colorOriginal;
			_panelOriginal.AddThemeStyleboxOverride("panel", estiloOriginal);
		}

		// 2) Cada panel de la grilla recibe su propio color random y se compara contra el original
		for (int i = 0; i < _paneles.Length; i++)
		{

			Color colorNuevo = Color.FromHsv(ColorRandomRaw(), 1, 1);

			StyleBoxFlat estilo = new StyleBoxFlat();
			estilo.BgColor = colorNuevo;
			_paneles[i].AddThemeStyleboxOverride("panel", estilo);

			double puntaje = _calculadora.EvaluarColor(colorNuevo, colorOriginal);

			if (i < _labels.Length && _labels[i] != null)
				_labels[i].Text = $"{puntaje:f2}";
		}
	}


	private float ColorRandomRaw()
	{
		return _random.NextSingle();
	}

	private bool ValidarNodos()
	{
		if (_paneles == null || _paneles.Length == 0)
		{
			GD.PrintErr("PruebaColoresGrilla: falta asignar el array de '_paneles' en el Inspector.");
			return false;
		}
		if (_labels == null || _labels.Length < _paneles.Length)
		{
			GD.PrintErr("PruebaColoresGrilla: '_labels' debe tener al menos tantos elementos como '_paneles'.");
			return false;
		}
		return true;
	}
}
