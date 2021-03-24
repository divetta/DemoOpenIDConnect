function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);
load();

function api() {
    var url = "https://localhost:44302/api/identity";

    var xhr = new XMLHttpRequest();
    xhr.open("GET", url);
    xhr.onload = function () {
        log(xhr.status, JSON.parse(xhr.responseText));
    }
    xhr.send();
}

function logout() {

    if (sessionStorage.getItem("isAuthenticated") === "true") {
        window.location = "/Authentication/Logout"
    }
    sessionStorage.removeItem("isAuthenticated");
    sessionStorage.removeItem("authData");

}

function load() {

    var isAuthenticated = false;

    var url = "https://localhost:44302/identity/isauthenticated";
    var xhr = new XMLHttpRequest();
    xhr.open("GET", url);
    xhr.onload = function () {
        isAuthenticated = JSON.parse(xhr.responseText);
        sessionStorage.setItem("isAuthenticated", isAuthenticated);
        if (!isAuthenticated) {
            window.location = "/Authentication/Login"
        } else {
            loadAuthData();
        }
    }
    xhr.send();
}

function loadAuthData() {
    if (sessionStorage.getItem("isAuthenticated") === "true") {
        var url = "https://localhost:44302/identity"

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            var jsonData = JSON.parse(xhr.responseText);
            sessionStorage.setItem("authData", JSON.stringify(jsonData, null, 2));
            document.getElementById('results').innerHTML = "Bem vindo: " + JSON.stringify(jsonData, null, 2);
        }
        xhr.send();
    }
}