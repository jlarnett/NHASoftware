function InitializeAllTopicPosts(post) {

    var table = $("#ForumPostTable").DataTable({
        data: post,
        columns:
        [
            {
                data: "id",
                render: function (data, type, post) 
                {
                    const formattedLastModifiedDate = new Date(post.lastModifiedDate).toLocaleDateString("en-US", {
                        month: "short",
                        day: "numeric",
                        year: "numeric"
                    });

                    return "<div class='container-fluid modern-forum-post-container shadow'>" + 
                                "<a class='modern-thread-link' role='button' post-id='" + post.id + "'>" +
                                    "<div class='row'>" +
                                        "<div class='col'>" + 
                                            "<h3 class='text-dark'>" + post.title + "</h3>" + 
                                        "</div>" +
                                    "</div>" +
                                    "<div class='row'>" +
                                        "<div class='col'>" +
                                            "<h3 class='text-dark text-truncate-multi-line'>" + post.forumText + "</h3>" +
                                        "</div>" +
                                    "</div>" +
                                    "<div class='row w-100'>" +
                                        "<div class='col row align-content-center m-auto'>" +
                                            "<div class='col-auto m-auto'>" +
                                                "<input type='image' title='Likes' style='max-height: 40px; max-width:40px;' src='/Images/Facebook-Like-Filled.png' class='img-fluid js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                            "</div>" +
                                            "<div class='col-auto m-auto'>" +
                                                "<h3>" + post.likeCount + "</h3>" +
                                            "</div>" +
                                            "<div class='col-auto m-auto'>" +
                                                "<input type='image' title='Comments' src='/Images/PostIcon.png' style='max-height: 40px; max-width:40px;' class='img-fluid js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                            "</div>" +
                                            "<div class='col-auto m-auto'>" +
                                                "<h3>" + post.commentCount + "</h3>" +
                                            "</div>" +
                                            "<div class='col m-auto'>" +
                                                "<h5 class='h6'>" + "Last Modified: <strong class='text-secondary-emphasis'>" + formattedLastModifiedDate + "</strong></h5>" +
                                            "</div>" +
                                        "</div>" +
                                "</a>" +
                            "</div>";
                }
            }
        ]
    });
}
