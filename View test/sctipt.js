window.onload = function () {

    let target = document.getElementById("display");
    //dsiplayTable(target, 6);
}

function dsiplayTable(div, rowCount) {
    let tableHeaders = ["Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek"]
    let width = tableHeaders.length;

    let table = document.createElement("table");
    table.classList.add("planTable");

    table.appendChild(createHeader(tableHeaders));
    for (let i = 0; i < rowCount; i++) {
        table.appendChild(crateRow(width))
    }

   
    div.appendChild(table);
}

function createHeader(headers) {
    let tr = document.createElement("tr");
    
    for (const header of headers) {
        let text = document.createTextNode(header);
        let th = document.createElement("th");
        th.appendChild(text);
        
        tr.appendChild(th);
    }

    return tr;
}

function crateRow(width) {
    let tr = document.createElement("tr");

    for (let i = 0; i < width; i++) {
        let td = document.createElement("td");
        td.classList.add("empty");

        tr.appendChild(td);
    }

    return tr;
}