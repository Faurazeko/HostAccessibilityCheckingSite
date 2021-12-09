'use strict';

//User actions
$("#CheckUserBtn").on("click", function () {

    var str = $("#UserCheckInput").val();

    var requestOptions = { method: 'GET' };

    fetch(`/api/user?login=${str}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            if (result.hasOwnProperty('result')) {
                alert(`${result.result}`);
            }
            else {
                var strAlert = "";
                for (var item in result) {
                    strAlert += `${item} = ${result[item]}\n`;
                }
                alert(strAlert);
            }
        })
        .catch(error => alert("error"));
});

$("#CreateUserBtn").on("click", function () {
    var user = $("#CreateUserInputU").val();
    var pass = $("#CreateUserInputP").val();

    var requestOptions = { method: 'POST' };

    fetch(`/api/user?login=${user}&password=${pass}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            alert(`${result.result}`);
        })
        .catch(error => alert("error"));
})

$("#DeleteUserBtn").on("click", function () {
    var user = $("#DeleteUserInput");

    var requestOptions = { method: 'DELETE' };

    fetch(`/api/user?login=${user}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            alert(`${result.result}`);
        })
        .catch(error => alert("error"));
})

$("#UpdateUserBtn").on("click", function () {
    var user = $("#UpdateUserInputU").val();
    var newPass = $("#UpdateUserInputP").val();

    var requestOptions = { method: 'PUT' };

    fetch(`/api/user?login=${user}&password=${newPass}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            alert(`${result.result}`);
        })
        .catch(error => alert("error"));
})
//User actions end

//Relation actions
$("#AddRelationBtn").on("click", function () {

    var Id = $("#AddRelationInputI").val();
    var UserId = $("#AddRelationInputU").val();

    var requestOptions = { method: 'POST' };

    fetch(`/api/relation?userId=${Id}&siteId=${UserId}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            alert(`${result.result}`);
        })
        .catch(error => alert("error"));
})

$("#DeleteRelationBtn").on("click", function () {
    var Id = $("#DeleteRelationInputI").val();
    var UserId = $("#DeleteRelationInputU").val();

    var requestOptions = { method: 'DELETE' };

    fetch(`/api/relation?userId=${Id}&siteId=${UserId}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            alert(`${result.result}`);
        })
        .catch(error => alert("error"));
})
//Relation actions end