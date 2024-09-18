// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/* Expandable/Toggle Div */
function expandCollapseDiv() {

    if ($('#chkToggler').length) {
        $('#chkToggler').click(function () {
            $('#crossAvgDiv').slideToggle();
        });
    }
}

// Helper Method : string format function like c#
String.format = function () {
    if (arguments.length == 0)
        return null;
    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
};
