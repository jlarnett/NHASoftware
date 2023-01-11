$(document).ready(function () {

    $("#SubmitBtn").click(function(e) {
        var SendButton = $(e.target);
        var userId = RetrieveCurrentUserId();

        var postContent = $($("#MainPostTextbox").summernote("code")).text()

        var newPostObject = {};
        newPostObject.Summary = postContent;
        newPostObject.UserId = userId;
        newPostObject.CreationDate = null;
        newPostObject.ParentPostId = null;

        $.ajax({
            url: '/api/Posts',
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: JSON.stringify(newPostObject),
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(data) {
                if (data.success) {
                    //Clear summernote textbox after successful submission.
                    $("#MainPostTextbox").summernote('reset');
                    console.log("Successfully submitted post to DB.");
                    $("#MainPostTextboxValidationMessage").hide("slow");
                }
            },
            error: function (data) {
                $("#MainPostTextboxValidationMessage").show("slow");
            }
            
        });
    });
});
