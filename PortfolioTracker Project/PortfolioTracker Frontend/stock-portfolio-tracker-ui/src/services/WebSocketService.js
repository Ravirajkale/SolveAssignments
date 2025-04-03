// websocketService.js
let socket = null;
let subscribers = [];

/**
 * Connects to the WebSocket server and listens for stock price updates.
 * @param {Function} onMessageCallback - Callback function to handle stock price updates.
 */
export const connectWebSocket = (onMessageCallback) => {
    if (socket) return; // Prevent multiple connections

    socket = new WebSocket("ws://localhost:5210/ws/stocks"); // Update with your WebSocket URL

    socket.onopen = () => {
        console.log("Connected to WebSocket");
    };

    socket.onmessage = (event) => {
        console.log("Received WebSocket message:", event.data);

        try {
            const message = JSON.parse(event.data);

            // Notify all subscribed components when a stock price update is received
            if (message.type === "price-updated") {
                subscribers.forEach((callback) => callback());
            }
        } catch (err) {
            console.error("Error processing WebSocket message:", err);
        }
    };

    socket.onerror = (error) => {
        console.error("WebSocket Error:", error);
    };

    socket.onclose = () => {
        console.log("WebSocket connection closed.");
        socket = null; // Reset socket to allow reconnection
    };
};

/**
 * Subscribes a component to WebSocket updates.
 * @param {Function} callback - The function to call when stock updates are received.
 */
export const subscribeToWebSocket = (callback) => {
    if (!subscribers.includes(callback)) {
        subscribers.push(callback);
    }
    connectWebSocket(); // Ensure WebSocket is connected
};

/**
 * Unsubscribes a component from WebSocket updates.
 * @param {Function} callback - The function to remove from the subscriber list.
 */
export const unsubscribeFromWebSocket = (callback) => {
    subscribers = subscribers.filter((sub) => sub !== callback);
};
