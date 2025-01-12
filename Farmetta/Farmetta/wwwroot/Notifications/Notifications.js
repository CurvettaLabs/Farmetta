
async function SubscribeToNotifications(vapidPublicKey) {
    if (!'Notification' in window) {
        console.log('This browser does not support notifications.');
    }
    
    // Request permission from the user
    try{
        let permission = await Notification.requestPermission();
        if (permission === 'granted') {
            console.log('Notification permission granted.');
            // You can now show notifications
        } else if (permission === 'denied') {
            console.log('Notification permission denied.');
            return;
        } else {
            console.log('Notification permission dismissed.');
            return;
        }
    } catch(error) {
        console.error('Error requesting notification permission:', error);
    }

    
    
    console.log(vapidPublicKey);
    let registration = null;
    if ('serviceWorker' in navigator) {
        try {
            registration = await navigator.serviceWorker.register('Notifications/ServiceWorker.js');
            console.log('Service Worker registered:', registration);
            
        }catch(err) {
            console.error('Service Worker registration failed:', err);
        }
    }else{
        registration = await navigator.serviceWorker.ready
    }
    console.log(vapidPublicKey);

    let subscription = await registration.pushManager.getSubscription();
    if (subscription)
        return subscription;

    console.log(vapidPublicKey);
    
    subscription = await registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: vapidPublicKey
    })

    console.log(vapidPublicKey);
    return subscription;
}

function UnsubscribeToNotifications() {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.getRegistration("service-worker.js").then(registration => {
            registration.unregister();
        });
    }
}
