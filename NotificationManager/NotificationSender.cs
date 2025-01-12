using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using NotificationManager.Data;
using WebPush;

namespace NotificationManager;

public class NotificationSender(NotificationDbContext notificationDbContext, IJSRuntime js)
{
    public async Task RequestNotificationPermission(int deviceId)
    {
        // Check if subscription for device exists
        var subscription = notificationDbContext.NotificationSubscriptions.FirstOrDefault(
            subscription => subscription.DeviceId == deviceId);
        if (subscription != null)
            return;
        
        // Fetch Vapid Public Key
        var vapidPublicKey = (await notificationDbContext.VapidDetails.FirstOrDefaultAsync())?.PublicKey;

        if (vapidPublicKey == null)
        {
            var vapidDetails = new Vapid(VapidHelper.GenerateVapidKeys());
            vapidDetails.Subject = "mailto:google@gmail.com";
            vapidPublicKey = vapidDetails.PublicKey;

            await notificationDbContext.VapidDetails.AddAsync(vapidDetails);
            await notificationDbContext.SaveChangesAsync();
        }

        // Get Subscription Details from browser
        var jsonSubscription = await js.InvokeAsync<JsonObject>("SubscribeToNotifications", vapidPublicKey);
        
        var endpoint = jsonSubscription["endpoint"]?.ToString();
        var p256dh = jsonSubscription["keys"]?["p256dh"]?.ToString();
        var auth = jsonSubscription["keys"]?["auth"]?.ToString();
        
        if (string.IsNullOrEmpty(endpoint))
            throw new ApplicationException("Missing endpoint in requestPermission");
        if (string.IsNullOrEmpty(p256dh))
            throw new ApplicationException("Missing p256dh in requestPermission");
        if (string.IsNullOrEmpty(auth))
            throw new ApplicationException("Missing auth in requestPermission");
        
        // Save Subscription Details to Database
        var notificationSubscription = new NotificationSubscription(deviceId, endpoint, p256dh, auth);
        await notificationDbContext.AddAsync(notificationSubscription);
        await notificationDbContext.SaveChangesAsync();
    }
    public async Task SendNotificationAll(string payload)
    {
        var webPushClient = GetWebPushClient();
        await foreach (var subscription in notificationDbContext.NotificationSubscriptions)
        {
            await webPushClient.SendNotificationAsync(subscription, payload);
        }
    }

    public async Task SendNotification(NotificationSubscription subscription)
    {
        var webPushClient = GetWebPushClient();
        
        await webPushClient.SendNotificationAsync(subscription);
    }
    
    private WebPushClient GetWebPushClient()
    {
        var webPushClient = new WebPushClient();
        var vapid = notificationDbContext.VapidDetails.FirstOrDefault();
        // if unable to find vapid
        if (vapid == null)
        {
            vapid = new Vapid(VapidHelper.GenerateVapidKeys());
            notificationDbContext.VapidDetails.Add(vapid);
        }
        
        webPushClient.SetVapidDetails(vapid);
        
        return webPushClient;
    }
}