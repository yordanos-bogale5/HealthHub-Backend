// signalr-client.js

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    console.log("SignalR Connected.");
}).catch(function (err) {
    console.error(err.toString());
});

function sendMessageToServer(user, message) {
    connection.invoke("SendMessage", user, message).catch(function (err) {
        console.error(err.toString());
    });

connection.on("ReceiveMessage", function (user, message) {
    console.log(user + " says: " + message);
});
}