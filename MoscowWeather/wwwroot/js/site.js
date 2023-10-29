$(document).ready(() => {

    const reader = new FileReader();
    $(".clickCard").click(function () {
        var clickedDate = $(this).attr("data-date");
        $.ajax({
            url: "/Home/DetailedDescriptionDay",
            method: "POST",
            data: { date: clickedDate },
            dataType: 'html',
            success: function (result) {
                if ($("#table").length > 0) {
                    $("#table").remove();
                }
                $('#container').append(result);
            }
        });   
    });

    $("#archive").click(function () {
        if ($("#ModalArchive").length > 0) {
            $("#ModalArchive").remove();
            $("#ModalArchiveBackground").remove();
        }
        $.ajax({
            url: "/Home/WeatherArchive",
            method: "POST",
            dataType: 'html',
            success: function (result) {             
                $('#container').append(result);
            }
        });       
    });

    $("#Upload").click(function () {
        if ($("#ModalUploadArchive").length > 0) {
            $("#ModalUploadArchive").remove();
            $("#ModalUploadArchiveBackground").remove();
        }
        $.ajax({
            url: "/Home/UploadWeatherArchive",
            method: "POST",
            dataType: 'html',
            success: function (result) {
                $('#container').append(result);
            }
        });
    });

    $('.card').on('click', function () {
        $('.card').removeClass('selected');
        $(this).addClass('selected').css("display", "block");;
    });

});
