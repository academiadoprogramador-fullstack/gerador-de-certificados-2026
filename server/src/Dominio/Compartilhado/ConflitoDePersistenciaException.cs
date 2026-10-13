namespace GeradorCertificadosOnline.Dominio.Compartilhado;

public sealed class ConflitoDePersistenciaException(string mensagem, Exception? innerException = null)
    : Exception(mensagem, innerException);
