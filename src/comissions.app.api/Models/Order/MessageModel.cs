namespace comissions.app.api.Models.Order;

public class MessageModel
{
    public int Id { get; set; }
    public string SenderId { get; set; }
    public string SenderDisplayName { get; set; }
    public string Message { get; set; }
    public int[] Attachments { get; set; }
}