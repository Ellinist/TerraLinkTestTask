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
        private ConcurrentDictionary<int, Document> _documentsInQueue = new();

        private int _recordsQuantity = 10; // Количество одновременно отправляемых документов
        private int _documentsIndex = 0; // Индекс для наполнения очереди
        private int _counterPrevValue = 0; // Счетчик для удаления уже отправленных документов

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
                    _sendTask = SendProcess(_cancellationToken); // Для отслеживания выполнения через Dispose
                }
            });
        }

        /// <summary>
        /// Асинхронный метод отправки документов из очереди
        /// </summary>
        private async Task SendProcess(CancellationToken token)
        {
            if (_documentsInQueue.Count == 0) return;

            // Этот список можно вынести в класс и перед использованием просто чистить
            List<Document> workDocumentsList = new(); // Рабочий список для отправки
            int tempCounter = 0; // Внутренний счетчик наполнения списка
            for (int i = _counterPrevValue; i < _counterPrevValue + _recordsQuantity; i++)
            {
                if (_documentsInQueue.ContainsKey(i)) // Если словарь имеет ключ
                {
                    workDocumentsList.Add(_documentsInQueue[i]); // Читаем документ
                    tempCounter++; // Инкремент счетчика
                }
            }

            // Вызов тяжелого метода отправки документов
            await _externalSystemConnector.SendDocuments(workDocumentsList, _cancellationToken);

            // Проверка на отмену операции
            if (!_cancellationToken.IsCancellationRequested)
            {
                for (int i = 0; i < _recordsQuantity; i++)
                {
                    // Чистим часть очереди документов
                    // В принципе, этого можно было бы и не делать, но заботимся о памяти
                    // на случай, если документы тяжелые - с вложениями и т.д.
                    if (!_documentsInQueue.TryRemove(_counterPrevValue + i, out var doc))
                    {
                        throw new Exception(); // Если не удалось удалить
                    }
                }
                _counterPrevValue += _recordsQuantity; 

                _progress.Report(tempCounter); // Увеличение прогресса на количество отправленных документов
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
            _documentsIndex++;
            _documentsInQueue.AddOrUpdate(_documentsIndex, document, (key, doc) => doc);
        }

        /// <summary>
        /// Освобождение ресурсов и остановка процессов
        /// </summary>
        public void Dispose()
        {
            _cancelTokenSource.Cancel();

            _documentsInQueue.Clear(); // Очищаем очередь документов

            // Если ввести дополнительный класс, например класс статуса отправки документов,
            // то можно в отчете вместо дженерика int использовать этот класс
            // Для примера в папке Models лежит такой класс.
            // И потом проверять DocumentsReport

            // Или использовать IAsyncDisposable<T>
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
