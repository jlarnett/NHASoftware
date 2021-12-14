$(document).ready(function() {
    $("#Submit").click(function() {
        var author = {};
        author.name = $("#name").val();
        author.address = $("#address").val();
        author.id = $("#IdHolder").attr("value");

        $.ajax({
            url: '/api/Authors',
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: JSON.stringify(author),
            success: function(data) {
                if (data.success) {
                    window.location.href = "/Authors/Index";
                }
            }
        });

    });
});