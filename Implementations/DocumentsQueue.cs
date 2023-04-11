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
        /// Потокобезопасный словарь
        /// </summary>
        private List<Document> _documentsInQueue = new();

        private int _recordsQuantity = 10; // Количество одновременно отправляемых документов

        private IProgress<int> _progress;
        private TaskStatus _taskStatus;
        private object locker = new();
        #endregion

        #region PROPS
        /// <summary>
        /// Интервал отправки документов.
        /// </summary>
        public double TimerSpan { get; set; } = 0.5; // Для задания извне (в секундах)
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
                    _taskStatus = await SendProcess(); // Для отслеживания выполнения через Dispose
                }
            });
        }

        /// <summary>
        /// Асинхронный метод отправки документов из очереди
        /// </summary>
        private async Task<TaskStatus> SendProcess()
        {
            lock (locker)
            {
                if(_documentsInQueue.Count == 0) return TaskStatus.WaitingForActivation;
            }

            var firstElements = _documentsInQueue.Take(_recordsQuantity).ToList();

            // Вызов тяжелого метода отправки документов
            await _externalSystemConnector.SendDocuments(firstElements, _cancellationToken);

            // Проверка на отмену операции
            if (_cancellationToken.IsCancellationRequested) return TaskStatus.Canceled;

            foreach (var element in firstElements)
            {
                lock (locker)
                {
                    _documentsInQueue.Remove(element);
                }
            }

            _progress.Report(firstElements.Count); // Увеличение прогресса на количество отправленных документов

            return TaskStatus.RanToCompletion;
        }

        /// <summary>
        /// Ставит документ в очередь на отправку.
        /// </summary>
        /// <param name="document">
        /// Документ, который нужно отправить.
        /// </param>
        public void Enqueue(Document document)
        {
            lock (locker)
            {
                _documentsInQueue.Add(document);
            }
        }

        /// <summary>
        /// Освобождение ресурсов и остановка процессов
        /// </summary>
        public void Dispose()
        {
            _cancelTokenSource.Cancel();

            _documentsInQueue.Clear(); // Очищаем очередь документов
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
