$(document).ready(function() {
    $("#Submit").click(function() {
        var authorDto = {};
        authorDto.name = $("#Name").val();
        authorDto.address = $("#Address").val();

        $.ajax({
            url: '/api/Authors',
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: JSON.stringify(authorDto),
            success: function(data) {
                if (data.success) {
                    window.location.href = "/Authors/Index";
                }
            }
        });

    });
});