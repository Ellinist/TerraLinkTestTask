using System.Reflection.Metadata;
using Ninject;
using TerraLinkTestTask.Infrastructure;
using TerraLinkTestTask.Interfaces;

namespace TerraLinkTestTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Инициализация ядра DI-контейнера.
            IKernel kernel = new StandardKernel(new NinjectRegistration());

            #region Формирование коллекции документов для теста
            List<Document> documents = new();
            for (int i = 0; i < 65; i++)
            {
                Document d = new();
                documents.Add(d);
            }
            #endregion

            #region Запуск отправки документов
            // Для тестирования можно запустить в отдельном независимом потоке
            var service = kernel.Get<IDocumentsQueue>();
            foreach (var document in documents)
            {
                service.Enqueue(document);
            }
            #endregion

            Console.ReadKey();
        }
    }
}