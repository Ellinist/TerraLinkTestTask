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
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancellationToken;

        private List<Document> _documentsInQueue = new(); // Документы в очереди
        private IProgress<int> _progress;
        #endregion

        #region PROPS
        /// <summary>
        /// Интервал отправки документов.
        /// </summary>
        public int TimerSpan { get; set; } = 5; // Для задания извне (в секундах)
        #endregion

        /// <summary>
        /// Цикл для таймера отправки документов
        /// </summary>
        private void TimerProcess()
        {
            Task.Factory.StartNew(() =>
            {
                // Цикл в другом потоке, чтобы не нарушать наполнение очереди документов
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var t = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(TimerSpan)); // Ожидание
                    });
                    t.Wait();
                    SendProcess();
                }
            });
        }

        /// <summary>
        /// Метод отправки документов из очереди
        /// </summary>
        private void SendProcess()
        {
            if (_documentsInQueue.Count == 0) return;

            var docsBlock = _documentsInQueue.Take(10).ToList();
            var res = _externalSystemConnector.SendDocuments(docsBlock, _cancellationToken);
            if(res.Status != TaskStatus.RanToCompletion) return;

            _progress.Report(docsBlock.Count); // Увеличение прогресса

            if (_cancellationToken.IsCancellationRequested) return;

            // Чистка отправленных документов
            foreach (var doc in docsBlock)
            {
                _documentsInQueue.Remove(doc);
            }
        }

        /// <summary>
        /// Ставит документ в очередь на отправку.
        /// </summary>
        /// <param name="document">
        /// Документ, который нужно отправить.
        /// </param>
        public void Enqueue(Document document)
        {
            _documentsInQueue.Add(document);
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
            
            _cancellationToken = new CancellationTokenSource().Token;
            _progress = new DocumentsReport();
            TimerProcess();
        }
        #endregion
    }
}
