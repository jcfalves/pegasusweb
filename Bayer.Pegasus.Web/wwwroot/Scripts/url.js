var baseUrl = "";


function ObeterURL() {

    baseUrl = $("#HiddenUrl").val();
    //baseUrl = baseUrl.substring(0, baseUrl.indexOf("Planejamento"));

}

$(function () {
    $(".linkVoltar")
    .button()
    .click(function (event) {
    });
});

$(function () {
    $("input[type=button], input[type=submit]")
    .button()
    .click(function (event) {
    });
});