﻿function LoadCommentsTable(comments, userId, adminUserBool) {

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

        commentsTable.row.add( ["<div class='container-fluid modern-forum-post-container p-4' role='button'>" +
                                    "<div class='row'>" +
                                        "<div class='col-2'>" +
                                            //
                                            "<div class='row border border-4 border-danger rounded-pill m-auto p-2'>" +
                                                "<div class='row col-auto align-contents-center w-100 m-auto'>" +
                                                    "<img src='" + commentPicture + "' class='col-10 img-fluid rounded-pill m-auto'/>" +
                                                "</div>" +
                                                "<div class='row align-contents-center w-100 m-auto'>" + 
                                                    "<div class='text-center text-danger h4 m-auto'>" +comments[i].user.displayName + "</div>" +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                        "<div class='col text-center align-self-center text-break'>" + 
                                            "<div class='text-cyan h2'>" + comments[i].commentText + "</div>" + 
                                        "</div>" +
                                    "</div>" +
                                    "<div class='row align-content-end'>" +
                                        "<div class='col-auto'>" +
                                            "<h3 >" + date + "</h3>" +
                                        "</div>" +
                                        checkIfCommentUser(comments[i], userId, adminUserBool) +
                                            "<div class='col-auto row'>" +
                                                "<div class='col-2'>" +
                                                    "<input type='image' src='/Images/LikeIcon.png' class='img-fluid js-like-comment' comment-id='" + comments[i].id + "'/>" +
                                                "</div>" +
                                            "<div class='h3 col-auto'>" + comments[i].likeCount + "</div>" +
                                        "</div>" + 
                                    "</div>" +
                                "</div>"] ).draw();

    }
}

function checkIfCommentUser(data, userIdString, adminUserBool) {

    if (userIdString === data.user.id || adminUserBool) {
        return "<div class='col-auto'>" +
            "<button class='btn-primary js-edit-comment' comment-id='" +
            data.id +
            "'>Edit Comment</button>" +
            "</div>" +
            "<div class='col'>" +
            "<button class='btn-primary js-delete-comment' comment-id='" +
            data.id +
            "'>Delete Comment</button>" +
            "</div>";
    } 
    else {
        return "";
    }
}
