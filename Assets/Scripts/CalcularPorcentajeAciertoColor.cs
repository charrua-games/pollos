using Godot;
using System;


public partial class CalcularPorcentajeAciertoColor : Node{
 
	public double EvaluarColor(Color colorEvaluado, Color colorReferencia)
	{
		// Convertimos el Hue (0-1) de cada color a radianes (0 a 2π)
		double colorRadEvaluado = colorEvaluado.H * 2 * Math.PI;
		double colorRadReferencia = colorReferencia.H * 2 * Math.PI;

		double diferenciaTotal = colorRadReferencia - colorRadEvaluado;

		// Atan2(sin, cos) "envuelve" la diferencia al rango correcto (-π, π],
		// así 350° vs 10° da 20° de diferencia y no 340°
		double difereciaHue = Math.Atan2(Math.Sin(diferenciaTotal), Math.Cos(diferenciaTotal));

		// Usamos el valor absoluto: nos interesa qué tan lejos están, no en qué dirección
		double difereciaAbsoluta = Math.Abs(difereciaHue);

		// La diferencia máxima posible entre dos matices es π (colores opuestos en el círculo)
		double resultado = 100 * (1 - difereciaAbsoluta / Math.PI);

		if (Mathf.IsNaN(resultado)) GD.Print("resultado is NaN");

		return resultado;
	}
	
	
	
	
	   
		 
	
}
