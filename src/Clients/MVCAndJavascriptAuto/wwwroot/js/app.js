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
    var url = "/api/identity";

    var xhr = new XMLHttpRequest();
    xhr.open("GET", url);
    xhr.onload = function () {
        log(xhr.status, JSON.parse(xhr.responseText));
    }
    xhr.send();
}

function logout() {
    window.location = "/Authentication/Logout"
}

function load() {
    var url = "/identity"

    var xhr = new XMLHttpRequest();
    xhr.open("GET", url);
    xhr.onload = function () {
        var jsonData = JSON.parse(xhr.responseText);
        document.getElementById('results').innerHTML = "Bem vindo: " + JSON.stringify(jsonData, null, 2);
    }
    xhr.send();
}