$(document).ready(function () {

    $("#HomeContentFeed").on("click", ".comment-send-btn", function (e) { 

        var SendButton = $(e.target);

        var uniquePostIdentifier = SendButton.attr("unique-identifier");
        var commentTextbox = $("div[unique-comment-identifier$="+ uniquePostIdentifier +"]");

        var userId = RetrieveCurrentUserId();
        var fatherPostId = commentTextbox.attr("post-id");
        var postContent = $($("div[unique-comment-identifier$="+ uniquePostIdentifier +"]").summernote("code")).text()

        var newPostObject = {};
        newPostObject.Summary = postContent;
        newPostObject.UserId = userId;
        newPostObject.ParentPostId = fatherPostId;
        newPostObject.CreationDate = null;

        $.ajax({
            url: '/api/Posts',
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: JSON.stringify(newPostObject),
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(data) {
                if (data.success) {
                    ////Clear summernote textbox after successful submission.
                    $(commentTextbox).summernote('reset');
                    $("span[unique-error-identifier$="+ uniquePostIdentifier +"]").hide("slow");
                    console.log("Successfully submitted comment to DB.");
                }
            },
            error: function (data) {
                $("span[unique-error-identifier$="+ uniquePostIdentifier +"]").show("slow");
                console.log(data);
            }
        });
    });
});