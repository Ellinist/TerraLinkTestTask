
namespace TerraLinkTestTask.Models
{
    /// <summary>
    ///  Вариант класса с учетом типа статуса операции
    /// </summary>
    public class SendStatus
    {
        public int DocumentsSent { get; set; }
        public TaskStatus DocumentSendStatus { get; set; }

        public SendStatus(int sent, TaskStatus status)
        {
            DocumentsSent = sent;
            DocumentSendStatus = status;
        }
    }
}
