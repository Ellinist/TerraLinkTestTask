using System.Reflection.Metadata;

namespace TerraLinkTestTask.Interfaces
{
    public interface IExternalSystemConnector
    {
        /// <summary>
        /// Выполняет отправку документов во внешнюю систему.
        /// </summary>
        /// <param name="documents">
        /// Документы, которые нужно отправить.
        /// </param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/> для отмены асинхронной операции.
        /// </param>
        /// <returns>
        /// Асинхронная операция, завершение которой означает успешную отправку документов.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Возникакет при попытке отправить более 10 документов за раз.
        /// </exception>
        Task SendDocuments(IReadOnlyCollection<Document> documents, CancellationToken cancellationToken);
    }
}
