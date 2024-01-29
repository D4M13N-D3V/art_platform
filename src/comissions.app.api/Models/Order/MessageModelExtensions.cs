using ArtPlatform.Database.Entities;
using comissions.app.database.Entities;

namespace comissions.app.api.Models.Order;

public static class MessageModelExtensions
{
    
    public static MessageModel ToModel(this SellerServiceOrderMessage sellerProfile)
    {
        return new MessageModel()
        {
            Id = sellerProfile.Id,
            SenderId = sellerProfile.SenderId,
            SenderDisplayName = sellerProfile.Sender.DisplayName,
            Message = sellerProfile.Message,
            Attachments = sellerProfile.Attachments.Select(x=>x.Id).ToArray()
        };
    }
}