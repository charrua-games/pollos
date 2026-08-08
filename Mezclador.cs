using Godot;
using System;

public partial class Mezclador : Node2D
{	public double Mezclar(Color color1, Color color2, double delta)
	{
		double color1Rad = color1.H * 2 * Math.PI;
		double color2Rad = color2.H * 2 * Math.PI;
		double step = 0.1;

		double newColor = color1Rad + step * delta;
		if (newColor == color2Rad)
		{
			return 1;
		}

		return newColor;
		}
}
