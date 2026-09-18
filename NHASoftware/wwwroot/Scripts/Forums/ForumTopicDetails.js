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

                    return "<div class='container-fluid modern-forum-post-container shadow-sm'>" + 
                                "<a class='modern-thread-link' role='button' post-id='" + post.id + "'>" +
                                    "<div class='row'>" +
                                        "<div class='col'>" + 
                                             "<h3 class='forum-post-title'>" + post.title + "</h3>" + 
                                        "</div>" +
                                    "</div>" +
                                    "<div class='row'>" +
                                        "<div class='col'>" +
                                             "<p class='forum-post-preview text-truncate-multi-line'>" + post.forumText + "</p>" +
                                        "</div>" +
                                    "</div>" +
                                     "<div class='row w-100 forum-post-stats-row'>" +
                                             "<div class='col row align-content-center m-auto'>" +
                                                 "<div class='col-auto m-auto forum-post-stat-icon'>" +
                                                "<input type='image' title='Likes' style='max-height: 40px; max-width:40px;' src='/Images/Facebook-Like-Filled.png' class='img-fluid js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                            "</div>" +
                                                 "<div class='col-auto m-auto'>" +
                                                     "<span class='forum-post-stat-value'>" + post.likeCount + "</span>" +
                                            "</div>" +
                                                 "<div class='col-auto m-auto forum-post-stat-icon'>" +
                                                "<input type='image' title='Comments' src='/Images/PostIcon.png' style='max-height: 40px; max-width:40px;' class='img-fluid js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                            "</div>" +
                                            "<div class='col-auto m-auto'>" +
                                                     "<span class='forum-post-stat-value'>" + post.commentCount + "</span>" +
                                            "</div>" +
                                                 "<div class='col m-auto forum-post-meta'>" +
                                                     "<span class='forum-post-meta-label'>Last Modified:</span> <span class='forum-post-meta-value'>" + formattedLastModifiedDate + "</span>" +
                                            "</div>" +
                                        "</div>" +
                                "</a>" +
                            "</div>";
                }
            }
        ]
    });
}
