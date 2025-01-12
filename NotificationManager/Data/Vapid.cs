using System.ComponentModel.DataAnnotations.Schema;
using WebPush;

namespace NotificationManager.Data;

public class Vapid : VapidDetails
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Vapid() : base() { }

    public Vapid(VapidDetails details)
    {
        PrivateKey = details.PrivateKey;
        PublicKey = details.PublicKey;
        Expiration = details.Expiration;
        Subject = details.Subject;
    }
}