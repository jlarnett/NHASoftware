﻿$(window).on("load", function () {

    $('#ContentFeed').on('click', '.delete-post-link' ,function (e) {

        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var isComment = EventBtn.attr("is-comment");
        let uuid = EventBtn.attr("uuid");

        ContentFeedAjaxCalls.DeletePost(postId).then(function (response) {
            if (response.success === true) {
                if (isComment === "True") {
                    ContentFeedUtility.RemoveCommentFromContentFeedUI(postId);
                    ContentFeedUtility.DecrementPostCommentCounter(uuid);
                }
                else {
                    ContentFeedUtility.RemovePostFromContentFeedUI(postId);
                }                    
            }
        }).catch(function (response) {
            console.log("Failed to delete post from Server.");
        });
    });

    $('#ContentFeed').on('click', '.hide-post-link' ,function (e) {

        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var isComment = EventBtn.attr("is-comment");
        let uuid = EventBtn.attr("uuid");

        ContentFeedAjaxCalls.HidePostFromProfile(postId).then(function (data) {
            if (data.success === true) {

                if (isComment === "True") {
                    ContentFeedUtility.RemoveCommentFromContentFeedUI(postId);
                    ContentFeedUtility.DecrementPostCommentCounter(uuid);
                }
                else {
                    ContentFeedUtility.RemovePostFromContentFeedUI(postId);
                }  
            }
        }).catch(function (response) {
            console.log("Failed to send hide post request to Server.");
        });
    });

    $('#ContentFeed').on('click', '.report-post-link' ,function (e) {

    });

    $("#ContentFeed").on('click', '.profile-link', function (e) {
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId");
        var profileUrl = "/Users/GetProfiles?userId=" + userId;
        UserNavigatedToLink(profileUrl);
    });

    var loadedPostsComments = [];

    //Testing Profile Hide Comments
    $("#ContentFeed").on("click", ".hide-comments", function(e) {
        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var uniquePostId = EventBtn.attr("unique-post-id");
        var commentCount = EventBtn.attr("comment-count");

        var commentSectionElement = $("div[unique-comment-section$="+ uniquePostId +"]");

        if (commentSectionElement.is(":visible")) {
            commentSectionElement.hide(250);
            EventBtn.text("Show Comments (" + commentCount + ")")
        }
        else {
            //Fired whenever user clicks on Show Comment link && comment section is not yet visible
            commentSectionElement.show(250);
            EventBtn.text("Hide Comments (" + commentCount + ")")

            if (!Utils.containsObject(uniquePostId, loadedPostsComments)) {
                ContentFeedUtility.LoadCommentsRedesign(postId, uniquePostId);
                loadedPostsComments.push(uniquePostId);
            }
        }
    });

    $("#SubmitBtn").on("click", function(e) {
        e.preventDefault();
        ContentFeedUtility.AddSpinnerSubmitPostBtn();
        let formData = ContentFeedInput.GetInputForm();

        ContentFeedAjaxCalls.CreatePost('/api/Posts/BasicPost', formData).then(function (response) {
            if (response.success) {
                //Clear summernote textbox after successful submission.
                ContentFeedUtility.RemoveSpinnerSubmitPostBtn();
                ContentFeedInput.ClearBasicPostForm();
                ContentFeedUtility.AddPostToContentFeedUI(response.post);
            }
        }).catch(function (response) {
            ContentFeedUtility.RemoveSpinnerSubmitPostBtn();
            if (response.responseJSON.message !== undefined) {
                $("#MainPostTextboxValidationMessage").text(response.responseJSON.message);
            }
            else {
                $("#MainPostTextboxValidationMessage").text(response.responseJSON.errors.Summary);
            }

            $("#MainPostTextboxValidationMessage").show(100);
        });
    });

   $("#SubmitCustomPostBtn").on("click", function(e) {
        e.preventDefault();
        ContentFeedUtility.AddSpinnerSubmitCustomPostBtn();


        let formData = ContentFeedInput.GetInputForm("custom");

        ContentFeedAjaxCalls.CreatePost('/api/Posts/CustomizedPost', formData).then(function (response) {

            if (response.success) {
                ContentFeedUtility.RemoveSpinnerSubmitCustomPostBtn();
                ContentFeedInput.ClearCustomPostForm();
                ContentFeedUtility.AddPostToContentFeedUI(response.post);
                ContentFeedUtility.HideCustomPostModal();
            }
        }).catch(function (response) {
            ContentFeedUtility.RemoveSpinnerSubmitCustomPostBtn();
            if (response.responseJSON.message !== undefined) {
                $("#CustomPostValidationMessage").text(response.responseJSON.message);
            }
            else {
                $("#CustomPostValidationMessage").text(response.responseJSON.errors.Summary);
            }
            $("#CustomPostValidationMessage").show(100);
        });
    });

    $("#ContentFeed").on("click", ".comment-send-btn", function (e) { 
        e.preventDefault();

        let SendButton = $(e.target);
        let uuid = SendButton.attr("unique-identifier");
        let parentPostId = SendButton.attr("parent-post-id");


        let formData = ContentFeedInput.GetInputForm("comment", uuid, parentPostId);

        ContentFeedAjaxCalls.CreatePost('/api/Posts/BasicPost', formData).then(function (response) {

            if (response.success) {
                //Clear summernote textbox after successful submission.
                ContentFeedInput.ClearCommentForm(uuid);
                ContentFeedUtility.AddCommentToContentFeedUI(uuid, response.post);
            }
        }).catch(function (response) {
            let validationMessageElement = $("span[unique-error-identifier$="+ uuid +"]");

            if (response.responseJSON.message !== undefined) {
                validationMessageElement.text(response.responseJSON.message);
            }
            else {
                validationMessageElement.text(response.responseJSON.errors.Summary);
            }

            validationMessageElement.show(100);
        });
    });

});
