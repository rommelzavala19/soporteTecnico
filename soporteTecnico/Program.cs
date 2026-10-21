using System.Text;

Queue<string> nombres = new Queue<string>();
Queue<string> motivos = new Queue<string>();
Stack<string> registro = new Stack<string>();
List<string> nombresAtendidos = new List<string>();
List<string> motivosAtendidos = new List<string>();

Dictionary<string, int> palabrasClave = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
{
    { "error", 0 },
    { "ayuda", 0 },
    { "urgente", 0 },
    { "gracias", 0 }
};

string opcion = "";

do
{
    MostrarMenu();

    opcion = (Console.ReadLine() ?? string.Empty).Trim();

    switch (opcion)
    {
        case "1":
            PedirDatos("Ingrese su nombre: ", "Ingrese el motivo de su consulta: ");
            break;
        case "2":
            AtenderCliente();
            break;
        case "3":
            BuscarCliente();
            break;
        case "4":
            MostrarHistorial();
            break;
        case "5":
            MostrarReporte();
            break;
        case "6":
            Console.WriteLine("Saliendo del programa...");
            break;
        default:
            Console.WriteLine("\nOpción inválida. Por favor, seleccione una opción válida.");
            break;

    }

} while (opcion != "6");

void MostrarMenu()
{
    Console.WriteLine("\n=================================");
    Console.WriteLine("==== Chat de soporte técnico ====");
    Console.WriteLine("=================================");
    Console.WriteLine("1. Registrar cliente");
    Console.WriteLine("2. Atender cliente");
    Console.WriteLine("3. Buscar cliente atendido");
    Console.WriteLine("4. Mostrar historial");
    Console.WriteLine("5. Mostrar reporte");
    Console.WriteLine("6. Salir");
    Console.Write("Seleccione una opcion: ");
}

void PedirDatos(string mensajeNombre, string mensajeMotivo)
{
    Console.WriteLine("\n=================================");
    Console.WriteLine("====== Registrar Cliente ========");
    Console.WriteLine("=================================");

    string nombre = "";
    //Pedir el nombre del usuario
    while (true)
    {
        Console.Write($"\n{mensajeNombre}");
        string entrada = (Console.ReadLine() ?? string.Empty).Trim();

        if (!string.IsNullOrWhiteSpace(entrada) && entrada.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
        {
            nombre = entrada;
            break;
        }

        Console.WriteLine("\nEl nombre ingresado es inválido. Intente nuevamente.");

    }

    string motivo = "";
    //Pedir el motivo de la consulta
    while (true)
    {
        Console.Write($"\n{mensajeMotivo}");
        string entradaMotivo = (Console.ReadLine() ?? string.Empty).Trim();

        if (!string.IsNullOrWhiteSpace(entradaMotivo) && entradaMotivo.Any(char.IsLetter))
        {
            motivo = entradaMotivo;
            break;
        }

        Console.WriteLine("\nEl motivo no es válido. Debe contener al menos texto descriptivo y no estar vacío.");
    }

    AnalizarMensaje(motivo);

    AgregarACola(nombre, motivo);

}

string LimpiarTexto(string texto)
{
    string entradaLimpia = new string(texto.ToLower()
        .Where(c => char.IsLetter(c) || char.IsWhiteSpace(c))
        .ToArray()).Trim();

    return string.Join(" ", entradaLimpia.Split(' ', StringSplitOptions.RemoveEmptyEntries));
}

void AgregarACola(string nombre, string motivo)
{
    nombres.Enqueue(nombre);
    motivos.Enqueue(motivo);

    MostrarAcciones(nombre, motivo);

    Console.WriteLine($"\nCliente '{nombre}' agregado a la cola con éxito.");
}

void MostrarAcciones(string nombre, string motivo)
{
    registro.Push($"Cliente registrado: {nombre} | Motivo: {motivo}");
}

void AtenderCliente()
{
    Console.WriteLine("\n=================================");
    Console.WriteLine("======= Atender Cliente =========");
    Console.WriteLine("=================================");

    if (nombres.Count == 0)
    {
        Console.WriteLine("\nNo hay clientes en espera para atender.");
        return;
    }

    string clienteAtendido = nombres.Dequeue();
    string motivoAtendido = motivos.Dequeue();

    registro.Push($"Atención iniciada: {clienteAtendido}");

    RegistrarAtendido(clienteAtendido, motivoAtendido);

    Console.WriteLine($"\nAtendiendo a '{clienteAtendido}'... (Motivo: '{motivoAtendido}')");

    registro.Push($"Caso cerrado: {clienteAtendido}");
}

void RegistrarAtendido(string nombre, string motivo)
{
    nombresAtendidos.Add(nombre);
    motivosAtendidos.Add(motivo);
}

void BuscarCliente()
{
    Console.WriteLine("\n=================================");
    Console.WriteLine("===== Buscar Cliente Atendido ===");
    Console.WriteLine("=================================");

    if (ContarAtendidos() == 0)
    {
        Console.WriteLine("\nAún no se ha atendido a ningún cliente.");
        return;
    }

    Console.Write("\nIngrese el nombre del cliente a buscar: ");
    string nombreBuscar = (Console.ReadLine() ?? string.Empty).Trim();

    string motivo = ObtenerMotivo(nombreBuscar);

    if (string.IsNullOrEmpty(motivo))
    {
        Console.WriteLine($"\nEl cliente '{nombreBuscar}' no fue encontrado o aún no ha sido atendido.");
        return;
    }

    Console.WriteLine($"\nCliente encontrado : '{nombreBuscar}'");
    Console.WriteLine($"Motivo registrado  : '{motivo}'");
}

int BuscarPorNombre(string nombre)
{
    if (string.IsNullOrWhiteSpace(nombre))
    {
        return -1;
    }

    return nombresAtendidos.FindIndex(n => n.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase));
}

string ObtenerMotivo(string nombre)
{
    int posicion = BuscarPorNombre(nombre);
    return posicion == -1 ? "" : motivosAtendidos[posicion];
}

void AnalizarMensaje(string mensaje)
{
    if (string.IsNullOrWhiteSpace(mensaje)) return;

    string mensajeLimpio = LimpiarTexto(mensaje);

    string[] palabrasOriginales = mensajeLimpio.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    List<string> palabrasProcesadas = new List<string>();
    foreach (var palabra in palabrasOriginales)
    {
        if (palabra.Equals("tonto", StringComparison.OrdinalIgnoreCase) ||
            palabra.Equals("malo", StringComparison.OrdinalIgnoreCase))
        {
            palabrasProcesadas.Add("[censurado]");
        }
        else
        {
            palabrasProcesadas.Add(palabra);
        }
    }

    string mensajeCensurado = string.Join(" ", palabrasProcesadas);

    int totalPalabras = palabrasOriginales.Length;

    string[] clavesABuscar = { "error", "ayuda", "urgente", "gracias" };
    Dictionary<string, int> conteoActual = new Dictionary<string, int>();

    foreach (var clave in clavesABuscar)
    {
        int cantidad = palabrasOriginales.Count(p => p.Equals(clave, StringComparison.OrdinalIgnoreCase));
        conteoActual[clave] = cantidad;

        if (palabrasClave.ContainsKey(clave))
        {
            palabrasClave[clave] += cantidad;
        }
        else
        {
            palabrasClave[clave] = cantidad;
        }
    }

    StringBuilder sb = new StringBuilder();
    sb.AppendLine("\n==== REPORTE DE ANÁLISIS DE MENSAJE ====");
    sb.AppendLine($"Mensaje censurado: {mensajeCensurado}");
    sb.AppendLine($"Total de palabras: {totalPalabras}");
    sb.AppendLine("Palabras clave encontradas:");

    foreach (var kvp in conteoActual)
    {
        if (kvp.Value > 0)
        {
            sb.AppendLine($"  - {kvp.Key}: {kvp.Value}");
        }
    }

    sb.AppendLine("Primeras 3 letras de cada palabra (usando AsSpan):");

    foreach (var palabra in palabrasOriginales)
    {
        ReadOnlySpan<char> span = palabra.AsSpan();
        int longitudExtraer = Math.Min(3, span.Length);
        ReadOnlySpan<char> primerasTres = span.Slice(0, longitudExtraer);
        sb.AppendLine($"  -> '{palabra}' : {primerasTres.ToString()}");
    }

    Console.WriteLine(sb.ToString());
    registro.Push("Mensaje analizado y procesado");
}

int ContarAtendidos()
{
    return nombresAtendidos.Count;
}

void MostrarPalabrasClave()
{
    Console.WriteLine("\n Palabras clave detectadas:");
    foreach (var kvp in palabrasClave)
    {
        Console.WriteLine($"  - '{kvp.Key}': {kvp.Value} veces");
    }
}

string ObtenerMotivoMasFrecuente()
{
    if (motivosAtendidos.Count == 0) return "Sin atendidos aún";

    return motivosAtendidos.GroupBy(m => m)
                          .OrderByDescending(g => g.Count())
                          .First().Key;
}
void MostrarHistorial()
{
    Console.WriteLine("\n=================================");
    Console.WriteLine("======= Historial Completo ======");
    Console.WriteLine("=================================");

    if (registro.Count == 0)
    {
        Console.WriteLine("\nNo hay acciones registradas en el historial.");
        return;
    }

    foreach (string accion in registro)
    {
        Console.WriteLine(accion);
        Console.WriteLine();
    }
    Console.WriteLine("\n=================================");
}

void MostrarUltimas5()
{
    Console.WriteLine("\n=================================");
    Console.WriteLine("======= Últimas 5 Acciones ======");
    Console.WriteLine("=================================");

    if (registro.Count == 0)
    {
        Console.WriteLine("\nNo hay acciones registradas.");
        return;
    }

    int contador = 0;

    foreach (string accion in registro)
    {
        if (contador == 5)
            break;

        Console.WriteLine(accion);
        Console.WriteLine();
        contador++;
    }
}

void MostrarReporte()
{
    Console.WriteLine("\n=================================");
    Console.WriteLine("======== Reporte General ========");
    Console.WriteLine("=================================");

    if (registro.Count == 0)
    {
        Console.WriteLine("No hay acciones registradas en el historial.");
        Console.WriteLine("\n=====================================");
        return;
    }

    Console.WriteLine($" Total de clientes atendidos : {ContarAtendidos()}");

    Console.WriteLine($" Total de clientes en espera : {nombres.Count}");

    Console.WriteLine($" Motivo más frecuente: {ObtenerMotivoMasFrecuente()}");

    MostrarPalabrasClave();

    MostrarUltimas5();

    Console.WriteLine("=====================================");
}
