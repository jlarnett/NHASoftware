function LoadCommentsTable(comments, userId, adminUserBool) {

    var arrayLength = comments.length;

    var commentsTable = $("#CommentsTable").DataTable({
        "columns": [
            null
        ]
    });

    for (var i = 0; i < arrayLength; i++) {
        var str = spacetime(comments[i].creationDate);
        var date = str.format('{month} {date-pad} {year} {time}{am pm}');

        var commentPicture = "/ProfilePictures/" + comments[i].user.profilePicturePath;

        commentsTable.row.add( ["<div class='modern-forum-post-container'>" +
                                    "<div class='modern-forum-post-row-container'>" +
                                        "<div class='modern-post-profile'>" +
                                            "<div class='modern-post-profile-picture-container'>" +
                                                "<img src='" + commentPicture + "' class='modern-comment-profile-picture'/>" +
                                            "</div>" +
                                                "<div class='modern-post-profile-displayname'>" + comments[i].user.displayName + "</div>" +
                                        "</div>" +
                                            "<div class='modern-post-details'>" + "<p class='modern-post-text'>" + comments[i].commentText + "</p>" + "</div>" +
                                            "</div>" +
                                    "<div class='modern-post-actions'>" +
                                        "<div class='modern-post-action'>" +
                                            "<h3 >" + date + "</h3>" +
                                        "</div>" +
                                        "<div class='modern-post-actions-like modern-post-action'>" +
                                            "<input type='image' src='/Images/LikeIcon.png' class='modern-post-action-like-picture js-like-comment' comment-id='" + comments[i].id + "'/>" +
                                            "<h3>" + comments[i].likeCount + "</h3>" +
                                        "</div>" + checkIfCommentUser(comments[i], userId, adminUserBool) +
                                    "</div>" +
                                "</div>"] ).draw();

    }
}

function checkIfCommentUser(data, userIdString, adminUserBool) {

    if (userIdString === data.user.id || adminUserBool) {
        return "<div class='modern-post-action'>" +
            "<button class='btn-primary js-edit-comment' comment-id='" +
            data.id +
            "'>Edit Comment</button>" +
            "</div>" +
            "<div class='modern-post-action'>" +
            "<button class='btn-primary js-delete-comment' comment-id='" +
            data.id +
            "'>Delete Comment</button>" +
            "</div>";
    } 
    else {
        return "";
    }
}
