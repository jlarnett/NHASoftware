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

        commentsTable.row.add( ["<div class='container-fluid shadow modern-forum-post-container p-4 mb-2' role='button'>" +
                                    "<div class='row mb-4 border-bottom border-dark p-2'>" +
                                        "<div class='col-3'>" +
                                            "<div class='row border border-2 border-dark shadow-lg rounded-pill m-auto p-4'>" +
                                                "<div class='row col-auto align-contents-center w-100 m-auto'>" +
                                                    "<img src='" + commentPicture + "' class='col-12 img-fluid rounded-pill m-auto'/>" +
                                                "</div>" +
                                                "<div class='row align-contents-center w-100 m-auto text-break'>" + 
            "<a class='interactable text-decoration-none' href='/Users/GetProfiles?userId=" + comments[i].user.id + "'><div class='text-center text-success fw-semibold h4 m-auto text-break'>" + comments[i].user.displayName + "</div></a>" +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                        "<div class='col text-center align-self-center text-break border-start border-dark'>" + 
                                            "<div class='h5'>" + comments[i].commentText + "</div>" + 
                                        "</div>" +
                                    "</div>" +
                                    "<div class='row align-content-end'>" +
                                        "<div class='col-auto'>" +
                                            "<h5>" + date + "</h3>" +
                                        "</div>" +
                                        checkIfCommentUser(comments[i], userId, adminUserBool) +
                                            "<div class='col-auto btn btn-primary js-like-comment interactable' comment-id='" + comments[i].id + "'>" +
                                                "<div class='row align-items-center'>" +
                                                "<div class='col-auto'>" +
                                                    "<input type='image' src='/Images/Facebook-Like-Filled.png' class='img-fluid col-10' />" +
                                                "</div>" +
                                            "<div class='h3 col-auto'>" + comments[i].likeCount + "</div>" +
                                            "</div>" +
                                        "</div>" + 
                                    "</div>" +
                                "</div>"] ).draw();

    }
}

function checkIfCommentUser(data, userIdString, adminUserBool) {

    if (userIdString === data.user.id || adminUserBool) {
        return "<div class='col-auto'>" +
            "<button class='btn btn-dark js-edit-comment interactable' comment-id='" +
            data.id +
            "'>Edit Comment</button>" +
            "</div>" +
            "<div class='col'>" +
            "<button class='btn btn-primary js-delete-comment interactable' comment-id='" +
            data.id +
            "'>Delete Comment</button>" +
            "</div>";
    } 
    else {
        return "";
    }
}
