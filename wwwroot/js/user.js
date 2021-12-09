'use strict';

$(document).ready(function () {

    var selects = [$("#select1"), $("#select2")];

    var requestOptions = { method: 'GET' };

    fetch(`/api/hostlist`, requestOptions)
        .then(response => response.json())
        .then(result => {
            if (result.hasOwnProperty('result')) {
                selects.forEach(i => {
                    var opt = document.createElement('option');
                    opt.innerHTML = "Host list is empty";

                    i.append(opt);
                })
            }
            else {
                selects.forEach(i => {
                    result.forEach(r => {
                        var opt = document.createElement('option');
                        opt.selected = true;
                        opt.value = r.id;
                        opt.innerHTML = `${r.host} interval: ${r.interval}`;

                        i.append(opt);
                    })
                })
            }
        })
        .catch(error => window.location.reload());
});

$("#CheckHostBtn").on("click", function () {

    var host = $("#HostInput").val();

    var requestOptions = { method: 'GET' };

    fetch(`/api/host?host=${host}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            if (result.hasOwnProperty('result')) {
                alert(`${result.result}`);
            }
            else {
                alert(`Status: ${result.status}`);
            }
        })
        .catch(error => alert("error"));
});

$("#AddHostBtn").on("click", function () {
    var host = $("#HostInputAdd").val();
    var interval = $("#IntervalInputAdd").val();

    var requestOptions = { method: 'POST' };

    fetch(`/api/host?host=${host}&interval=${interval}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            if (result.hasOwnProperty('hostId')) {
                fetch(`/api/relation?siteId=${result.hostId}`, requestOptions)
                    .then(response => response.json())
                    .then(result => {
                        alert(`${result.result}`);
                        window.location.reload();
                    })
            }
            else {
                alert("error");
            }
        })
        .catch(error => alert("error"));
});

$("#RemoveHostBtn").on("click", function () {
    var host = $("#select2").val();

    var requestOptions = { method: 'DELETE' };

    fetch(`/api/relation?siteId=${host}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            alert(`${result.result}`);
            window.location.reload();
        })
        .catch(error => alert("error"));
})

$("#GetHistoryBtn").on("click", function () {
    var siteId = $("#select1").val();

    window.location = `/home/history?siteId=${siteId}`;
});

$("#ChangePasswordBtn").on("click", function () {
    var newPass = $("#PasswordInput").val();

    var requestOptions = { method: 'PUT' };

    fetch(`/api/user?password=${newPass}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            alert(`${result.result}`);
            window.location = "/home/logout";
        })
        .catch(error => alert("error"));

});