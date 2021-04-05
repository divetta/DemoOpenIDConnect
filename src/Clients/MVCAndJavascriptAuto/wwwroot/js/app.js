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

document.getElementById("userinfo").addEventListener("click", userinfo, false);
document.getElementById("list").addEventListener("click", list, false);
document.getElementById("logout").addEventListener("click", logout, false);
load();

function list() {

    fetch('/api/todoitems')
        .then(response => {
            if (!response.ok) {
                throw new Error('HTTP - ' + response.status);
            }
            return response.json();
        })
        .then(myJson => {
            document.getElementById('results').innerHTML = JSON.stringify(myJson,null,2);
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}



function userinfo
    () {
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
    fetch('/identity')
        .then(response => {
            if (!response.ok) {
                throw new Error('HTTP - ' + response.status);
            }
            return response.json();
        })
        .then(myJson => {
            document.getElementById('results').innerHTML = "Welcome: " + JSON.stringify(myJson, null, 2);
        })
        .catch(error => {
            document.getElementById('results').innerHTML = error;
        });
}