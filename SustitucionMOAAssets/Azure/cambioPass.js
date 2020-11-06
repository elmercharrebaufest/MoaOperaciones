$('document').ready(function () {
    $("#emailVerificationControl_success_message").html('Se ha enviado el código de verificación a su e-mail');

    //$("input:radio").css("width", "10%");
    //$("#newPassword_label").addClass("col-sm-6");
    //$("#newPassword_label").parent().addClass("row");

    //$("#email_label").addClass("col-sm-6");
    //$("#email_label").parent().addClass("row");

    //$("#reenterPassword_label").addClass("col-sm-6");
    //$("#reenterPassword_label").parent().addClass("row");

    //$("#extension_CUIT_label").addClass("col-sm-6");
    //$("#extension_CUIT_label").parent().addClass("row");

    //$("#emailVerificationCode_label").addClass("col-sm-6");
    //$("#emailVerificationCode_label").parent().addClass("row");

    $(".attrEntry.row > .error").remove();
    $(".attrEntry.row").append("<div class=\"error itemLevel\" role=\"alert\"></div>");

    $("#emailVerificationControl_but_send_code").html('Verificar e-mail');

    $("#emailVerificationControl_but_send_new_code").html('Enviar código nuevo');

    $("#emailVerificationControl_but_verify_code").html('Verificar código');

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

    $("#continue").prop("disabled", false);
    $('#emailVerificationControl_but_send_code').text("Enviar Código");
    $('#emailVerificationControl_but_verify_code').text("Verificar");
    $('#emailVerificationControl_but_send_new_code').text("Nuevo Código");
    $('#emailVerificationControl_but_change_claims').text("Cambiar e-mail");

    $('#emailVerificationControl_but_change_claims').click(function () {
        $('input#email').prop('disabled', false);
    })

    $('#emailVerificationControl_but_verify_code').click(function () {
        $('#continue').prop('disabled', false);
    })
});