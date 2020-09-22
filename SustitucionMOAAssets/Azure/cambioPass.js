$('document').ready(function () {
    $("#emailVerificationControl_success_message").html('Se ha enviado el código de verificación a su e-mail');

    $("input:radio").css("width", "10%");
    $("#newPassword_label").addClass("col-sm-6");
    $("#newPassword_label").parent().addClass("row");

    $("#email_label").addClass("col-sm-6");
    $("#email_label").parent().addClass("row");

    $("#reenterPassword_label").addClass("col-sm-6");
    $("#reenterPassword_label").parent().addClass("row");

    $("#extension_CUIT_label").addClass("col-sm-6");
    $("#extension_CUIT_label").parent().addClass("row");

    $("#emailVerificationCode_label").addClass("col-sm-6");
    $("#emailVerificationCode_label").parent().addClass("row");

    $(".attrEntry.row > .error").remove();
    $(".attrEntry.row").append("<div class=\"error itemLevel\" role=\"alert\"></div>");

    $("#continue").html('Registrarse');
    $("#emailVerificationControl_but_send_code").html('Verificar e-mail');

    $("#email_ver_but_resend").html('Enviar código nuevo');

    $("#email_ver_but_verify").html('Verificar código');

    $("#continue").removeAttr("disabled");
    $("label[for='email']").html('Ingrese su e-mail');
    $("#email").attr('placeholder', 'Ingrese su e-mail');

    $("#email").parent().find("[role='alert']").html('Introduzca una direccion de e-mail válida');

    var info = $(".verificationInfoText").html();
    var success = $(".verificationSuccessText").html();
    var error = $(".verificationErrorText").html();

    $(".verificationInfoText").remove();
    $(".verificationSuccessText").remove();
    $(".verificationErrorText").remove();
    $(".verificationControlContent > .buttons").prepend("<div class=\"verificationInfoText\" role=\"alert\">" + info + "</div>");
    $(".verificationControlContent > .buttons").prepend("<div class=\"verificationSuccessText\" role=\"alert\">" + success + "</div>");
    $(".verificationControlContent > .buttons").prepend("<div class=\"verificationErrorText error\" role=\"alert\">" + error + "</div>");

});