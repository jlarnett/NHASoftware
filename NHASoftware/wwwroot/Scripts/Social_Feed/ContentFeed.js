$(document).ready(function () {

    $('#ContentFeed').on('click', '.delete-post-link' ,function (e) {

        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var isComment = EventBtn.attr("is-comment");

        DeletePost(postId).then(function (data) {
            if (data.success === true) {
                isComment === 'true' ? RemoveCommentFromContentFeed(postId) : RemovePostFromContentFeed(postId);
            }
            else {
                console.log("Failed to send DELETE Post request to api. ");
            }
        });
    });

    $('#ContentFeed').on('click', '.hide-post-link' ,function (e) {

        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var isComment = EventBtn.attr("is-comment");

        DeletePost(postId).then(function (data) {
            if (data.success === true) {
                isComment === 'true' ? RemoveCommentFromContentFeed(postId) : RemovePostFromContentFeed(postId);
            }
            else {
                console.log("Failed to send DELETE Post request to api. ");
            }
        });
    });

    $('#ContentFeed').on('click', '.report-post-link' ,function (e) {

    });

});

function RemovePostFromContentFeed(postId) {
    //Removes the post from the content feed'Ss HTML. This doesn't affect the BD just HTML. 
    var postContainer = $('[post-delete-id=' + postId + ']');
    postContainer.remove();
}

function RemoveCommentFromContentFeed(postId) {
    //Removes comment from the content feed's HTML. This doesn't affect the DB just HTML.
    var commentContainer = $('[comment-delete-id=' + postId + ']');
    commentContainer.remove();
}

function DeletePost(postId) {
    //sends DELETE request to post API. Sets up isDeletedFlag in DB. 
    var apiURL = '/api/posts/' + postId;

    return $.ajax({
        url: apiURL,
        method: 'DELETE',
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
    });
}
