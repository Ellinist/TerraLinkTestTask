﻿using System.Reflection.Metadata;
using TerraLinkTestTask.Interfaces;

namespace TerraLinkTestTask.Implementations
{
    public sealed class ExternalSystemConnector : IExternalSystemConnector
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
        /// Возникает при попытке отправить более 10 документов за раз.
        /// </exception>
        public async Task SendDocuments(IReadOnlyCollection<Document> documents, CancellationToken cancellationToken)
        {
            if (documents.Count > 10)
            {
                throw new ArgumentException("Can't send more than 10 documents at once.", nameof(documents));
            }
            // тестовая реализация, просто ничего не делаем 2 секунды.
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
}
