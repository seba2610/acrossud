function setRemoveButton(dropzone_obj, file) {
    var removeButton = Dropzone.createElement("<span class=\"btn btn-link btn-file-rem\">Eliminar</span>");

    var _this = dropzone_obj;

    removeButton.addEventListener("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        bootbox.confirm({
            title: "Eliminar imagen",
            message: "¿Está seguro que desea eliminar la imagen?",
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> Cancelar'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Eliminar'
                }
            },
            callback: function (result) {
                if (result) {
                    $.ajax({
                        method:"GET",
                        url: $("#delete_picture_url").html() + "&file_name=" + file.name,
                        success: function () {
                            _this.removeFile(file);
                        }
                    });
                }
            }
        });
    });

    file.previewElement.appendChild(removeButton);
}
$(document).ready(function () {
    //Control maindropzone
    if ($("#maindropzone") !== undefined) {
        //Control de contenedor de imágenes
        Dropzone.options.maindropzone = {
            init: function () {
                this.on("complete", function (file) {
                    setRemoveButton(this, file);
                });

                var dropzone_obj = this;
                if ($("#entity_picture_url") !== undefined) {
                    $.getJSON($("#entity_picture_url").html(), function (data) {
                        $.each(data, function (key, val) {
                            var file = { name: val.FileName, size: val.FileSize };
                            dropzone_obj.options.addedfile.call(dropzone_obj, file);
                            dropzone_obj.options.thumbnail.call(dropzone_obj, file, val.Path);
                            dropzone_obj.emit("complete", file);
                        });
                    });
                }
            },
            dictCancelUpload: "Cancelar",
            renameFilename: false
            }
        };

    //Controlbootbox
    if ($("#remove-entity") != undefined) {
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
    }

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