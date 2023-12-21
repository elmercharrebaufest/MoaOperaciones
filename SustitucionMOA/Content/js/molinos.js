// JavaScript Document
(function($) {
    "use strict"; // Start of use strict

    $('#mySidenav .nav li a').click(function () {
        closeNav()
    })

    $('#userSmall .nav li a').click(function () {
        userCloseSmall()
    })

    $('#coverAll').click(function() {
        $("#coverAll").fadeOut();
        notificationCloseSmall()
        closeNav()
        userCloseSmall()
    })

})(jQuery);

// /* --- Cambios con Incio --- */
// $(document).ready(function () {
//     $('#tableModal').DataTable({
//         responsive: {
//             details: {
//                 display: $.fn.dataTable.Responsive.display.modal({
//                     header: function (row) {
//                         var data = row.data();
//                         return 'DETALLES DEL ' + data[0];
//                     }
//                 }),
//                 renderer: function (api, rowIdx, columns) {
//                     var data = $.map(columns, function (col, i) {
//                         return '<tr>' +
//                         '<td>' + col.title + ':' + '</td> ' +
//                         '<td>' + col.data + '</td>' +
//                         '</tr>';
//                     }).join('');

//                     return $('<table/>').append(data);
//                 }
//             }
//         }
//     });
// });

var userOpenSmall = function() {
    notificationCloseSmall()
    closeNav()
    document.getElementById("userSmall").style.display = "block";
    document.getElementById("userCloseSmall").style.display = "block";
    document.getElementById("userOpenSmall").style.display = "none";
    $("#liUser").addClass("liSelectClass");
    document.getElementById("userSmall").style.right = "0";
    $("#coverAll").fadeIn();
}

var userCloseSmall = function() {
    document.getElementById("userCloseSmall").style.display = "none";
    document.getElementById("userOpenSmall").style.display = "block";
    $("#liUser").removeClass("liSelectClass");
    document.getElementById("userSmall").style.right = "-270px";
    $("#coverAll").fadeOut();
    document.getElementById("userSmall").style.display = "none";
}

function notificationOpenSmall() {
   
    closeNav()
    userCloseSmall()
    $("#liNoti").addClass("liSelectClass");
    document.getElementById("notificationSmall").style.display = "block";
    document.getElementById("notificationCloseSmall").style.display = "block";
    document.getElementById("notificationOpenSmall").style.display = "none";
    document.getElementById("notificationSmall").style.right = "0";
        $("#coverAll").fadeIn();
   
}

function notificationCloseSmall() {
   
    $("#liNoti").removeClass("liSelectClass");
    document.getElementById("notificationCloseSmall").style.display = "none";
    document.getElementById("notificationOpenSmall").style.display = "block";
    document.getElementById("notificationSmall").style.right = "-270px";
    $("#notificationSmall").css({ "right": "0" });
    $("#coverAll").fadeOut();
        document.getElementById("notificationSmall").style.display = "none";
    
}

function openNav() {
    userCloseSmall()
    notificationCloseSmall()
    document.getElementById("myMenuOpen").style.display = "none";
    document.getElementById("myMenuClose").style.display = "block";
    document.getElementById("mySidenav").style.right = "0";
    $("#coverAll").fadeIn();
}

function closeNav() {
    var mySidenav = document.getElementById("mySidenav");
    if (mySidenav) {
        mySidenav.style.right = "-270px";
    } else {
        console.log("El elemento con ID 'mySidenav' no se encontró en el DOM.");
    }
    // document.getElementById("mySidenav").style.right = "-270px";
    document.getElementById("myMenuClose").style.display = "none";
    document.getElementById("myMenuOpen").style.display = "block";
    $("#coverAll").fadeOut();
}

function notificationOpen() {
   
    $("#notificationSmall, #notificationClose").css({"display" : "block"});
    $("#notificationOpen").css({ "display" : "none" });
    $("#notificationSmall").css({ "right" : "0" });
     $("#coverAll").fadeIn();
   
}

function notificationClose() {
  
        $("#notificationClose, #notificationSmall").css({ "display": "none" });
        $("#notificationOpen").css({ "display": "block" });
        $("#notificationSmall").css({ "right": "-270px" });
        $("#coverAll").fadeOut();
     
}

function backHome() {
    notificationCloseSmall()
    closeNav()
    userCloseSmall()
    notificationClose()
}

function openSearch() {
    $("#showSearch").removeClass("showBtnFiltros").addClass("hideBtnFiltros");
    $("#hideSearch").removeClass("hideBtnFiltros").addClass("showBtnFiltros");
    $("#searchProducto").removeClass("hideFiltror").addClass("showFiltros");
    $("#searchMostrar").removeClass("hideFiltror").addClass("showFiltros");
    $("#searchProforma").removeClass("hideFiltror").addClass("showFiltros");
    $("#searchContrato").removeClass("hideFiltror").addClass("showFiltros");
    $("#searchCCPP").removeClass("hideFiltror").addClass("showFiltros");
    $("#searchNroLegal").removeClass("hideFiltror").addClass("showFiltros");
    $("#searchVendedor").removeClass("hideFiltror").addClass("showFiltros");
    $("#searchID").removeClass("hideFiltror").addClass("showFiltros");
}
function closeSearch() {
    $("#showSearch").removeClass("hideBtnFiltros").addClass("showBtnFiltros");
    $("#hideSearch").removeClass("showBtnFiltros").addClass("hideBtnFiltros");
    $("#searchProducto").removeClass("showFiltros").addClass("hideFiltror");
    $("#searchMostrar").removeClass("showFiltros").addClass("hideFiltror");
    $("#searchProforma").removeClass("showFiltros").addClass("hideFiltror");
    $("#searchContrato").removeClass("showFiltros").addClass("hideFiltror");
    $("#searchCCPP").removeClass("showFiltros").addClass("hideFiltror");
    $("#searchNroLegal").removeClass("showFiltros").addClass("hideFiltror");
    $("#searchVendedor").removeClass("showFiltros").addClass("hideFiltror");
    $("#searchID").removeClass("showFiltros").addClass("hideFiltror");
}

//Begin Corregidos
function showCaracteristicas() {
    $("#showCaracteristicas").removeClass("showBtn").addClass("hideBtn");
    $("#hideCaracteristicas").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#caracteristicas").removeClass("hideMobile");
}
function closeCaracteristicas() {
    $("#showCaracteristicas").removeClass("hideBtn").addClass("showBtn");
    $("#hideCaracteristicas").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#caracteristicas").addClass("hideMobile");
}

function showCondiciones() {
    $("#showCondiciones").removeClass("showBtn").addClass("hideBtn");
    $("#hideCondiciones").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#condiciones").removeClass("hideMobile");
}
function closeCondiciones() {
    $("#showCondiciones").removeClass("hideBtn").addClass("showBtn");
    $("#hideCondiciones").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#condiciones").addClass("hideMobile");
}

function showBonificaciones() {
    $("#showBonificaciones").removeClass("showBtn").addClass("hideBtn");
    $("#hideBonificaciones").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#bonificaciones").removeClass("hideMobile");
}
function closeBonificaciones() {
    $("#showBonificaciones").removeClass("hideBtn").addClass("showBtn");
    $("#hideBonificaciones").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#bonificaciones").addClass("hideMobile");
}

function showPagos() {
    $("#showPagos").removeClass("showBtn").addClass("hideBtn");
    $("#hidePagos").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myPagos").removeClass("hideMobile");
}
function closePagos() {
    $("#showPagos").removeClass("hideBtn").addClass("showBtn");
    $("#hidePagos").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myPagos").addClass("hideMobile");
}

function showLiquidaciones() {
    $("#showLiquidaciones").removeClass("showBtn").addClass("hideBtn");
    $("#hideLiquidaciones").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myLiquidaciones").removeClass("hideMobile");
}
function closeLiquidaciones() {
    $("#showLiquidaciones").removeClass("hideBtn").addClass("showBtn");
    $("#hideLiquidaciones").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myLiquidaciones").addClass("hideMobile");
}

function showFijaciones() {
    $("#showFijaciones").removeClass("showBtn").addClass("hideBtn");
    $("#hideFijaciones").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myFijaciones").removeClass("hideMobile");
}
function closeFijaciones() {
    $("#showFijaciones").removeClass("hideBtn").addClass("showBtn");
    $("#hideFijaciones").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myFijaciones").addClass("hideMobile");
}

function showAmpliaciones() {
    $("#showAmpliaciones").removeClass("showBtn").addClass("hideBtn");
    $("#hideAmpliaciones").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myAmpliaciones").removeClass("hideMobile");
}
function closeAmpliaciones() {
    $("#showAmpliaciones").removeClass("hideBtn").addClass("showBtn");
    $("#hideAmpliaciones").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myAmpliaciones").addClass("hideMobile");
}

function showHijos() {
    $("#showHijos").removeClass("showBtn").addClass("hideBtn");
    $("#hideHijos").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myHijos").removeClass("hideMobile");
}
function closeHijos() {
    $("#showHijos").removeClass("hideBtn").addClass("showBtn");
    $("#hideHijos").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myHijos").addClass("hideMobile");
}

function showAplicaciones() {
    $("#showAplicaciones").removeClass("showBtn").addClass("hideBtn");
    $("#hideAplicaciones").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myAplicaciones").removeClass("hideMobile");
}
function closeAplicaciones() {
    $("#showAplicaciones").removeClass("hideBtn").addClass("showBtn");
    $("#hideAplicaciones").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myAplicaciones").addClass("hideMobile");
}

function showDescarga() {
    $("#showDescarga").removeClass("showBtn").addClass("hideBtn");
    $("#hideDescarga").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myDescarga").removeClass("hideMobile");
}
function closeDescarga() {
    $("#showDescarga").removeClass("hideBtn").addClass("showBtn");
    $("#hideDescarga").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myDescarga").addClass("hideMobile");
}

function showDatos() {
    $("#showDatos").removeClass("showBtn").addClass("hideBtn");
    $("#hideDatos").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#Datos").removeClass("hideMobile");
}
function closeDatos() {
    $("#showDatos").removeClass("hideBtn").addClass("showBtn");
    $("#hideDatos").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#Datos").addClass("hideMobile");
}

function showExenciones() {
    $("#showExenciones").removeClass("showBtn").addClass("hideBtn");
    $("#hideExenciones").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myExenciones").removeClass("hideMobile");
}

function closeExenciones() {
    $("#showExenciones").removeClass("hideBtn").addClass("showBtn");
    $("#hideExenciones").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myExenciones").addClass("hideMobile");
}

function showCuentasHabilitadas() {
    $("#showCuentasHabilitadas").removeClass("showBtn").addClass("hideBtn");
    $("#hideCuentasHabilitadas").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myCuentasHabilitadas").removeClass("hideMobile");
}

function closeCuentasHabilitadas() {
    $("#showCuentasHabilitadas").removeClass("hideBtn").addClass("showBtn");
    $("#hideCuentasHabilitadas").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myCuentasHabilitadas").addClass("hideMobile");
}

function showConveniosMultilaterales() {
    $("#showConveniosMultilaterales").removeClass("showBtn").addClass("hideBtn");
    $("#hideConveniosMultilaterales").removeClass("hideBtn").addClass("showBtn btnInfoACTIVE");
    $("#myConveniosMultilaterales").removeClass("hideMobile");
}

function closeConveniosMultilaterales() {
    $("#showConveniosMultilaterales").removeClass("hideBtn").addClass("showBtn");
    $("#hideConveniosMultilaterales").removeClass("showBtn btnInfoACTIVE").addClass("hideBtn");
    $("#myConveniosMultilaterales").addClass("hideMobile");
}

//End Corregidos

$('.form_datetime').datetimepicker({
    language: 'es',
    weekStart: 1,
    todayBtn: 1,
    autoclose: 1,
    todayHighlight: 1,
    startView: 2,
    forceParse: 0,
    showMeridian: 1
});
$('.form_date').datetimepicker({
    language: 'es',
    weekStart: 1,
    todayBtn: 1,
    autoclose: 1,
    todayHighlight: 1,
    startView: 2,
    minView: 2,
    forceParse: 0
});
$('.form_time').datetimepicker({
    language: 'es',
    weekStart: 1,
    todayBtn: 1,
    autoclose: 1,
    todayHighlight: 1,
    startView: 1,
    minView: 0,
    maxView: 1,
    forceParse: 0
});

/* --- Fin Cambios con Incio --- */

/*----- Init Fletes ------*/

function openDetalleFletes(elem) {
    
    $(elem).css("display", "none");
    $(elem).parent().find("#tablaFlete").css("display", "block");
    $(elem).parent().find("#extraDataFlete").css("display", "block");
    $(elem).parent().find("#closeDetalleFletes").css("display", "block");
    $(elem).parent().find("#validarFletes").css("display", "block");
    $(elem).parent().find("#descargarFletes").removeClass("col-sm-offset-6");
    $(elem).parent().find("#modificarFletes").css("display", "block");
    return false;
}
function closeDetalleFletes(elem) {
    $(elem).css("display", "none");
    $(elem).parent().find("#tablaFlete").css("display", "none");
    $(elem).parent().find("#extraDataFlete").css("display", "none");
    $(elem).parent().find("#validarFletes").css("display", "none");
    $(elem).parent().find("#modificarFletes").css("display", "none");
    $(elem).parent().find("#descargarFletes").addClass("col-sm-offset-6");
    $(elem).parent().find("#openDetalleFletes").css("display", "block");
    return false;
}

/*----- End Fletes ------*/

$(window).resize(function () {
    if (window.screen.width >= 990) {
        closeNav();
        notificationClose();
    } else {
        notificationCloseSmall();
    }
});

$(document).keydown(function (e) {
    if (e.which == 8 && (document.activeElement.id == 'noCursor')) {
        e.preventDefault();
        return false;
    }
});