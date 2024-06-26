﻿function InitializeAllTopicPosts(post) {

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

                    return "<div class='container-fluid modern-forum-post-container'>" + 
                                "<a class='modern-thread-link' role='button' post-id='" + post.id + "'>" +
                                    "<div class='row'>" +
                                        "<div class='col'>" + 
                                            "<h3 class='modern-post-text'>" + post.title + "</h3>" + 
                                        "</div>" +
                                    "</div>" +
                                    "<div class='row align-content-center m-auto'>" +
                                        "<div class='col-2 row'>" +
                                            "<div class='col-5 m-auto'>" +
                                                "<input type='image' src='/Images/likeIcon.png' class='img-fluid js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                            "</div>"+
                                            "<div class='h3 col'>" + post.likeCount + "</div>" +
                                        "</div>" +
                                        "<div class='col-2 row'>" +
                                            "<div class='col-5 m-auto'>" +
                                                "<input type='image' src='/Images/PostIcon.png' class='img-fluid js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                             "</div>" +
                                            "<div class='h3 col'>" + post.commentCount + "</div>" +
                                        "</div>" +
                                        "<div class='col-auto'>" +
                                            "<div class='m-auto'>" + "Thread Last Updated: " + date + "</div>" +
                                        "</div>" +
                                    "</div>" +
                                "</a>" +
                            "</div>";
                }
            }
        ]
    });
}
