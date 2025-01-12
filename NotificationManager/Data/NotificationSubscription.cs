using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using WebPush;

namespace NotificationManager.Data;

public class NotificationSubscription : PushSubscription
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int DeviceId { get; init; }
    
    public NotificationSubscription() : base() { }
    

    public NotificationSubscription(int deviceId, string endpoint, string p256Dh, string auth)
    {
        DeviceId = deviceId;
        Endpoint = endpoint;
        P256DH = p256Dh;
        Auth = auth;
    }

    
}