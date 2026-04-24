namespace SistemaInventario.Negocio;

public class ValidationResult
{
    private readonly List<string> _errores = [];

    public bool EsValido => _errores.Count == 0;
    public IReadOnlyList<string> Errores => _errores;

    public void AgregarError(string mensaje)
    {
        _errores.Add(mensaje);
    }
}
