$(document).ready(function () {

    $("#HomeContentFeed").on("click", ".hide-comments", function(e) {
        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var uniquePostId = EventBtn.attr("unique-post-id");

        var commentSectionElement = $("div[unique-post-comment-section-id$="+ uniquePostId +"]");

        if (commentSectionElement.is(":visible")) {
            commentSectionElement.hide("slow");
            EventBtn.text("Show Comments")
        }
        else {
            //Fired whenever user clicks on Show Comment link && comment section is not yet visible
            commentSectionElement.show("slow");
            EventBtn.text("Hide Comments")

            //Fires loading of children post HTML
            LoadPostComments(postId, uniquePostId);
        }
    });

    //Testing Profile Hide Comments
    $("#ContentFeed").on("click", ".hide-comments", function(e) {
        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var uniquePostId = EventBtn.attr("unique-post-id");

        var commentSectionElement = $("div[unique-comment-section$="+ uniquePostId +"]");

        if (commentSectionElement.is(":visible")) {
            commentSectionElement.hide("slow");
            EventBtn.text("Show Comments")
        }
        else {
            //Fired whenever user clicks on Show Comment link && comment section is not yet visible
            commentSectionElement.show("slow");
            EventBtn.text("Hide Comments")

            //Fires loading of children post HTML
            LoadCommentsRedesign(postId, uniquePostId);
        }
    });
});