namespace GeradorCertificadosOnline.Api.Compartilhado.Http;

public static class ProblemDetailsTypes
{
    public static string ObterPorStatus(int status) => status switch
    {
        400 => "https://www.rfc-editor.org/rfc/rfc9110.html#section-15.5.1",
        401 => "https://www.rfc-editor.org/rfc/rfc9110.html#section-15.5.2",
        403 => "https://www.rfc-editor.org/rfc/rfc9110.html#section-15.5.4",
        404 => "https://www.rfc-editor.org/rfc/rfc9110.html#section-15.5.5",
        409 => "https://www.rfc-editor.org/rfc/rfc9110.html#section-15.5.10",
        _ => "https://www.rfc-editor.org/rfc/rfc9110.html#section-15.6.1"
    };
}
