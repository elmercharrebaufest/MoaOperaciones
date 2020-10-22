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
});
