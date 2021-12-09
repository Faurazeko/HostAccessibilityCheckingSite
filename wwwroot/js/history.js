'use strict';

$(document).ready(function () {

    var url = new URL(window.location.href);

    var requestOptions = { method: 'GET' };

    fetch(`/api/selectHistory?SiteId=${url.searchParams.get("siteId")}&StartDateTime=${url.searchParams.get("StartDateTime")}&EndDateTime=${url.searchParams.get("EndDateTime")}`, requestOptions)
        .then(response => response.json())
        .then(result => {
            if (result.hasOwnProperty('result')) {
                alert(`${result.result}`);
            }
            else {
                $("#Header").text(`Viewing history for ${result.site.host} (interval = ${result.site.interval})`);

                var resultContainer = $("#Results");

                result.notes.forEach(i => {
                    var element = document.createElement("div");
                    element.className = "container bg-dark text-white p-3 my-3";

                    var timeElem = document.createElement("h3");
                    timeElem.innerHTML = `Time: ${i.time}`;
                    var statusElem = document.createElement("h3");
                    statusElem.innerHTML = `Status: ${i.status}`;

                    element.appendChild(timeElem);
                    element.appendChild(statusElem);
                    resultContainer.append(element);
                });
            }
        })
        .catch(error => { alert(`error`); history.back();});
});