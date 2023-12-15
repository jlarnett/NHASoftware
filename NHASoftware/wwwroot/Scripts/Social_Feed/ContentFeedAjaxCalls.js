class ContentFeedAjaxCalls {
    static RetrieveMorePosts() {
        //AJAX CALL TO /Home/ReturnSocialPosts endpoint. Returns async result of posts inside MultiPost partial view ready for consumption
        return $.get("/Home/ReturnSocialPosts", function (data) {});
    }

    static RetrievePostComments(postId, uuid) {
        ///AJAX CALL TO Home/Return/CommentPosts/ endpoint. 
        //Returns async result all 'Comments' for specified postId. Returns them in a MultiComment partial view ready for appending.
        return $.get("/Home/ReturnCommentPosts?id=" + postId + "&uuid=" + uuid, function(data) {});
    }

    static ConvertPostDTOToPartialView(postDTO) {
        //Takes PostDTO data in and sends it to Home controller /ReturnPostPartialView endpoint to convert into partial view.
        return $.ajax({
            url: '/ReturnPostPartialView',
            method: 'POST',
            data: postDTO,
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(post) {

            },
            error: function (data) {

            }
        });
    }

    static ConvertCommentDTOToPartialView(commentDTO) {
        //Takes PostDTO data in and sends it to Home controller /ReturnCommentPartialView endpoint to convert into partial view.
        return $.ajax({
            url: '/ReturnCommentPartialView',
            method: 'POST',
            data: commentDTO,
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(commentPartialView) {
            },
            error: function (data) {

            }
        });
    }

    static DeletePost(postId) {
        //sends DELETE request to post API. Sets the isDeletedFlag to true in DB. 
        var apiURL = '/api/posts/' + postId;

        return $.ajax({
            url: apiURL,
            method: 'DELETE',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
        });
    }

    static HidePostFromProfile(postId) {
        //sends DELETE request to post API. Sets the isHiddenFromUserProfile flag in DB to true. 
        var apiURL = '/api/posts/hide/' + postId;

        return $.ajax({
            url: apiURL,
            method: 'DELETE',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
        });
    }

    static RetrieveAllPostForUser(userId) {
        //Calls the Post WebAPI & gets all post created under supplied userId
        return $.get("/GetAllPostForUser/" + userId, function(data) {});
    }

    static RetrieveImagesForPost(postId) {
        //Retrieves images for specified postId from API. 
        return $.get("/api/posts/GetPostImages/" + postId, function (data) {});
    }

    static CreatePost(ApiUrl, postForm) {
        //Generic method for sending post objects to API for creation. Takes in the API endpoint & the postForm to send. 
        return $.ajax({
            url: ApiUrl,
            method: 'POST',
            contentType: false,
            processData: false,
            data: postForm,
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(data) {
                if (data.success) {
                }
            },
            error: function (data) {
            }
        });
    }
}