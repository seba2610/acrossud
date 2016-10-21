$(document).ready(function () {

    //Control maindropzone
    if ($("#maindropzone") !== undefined) {
        //Control de contenedor de imágenes
        Dropzone.options.maindropzone = {
            init: function () {
                this.on("complete", function (data) {
                    //var res = eval('(' + data.xhr.responseText + ')');
                    var res = JSON.parse(data.xhr.responseText);
                });
            },
            addRemoveLinks: true,
            dictRemoveFile: "Eliminar",
            dictCancelUpload: "Cancelar",
            renameFilename: false
        };
    }

    //Controlbootbox
    $("#remove-entity").on("click", function (event) {
        event.preventDefault();
        bootbox.confirm({
            title: "Eliminar propiedad",
            message: "¿Está seguro que desea eliminar la propiedad?",
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> Cancelar'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Eliminar'
                }
            },
            callback: function (result) {
                if (result)
                    window.location.href = $("#remove-entity").attr("href");
            }
        });
    });

    //Control bootbox para mostrar aviso
    $.urlParam = function (name) {
        var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
        return results[1] || 0;
    }

    if ($.urlParam('alert') !== null) {
        var alert = bootbox.alert({
            title: decodeURI($.urlParam('alert_title')),
            message: '<p><i class="fa fa-spin fa-spinner"></i>' + decodeURI($.urlParam('alert')) + '</p>'
        });
        alert.init(function () {
            setTimeout(function () {
                dialog.find('.bootbox-body').html();
            }, 3000);
        });
    }
});