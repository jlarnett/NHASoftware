function LoadCommentsTable(comments, userId, adminUserBool) {

    var arrayLength = comments.length;

    var commentsTable = $("#CommentsTable").DataTable({
        "columns": [
            null
        ]
    });

    for (var i = 0; i < arrayLength; i++) {

        var comment = comments[i].comment;
        var commentPicture = "/ProfilePictures/" + comment.user.profilePicturePath;

        const formattedLastModifiedDate = new Date(comment.lastModifiedDate).toLocaleDateString("en-US", {
            month: "short",
            day: "numeric",
            year: "numeric"
        });

        const formattedCreationDate = new Date(comment.creationDate).toLocaleDateString("en-US", {
            month: "short",
            day: "numeric",
            year: "numeric"
        });

        const formattedUserCreationDate = new Date(comment.user.dateJoined).toLocaleDateString("en-US", {
            month: "short",
            day: "numeric",
            year: "numeric"
        });

        commentsTable.row.add(['<div class="container-fluid modern-forum-post-container shadow bg-dark">' +
            '<div class= "row align-items-xl-stretch">' +
                '<div class="col-12 col-sm-4 col-md-3 col-lg-2 align-self-start p-4 night-gradient">' +
                    '<div class="row h5 text-center text-black text-break">' +
                        '<a asp-controller="Users" href="/Users/GetProfiles?userId=' + comment.user.id + '" class="text-decoration-none interactable">' +
                            '<div class="text-break col text-black fw-semibold text-glow-red">' + comment.user.displayName + '</div >' +
                        '</a>' +
                    '</div>' +
                    '<div class="row col m-auto">' +
                        '<img src="' + commentPicture + '" class="img-fluid rounded-pill col-12 m-auto"/>' +
                    '</div>' +
                    '<div class="row align-items-center">' +
                        '<div class="col-12 d-flex justify-content-between align-items-center p-2 me-4 text-center">' +
                        '<i class="bi bi-clock" title="Date Joined"></i>' +
                            formattedUserCreationDate +
                        '</div>' +
                    '</div>' +
                    '<div class="row align-items-center">' +
                        '<div class="col-12 d-flex justify-content-between align-items-center p-2 me-4 text-center">' +
                            '<i class="bi bi-chat-fill" title="Messages"></i>' +
                            comments[i].totalUserMessages +
                        '</div>' +
                    '</div>' +
                    '<div class="row align-items-center">' +
                        '<div class="col-12 d-flex justify-content-between align-items-center p-2 me-4 text-center">' +
                        '<i class="bi bi-hand-thumbs-up-fill" title="Likes"></i>' +
                            comments[i].totalUserLikes +
                        '</div>' +
                    '</div>' +
                    '<div class="row align-items-center">' +
                        '<div class="col-12 d-flex justify-content-between align-items-center p-2 me-4 text-center">' +
                        '<i class="bi bi-cash" title="Trophies"></i>' +
                            comment.user.userCash +
                        '</div>' +
                    '</div>' +
                '</div>' +
                '<div class="col bg-body-tertiary ps-5 pe-5 pt-2 pb-2 d-flex flex-column">' +
                    '<div class="row">' +
                        '<h5 id="PostDate" class="text-body fs-6">' + formattedCreationDate + '</h5>' +
                    '</div>' +
                    '<div class="row h5 text-center">' + comment.commentText + '</div>' +
                    '<div class="row align-self-end mt-auto">' +
                        'Last Edited: ' + formattedLastModifiedDate +
                    '</div>' +
                    '<hr />' +
                    '<div class="row align-self-end">' +
                        '<div class="col m-auto"></div>' +
                        checkIfCommentUserV2(comment, userId, adminUserBool) +
                        '<div class="col-auto m-auto">' +
                            '<div class="btn btn-link js-like-comment interactable" comment-id="' + comment.id + '">' +
                                '<div class="d-flex align-items-center gap-1">' +
                                    '<span class="fs-6 mb-0">' + comment.likeCount + '</span>' +
                                    '<input type="image" src="/Images/Facebook-Like-Filled.png" class="img-fluid" style="height:1.5rem;" />' +
                                '</div>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>' +

        '</div >']).draw();
    }
}

function checkIfCommentUserV2(data, userIdString, adminUserBool) {

    if (userIdString === data.user.id || adminUserBool) {
        return  '<div class="col-auto">' +
                    '<button class="btn btn-link js-edit-comment interactable" comment-id="' + data.id + '">Edit Comment</button>' +
                '</div>' +
                '<div class="col-auto">' +
                    '<button class="btn btn-link js-delete-comment interactable"comment-id="' + data.id + '">Delete Comment</button>' +
                '</div>'
    }
    else {
        return "";
    }
}
