
$(document).ready(function () {
	//$("#btnImportar").click(
	//	mostrarNav()
	//);
});

function procesarArchivo() {
	var splitPath = $("#file-selector").val().split("\\");
	$("#file-title").text(splitPath[splitPath.length - 1]);
}

function mostrarNav() {
	$(".isFilled").toggleClass("hayDatosClass");
}