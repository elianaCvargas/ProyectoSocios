
$(document).ready(function () {

});

function procesarArchivo() {
	var splitPath = $("#file-selector").val().split("\\");
	$("#file-title").text(splitPath[splitPath.length - 1]);
}

function mostrarNav() {
	$(".isFilled").toggleClass("hayDatosClass");
}

function VerificarSpan() {
	var nameFile = $("#file-title").text();
	if (nameFile != null || nameFile != undefined || nameFile != "") {
		$("#btnImportar").prop('disabled', true);
	}
	else {
		$("#btnImportar").prop('disabled', false);
	}
}

