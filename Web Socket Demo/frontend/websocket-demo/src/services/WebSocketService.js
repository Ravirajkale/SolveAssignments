const WEBSOCKET_URL = "ws://localhost:5282/ws/members";

class WebSocketService {
    constructor() {
        this.socket = null;
        this.listeners = new Set(); // Set to store unique listeners
    }

    connect() {
        if (this.socket && this.socket.readyState === WebSocket.OPEN) {
            return; // Prevent multiple WebSocket instances
        }

        this.socket = new WebSocket(WEBSOCKET_URL);

        this.socket.onopen = () => {
            console.log("Connected to WebSocket server.");
        };

        this.socket.onmessage = (event) => {
            const data = JSON.parse(event.data);
            this.listeners.forEach((listener) => listener(data));
        };

        this.socket.onerror = (error) => {
            console.error("WebSocket error:", error);
        };

        this.socket.onclose = () => {
            console.log("WebSocket disconnected. Attempting to reconnect...");
            setTimeout(() => this.connect(), 5000);
        };
    }

    addListener(callback) {
        this.listeners.add(callback); //  Set to store listeners uniquely
    }

    removeListener(callback) {
        this.listeners.delete(callback); // remove the listener
    }
}

const webSocketService = new WebSocketService();
export default webSocketService;