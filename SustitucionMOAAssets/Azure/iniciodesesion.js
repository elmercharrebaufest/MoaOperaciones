$("document").ready(function () {
    $("label[for='email']").html("Ingrese su e-mail");

    $("#email").attr("placeholder", "Ingrese su e-mail");

    $(".intro").html(
        "Información de operaciones y entregas para proveedores de Materias Primas"
    );

    //Muevo el tag <a> a la sección de botones
    var currentLink = $("#forgotPassword");
    var newLink = $('<a>', { 'href': currentLink.attr("href"), 'text': currentLink.text() });

    $(".buttons").prepend(newLink);

    //Limpio cosas que no quiero y dejo solo el botón de registro
    var signUpLink = $(".create > p > a");
    $(".create > p").remove();
    $(".create").prepend(signUpLink);
    $(".create > a").text("nuevo usuario");
    
    var ticketPesadaUrl = ""

    var currentURL = window.location.href;
    if (currentURL.includes("moagro.b2clogin.com"))
    {
        ticketPesadaUrl = "https://moaoperaciones.com.ar/web/ticket-pesada"
    } else if (currentURL.includes("moagroqa.b2clogin.com"))
    { 
        ticketPesadaUrl = "http://moaoperacionesqa.molinosagro.com.ar/web/ticket-pesada"
    }
    else {
        ticketPesadaUrl = "http://localhost:4200/ticket-pesada"
    }

    $(".create").after('<div class="create"><a style="color: white; border-radius: 2px; padding: 5px; width: 145px;  height: 55px; margin: 0px 0; text-decoration: none; text-transform: uppercase;  border: none; font-size: 14px;background: #959595 !important;text-align: center;padding-top: 8px !important;" href="' + ticketPesadaUrl + '">Comprobantes del transporte</a></div>')

});
