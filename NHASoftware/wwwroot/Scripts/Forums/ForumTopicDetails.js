function InitializeAllTopicPosts(post) {

    var table = $("#ForumPostTable").DataTable({
        data: post,
        columns:
        [
            {
                data: "id",
                render: function (data, type, post) 
                {
                    var str = spacetime(post.creationDate);
                    var date = str.format('{month}-{date-pad}-{year} {time}{am pm}');

                    return "<div class='modern-forum-post-container'> <a class='modern-thread-link' post-id='" + post.id + "'>" +
                        "<div class='modern-forum-post-row-container'>" +
                        "<div class='modern-forum-topic-title'>" + "<h3 class='modern-post-text'>" + post.title + "</h3>" + "</div>" +
                        "</div>" +
                        "<div class='modern-post-actions'>" +
                        "<div class='modern-post-actions-like modern-post-action'>" +
                        "<input type='image' src='/Images/likeIcon.png' class='modern-threads-picture js-thread-comment' comment-id='" + "commentid" + "'/>" +
                        "<h3>" + post.likeCount + "</h3>" +
                        "</div>" +
                        "<div class='modern-post-actions-like modern-post-action'>" +
                        "<input type='image' src='/Images/PostIcon.png' class='modern-threads-picture js-thread-comment' comment-id='" + "commentid" + "'/>" +
                        "<h3>" + post.commentCount + "</h3>" +
                        "</div>" +
                        "<div class='modern-post-action'>" +
                        "<h5>" + "Thread Last Updated: " + date + "</h5>" +
                        "</div>" +
                        "</div>" +
                        "</a></div>";
                }
            }
        ]
    });
}
