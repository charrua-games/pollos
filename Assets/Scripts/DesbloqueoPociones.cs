using Godot;
using System.Collections.Generic;

public partial class DesbloqueoPociones : Node
{
	private const string RutaGuardado = "user://desbloqueos_pociones.cfg";
	private const string Seccion = "pociones";

	private HashSet<string> _desbloqueadas = new();

	public override void _Ready()
	{
		Cargar();
	}

	public bool EstaDesbloqueada(string id)
	{
		return _desbloqueadas.Contains(id);
	}

	public void Desbloquear(string id)
	{
		if (string.IsNullOrEmpty(id)) return;

		if (_desbloqueadas.Add(id))
		{
			Guardar();
		}
	}

	private void Guardar()
	{
		var config = new ConfigFile();

		int i = 0;
		foreach (string id in _desbloqueadas)
		{
			config.SetValue(Seccion, $"id_{i}", id);
			i++;
		}

		config.Save(RutaGuardado);
	}

	private void Cargar()
	{
		var config = new ConfigFile();
		Error err = config.Load(RutaGuardado);

		if (err != Error.Ok)
			return;

		foreach (string key in config.GetSectionKeys(Seccion))
		{
			string id = (string)config.GetValue(Seccion, key);
			_desbloqueadas.Add(id);
		}
	}
}
