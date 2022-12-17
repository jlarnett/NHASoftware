$(document).ready(function () {

    $("#HomeContentFeed").on("click", ".hide-comments", function(e) {

        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var uniquePostId = EventBtn.attr("unique-post-id");
        //console.log(postId);

        var commentSectionElement = $("div[unique-post-comment-section-id$="+ uniquePostId +"]");
        //console.log("Comment Section jquery elemnt -" + commentSectionElement);
        //console.log(commentSectionElement.is(":visible"));

        if (commentSectionElement.is(":visible")) {
            //commentSectionElement.slideUp("slow");
            commentSectionElement.hide("slow");
            EventBtn.text("Show Comments")
        }
        else {
            //commentSectionElement.slideDown("slow");
            commentSectionElement.show("slow");
            EventBtn.text("Hide Comments")
        }
    });
});