using Godot;
using System;


public partial class CalcularPorcentajeAciertoColor : Node{
 
	public double EvaluarColor(Color colorEvaluado, Color colorReferencia)
	{
		Double resultado;
		double difereciaTotal;
		
		float difereciaRojo = MathF.Pow(colorEvaluado.R - colorReferencia.R, 2);
		float difereciaVerde = MathF.Pow(colorEvaluado.G - colorReferencia.G, 2);
		float difereciaAzul = MathF.Pow(colorEvaluado.B - colorReferencia.B, 2);

		difereciaTotal = Math.Sqrt( difereciaAzul + difereciaVerde + difereciaRojo);
		
		resultado = 100 * (1 -  difereciaTotal / 765);


		return resultado;
	}
	
	
	
}
