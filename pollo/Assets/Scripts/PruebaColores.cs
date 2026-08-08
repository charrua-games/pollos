using Godot;
using System;

public partial class PruebaColores : Node
{

	[Export] private Panel _color1;
	[Export] private Panel _color2;
	[Export] private Label _panelPorcentaje;
	[Export] private Button _botonEvaluar;


	[Export] private LineEdit _r1;
	[Export] private LineEdit _g1;
	[Export] private LineEdit _b1;

	[Export] private LineEdit _r2;
	[Export] private LineEdit _g2;
	[Export] private LineEdit _b2;

	private CalcularPorcentajeAciertoColor _calculadora;

	public override void _Ready()
	{
		_calculadora = new CalcularPorcentajeAciertoColor();

		if (_botonEvaluar != null)
			_botonEvaluar.Pressed += Evaluar;
		else
			GD.PrintErr("PruebaColores: falta asignar '_botonEvaluar' en el Inspector.");

		EstablecerValorInicial(_r1, "255");
		EstablecerValorInicial(_g1, "0");
		EstablecerValorInicial(_b1, "0");
		EstablecerValorInicial(_r2, "200");
		EstablecerValorInicial(_g2, "0");
		EstablecerValorInicial(_b2, "0");

		Evaluar();
	}

	public override void _ExitTree()
	{
		if (_botonEvaluar != null)
			_botonEvaluar.Pressed -= Evaluar;
	}

	private void EstablecerValorInicial(LineEdit campo, string valor)
	{
		if (campo != null && string.IsNullOrEmpty(campo.Text))
			campo.Text = valor;
	}

	private void Evaluar()
	{
		if (!ValidarNodos()) return;

		if (!TryValoresDesdeLineEdits(_r1, _g1, _b1, out int r1, out int g1, out int b1))
		{
			_panelPorcentaje.Text = "Error: revisá los valores de Color 1 (0-255)";
			return;
		}

		if (!TryValoresDesdeLineEdits(_r2, _g2, _b2, out int r2, out int g2, out int b2))
		{
			_panelPorcentaje.Text = "Error: revisá los valores de Color 2 (0-255)";
			return;
		}

		Color colorVisual1 = new Color(r1 / 255f, g1 / 255f, b1 / 255f);
		Color colorVisual2 = new Color(r2 / 255f, g2 / 255f, b2 / 255f);

		StyleBoxFlat estilo1 = new StyleBoxFlat();
		estilo1.BgColor = colorVisual1;
		_color1.AddThemeStyleboxOverride("panel", estilo1);

		StyleBoxFlat estilo2 = new StyleBoxFlat();
		estilo2.BgColor = colorVisual2;
		_color2.AddThemeStyleboxOverride("panel", estilo2);

		Color colorCalculo1 = new Color(r1, g1, b1);
		Color colorCalculo2 = new Color(r2, g2, b2);

		double porcentaje = _calculadora.EvaluarColor(colorCalculo1, colorCalculo2);
		_panelPorcentaje.Text = $"{porcentaje:F1}";
	}


	private bool TryValoresDesdeLineEdits(LineEdit r, LineEdit g, LineEdit b, out int ri, out int gi, out int bi)
	{
		ri = gi = bi = 0;

		if (!TryParseComponente(r.Text, out ri)) return false;
		if (!TryParseComponente(g.Text, out gi)) return false;
		if (!TryParseComponente(b.Text, out bi)) return false;

		return true;
	}

	
	private bool TryParseComponente(string texto, out int valor)
	{
		if (!int.TryParse(texto, out valor))
			return false;

		valor = Mathf.Clamp(valor, 0, 255);
		return true;
	}

	private bool ValidarNodos()
	{
		if (_color1 == null || _color2 == null || _panelPorcentaje == null ||
			_r1 == null || _g1 == null || _b1 == null ||
			_r2 == null || _g2 == null || _b2 == null)
		{
			GD.PrintErr("PruebaColores: faltan nodos por asignar en el Inspector.");
			return false;
		}
		return true;
	}
}
