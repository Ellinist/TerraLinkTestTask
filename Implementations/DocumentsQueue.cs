using System.Collections.Concurrent;
using System.Reflection.Metadata;
using TerraLinkTestTask.Interfaces;
using TerraLinkTestTask.Models;

namespace TerraLinkTestTask.Implementations
{
    /// <summary>
    /// Класс реализации отправки документов из очереди документов через заданные промежутки времени.
    /// </summary>
    public class DocumentsQueue : IDocumentsQueue, IDisposable
    {
        #region PRIVATE FIELDS
        private readonly IExternalSystemConnector _externalSystemConnector;
        private CancellationTokenSource _cancelTokenSource = new();
        private CancellationToken _cancellationToken;

        /// <summary>
        /// Потокобезопасная коллекция
        /// </summary>
        private ConcurrentQueue<Document> _documentsInQueue = new();
        private IProgress<int> _progress;
        private Task _sendTask;
        #endregion

        #region PROPS
        /// <summary>
        /// Интервал отправки документов.
        /// </summary>
        public int TimerSpan { get; set; } = 3; // Для задания извне (в секундах)
        #endregion

        /// <summary>
        /// Цикл для таймера отправки документов
        /// </summary>
        private void TimerProcess()
        {
            Task.Run(async () =>
            {
                // Цикл в другом потоке, чтобы не нарушать наполнение очереди документов
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(TimerSpan));
                    _sendTask = SendProcess(_cancellationToken); // Для отслеживания выполнения
                }
            });
        }

        /// <summary>
        /// Асинхронный метод отправки документов из очереди
        /// </summary>
        private async Task SendProcess(CancellationToken token)
        {
            if (_documentsInQueue.Count == 0) return;

            var docsBlock = _documentsInQueue.Take(10).ToList();
            // Чистка отправляемых документов
            foreach (var doc in docsBlock)
            {
                _documentsInQueue.TryDequeue(out var d);
            }
            await _externalSystemConnector.SendDocuments(docsBlock, _cancellationToken);
            _progress.Report(docsBlock.Count); // Увеличение прогресса
        }

        /// <summary>
        /// Ставит документ в очередь на отправку.
        /// </summary>
        /// <param name="document">
        /// Документ, который нужно отправить.
        /// </param>
        public void Enqueue(Document document)
        {
            _documentsInQueue.Enqueue(document);
        }

        /// <summary>
        /// Освобождение ресурсов и остановка процессов
        /// </summary>
        public void Dispose()
        {
            _cancelTokenSource.Cancel();
            _documentsInQueue.Clear();
        }

        





        #region CTOR
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="externalSystemConnector">
        /// Коннектор отправки документов.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public DocumentsQueue(IExternalSystemConnector externalSystemConnector)
        {
            _externalSystemConnector = externalSystemConnector ??
                                       throw new ArgumentNullException(nameof(externalSystemConnector));
            
            _cancellationToken = _cancelTokenSource.Token;
            _progress = new DocumentsReport();
            TimerProcess();
        }
        #endregion
    }
}
