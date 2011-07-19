/// <reference path="jquery-1.4.4-vsdoc.js" />

var Therefore = { rootUrl = "/" };

Therefore.parse = function (statement, callback) {
    $.get(this.rootUrl + "Game/Parse", { statement: statement }, callback, "json");
}
