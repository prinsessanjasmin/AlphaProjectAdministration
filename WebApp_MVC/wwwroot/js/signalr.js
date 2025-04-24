//SignalR
window.appConnections = {
    notification: null,
    chat: null
};

function initializeSignalR() {
    // First, check if SignalR is available
    if (typeof signalR === 'undefined') {
        console.warn('SignalR not available yet, will retry...');
        return false;
    }

    console.log('Initializing SignalR connections...');

    // Initialize notification hub
    if (!window.appConnections.notification) {
        window.appConnections.notification = new signalR.HubConnectionBuilder()
            .withUrl("/notificationhub")
            .withAutomaticReconnect()
            .build();

        window.appConnections.notification.on("AllReceiveNotification", handleNotification);
        window.appConnections.notification.on("AdminReceiveNotification", handleAdminNotification);
        window.appConnections.notification.on("NotificationDismissed", handleDismissNotification);
    }

    // Initialize chat hub
    if (!window.appConnections.chat) {
        window.appConnections.chat = new signalR.HubConnectionBuilder()
            .withUrl("/chathub")
            .withAutomaticReconnect()
            .build();

        window.appConnections.chat.on("ReceiveMessage", handleChatMessage());
    }

    return true;
}

async function startNotificationConnection() {
    if (!initializeSignalR()) {
        waitForSignalR(startNotificationConnection);
        return;
    }

    try {
        if (window.appConnections.notification.state !== signalR.HubConnectionState.Connected) {
            await window.appConnections.notification.start();
            console.log("Notification hub connected");
        }
    } catch (error) {
        console.error("Error connecting to notification hub:", error);
        setTimeout(startNotificationConnection, 5000);
    }
}

// Start chat connection
async function startChatConnection() {
    if (!initializeSignalR()) {
        waitForSignalR(startChatConnection);
        return;
    }

    try {
        if (window.appConnections.chat.state !== signalR.HubConnectionState.Connected) {
            await window.appConnections.chat.start();
            console.log("Chat hub connected");
        }
    } catch (error) {
        console.error("Error connecting to chat hub:", error);
        setTimeout(startChatConnection, 5000);
    }
}


function handleNotification(notification) {
    console.log("Received notification:", notification);

    const notifications = document.querySelector('.notifications');
    if (!notifications) {
        return;
    }
    const timeAgo = updateRelativeTimes();

    const item = document.createElement('div');
    item.className = 'notification-item';
    item.setAttribute('data-id', notification.id)
    item.innerHTML =
    `
        <img class="image" src="${notification.icon}" />
        <div class="vertical vertical-notification">
	        <div class="notification-message">${notification.message}</div>
	        <div class="time-stamp" data-created="${new Date(notification.created).toISOString()}">${no}</div>
        </div>
        <div class="btn-x" onclick="dismissNotification('${notification.id}')"></div>
    `;

    notifications.insertBefore(item, notifications.firstChild);
    
    updateNotificationCount();
}

function handleAdminNotification(notification) {
    console.log("Received admin notification:", notification);

    const notifications = document.querySelector('.notifications');
    if (!notifications) {
        return;
    }
    const item = document.createElement('div');
    item.className = 'notification-item';
    item.setAttribute('data-id', notification.id)
    item.innerHTML =
        `
        <img class="account-img" src="${notification.icon}" />
        <div class="vertical vertical-notification">
	        <div class="notification-message">${notification.message}</div>
	        <div class="time-stamp" data-created="${new Date(notification.created).toISOString()}">${notification.created}</div>
        </div>
        <div class="btn-x" onclick="dismissNotification('${notification.id}')"></div>
    `;

    notifications.insertBefore(item, notifications.firstChild);
    updateRelativeTimes();
    updateNotificationCount();
}

function handleDismissNotification(notification) {
    removeNotification(notification);
}

function handleChatMessage(userName, message) {
    console.log("Received chat message:", message);

    const chatContainer = document.querySelector('#chat-messages');
    if (!chatContainer) return;

    const div = document.createElement('div');
    div.innerHTML =
        `
        <div class="item">
            <div class="name">${userName}</div>
            <div class="chat-message"${message}</div>
        </div>
    `;

    chatContainer.appendChild(div);
}

function waitForSignalR(callback, attempts = 0) {
    if (typeof signalR !== 'undefined') {
        callback();
        return;
    }

    if (attempts < 20) {
        setTimeout(() => waitForSignalR(callback, attempts + 1), 300);
    } else {
        console.error("SignalR failed to load after multiple attempts");
    }
}

function waitForSignalR(callback, attempts = 0) {
    if (typeof signalR !== 'undefined') {
        callback();
        return;
    }

    if (attempts < 20) {
        setTimeout(() => waitForSignalR(callback, attempts + 1), 300);
    } else {
        console.error("SignalR failed to load after multiple attempts");
    }
}

window.dismissNotification = async function (notificationId) {
    try {
        const response = await fetch(`/api/notifications/dismiss/${notificationId}`, {
            method: 'POST'
        });

        if (response.ok) {
            removeNotification(notificationId);
        } else {
            console.error('Error dismissing notification:', await response.text());
        }
    } catch (error) {
        console.error('Error dismissing notification:', error);
    }
};


function removeNotification(notificationId) {
	const element = document.querySelector(`.notification-item[data-id="${notificationId}"]`);
    if (element)
    {
	    element.remove();
        updateNotificationCount();
	}
}

function updateNotificationCount() {
	const notifications = document.querySelector('.notifications');
    const notificationNumber = document.querySelector('.notification-number');
    const notificationDropdownButton = document.getElementById('notification-dropdown-button');

    if(!notifications) {
        return;
    }

    const count = notifications.querySelectorAll('.notification-item').length;

    if (notificationNumber) {
	    notificationNumber.textContent = count;
	}

    if (notificationDropdownButton) {
        let dot = notificationDropdownButton.querySelector('.dot.red-dot');

        if (count > 0 && !dot) {
            dot = document.createElement('div');
            dot.className = 'dot red-dot';
            notificationDropdownButton.appendChild(dot);
        }
        if (count === 0 && dot) {
            dot.remove();
        }
    }
}

function updateRelativeTimes() {
    const elements = document.querySelectorAll('.notification-item .time-stamp');
    const now = new Date();

    elements.forEach(el => {
        const created = new Date(el.getAttribute('data-created'));
        const diff = now - created;
        const diffSeconds = Math.floor(diff / 1000);
        const diffMinutes = Math.floor(diffSeconds / 60);
        const diffHours = Math.floor(diffMinutes / 60);
        const diffDays = Math.floor(diffHours / 24);
        const diffWeeks = Math.floor(diffDays / 7);

        let relativeTime = '';

        if (diffMinutes < 1) {
            relativeTime = '0 min ago';
        } else if (diffMinutes < 60) {
            relativeTime = diffMinutes + ' min ago';
        } else if (diffHours < 2) {
            relativeTime = diffHours + ' hour ago';
        } else if (diffHours < 24) {
            relativeTime = diffHours + ' hours ago';
        } else if (diffDays < 2) {
            relativeTime = diffDays + ' day ago';
        } else if (diffDays < 7) {
            relativeTime = diffDays + ' days ago';
        } else if (diffWeeks < 2) {
            relativeTime = diffWeeks + ' week ago';
        } else {
            relativeTime = diffWeeks + ' weeks ago';
        }

        el.textContent = relativeTime;
    });
}

function sendMessage() {
    const username = document.getElementById("username").value;
    const message = document.getElementById("message").value;

    window.appConnections.chat.invoke("SendMessage", username, message)
        .catch(error => console.error(error.toString()));
    document.getElementById("message").value = "";
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    // Start with a small delay to ensure the page is fully loaded
    setTimeout(function () {
        // Check if we need notification functionality
        if (document.querySelector('.notifications') || document.getElementById('notification-dropdown')) {
            startNotificationConnection();
        }

        // Check if we need chat functionality
        if (document.getElementById('chat-container') || document.getElementById('chat-messages')) {
            startChatConnection();
        }
    }, 500);
});