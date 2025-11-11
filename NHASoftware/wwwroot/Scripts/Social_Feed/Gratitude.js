$(document).ready(function () {

    //Fired whenever the like icon is clicked for post
    $('#ContentFeed').on('click', '.post-like' ,function (e) {
        var EventBtn = $(e.target);
        var currentBtnImageSrc = EventBtn.attr("src");
        var postId = EventBtn.attr("post-id");
        var uuid = EventBtn.attr("uuid");

        var userSessionActive = CheckUserSessionIsActive();

        if (userSessionActive === "False") {
            console.log("User Login required to like social media posts & comments");
            let validationMessageElement = $("span[unique-error-identifier$=" + uuid + "]");
            validationMessageElement.text("Login required to like social media posts & comments");
            validationMessageElement.show(100);
            return;
        }

        var userId = RetrieveCurrentUserId();

        if (userSessionActive) {

            if (currentBtnImageSrc.toLowerCase() === "/images/facebook-like.png") {

                LikePost(userId, postId).then(function (data) {
                    if (data.success === true) {
                        EventBtn.attr("src", "/images/facebook-like-filled.png");
                        IncrementLikeCounterRedesign(EventBtn);
                    }
                    else {
                        console.log("Failed sending like to API.");
                    }
                });
            }
            else if (currentBtnImageSrc.toLowerCase() === "/images/facebook-like-filled.png") {
                DeleteLike(userId, postId, false).then(function (data) {
                    if (data.success === true) {
                        EventBtn.attr("src", "/images/facebook-like.png");
                        DecrementLikeCounterRedesign(EventBtn);
                    }
                    else {
                        console.log("Failed to send DELETE like request to API");
                    }
                });
            }
        }
    });

    //Fired whenever the dislike icon is pressed on pages content feed. 
    $('#ContentFeed').on('click', '.post-dislike' ,function (e) {

        var EventBtn = $(e.target);
        var currentBtnImageSrc = EventBtn.attr("src");

        var postId = EventBtn.attr("post-id");
        var userSessionActive = CheckUserSessionIsActive();
        var userId = RetrieveCurrentUserId();
        var uuid = EventBtn.attr("uuid");

        if (userSessionActive === "False") {
            console.log("User Login required to like social media posts & comments");
            let validationMessageElement = $("span[unique-error-identifier$=" + uuid + "]");
            validationMessageElement.text("Login required to dislike social media posts & comments");
            validationMessageElement.show(100);
            return;
        }

        if (userSessionActive) {

            if (currentBtnImageSrc.toLowerCase() === '/images/dislike_remake.png') {
                //SEND DISLIKE TO API
                LikePost(userId, postId, true).then(function (data) {
                    if (data.success === true) {
                        EventBtn.attr("src", "/images/dislike-filled.png");
                        IncrementDisLikeCounterRedesign(EventBtn);
                    }
                    else {
                        console.log("Failed sending dislike to API.");
                    }
                });
            }
            else if (currentBtnImageSrc.toLowerCase() === "/images/dislike-filled.png") {
                //SEND DELETE DISLIKE REQUEST TO API
                DeleteLike(userId, postId, true).then(function (data) {
                    if (data.success === true) {
                        EventBtn.attr("src", "/images/dislike_remake.png");
                        DecrementDisLikeCounterRedesign(EventBtn);
                    }
                    else {
                        console.log("Failed to send DELETE dislike request to API");
                    }
                });

            }
        }
    });

    function LikePost(userId, postId, isDislike = false) {
        //Send POST request to Gratitude API. Returns async promise of request response. 
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
        //Send DELETE request to Gratitude API. Returns async promise of request response. 
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

    function IncrementLikeCounterRedesign(EventBtn) {
        //Increments the like counter for specified like button & all other with matching postIds for Redesign gratitude system
        var postId = EventBtn.attr("post-id");
        var counterElement = $('[like-counter-post-id=' + postId + ']');
        var likeCount = Number(counterElement.first().text());
        likeCount++;
        counterElement.text(likeCount);
    }

    function DecrementLikeCounterRedesign(EventBtn) {
        //Decrements the like counter for specified like button & all other with matching postIds for Redesign gratitude system
        var postId = EventBtn.attr("post-id");
        var counterElement = $('[like-counter-post-id=' + postId + ']');
        var likeCount = Number(counterElement.first().text());
        likeCount--;
        counterElement.text(likeCount);
    }

    function IncrementDisLikeCounterRedesign(EventBtn) {
        //Incrments the dislike counter for the specified dislike button & all other with matching postIds for Redesign gratitude system
        var postId = EventBtn.attr("post-id");
        var counterElement = $('[dislike-counter-post-id=' + postId + ']');
        var likeCount = Number(counterElement.first().text());
        likeCount++;
        counterElement.text(likeCount);
    }

    function DecrementDisLikeCounterRedesign(EventBtn) {
        //Decrements the dislike counter for the specified button & all other with matching postIds for Redesign gratitude system
        var postId = EventBtn.attr("post-id");
        var counterElement = $('[dislike-counter-post-id=' + postId + ']');
        var likeCount = Number(counterElement.first().text());
        likeCount--;
        counterElement.text(likeCount);
    }

});