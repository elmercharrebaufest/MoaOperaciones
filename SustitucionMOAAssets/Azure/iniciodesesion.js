$('document').ready(function () {
    $("label[for='email']").html('E-mail')

    $('#email').attr('placeholder', 'Ingrese su e-mail')

    $('.intro').html(
        'Información de operaciones y entregas para proveedores de Materias Primas'
    )

    //Muevo el tag <a> a la sección de botones
    var currentLink = $('#forgotPassword')
    var newLink = $('<a>', {
        href: currentLink.attr('href'),
        text: currentLink.text(),
    })

    $('.buttons').append(newLink)

    //Limpio cosas que no quiero y dejo solo el botón de registro
    var signUpLink = $('.create > p > a')
    $('.create > p').remove()
    $('.create').prepend(signUpLink)
    $('.create > a').text('Crear nuevo usuario')

    var ticketPesadaUrl = '';
    var qrCamionesUrl = '';

    var currentURL = window.location.href
    if (currentURL.includes('moagro.b2clogin.com')) {
        ticketPesadaUrl = 'https://moaoperaciones.com.ar/web/ticket-pesada';
        qrCamionesUrl = 'https://moaoperaciones.com.ar/QRCamiones';
    } else if (currentURL.includes('moagroqa.b2clogin.com')) {
        ticketPesadaUrl = 'http://qacompras.moaoperaciones.com.ar/web/ticket-pesada';
        qrCamionesUrl = 'http://qacompras.moaoperaciones.com.ar/QRCamiones';
    } else {
        ticketPesadaUrl = 'http://localhost:4200/ticket-pesada';
        qrCamionesUrl = 'http://localhost:4200/';
    }

    var customStyles =
        '<style>' +
        '.custom-buttons-container { display: flex; justify-content: space-between; gap: 15px; margin-top: 30px; width: 100%; box-sizing: border-box; }' +

        '.blue-icon-btn { background-color: #061C81; color: white !important; border-radius: 5px; width: 100%; min-height: 70px; text-decoration: none !important; border: none; font-size: 1rem !important; display: flex; flex-direction: column; align-items: center; justify-content: center; text-align: center; padding: 10px; transition: background-color 0.3s ease; line-height: 1.2; cursor: pointer; box-shadow: 0 2px 4px rgba(0,0,0,0.2); }' +

        '.blue-icon-btn:hover { background-color: #061763; color: #fff; }' +

        '.blue-icon-btn img { width: 35px; height: 35px; margin-bottom: 8px; filter: brightness(0) invert(1); }' +

        '@media (max-width: 480px) { .custom-buttons-container { flex-direction: column; } .blue-icon-btn { width: 100%; margin-bottom: 0; } }' +
        '</style>';

    var buttonsHtml =
        '<div class="custom-buttons-container">' +
        '<a href="' + ticketPesadaUrl + '" class="blue-icon-btn">' +
        '<img src="https://b2cmoagro.blob.core.windows.net/moaoperaciones/iniciodesesion_comprobantes_del_transporte_icon.svg" alt="Comprobantes">' +
        '<span>Comprobantes del<br>transporte</span>' +
        '</a>' +

        '<a href="' + qrCamionesUrl + '" class="blue-icon-btn">' +
        '<img src="https://b2cmoagro.blob.core.windows.net/moaoperaciones/iniciodesesion_seguimiento_en_planta_icon.svg" alt="Seguimiento">' +
        '<span>Seguimiento en<br>planta</span>' +
        '</a>' +
        '</div>';

    $('.create').after(customStyles + buttonsHtml);
})