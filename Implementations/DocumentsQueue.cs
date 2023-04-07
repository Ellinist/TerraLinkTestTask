using System.Reflection.Metadata;
using TerraLinkTestTask.Interfaces;

namespace TerraLinkTestTask.Implementations
{
    public class DocumentsQueue : IDocumentsQueue, IObservable, IDisposable
    {
        private event Action<IObserver> StartDocumentSending; // Начало отправки документов
        #region PRIVATE FIELDS
        private readonly IExternalSystemConnector _externalSystemConnector;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancellationToken;
        #endregion

        /// <summary>
        /// Ставит документ в очередь на отправку.
        /// </summary>
        /// <param name="document">
        /// Документ, который нужно отправить.
        /// </param>
        public void Enqueue(Document document)
        {

        }

        /// <summary>
        /// Временной цикл отправки документов
        /// </summary>
        /// <param name="obs"></param>
        private void TimeCycle(IObserver obs)
        {
            //Task.Factory.StartNew(() =>
            //{
            //    Thread.Sleep(TimerSpan);

            //    if (Documents.Count < 10) return; // Если документов в очереди меньше 10 продолжаем накопление

            //    while (Documents.Count > 0)
            //    {
            //        var docs = Documents.Take(10).ToList();

            //        SendDocumentFromQueue(obs, docs);
            //        DocumentProgress.Report(docs.Count);

            //        foreach (var doc in docs)
            //        {
            //            Documents.Remove(doc);
            //        }
            //        foreach (var observer in Observers.ToList())
            //        {
            //            RemoveObserver(observer);
            //        }
            //    }
            //});
        }

        public void AddObserver(IObserver o)
        {
            throw new NotImplementedException();
        }

        public void RemoveObserver(IObserver o)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }






        #region CTOR
        public DocumentsQueue(IExternalSystemConnector externalSystemConnector)
        {
            _externalSystemConnector = externalSystemConnector ??
                                       throw new ArgumentNullException(nameof(externalSystemConnector));

            StartDocumentSending += TimeCycle;

            _cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancelTokenSource.Token;
        }
        #endregion
    }
}
