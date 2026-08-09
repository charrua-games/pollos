using System;

public class Nota
{
   public string AccionHit { get; private set; } = string.Empty;
   public double TiempoHitSegundos { get; private set; }

   public void Configurar(string accionHit, double tiempoHitSegundos)
   {
	  if (string.IsNullOrWhiteSpace(accionHit))
	  {
		 throw new ArgumentException("La accion de hit no puede ser vacia.", nameof(accionHit));
	  }

	  AccionHit = accionHit;
	  TiempoHitSegundos = tiempoHitSegundos;
   }

	public bool CorrespondeAccion(string accionPresionada)
	{
		return string.Equals(AccionHit, accionPresionada, StringComparison.Ordinal);
	}

	public bool EstaEnVentana(double tiempoActualSegundos, double ventanaSegundos)
	{
		return Math.Abs(tiempoActualSegundos - TiempoHitSegundos) <= ventanaSegundos;
	}

   public bool IntentarHit(string accionPresionada, double tiempoActualSegundos, double ventanaSegundos)
   {
	  return CorrespondeAccion(accionPresionada) && EstaEnVentana(tiempoActualSegundos, ventanaSegundos);
   }
}
