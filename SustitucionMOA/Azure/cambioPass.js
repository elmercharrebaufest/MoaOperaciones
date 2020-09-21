$("document").ready(function () {
  $("#emailVerificationControl_success_message").html(
    "Se ha enviado el código de verificación a su e-mail"
  );

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
  $(".attrEntry.row").append(
    '<div class="error itemLevel" role="alert"></div>'
  );

  $("#continue").html("Registrarse");
  $("#emailVerificationControl_but_send_code").html("Verificar e-mail");

  $("#email_ver_but_resend").html("Enviar código nuevo");

  $("#email_ver_but_verify").html("Verificar código");

  $("#continue").removeAttr("disabled");
  $("label[for='email']").html("Ingrese su e-mail");
  $("#email").attr("placeholder", "Ingrese su e-mail");

  $("#email")
    .parent()
    .find("[role='alert']")
    .html("Introduzca una direccion de e-mail válida");

  var info = $(".verificationInfoText").html();
  var success = $(".verificationSuccessText").html();
  var error = $(".verificationErrorText").html();

  $(".verificationInfoText").remove();
  $(".verificationSuccessText").remove();
  $(".verificationErrorText").remove();
  $(".verificationControlContent > .buttons").prepend(
    '<div class="verificationInfoText" role="alert">' + info + "</div>"
  );
  $(".verificationControlContent > .buttons").prepend(
    '<div class="verificationSuccessText" role="alert">' + success + "</div>"
  );
  $(".verificationControlContent > .buttons").prepend(
    '<div class="verificationErrorText error" role="alert">' + error + "</div>"
  );
  $("#extension_CUIT").mask("99-99999999-9");

  $("#extension_CUIT").change(function () {
    cuit = $("#extension_CUIT").val().toString().replace(/[-_]/g, "");
    if (cuit.length == 11) {
      var acumulado = 0;
      var digitos = cuit.split("");
      var digito = digitos.pop();

      for (var i = 0; i < digitos.length; i++) {
        acumulado += digitos[9 - i] * (2 + (i % 6));
      }

      var verif = 11 - (acumulado % 11);
      if (verif == 11) {
        verif = 0;
      } else if (verif == 10) {
        verif = 9;
      }
      if (digito != verif) {
        $("#extension_CUIT")
          .parent()
          .children(".error")
          .html("Ingrese un CUIT válido");
        $("#extension_CUIT").parent().children(".error").css("display", "");
      } else {
        $("#extension_CUIT").parent().children(".error").css("display", "none");
        $("#extension_CUIT")
          .parent()
          .children(".error")
          .html("Esta información es obligatoria.");
      }
    } else {
      if (
        $("#extension_CUIT").parent().children(".error").html() ==
        "Ingrese un CUIT válido"
      ) {
        $("#extension_CUIT").parent().children(".error").css("display", "none");
        $("#extension_CUIT")
          .parent()
          .children(".error")
          .html("Esta información es obligatoria.");
      }
    }
    return true;
  });
  /*
$(#newPassword).addClass('input-error');
$(#email).addClass('input-error');
$(#emailVerificationCode).addClass('input-error');
$(#reenterPassword).addClass('input-error');
$(#extension_CUIT ).addClass('input-error');
*/
});
