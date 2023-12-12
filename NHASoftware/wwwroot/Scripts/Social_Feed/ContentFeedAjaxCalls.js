class ContentFeedAjaxCalls {
    static RetrieveMorePosts() {
        //AJAX CALL TO api/posts webapi. Returns async result of posts
        return $.get("/Home/ReturnSocialPosts", function (data) {});
    }

    static RetrievePostComments(postId) {
        ///AJAX CALL TO api/posts/findchildrenposts webapi endpoint. 
        //Returns async result all 'Comments' => Comments are just post with fatherPostId populated.
        return $.get("/Home/ReturnCommentPosts/" + postId, function(data) {});
    }

    static ConvertPostDTOToPartialView(postDTO) {
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
        return $.get("/api/posts/GetPostImages/" + postId, function (data) {});
    }

    static CreatePost(ApiUrl, postForm) {
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