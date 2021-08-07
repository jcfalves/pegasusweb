$(document).ready(function () {

    if (window.location.href.indexOf("login") == -1) {


        $("#username").html(localStorage.getItem('IV-USER-NAME'))

        $("#area").html(localStorage.getItem('IV-USER-DESCRIPTION'))


        if (localStorage.getItem('IV-USER') == null) {
            document.location.href = "login.html";
        }
        else {
            if (window.location.hash.length > 0) {
                loadIframe(window.location.hash.replace('#', '/'));
            } else {
                loadIframe('/Dashboard');
            }
        }

    }

    $('#menu .nav-item li, #menu .nav-item span').click(function (e) {
        if (typeof $(this).attr('href') != "undefined") {
            var link = $(this).attr('href');
        } else {
            if (typeof $(this).find('a').attr('href') == "undefined") {
                return;
            }
            var link = $(this).find('a').attr('href');
        }
        link = link.replace('#', '/');
        loadIframe(link);
        if ($(window).width() < 768) {
            $('#navigation').slideUp(400, function () {
                adjustSizes();
            });
        }
    });


    $('#menu div#navigation .nav-item.has-second-level').on('touchstart', function (e) {
        $(this).find('ul.nav-second-level').show();
    });
    
    $('#menu div#navigation .nav-item.has-second-level').hover(function () {
        $(this).find('ul.nav-second-level').show();
    }, function (e) {
        $(this).find('ul.nav-second-level').hide();
    });
    
    $('#mobile-button').click(function () {

        if ($('#navigation').is(":visible")) {
            $('#navigation').slideUp(400, function () {
                adjustSizes();
            });
        } else {
            $('#navigation').slideDown();
        }
    });


    $('#enviar-login').click(function (e) {
        e.preventDefault();

        var login = $('input[name="cwid"]').val().toUpperCase();
        var password = $('input[name="pass"]').val();        
        var ip = "";
        var cultureName = "";
        
        var domain = "http://localhost:7305";

        var url = domain + "/authentication/login";

        $.ajax({
            url: url,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ login, password, ip, cultureName }),
            success: function (response) {
                onLogin(response);
            },
            error: function (xhr, status, message) {
                console.log()
            }
        });        
    });


    $('#logout').click(function (e) {
        e.preventDefault();

        var login = localStorage.getItem('IV-USER');

        var domain = "http://localhost:7305";

        var url = domain + "/authentication/logout";

        $.ajax({
            url: url,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ login }),
            success: function (response) {
                onLogout(response);
            },
            error: function (xhr, status, message) {
                console.log()
            }
        });
    });

});

function onLogin(results) {

    if (results.result.code == "1") {
        
        var data = results.return;
        
        localStorage.setItem('IV-USER', $('input[name="cwid"]').val());
        localStorage.setItem('IV-USER-NAME', data.nameUser);
        localStorage.setItem('IV-USER-DESCRIPTION', data.systemName);

        var role = data.roles[0];
        var level = role.level;

        if (level.restrictionCodes == null) {
            var restrictionCode = "";
        } else {
            var restrictionCode = level.restrictionCodes[0];
        }

        localStorage.setItem('IV-USER-PROFILE', role.name);
        localStorage.setItem('IV-USER-AREA', restrictionCode);

        $.ajax({
            type: "GET",
            url: "/Dashboard",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("IV-USER", localStorage.getItem('IV-USER'));
            },
            success: function (data) {
                document.location.href = "index.html";
            }
        });

    }
    else {
        var code = results.result.code;
        var description = results.result.description;

        errorMessage = "Erro ao fazer login: code: " + code + " - description: " + description;
        console.log(errorMessage);
        alert("Erro ao fazer login: " + description);
    }

}


function onLogout(results) {

    var textoAVerificar = results.result.description;

    if (textoAVerificar.indexOf('|') > 0) {

        var aux = textoAVerificar.split('|');
        var hora = aux[1];
        var token = aux[2];
        alert(hora + ' - ' + token);
    }

    if (results.result.code == "8") {

        localStorage.removeItem('IV-USER');
        localStorage.removeItem('IV-USER-NAME');
        localStorage.removeItem('IV-USER-DESCRIPTION');
        localStorage.removeItem('IV-USER-PROFILE');
        localStorage.removeItem('IV-USER-AREA');
        localStorage.clear();

        document.location.href = "index.html";

    }
    else {
        var code = results.result.code;
        var description = results.result.description;

        errorMessage = "Erro ao fazer logout: code: " + code + " - description: " + description;
        console.log(errorMessage);
        alert("Erro ao fazer logout: " + description);
    }

}


function loadIframe(url) {

    var iframe = document.getElementById('content-pegasus');
    iframe.src = url;

    $('#menu ul.nav-second-level').hide();
    adjustSizes();
}

function adjustSizes() {
    $('#content-pegasus').parent().css({ 'padding-top': $('#menu').height(), 'height': 'calc(100% - ' + ($('#menu').height() - 5) + 'px)' });
}


