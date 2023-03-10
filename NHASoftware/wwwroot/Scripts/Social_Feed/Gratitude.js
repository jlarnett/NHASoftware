$(document).ready(function () {

    //Fired whenever the like icon is clicked for post
    $('#HomeContentFeed').on('click', '.post-like' ,function (e) {
        var EventBtn = $(e.target);
        var currentBtnImageSrc = EventBtn.attr("src");
        var postId = EventBtn.attr("post-id");

        var userSessionActive = $("#HomeContentFeed").attr("user-session-active");

        if (userSessionActive === "False") {
            console.log("User Login required to like social media posts & comments");
        }

        var userId = $("#HomeContentFeed").attr("userId");

        if (userSessionActive) {

            if (currentBtnImageSrc.toLowerCase() === "images/facebook-like.png") {

                LikePost(userId, postId).then(function (data) {
                    if (data.success === true) {
                        EventBtn.attr("src", "images/facebook-like-filled.png");
                        IncrementLikeCounter(EventBtn);
                    }
                    else {
                        console.log("Failed sending like to API.");
                    }
                });
            }
            else if (currentBtnImageSrc.toLowerCase() === "images/facebook-like-filled.png") {
                DeleteLike(userId, postId, false).then(function (data) {
                    if (data.success === true) {
                        EventBtn.attr("src", "images/facebook-like.png");
                        DecrementLikeCounter(EventBtn);
                    }
                    else {
                        console.log("Failed to send DELETE like request to API");
                    }
                });
            }
        }
    });

    $('#HomeContentFeed').on('click', '.post-dislike' ,function (e) {

        var EventBtn = $(e.target);
        var currentBtnImageSrc = EventBtn.attr("src");

        var postId = EventBtn.attr("post-id");
        var userSessionActive =$("#HomeContentFeed").attr("user-session-active");
        var userId = $("#HomeContentFeed").attr("userId");

        if (userSessionActive) {

            if (currentBtnImageSrc.toLowerCase() === "images/dislike.png") {
                //SEND DISLIKE TO API
                LikePost(userId, postId, true).then(function (data) {
                    if (data.success === true) {
                        EventBtn.attr("src", "images/dislike-filled.png");
                        IncrementLikeCounter(EventBtn);
                    }
                    else {
                        console.log("Failed sending dislike to API.");
                    }
                });
            }
            else if (currentBtnImageSrc.toLowerCase() === "images/dislike-filled.png") {
                //SEND DELETE DISLIKE REQUEST TO API
                DeleteLike(userId, postId, true).then(function (data) {
                    if (data.success === true) {
                        EventBtn.attr("src", "images/dislike.png");
                        DecrementLikeCounter(EventBtn);
                    }
                    else {
                        console.log("Failed to send DELETE dislike request to API");
                    }
                });

            }
        }
    });

    function LikePost(userId, postId, isDislike = false) {

        var userLikeObject = {};
        userLikeObject.UserId = userId;
        userLikeObject.PostId = postId;
        userLikeObject.IsDislike = isDislike;

        return $.ajax({
            url: '/api/Gratitude/UserLike',
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: JSON.stringify(userLikeObject),
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
        });
    }

    function DeleteLike(userId, postId, isDislike = false) {
        var userLikeObject = {};
        userLikeObject.UserId = userId;
        userLikeObject.PostId = postId;
        userLikeObject.IsDislike = isDislike;

        return $.ajax({
            url: '/api/Gratitude/UserLike',
            method: 'DELETE',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: JSON.stringify(userLikeObject),
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
        });
    }

    function IncrementLikeCounter(EventBtn) {
        var counterElement = EventBtn.prev();
        var likeCount = Number(counterElement.text());
        likeCount++;

        counterElement.text(likeCount);
    }

    function DecrementLikeCounter(EventBtn) {

        var counterElement = EventBtn.prev();
        var likeCount = Number(counterElement.text());
        likeCount--;

        counterElement.text(likeCount);
    }


});