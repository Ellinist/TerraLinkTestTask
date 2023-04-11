using System.Reflection.Metadata;
using Ninject;
using TerraLinkTestTask.Infrastructure;
using TerraLinkTestTask.Interfaces;

namespace TerraLinkTestTask
{
    internal class Program
    {
        // Наверное из-за двух вещей ссылки не работали:
        // 1 - не было синглтона
        // 2 - не было статического (глобального) идентификатора сервиса
        public static IDocumentsQueue Service;

        static void Main(string[] args)
        {
            // Инициализация ядра DI-контейнера.
            IKernel kernel = new StandardKernel(new NinjectRegistration());
            Service = kernel.Get<IDocumentsQueue>();

            #region Формирование коллекции документов для теста
            List<Document> documents = new();
            for (int i = 0; i < 133; i++)
            {
                Document d = new();
                documents.Add(d);
            }
            #endregion

            #region Запуск отправки документов
            // Для тестирования можно запустить в отдельном независимом потоке
            Task.Run(() =>
            {
                foreach (var document in documents)
                {
                    Service.Enqueue(document);
                }
            });
            // И второй поток
            Task.Run(() =>
            {
                foreach (var document in documents)
                {
                    Service.Enqueue(document);
                }
            });
            #endregion

            Console.ReadKey();
        }
    }
}